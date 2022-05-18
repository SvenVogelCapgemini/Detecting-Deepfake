using System.Diagnostics;

namespace Worker_Node.Video;

public class Video
{
    public static async Task<string> GetVideo(string url, int id)
    {
        try
        {
            var filepath = $"{Environment.CurrentDirectory}\\Videos\\temp";
            var ffmpegpath = $"{Environment.CurrentDirectory}\\Videos\\ffmpeg";
            Directory.CreateDirectory(filepath);
            var pyPath = "yt-dlp";
            var arguments =
                $"-o \"{filepath}\\{id}.mp4\" -f mp4 --no-playlist --ffmpeg-location \"{ffmpegpath}\" {url}";
            var pythonProcess = new Process();
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = pyPath,
                Arguments = arguments,
                CreateNoWindow = false,
                RedirectStandardInput = true
            };

            pythonProcess.StartInfo = startInfo;
            pythonProcess.Start();

            await pythonProcess.WaitForExitAsync();
            pythonProcess.Close();


            return $"{filepath}\\{id}.mp4";
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public static async Task<bool> DeleteVideo(int id)
    {
        try
        {
            File.Delete(Environment.CurrentDirectory + $"\\videos\\temp\\{id}.mp4");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}