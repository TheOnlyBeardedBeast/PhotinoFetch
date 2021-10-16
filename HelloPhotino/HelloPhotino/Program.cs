using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Photino.Fetch;
using PhotinoNET;
using System;
using System.Drawing;

namespace HelloPhotinoApp
{
    public class Query
    {
        public string Hello() => "World!";
    }

    public interface IDataResult : IExecutionResult
    {
        public object Data { get; }
    }

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
            .BuildServiceProvider();

            var schema = SchemaBuilder.New()
                                      .AddServices(serviceProvider)
                                      .AddQueryType<Query>()
                                      .Create();

            var executor = schema.MakeExecutable();

            // Window title declared here for visibility
            string windowTitle = "Photino for .NET Demo App";

            // Creating a new PhotinoWindow instance with the fluent API
            var window = new PhotinoWindow()
                .SetTitle(windowTitle)
                .SetDevToolsEnabled(true)
                // .SetChromeless(true)
                .SetUseOsDefaultSize(false)
                .SetSize(new Size(600, 400))
                .Center()
                .SetResizable(true)
                .UsePhotinoFetch(async (object sender, PhotinoGraphqlBody data) =>
                {
                    var response = await executor.ExecuteAsync(data.Query, data.Variables);

                    return new PhotinoHandlerResponse { Data = (response as IReadOnlyQueryResult).Data, Error = null };
                })
                 .Load(new Uri("http://localhost:3000/")); // for development
                 // .Load("wwwroot/index.html"); // Can be used with relative path strings or "new URI()" instance to load a website.

            window.WaitForClose(); // Starts the application event loop
        }
    }
}
