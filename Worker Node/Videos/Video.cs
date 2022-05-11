using System.Diagnostics;

namespace Worker_Node;

internal class Video
{
    public async Task<string> GetVideo(string url, string id)
    {
        try
        {
            var filepath = $"{Environment.CurrentDirectory}\\Videos\\temp";
            Directory.CreateDirectory(filepath);
            var pyPath = "youtube-dl";
            var arguments = $"-o \"{filepath}\\{id}.mp4\" --ffmpeg-location C:\\Users\\svenv\\Desktop\\ffmpeg-2022-05-08-git-f77ac5131c-essentials_build\\bin {url}";
            var pythonProcess = new Process();
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = pyPath,
                Arguments = arguments,
                CreateNoWindow = false,
                RedirectStandardInput = true,
            };
            await Task.Run(async () =>
            {
                
                pythonProcess.StartInfo = startInfo;
                pythonProcess.Start();
                int i = 0;
                while (!pythonProcess.HasExited)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(i);
                    Console.WriteLine(pythonProcess.HasExited);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    if (i > 10)
                        pythonProcess.Close();
                    await Task.Delay(1000);
                    i++;
                }

                await pythonProcess.WaitForExitAsync();
                pythonProcess.Close();

            });
            
            
            return $"{filepath}\\{id}.mp4";
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<bool> DeleteVideo(string id)
    {
        try
        {
            //File.Delete(Environment.CurrentDirectory + $"\\videos\\temp\\{id}.mp4");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}