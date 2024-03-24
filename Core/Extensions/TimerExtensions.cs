using Timer = System.Timers.Timer;

namespace Core.Extensions
{
    public static class TimerExtensions
    {
        public static void Restart(this Timer timer)
        {
            timer.Stop();
            timer.Start();
        }
    }
}
