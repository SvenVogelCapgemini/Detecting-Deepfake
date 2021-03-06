using Microsoft.AspNetCore.SignalR.Client;
using Worker_Node.Python;
using Worker_Node.Settings;

namespace Worker_Node;

internal class SignalRConnection
{
    private static readonly object locker = new();
    private static SignalRConnection instance;
    private readonly HubConnection _connection;

    protected SignalRConnection()
    {
        // Prepare the hub connection
        var hubAddress = SettingsSetup.Instance().Setting.HubIp;
        _connection = new HubConnectionBuilder().WithUrl(hubAddress, (opts) =>
        {
            opts.HttpMessageHandlerFactory = (message) =>
            {
                if (message is HttpClientHandler clientHandler)
                    // always verify the SSL certificate as true
                    clientHandler.ServerCertificateCustomValidationCallback +=
                        (sender, certificate, chain, sslPolicyErrors) => { return true; };
                return message;
            };
        }).Build();
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

    /// <summary>
    /// Send the result to the main node
    /// </summary>
    /// <param name="id"></param>
    /// <param name="result"></param>
    public async void SendResult(int id, string result)
    {
        await _connection.InvokeAsync("ReceiveResult", id, result);
    }

    /// <summary>
    /// Send The status of the worker to the main node
    /// </summary>
    /// <param name="id"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public async Task<bool> SendStatus(int id, TaskReceived.Status status)
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
        _connection.On<int, string, string>("Task", (taskID, videoURL, algo) =>
        {
            Console.WriteLine($"id: {taskID}, URL: {videoURL}, algo: {algo}");
            var script = (PythonScripts.ScriptType) int.Parse(algo);
            Console.WriteLine("Received task");
            var task = new TaskReceived(taskID, videoURL, script);
            task.RunTask();
        });
        //When a user count is recieved
        _connection.On<int>("ReceiveUserCount", count => { Console.WriteLine($"connected users: {count} "); });
    }

    public static SignalRConnection Instance()
    {
        if (instance == null)
            lock (locker)
            {
                if (instance == null) instance = new SignalRConnection();
            }

        return instance;
    }
}