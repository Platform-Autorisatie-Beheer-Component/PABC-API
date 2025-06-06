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
    public required string Id { get; init; }
    public required string Type { get; init; }
}

public class ApplicationRoleModel
{
    public required string Name { get; set; }
    public required string Application { get; set; }
}
