var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var postgresdb = postgres.AddDatabase("Pabc");

var migrations = builder.AddProject<Projects.PABC_MigrationService>("migrations")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.PABC_Server>("pabc-server")
    .WithEnvironment("API_KEY__0", "unsafe-test-api-key")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WaitForCompletion(migrations);

builder.Build().Run();
