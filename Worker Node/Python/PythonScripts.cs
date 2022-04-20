﻿using System.Diagnostics;

namespace Worker_Node.Python;

public class PythonScripts
{
    public enum ScriptType
    {
        test,
        testAlgo
    }

    public string[] scripts;

    public PythonScripts()
    {
        scripts = new string[1];
        scripts[(int) ScriptType.test] = $"{Environment.CurrentDirectory}\\Python\\Scripts\\test.py";
    }

    public string Run(string algorithm, string videopath)
    {
        var pyPath = "C:\\Python\\python.exe";
        var arguments = string.Format("\"{0}\" {1}", scripts[0], videopath);

        var startInfo = new ProcessStartInfo
        {
            FileName = pyPath,
            Arguments = arguments,
            RedirectStandardOutput = true, // enable standard output
            UseShellExecute = false // need set to false to get output
        };

        var pythonProcess = Process.Start(startInfo);

        var output = pythonProcess.StandardOutput.ReadToEnd().Trim();
        return output;

        pythonProcess.WaitForExit();
    }
}