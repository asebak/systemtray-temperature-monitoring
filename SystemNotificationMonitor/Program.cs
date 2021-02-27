using System;
using System.Media;
using System.Windows.Forms;
using System.Management;
using System.Diagnostics;
using NvAPIWrapper;
using NvAPIWrapper.Display;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Mosaic;
using System.Linq;

namespace SystemNotificationMonitor
{
    class Program
    {
        private static NotifyIcon _notify;

        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to exit");
            SetIcon();



            //Console.WriteLine("CPU temp : " + CPUtprt.ToString() + " °C");


            SystemSounds.Asterisk.Play();

            _notify.MouseMove += _notify_MouseMove;

            Application.Run();
            Console.ReadLine();
        }

        private static void _notify_MouseMove(object sender, MouseEventArgs e)
        {
            var cpu = GetCpuTemperature();
            var gpu = GetGpuTemperature();
            _notify.Text = $"GPU Temperature: {gpu} CPU Temperature: {cpu}";
        }

        private static void SetIcon()
        {
            _notify = new NotifyIcon();
            _notify.Icon = new System.Drawing.Icon("./ico.ico");
            _notify.Click += (s, e) => { };
            _notify.Visible = true;
        }

        private static double GetCpuTemperature()
        {
            double CPUtprt = 0;
            ManagementObjectSearcher mos = new ManagementObjectSearcher(@"root\WMI", "Select * From MSAcpi_ThermalZoneTemperature");
            foreach (ManagementObject mo in mos.Get())
            {
                CPUtprt = Convert.ToDouble(Convert.ToDouble(mo.GetPropertyValue("CurrentTemperature").ToString()) - 2732) / 10;
            }
            return CPUtprt;
        }

        private static int GetGpuTemperature()
        {
            int gpuTemp = 0;

            var gpu = PhysicalGPU.GetPhysicalGPUs().FirstOrDefault();
            if (gpu != null)
            {
                gpuTemp = gpu.ThermalInformation.ThermalSensors.Select(x => x.CurrentTemperature).FirstOrDefault();
            }
            return gpuTemp;
        }
    }
}
