using System.Text.Json;
using Corvus.Text.Json;
using PABC.Data.Entities;

namespace PABC.MigrationService.Features.DatabaseInitialization;

public interface IDatasetParser
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="JsonSchemaValidationException"></exception>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    Task<DataSet> Parse(Stream stream, CancellationToken token);
}

public class JsonSchemaValidationException : Exception
{
    public required IReadOnlyList<JsonSchemaValidationError> Errors { get; init; }
}

public class JsonSchemaValidationError
{
    public required string InstanceLocation { get; init; }
    public required IReadOnlyDictionary<string, string> Errors { get; init; }
}

public class DatasetParser : IDatasetParser
{
    public async Task<DataSet> Parse(Stream stream, CancellationToken token)
    {
        using var parsed = await ParsedJsonDocument<DataSetSchema>.ParseAsync(stream, cancellationToken: token);
        Validate(parsed.RootElement);
        var entityTypes = new Dictionary<Guid, EntityType>();
        var domains = new List<Domain>();
        var applications = new List<Application>();
        var appRoles = new List<ApplicationRole>();
        var functionalRoles = new List<FunctionalRole>();
        var mappings = new List<Mapping>();

        foreach (var item in parsed.RootElement.EntityTypes.EnumerateArray())
        {
            entityTypes.Add(item.Id, new()
            {
                Id = item.Id,
                Name = item.Name.GetString()!,
                Type = item.Type.GetString()!,
                EntityTypeId = item.EntityTypeId.GetString()!,
                Uri = Uri.TryCreate(item.UriValue.GetString(), UriKind.RelativeOrAbsolute, out var uri) ? uri : null,
            });
        }

        foreach (var item in parsed.RootElement.Domains.EnumerateArray())
        {
            var domain = new Domain
            {
                Id = item.Id,
                Name = item.Name.GetString()!,
                Description = item.Description.GetString()!,
            };
            foreach (var entityTypeId in item.EntityTypeIds.EnumerateArray())
            {
                if (entityTypes.TryGetValue(entityTypeId, out var entityType))
                {
                    domain.EntityTypes.Add(entityType);
                }
            }
            domains.Add(domain);
        }

        foreach (var item in parsed.RootElement.Applications.EnumerateArray())
        {
            applications.Add(new()
            {
                Id = item.Id,
                Name = item.Name.GetString()!,
            });
        }

        foreach (var item in parsed.RootElement.ApplicationRoles.EnumerateArray())
        {
            appRoles.Add(new()
            {
                Id = item.Id,
                ApplicationId = item.ApplicationId,
                Name = item.Name.GetString()!,
            });
        }

        foreach (var item in parsed.RootElement.FunctionalRoles.EnumerateArray())
        {
            functionalRoles.Add(new()
            {
                Id = item.Id,
                Name = item.Name.GetString()!,
            });
        }

        foreach (var item in parsed.RootElement.Mappings.EnumerateArray())
        {
            mappings.Add(new()
            {
                Id = item.Id,
                ApplicationRoleId = item.ApplicationRoleId,
                FunctionalRoleId = item.FunctionalRoleId,
                DomainId = item.DomainId.IsNullOrUndefined() ? null : item.DomainId,
                IsAllEntityTypes = item.IsAllEntityTypes.IsNullOrUndefined() ? false : item.IsAllEntityTypes,
            });
        }

        return new DataSet
        {
            EntityTypes = entityTypes.Values,
            Domains = domains,
            Applications = applications,
            ApplicationRoles = appRoles,
            FunctionalRoles = functionalRoles,
            Mappings = mappings
        };
    }

    private static void Validate(DataSetSchema rootElement)
    {
        using var collector = JsonSchemaResultsCollector.Create(JsonSchemaResultsLevel.Detailed);

        if (rootElement.EvaluateSchema(collector))
            return;

        var errorsByLocation = new Dictionary<string, Dictionary<string, string>>();

        foreach (var result in collector.EnumerateResults())
        {
            if (result.IsMatch)
                continue;

            var instanceLocation = result.GetDocumentEvaluationLocationText();
            var schemaLocation = result.GetSchemaEvaluationLocationText();
            var message = result.GetMessageText();

            var keyword = ExtractKeyword(schemaLocation);
            if (keyword is null)
                continue;

            // Skip intermediate "match the subschema" propagation results
            if (message.Contains("match the subschema", StringComparison.Ordinal))
                continue;

            // For "required" errors, group by the parent path since Corvus reports
            // one error per missing property, but we want a single "required" error per location
            var groupKey = keyword == "required" ? GetParentPath(instanceLocation) : instanceLocation;

            if (!errorsByLocation.TryGetValue(groupKey, out var errors))
            {
                errors = new Dictionary<string, string>();
                errorsByLocation[groupKey] = errors;
            }

            errors.TryAdd(keyword, message);
        }

        if (errorsByLocation.Count > 0)
        {
            // Filter out parent-level propagation errors — keep only the deepest errors
            var keys = errorsByLocation.Keys.ToList();
            var keysToRemove = keys.Where(k => keys.Any(other => other.Length > k.Length && other.StartsWith(k))).ToList();
            foreach (var key in keysToRemove)
                errorsByLocation.Remove(key);

            var validationErrors = errorsByLocation
                .Select(kvp => new JsonSchemaValidationError
                {
                    InstanceLocation = kvp.Key,
                    Errors = kvp.Value,
                })
                .ToList();
            throw new JsonSchemaValidationException { Errors = validationErrors };
        }
    }

    private static string? ExtractKeyword(string schemaLocation)
    {
        var lastSlash = schemaLocation.LastIndexOf('/');
        if (lastSlash < 0)
            return null;
        return schemaLocation[(lastSlash + 1)..];
    }

    private static string GetParentPath(string path)
    {
        var lastSlash = path.LastIndexOf('/');
        return lastSlash <= 0 ? "" : path[..lastSlash];
    }
}
