using Microsoft.AspNetCore.SignalR.Client;
using Worker_Node.Python;

namespace Worker_Node;

internal class SignalRConnection
{
    private readonly HubConnection _connection;

    private SignalRConnection()
    {
        // Prepare the hub connection
        var hubAddress = "https://localhost:7062/taskHub";
        _connection = new HubConnectionBuilder().WithUrl(hubAddress).Build();
        // When connection closed wait then reconnect
        _connection.Closed += async error =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await _connection.StartAsync();
        };
        CallBacks();
        // Connect to the HUB
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
        await _connection.InvokeAsync("ReceiveResult", id, result);
    }

    public async Task<bool> SendStatus(string id, TaskReceived.Status status)
    {
        switch (status)
        {
            case TaskReceived.Status.Received:
                await _connection.InvokeAsync("ReceiveStatus", id, "Recieved");
                break;
            case TaskReceived.Status.Downloading:
                await _connection.InvokeAsync("ReceiveStatus", id, "Downloading Video");
                break;
            case TaskReceived.Status.Failed:
                await _connection.InvokeAsync("ReceiveStatus", id, "Failed");
                break;
            case TaskReceived.Status.Done:
                await _connection.InvokeAsync("ReceiveStatus", id, "Done");
                break;
            case TaskReceived.Status.CheckingForDeepfake:
                await _connection.InvokeAsync("ReceiveStatus", id, "Running Algorithm");
                break;
        }
        return true;
        
    }

    public async void SendAlgorithms(int[] indexes, string description)
    {
        await _connection.InvokeAsync("ReceiveAlgorithms", indexes, description);
    }

    private void CallBacks()
    {
        // When ReceiveMessage is received write the message
        _connection.On<string, string, string>("Task", (taskID, videoURL, algo) =>
        {
            Console.WriteLine($"id: {taskID}, URL: {videoURL}, algo: {algo}");
            PythonScripts.ScriptType script = (PythonScripts.ScriptType)int.Parse(algo);
            Console.WriteLine("Received task");
            var task = new TaskReceived(taskID, videoURL, script);
            task.RunTask();
        });

        _connection.On<int>("ReceiveUserCount", (count) => { Console.WriteLine($"connected users: {count} "); });

        // TODO: Send the algorithms back
        _connection.On<string>("GetAlgorithms", (message) => { Console.WriteLine("SendThemAlgo"); });
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