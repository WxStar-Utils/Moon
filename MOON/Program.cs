using System.Data.SQLite;
using System.Xml.Serialization;
using Moon;
using Moon.API;
using Moon.API.Products;
using Moon.MQTT;
using Moon.RecordGeneration;
using Moon.Schema.ibm;
using Moon.Schema.System;
using Moon.Tasks;

public class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("\r\n\r\n888b     d888  .d88888b.   .d88888b.  888b    888 \r\n8888b   d8888 d88P\" \"Y88b d88P\" \"Y88b 8888b   888 \r\n88888b.d88888 888     888 888     888 88888b  888 \r\n888Y88888P888 888     888 888     888 888Y88b 888 \r\n888 Y888P 888 888     888 888     888 888 Y88b888 \r\n888  Y8P  888 888     888 888     888 888  Y88888 \r\n888   \"   888 Y88b. .d88P Y88b. .d88P 888   Y8888 \r\n888       888  \"Y88888P\"   \"Y88888P\"  888    Y888 \r\n                                                  \r\n                                                  \r\n                                                  \r\n\r\n");
        
        Console.WriteLine("(C) Overcast Systems");
        Console.WriteLine("Developed by April P. with <3");
        Console.WriteLine("Weather information collected from The National Weather Service & The Weather Company");
        Console.WriteLine("--------------------------------------------------------------------------------------");
        Log.Info("Initializing..");
        
        Config config = Config.Load();
        
        if (args.Length > 0)
        {
            Log.Debug($"Arguments passed: {String.Join(",", args)}");
            
            foreach (string arg in args)
            {
                if (arg == "--national" || arg == "-n")
                    Config.config.UseNationalLocations = true;

                if (arg == "--config" || arg == "-c")
                {
                    var index = Array.IndexOf(args, arg);
                    Config.ConfigPath = args[index + 1];

                    config = Config.Load();
                }
                
                if (arg == "--log-level" || arg == "-l")
                {
                    var index = Array.IndexOf(args, arg);

                    Config.config.LogLevel = args[index + 1];
                }
            }
        }

        Mist.InitWxnetApi();
        MqttDistributor.Connect();

        await Task.Delay(1 * 1000);
        if (!MqttDistributor.Client.IsConnected)
        {
            Log.Error("Failed to connect to the MQTT Broker.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return;
        }
        
        Log.SetLogLevel(config.LogLevel);

        string[] locations;
        
        if (config.UseNationalLocations)
        {
            Log.Warning("Collecting data for national locations can take a while.");
            locations = new string[]
            {
                "USNM0004", "USGA0028", "USMD0018", "USME0017", "USMT0031", "USAL0054", "USND0037", "USID0025",
                "USMA0046", "USNY0081", "USVT0033", "USNC0121", "USIL0225", "USOH0188", "USOH0195", "USTX0327",
                "USCO0105", "USIA0231", "USMI0229", "USAZ0068", "USSC0140", "USCT0094", "USTX0617", "USIN0305",
                "USFL0228", "USMO0460", "USNV0049", "USAR0337", "USCA0638", "USKY1096", "USTN0325", "USFL0316",
                "USWI0455", "USMN0503", "USTN0357", "USLA0338", "USNY0996", "USNJ0355", "USVA0557", "USOK0400",
                "USNE0363", "USFL0372", "USPA1276", "USAZ0166", "USPA1290", "USME0328", "USOR0275", "USNC0558",
                "USSD0283", "USNV0076", "USCA0967", "USUT0225", "USTX1200", "USCA0982", "USCA0987", "USWA0395",
                "USWA0422", "USMO0787", "USFL0481", "USOK0537", "USDC0001"
            };
        }
        else
        {
            await Mist.GetActiveLocations();
            locations = Mist.Locations;
        }
        
        Log.Info("Initialization complete!");

        // Data generation tasks
        Task checkAlerts = TimedTasks.CheckForAlerts(locations, config.CheckAlertTimeSeconds);
        Task recordGenTask = TimedTasks.HourlyRecordGenTask(locations);
        Task clearAlertsCache = TimedTasks.ClearExpiredAlerts();
        
        // Mist API tasks
        Task refreshLocations = TimedTasks.UpdateLocations();
        Task sendUptimeNotifications = TimedTasks.SendUptimeNotifications();
        
        
        await Task.WhenAll(
            checkAlerts, 
            recordGenTask, 
            clearAlertsCache,
            refreshLocations,
            sendUptimeNotifications);

    }
}