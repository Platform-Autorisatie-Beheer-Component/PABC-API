using System.Text.Json.Serialization;

namespace PABC.Server.Features.GetApplicationRolesPerEntityType;

public class GetApplicationRolesResponse
{
    public required IReadOnlyCollection<GetApplicationRolesResponseModel> Results { get; set; }
}

public class GetApplicationRolesResponseModel
{
    /// <summary>
    /// The entity type details
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EntityTypeModel? EntityType { get; set; }
    
    /// <summary>
    /// List of application roles associated with this entity type
    /// </summary>
    public required List<ApplicationRoleModel> ApplicationRoles { get; set; } = new();
}

public class EntityTypeModel
{
    /// <summary>
    /// The unique identifier for the entity type
    /// </summary>
    /// <example>melding-klein-kansspel</example>
    public required string Id { get; init; }
    
    /// <summary>
    /// The name of the entity type
    /// </summary>
    /// <example>Melding klein kansspel</example>
    public required string Name { get; init; }
    
    /// <summary>
    /// The kind of entity
    /// </summary>
    /// <example>zaaktype</example>
    public required string Type { get; init; }
}

public class ApplicationRoleModel
{
    /// <summary>
    /// The name of the application role
    /// </summary>
    /// <example>behandelaar</example>
    public required string Name { get; set; }

    /// <summary>
    /// The name of the application
    /// </summary>
    /// <example>zaakafhandelcomponent</example>
    public required string Application { get; set; }
}
