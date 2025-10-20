namespace PABC.Server.Auth
{
    public class PabcUser
    {
        public bool IsLoggedIn { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string[] Roles { get; set; } = [];
        public bool HasFunctioneelBeheerderAccess { get; set; }
    }
}
