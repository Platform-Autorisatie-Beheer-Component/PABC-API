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
                if (string.IsNullOrWhiteSpace(options.ApiUrl) || !Uri.TryCreate(options.ApiUrl, UriKind.Absolute, out var apiUri))
                    throw new InvalidOperationException("ZgwZaakregister:ApiUrl must be an absolute URL when Enabled=true.");
                client.BaseAddress = new Uri(apiUri, "/catalogi/api/v1/");
            });
    }
}
