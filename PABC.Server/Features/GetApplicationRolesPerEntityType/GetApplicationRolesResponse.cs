namespace PABC.Server.Features.GetApplicationRolesPerEntityType;

public class GetApplicationRolesResponse
{
    public required IReadOnlyCollection<GetApplicationRolesResponseModel> Results { get;set; }
}

public class GetApplicationRolesResponseModel
{
    public required EntityTypeModel EntityType { get; set; }
    public required List<ApplicationRoleModel> ApplicationRoles { get; set; }
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
    /// <example>Behandelaar</example>
    public required string Name { get; set; }
    /// <summary>
    /// The application that this role applies to
    /// </summary>
    /// <example>ZAC</example>
    public required string Application { get; set; }
}
