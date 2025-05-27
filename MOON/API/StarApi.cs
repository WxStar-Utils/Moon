using System.Net;
using WxStarManager;
using WxStarManager.Models;

namespace Moon.API;

public static class StarApi
{
    private static Api _api;
    public static string[] Locations = { };


    public static async Task Init()
    {
        _api = new Api(Config.config.StarApiEndpoint);
        bool apiState = await _api.IsApiUp();

        if (!apiState)
        {
            if (Config.config.UseNationalLocations)
            {
                Log.Error("An error occurred while connecting to StarAPI");
                Log.Warning("National data enabled -- Ignoring connection failure.");
                Log.Warning("Stats for national data will not be available.");
                return;
            }
            
            Log.Error("An error occurred while connecting to StarAPI.");
            throw new Exception("Unable to continue initialization.");
        }

        await RegisterMoonService();
    }

    public static async Task<string[]> GetActiveLocations(WxStarModel starModel)
    {
        var locations = await _api.GetMoonLocations(starModel);
        Locations = locations.ToArray();

        return Locations;
    }

    public static async Task SendUptimeReport()
    {
        try
        {
            SystemServiceReport report = new SystemServiceReport();
            report.Online = true;
        
            await _api.ReportUptime(report, await File.ReadAllTextAsync("service-uuid"));
            
            Log.Info("Successfully reported uptime.");
            // TODO : Add generated Moon json stats as a string and push it.
        }
        catch (Exception e)
        {
            Log.Warning("Failed to report uptime.");
        }

    }

    public static async Task RegisterMoonService()
    {
        string serviceUuid = await File.ReadAllTextAsync("service-uuid");

        try
        {
            await _api.GetServiceInfo(serviceUuid);

            return;
        }
        catch (HttpRequestException e)
        {
            Log.Debug("Potentially old registration.. Re-registering.");
            File.Delete("service-uuid");
        }

        SystemServiceIn serviceInfo = new SystemServiceIn();

        if (Config.config.UseNationalLocations)
        {
            serviceInfo.Name = "MOON-NAT";
        }
        else
        {
            serviceInfo.Name = "MOON";
        }

        serviceInfo.Host = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
        serviceInfo.ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;

        string newServiceUuid = await _api.RegisterService(serviceInfo);

        await File.WriteAllTextAsync("service-uuid", newServiceUuid.Trim('"'));
        
        Log.Info($"Registered as {serviceInfo.Name} with uuid {newServiceUuid}.");
    }
}