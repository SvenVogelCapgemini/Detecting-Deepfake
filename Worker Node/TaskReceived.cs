using Worker_Node.Python;

namespace Worker_Node;

internal class TaskReceived
{
    private readonly PythonScripts.ScriptType _algorithm;
    private readonly string _taskId;
    private readonly string _videoURL;
    private Status _status;

    public TaskReceived(string Id, string videoURL, PythonScripts.ScriptType algo)
    {
        _taskId = Id;
        _status = Status.Received;
        SignalRConnection.Instance.SendStatus(_taskId, _status);
        _videoURL = videoURL;
        _algorithm = algo;
    }

    public async Task<bool> RunTask()
    {
        bool madeit = false;
        // Download the video
        var video = new Video();
        var file = "";
        try
        {
            var download = video.GetVideo(_videoURL, _taskId);
            _status = Status.Downloading;
            SignalRConnection.Instance.SendStatus(_taskId, _status);
            await download;
            file = download.Result;
        }
        // If download failed
        catch (Exception)
        {
            _status = Status.Failed;
            SignalRConnection.Instance.SendStatus(_taskId, _status);
            return false;
        }

        try
        {
            // run algorime on file
            var pythonScripts = new PythonScripts();
            _status = Status.CheckingForDeepfake;
            SignalRConnection.Instance.SendStatus(_taskId, _status);

            var result = pythonScripts.Run(_algorithm, file);
            _status = Status.Done;
            SignalRConnection.Instance.SendStatus(_taskId, _status);

            Console.WriteLine("done");
            //TODO: find a way to send the result back
            //SignalRConnection.Instance.SendResult(_taskId, result);
            Console.WriteLine(result);
            madeit = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            try
            {
                await video.DeleteVideo(_taskId);
                Console.WriteLine("Video Deleted");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        return madeit;
        
    }

    public enum Status
    {
        Received,
        Downloading,
        CheckingForDeepfake,
        Done,
        Failed
    }
}