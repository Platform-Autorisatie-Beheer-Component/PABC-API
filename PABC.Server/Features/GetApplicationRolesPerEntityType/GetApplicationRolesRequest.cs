namespace PABC.Server.Features.GetApplicationRolesPerEntityType;

public class GetApplicationRolesRequest
{
    /// <summary>
    /// The functional roles of the logged in user
    /// </summary>
    /// <example>["GemeenteRol1", "GemeenteRol2"]</example>
    public required string[] FunctionalRoleNames { get; set; }
}
