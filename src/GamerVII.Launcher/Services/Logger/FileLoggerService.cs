using System;
using System.IO;

namespace GamerVII.Launcher.Services.Logger;

public class FileLoggerService : ILoggerService
{
    private readonly string _filePath = $"logs/launcher {DateTime.Now:yyyy-mm-dd hh-mm-ss}.log";
    private readonly StreamWriter fileWriter;

    public FileLoggerService()
    {

        // var logFile = new FileInfo(_filePath);
        //
        // if (!logFile.Directory.Exists)
        // {
        //     logFile.Directory.Create();
        // }
        //
        // fileWriter = new StreamWriter(_filePath, true)
        // {
        //     AutoFlush = true
        // };
    }

    public void Log(string message)
    {
        // fileWriter.WriteLine($"[{DateTime.Now:dd.MM.yyyy HH:mm:ss / zz}] {message}");
    }

    public void Log(string message, Exception exception)
    {
        Console.WriteLine(message);
    }

    public void Dispose()
    {
        // fileWriter.Close();
    }
}
