namespace PABC.Server.Features.ApplicationRoles
{
    public class ApplicationRoleResponse
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Application { get; init; }
        public required Guid ApplicationId { get; init; }
    }
}
