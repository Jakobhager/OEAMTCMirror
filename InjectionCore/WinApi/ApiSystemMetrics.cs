using System.Runtime.InteropServices;

namespace InjectionCore.WinApi
{
    internal class ApiSystemMetrics
    {
        public static int Get(SystemMetric smIndex)
        {
            return GetSystemMetrics(smIndex);
        }

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(SystemMetric smIndex);
    }
}