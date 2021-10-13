using PhotinoNET;
using System;
using System.Threading.Tasks;

namespace Photino.Access
{
    public static class PhotinoExtension
    {
        public static PhotinoWindow UsePhotinoFetch<T>(this PhotinoWindow window, Func<object, T, Task<PhotinoHandlerResponse>> handler)
        {
            return window.RegisterWebMessageReceivedHandler(async (object sender, string message) =>
                {

                    var window = (PhotinoWindow)sender;
                    var req = System.Text.Json.JsonSerializer.Deserialize<PhotinoRequest<T>>(message);

                    try
                    {
                        var response = await handler(sender, req.Body);

                        PhotinoResponse resp = new() { Id = req.Id, Body = response.Data, Error = response.Error };
                        window.SendWebMessage(System.Text.Json.JsonSerializer.Serialize(resp));
                    }
                    catch (System.Exception e)
                    {
                        PhotinoResponse resp = new() { Id = req.Id, Body = null, Error = e.Message };
                        window.SendWebMessage(System.Text.Json.JsonSerializer.Serialize(resp));
                    }
                });
        }
    }
}