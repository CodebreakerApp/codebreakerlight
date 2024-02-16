# Part 2 - Add .NET Aspire

## Try out .NET Aspire

Create an .NET Aspire sample solution:

1. dotnet new aspire-starter -o AspireSample
2. Check the different projects created
3. Check the AppHost project: csproj file, how the projects depend
4. Check Program.cs how the projects connect
5. Check the client application how it retrieves the connection to the API
6. Check the common library - how are the settings from this library applied to the web application and API service?
7. Run the application, check the dashboard - resources, logging, structured logging, metrics, distributed tracing

## Add .NET Aspire to the Codebreaker solution

1. Add .NET Aspire support to the games API
2. Add the Bot project to the solution
3. Integrate the Bot project with the .NET Aspire host
4. Configure the Bot project to retrieve the address from the API service with Aspira orchestration
5. Run the application locally and play games using the bot service
 