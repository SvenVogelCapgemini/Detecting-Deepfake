using System.Diagnostics;

namespace Worker_Node.Python;

public class PythonScripts
{
    public enum ScriptType
    {
        Test,
        AlwaysFalse,
        AlwaysTrue,
        XceptionNet
    }

    public List<string> Scripts;

    public PythonScripts()
    {
        // store the location of the scripts
        Scripts = new List<string>
        {
            $"{Environment.CurrentDirectory}\\Python\\Scripts\\test.py",
            $"{Environment.CurrentDirectory}\\Python\\Scripts\\false.py",
            $"{Environment.CurrentDirectory}\\Python\\Scripts\\true.py",
            $"{Environment.CurrentDirectory}\\Python\\Scripts\\XceptionNet\\Start.py"
        };
    }

    /// <summary>
    /// Runs a python script with the videopath as the first argument.
    /// </summary>
    /// <param name="algorithm"></param>
    /// <param name="videopath"></param>
    /// <returns></returns>
    public string Run(ScriptType algorithm, string videopath)
    {
        Console.WriteLine(algorithm);
        Console.WriteLine(videopath);
        var pyPath = "python";
        var arguments = $"\"{Scripts[(int) algorithm]}\" \"{videopath}\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = pyPath,
            Arguments = arguments,
            RedirectStandardOutput = true, // enable standard output
            UseShellExecute = false // need set to false to get output
        };

        var pythonProcess = Process.Start(startInfo);

        var output = pythonProcess.StandardOutput.ReadToEnd().Trim();
        pythonProcess.WaitForExit();
        Console.WriteLine(output);
        return output;

    }
}