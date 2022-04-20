using Microsoft.AspNetCore.SignalR.Client;
using Worker_Node.Python;

namespace Worker_Node;

internal class SignalRConnection
{
    private readonly HubConnection _connection;

    private SignalRConnection()
    {
        //prepare the hub connection
        var hubAddress = "https://localhost:7062/taskHub";
        _connection = new HubConnectionBuilder().WithUrl(hubAddress).Build();
        _connection.Closed += async error =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await _connection.StartAsync();
        };
        CallBacks();
        //connect to the HUB
        Task.Run(async () =>
        {
            try
            {
                await _connection.StartAsync();
                Console.WriteLine("Connected");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        });
    }

    public static SignalRConnection Instance => Nested.instance;

    public async void SendResult(string id, string result)
    {
        await _connection.InvokeAsync("Result", id, result);
    }

    private void CallBacks()
    {
        //when connection closed wait then reconnect
        

        //when RecieveMessgage is recieved write the message
        _connection.On<string, string, string>("Task", (taskID, videoURL, algo) =>
        {
            Console.WriteLine("recieved task");
            var task = new TaskRecieved(taskID, videoURL, algo);
            task.RunTask();
        });

        _connection.On<int>("ReceiveUserCount", (count) => { Console.WriteLine($"connected users: {count} "); });

        _connection.On<string, string>("ReceiveMessage", (user, message) => { Console.WriteLine(message); });
    }

    private enum MessageMethode
    {
        Result
    }

    private class Nested
    {
        internal static readonly SignalRConnection instance = new();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Nested()
        {
        }
    }
}