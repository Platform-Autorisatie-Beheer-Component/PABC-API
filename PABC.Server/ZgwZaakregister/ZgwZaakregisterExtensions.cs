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
                var baseUrl = options.ApiUrl.TrimEnd('/');
                client.BaseAddress = new Uri($"{baseUrl}/catalogi/api/v1/");
            });
    }
}
