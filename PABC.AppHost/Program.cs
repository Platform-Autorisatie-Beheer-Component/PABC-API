using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithPgAdmin(a => a.WithLifetime(ContainerLifetime.Persistent));

var postgresdb = postgres.AddDatabase("Pabc");

var keycloak = builder.AddKeycloak("keycloak", 8080)
    .WithRealmImport("./Realms")
    .WithLifetime(ContainerLifetime.Persistent);

var migrations = builder.AddProject<Projects.PABC_MigrationService>("migrations")
    .WithEnvironment("JSON_DATASET_PATH", "../test-dataset.json")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.PABC_Server>("pabc-server")
    .WithEnvironment("API_KEY__0", "unsafe-test-api-key")
    .WithEnvironment("Oidc__Authority", "http://localhost:8080/realms/zaakafhandelcomponent/")
    .WithEnvironment("Oidc__ClientId", "zaakafhandelcomponent")
    .WithEnvironment("Oidc__ClientSecret", "keycloakZaakafhandelcomponentClientSecret")
    .WithEnvironment("Oidc__RequireHttps", "false")
    .WithEnvironment("Keycloak__Admin__RealmUrl", "http://localhost:8080/admin/realms/zaakafhandelcomponent/")
    .WithEnvironment("Keycloak__Admin__TokenEndpoint", "http://localhost:8080/realms/zaakafhandelcomponent/protocol/openid-connect/token")
    .WithEnvironment("Keycloak__Admin__ClientId", "zaakafhandelcomponent-admin-client")
    .WithEnvironment("Keycloak__Admin__ClientSecret", "zaakafhandelcomponentAdminClientSecret")
    .WithReference(postgresdb)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WaitFor(postgresdb)
    .WaitForCompletion(migrations);

builder.Build().Run();
