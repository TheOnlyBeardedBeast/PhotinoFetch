using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Photino.Access;
using PhotinoNET;
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace HelloPhotinoApp
{
    public class Query
    {
        public string Hello() => "World!";
    }

    public interface IDataResult : IExecutionResult {
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
                //.SetChromeless(true)
                // Resize to a percentage of the main monitor work area
                .SetUseOsDefaultSize(false)
                .SetSize(new Size(600, 400))
                // Center window in the middle of the screen
                .Center()
                // Users can resize windows by default.
                // Let's make this one fixed instead.
                .SetResizable(true)
                //.RegisterCustomSchemeHandler("app", (object sender, string scheme, string url, out string contentType) =>
                //{
                //    contentType = "text/javascript";
                //    return new MemoryStream(Encoding.UTF8.GetBytes(@"
                //        (() =>{
                //            window.setTimeout(() => {
                //                alert(`🎉 Dynamically inserted JavaScript.`);
                //            }, 1000);
                //        })();
                //    "));
                //})
                // Most event handlers can be registered after the
                // PhotinoWindow was instantiated by calling a registration 
                // method like the following RegisterWebMessageReceivedHandler.
                // This could be added in the PhotinoWindowOptions if preferred.
                .RegisterWebMessageReceivedHandler(async (object sender, string message) =>
                {
                    var window = (PhotinoWindow)sender;

                    var req = System.Text.Json.JsonSerializer.Deserialize<PhotinoGraphqlRequest>(message);

                    // The message argument is coming in from sendMessage.
                    // "window.external.sendMessage(message: string)"
                    var response = await executor.ExecuteAsync(req.Body.Query,req.Body.Variables);

                    PhotinoResponse resp = new() { Id = req.Id, Body = (response as IReadOnlyQueryResult).Data };

                    // Send a message back the to JavaScript event handler.
                    // "window.external.receiveMessage(callback: Function)"
                    window.SendWebMessage(System.Text.Json.JsonSerializer.Serialize(resp));
                })
                 .Load(new Uri("http://localhost:3000/"));
                //.Load("wwwroot/index.html"); // Can be used with relative path strings or "new URI()" instance to load a website.

            window.WaitForClose(); // Starts the application event loop
        }
    }
}
