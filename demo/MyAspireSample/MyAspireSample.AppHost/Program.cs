var builder = DistributedApplication.CreateBuilder(args);



var apiService = builder.AddProject<Projects.MyAspireSample_ApiService>("apiservice");

builder.AddProject<Projects.MyAspireSample_Web>("webfrontend")
    .WithReference(apiService);

builder.Build().Run();
