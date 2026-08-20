namespace PABC.Server.ZgwZaakregister;

public static class ZgwZaakregisterExtensions
{
    public static void AddZgwZaakregister(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new ZgwZaakregisterOptions();
        configuration.GetSection(ZgwZaakregisterOptions.SectionName).Bind(options);

        services.AddSingleton(options);

        if (!options.Enabled)
        {
            services.AddSingleton<IZgwCatalogiClient, DisabledZgwCatalogiClient>();
            return;
        }

        services.AddSingleton(new ZgwTokenProvider(options));
        services.AddTransient<ZgwTokenHandler>();

        services.AddHttpClient<IZgwCatalogiClient, ZgwCatalogiClient>()
            .AddHttpMessageHandler<ZgwTokenHandler>()
            .ConfigureHttpClient(client =>
            {
                if (string.IsNullOrWhiteSpace(options.CatalogiBaseUrl) || !Uri.TryCreate(options.CatalogiBaseUrl, UriKind.Absolute, out var baseUri))
                    throw new InvalidOperationException("ZgwZaakregister:CatalogiBaseUrl must be an absolute URL when Enabled=true.");
                // Trailing slash is required for correct relative URL resolution by HttpClient
                client.BaseAddress = new Uri(baseUri.ToString().TrimEnd('/') + "/");
            });
    }
}
