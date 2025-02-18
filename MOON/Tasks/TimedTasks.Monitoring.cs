using Moon.MQTT;

namespace Moon.Tasks;

public partial class TimedTasks
{
    /// <summary>
    /// Publishes uptime notifications to the MQTT monitoring channels.
    /// </summary>
    public static async Task RefreshUptime()
    {
        while (!Globals.GracefulShutdown)
        {
            MqttDistributor.PublishUptime();
            await Task.Delay(120 * 1000);
        }
    }
}