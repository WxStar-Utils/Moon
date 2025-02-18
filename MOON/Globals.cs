using Microsoft.Extensions.Caching.Memory;

namespace Moon;

public static class Globals
{
    /// <summary>
    /// Flag used to determine whether or not to perform data collection on startup
    /// </summary>
    public static bool FreshStart = true;

    /// <summary>
    /// Flag for when the server has been stopped. Turned to true to properly dispose of everything and close opened
    /// files.
    /// </summary>
    public static bool GracefulShutdown = false;
    
    /// <summary>
    /// Memory cache for storing LFRecord objects
    /// </summary>
    public static IMemoryCache LocationCache = new MemoryCache(new MemoryCacheOptions());
    /// <summary>
    /// Memory cache for storing AlertHeadline IDs and their UTC expiration times
    /// </summary>
    public static IMemoryCache AlertsCache = new MemoryCache(new MemoryCacheOptions());
    /// <summary>
    /// List of the alert detail keys for the AlertsCache, primarily used for clearing
    /// old alerts.
    /// </summary>
    public static List<string> AlertDetailKeys = new();
}