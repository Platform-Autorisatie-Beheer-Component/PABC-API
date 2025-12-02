namespace PABC.Server.Auth
{
    public class AuthOptions
    {
        public string Authority { get; set; } = "";
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public string FunctioneelBeheerderRole { get; set; } = "";
        public string? RoleClaimType { get; set; }
        public string? NameClaimType { get; set; }
        public string? EmailClaimType { get; internal set; }
        public bool? RequireHttpsForIdentityProvider { get; set; }
        public bool? LogoutFromIdentityProvider { get; set; }
    }
}
