using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithPgAdmin(a => a.WithLifetime(ContainerLifetime.Persistent));

var postgresdb = postgres.AddDatabase("Pabc");

var keycloak = builder.AddKeycloak("keycloak", 8080, adminPassword: builder.AddParameter("keycloakAdminPassword", "unsafe-development-password", secret: true))
    .WithRealmImport("./Realms")
    .WithLifetime(ContainerLifetime.Persistent);

var migrations = builder.AddProject<Projects.PABC_MigrationService>("migrations")
    .WithEnvironment("JSON_DATASET_PATH", "../test-dataset.json")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.PABC_Server>("pabc-server")
    .WithEnvironment("API_KEY__0", "unsafe-test-api-key")
    .WithEnvironment("Oidc__Authority", "http://localhost:8080/realms/pabc/")
    .WithEnvironment("Oidc__ClientId", "pabc")
    .WithEnvironment("Oidc__ClientSecret", "unsafe-development-secret")
    .WithEnvironment("Oidc__RequireHttps", "false")
    .WithEnvironment("KeycloakAdmin__ClientId", "pabc-admin-client")
    .WithEnvironment("KeycloakAdmin__ClientSecret", "unsafe-development-secret")
    .WithReference(postgresdb)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WaitFor(postgresdb)
    .WaitForCompletion(migrations);

builder.Build().Run();
