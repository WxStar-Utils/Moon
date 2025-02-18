using Moon.MQTT;

namespace Moon.Tasks;

public partial class TimedTasks
{
    /// <summary>
    /// Publishes uptime notifications to the MQTT monitoring channels every 120 seconds.
    /// </summary>
    public static async Task RefreshUptime()
    {
        while (true)
        {
            await MqttDistributor.PublishUptime();
            await Task.Delay(120 * 1000);
        }
    }
}