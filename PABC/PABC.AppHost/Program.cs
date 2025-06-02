var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithPgAdmin();
var postgresdb = postgres.AddDatabase("PabcConnection");

var migrations = builder.AddProject<Projects.PABC_Server>("migrations")
    .WithArgs("migrations")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.PABC_Server>("pabc-server")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WaitForCompletion(migrations);

builder.Build().Run();
