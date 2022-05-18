using Worker_Node.Python;

namespace Worker_Node;

internal class TaskReceived
{
    public enum Status
    {
        Received,
        Downloading,
        CheckingForDeepfake,
        Done,
        Failed
    }

    private readonly PythonScripts.ScriptType _algorithm;
    private readonly int _taskId;
    private readonly string _videoURL;
    private Status _status;

    public TaskReceived(int Id, string videoURL, PythonScripts.ScriptType algo)
    {
        _taskId = Id;
        _status = Status.Received;
        SignalRConnection.Instance().SendStatus(_taskId, _status);
        _videoURL = videoURL;
        _algorithm = algo;
    }

    public async Task<bool> RunTask()
    {
        var madeit = false;
        // Download the video
        var file = "";
        try
        {
            var download = Video.Video.GetVideo(_videoURL, _taskId);
            _status = Status.Downloading;
            SignalRConnection.Instance().SendStatus(_taskId, _status);
            await download;
            file = download.Result;
        }
        // If download failed
        catch (Exception)
        {
            _status = Status.Failed;
            SignalRConnection.Instance().SendStatus(_taskId, _status);
            return false;
        }

        try
        {
            // run algorime on file
            var pythonScripts = new PythonScripts();
            _status = Status.CheckingForDeepfake;
            SignalRConnection.Instance().SendStatus(_taskId, _status);

            var result = pythonScripts.Run(_algorithm, file);
            _status = Status.Done;
            SignalRConnection.Instance().SendStatus(_taskId, _status);
            SignalRConnection.Instance().SendResult(_taskId, result);


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
                await Video.Video.DeleteVideo(_taskId);
                Console.WriteLine("Video Deleted");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return madeit;
    }
}