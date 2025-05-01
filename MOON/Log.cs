using System.Drawing;
using Pastel;

namespace Moon;


internal class Log
{
    private static string PREFIX_DEBUG = " [DEBUG] ";
    private static string PREFIX_ERROR = " [ERROR] ";
    private static string PREFIX_WARNING = " [WARN] ";
    private static string PREFIX_INFO = " [INFO] ";
    
    // Colors for console logging
    private static Color COLOR_DEBUG = Color.LightSeaGreen;
    private static Color COLOR_ERROR = Color.Red;
    private static Color COLOR_WARNING = Color.Yellow;
    private static Color COLOR_INFO = Color.Azure;

    private static LogLevel LOG_LEVEL = LogLevel.Debug;
    public static string LOG_START_DATE = DateTime.Now.ToString("MMddyyyy-HH_mm_ss");

    private static string GetCurrentTime()
    {
        return DateTime.Now.ToString(@"MM/dd/yyyy HH:mm:ss");
    }

    public static void SetLogLevel(string newLevel)
    {
        newLevel = newLevel.ToLower();

        switch (newLevel)
        {
            case "debug": 
                Warning("LogLevel is set to debug. This can cause minor slowdowns when generating data.");
                LOG_LEVEL = LogLevel.Debug;
                break;
            case "info": LOG_LEVEL = LogLevel.Info; break;
            case "warning": LOG_LEVEL = LogLevel.Warning; break;
        }
    }


    public static void Debug(string str)
    {
        if (LOG_LEVEL > LogLevel.Debug) return;
        str = GetCurrentTime() + PREFIX_DEBUG + str;
        Console.WriteLine(str.Pastel(COLOR_DEBUG));
        WriteLogToFile(str);
    }
    
    // TODO: Error/Warning logs could actually be used to post information in various text chats (Discord, Matrix..)
    //       in the event that we want to start logging that sort of thing. However, it might also be a good idea to
    //       try and utilize the concept of MultiLog on CIRRUS with the rest of the server software? Downside is that
    //       this might make a single point of failure, so it could be good to make MultiLog a library instead of a 
    //       server that handles it all. ~ April
    
    public static void Error(string str)
    {
        if (LOG_LEVEL > LogLevel.Warning) return;
        str = GetCurrentTime() + PREFIX_ERROR + str;
        Console.WriteLine(str.Pastel(COLOR_ERROR));
        WriteLogToFile(str);
    }
    
    public static void Warning(string str)
    {
        if (LOG_LEVEL > LogLevel.Warning) return;
        str = GetCurrentTime() + PREFIX_WARNING + str;
        Console.WriteLine(str.Pastel(COLOR_WARNING));
        WriteLogToFile(str);
    }
    
    public static void Info(string str)
    {
        if (LOG_LEVEL > LogLevel.Info) return;
        str = GetCurrentTime() + PREFIX_INFO + str;
        Console.WriteLine(str.Pastel(COLOR_INFO));
        WriteLogToFile(str);
    }


    private static void WriteLogToFile(string str)
    {
        // Create log folder
        if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "Logs")))
        {
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Logs"));
        }
        
        string fileName = Path.Combine(AppContext.BaseDirectory, "Logs", $"{LOG_START_DATE}.log");

        using (StreamWriter sw = File.AppendText(fileName))
        {
            sw.WriteLine(str);
        }
    }

    enum LogLevel
    {
        Debug,
        Info,
        Warning,
    }
}

