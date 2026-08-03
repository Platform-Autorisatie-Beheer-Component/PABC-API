namespace PABC.Server.ZgwZaakregister;

public class ZgwZaakregisterOptions
{
    public const string SectionName = "ZgwZaakregister";

    public bool Enabled { get; set; }
    public string ApiUrl { get; set; } = "";
    public string ClientId { get; set; } = "";
    public string ClientSecret { get; set; } = "";
    public string CatalogusDomein { get; set; } = "";
}
