using System.Diagnostics;

namespace Worker_Node.Python;

public class PythonScripts
{
    public enum ScriptType
    {
        test,
        alwaysFalse,
        alwaysTrue
    }

    public string[] scripts;

    public PythonScripts()
    {
        scripts = new string[3];
        scripts[(int) ScriptType.test] = $"{Environment.CurrentDirectory}\\Python\\Scripts\\test.py";
        scripts[(int)ScriptType.alwaysFalse] = $"{Environment.CurrentDirectory}\\Python\\Scripts\\false.py";
        scripts[(int)ScriptType.alwaysTrue] = $"{Environment.CurrentDirectory}\\Python\\Scripts\\true.py";

    }

    public string Run(ScriptType algorithm, string videopath)
    {
        Console.WriteLine(algorithm);
        Console.WriteLine(videopath);
        var pyPath = "python";
        var arguments = $"\"{scripts[(int) algorithm]}\" {videopath}";

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