using Worker_Node.Python;

namespace Worker_Node;

internal class TaskRecieved
{
    private readonly string _algorithm;
    private readonly string _taskId;
    private readonly string _videoURL;
    private Status _status;

    public TaskRecieved(string Id, string videoURL, string algo)
    {
        _taskId = Id;
        _status = Status.Recieved;
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
            await download;
            file = download.Result;
        }
        // If download failed
        catch (Exception)
        {
            _status = Status.Failed;
            return false;
        }

        try
        {
            // run algorime on file
            var pythonScripts = new PythonScripts();
            _status = Status.CheckingForDeepfake;
            var result = pythonScripts.Run(_algorithm, file);
            _status = Status.Done;
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
                Console.WriteLine("video Deleted");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        return madeit;
        
    }

    private enum Status
    {
        Recieved,
        Downloading,
        CheckingForDeepfake,
        Done,
        Failed
    }
}