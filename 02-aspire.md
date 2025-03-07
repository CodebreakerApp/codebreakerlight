# Part 2 - Add .NET Aspire

## Try out .NET Aspire

Create an .NET Aspire sample solution:

1. `dotnet new aspire-starter -f net9.0 --aspire-version 9.1 -o AspireSample`
2. Check the different projects created
3. Check the AppHost project: csproj file, how the projects depend
4. Check Program.cs how the projects connect
5. Check the client application how it retrieves the connection to the API
6. Check the common library - how are the settings from this library applied to the web application and API service?
7. Run the application, check the dashboard - resources, logging, structured logging, metrics, distributed tracing

## Add .NET Aspire to the Games API solution

Use the project you created with part 1, or use the starter from *02-Start*

Add .NET Aspire support to the games API project.

Using Visual Studio, you can use the context menu within the Solution Explorer:

Add | .NET Aspire Orchtestrator Support

Select Aspire 9.1.

You can also use the .NET CLI:
- `dotnet new aspire-servicedefaults` to create the common library
- `dotnet new aspire-apphost` to create the app host

> Make sure to use the .NET Aspire projects location is not the same directory as the games API project!

Run the API.

From 02-Start, copy the Bot project.

1. Add the Bot project to the solution
2. With the Bot project, add a project reference to the Service Defaults project
3. With the AppHost project, add a project reference to the Bot project

With the App-Host specify this app model:

```csharp
var gamesApi = builder.AddProject<Projects.GamesApi>("gamesapi");

var bot = builder.AddProject<Projects.Bot>("bot")
    .WithReference(gamesApi)
    .WaitFor(gamesApi);
```

Verify how the bot gets the HTTP address of the games API.

Start the App Host, and let the bot play some games. Use the Games API with the HTTP File or Swagger from the Games API project to see how the bot plays games. Monitor the solution with the .NET Aspire Dashboard.

![.NET Aspire Dashboard](/images/02-aspiredashboard.png)

![Distributed Tracing](/images/02-aspiretracing.png)
