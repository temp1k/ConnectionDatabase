using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DataSet_WPF_DB_App
{
    /// <summary>
    /// Логика взаимодействия для MonitoringPC.xaml
    /// </summary>
    public partial class MonitoringPC : Window
    {
        public MonitoringPC()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Событие загрузки окна MonitoringPC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetValue();
        }

        /// <summary>
        /// Получение и вывод значений аппаратной части ПК
        /// </summary>
        private async void GetValue()
        {
            var CpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            var MemCounter = new PerformanceCounter("Memory", "% Committed Bytes in Use");
            string cardname = new PerformanceCounterCategory("Network Interface").GetInstanceNames()[0];
            string hdddisk = new PerformanceCounterCategory("PhysicalDisk").GetInstanceNames()[0];
            CpuCounter.NextValue();
            PerformanceCounter dataSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", cardname);
            PerformanceCounter dataReceivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", cardname);
            PerformanceCounter bandwidthCounter = new PerformanceCounter("Network Interface", "Current Bandwidth", cardname);
            PerformanceCounter DriveCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", hdddisk);
            float bandwidth = bandwidthCounter.NextValue();//valor fixo 10Mb/100Mn/
            float sendSum = 0;
            float receiveSum = 0;
            float drivecounter = 0;
            while (true)
            {
                drivecounter = DriveCounter.NextValue();
                sendSum = dataSentCounter.NextValue();
                receiveSum = dataReceivedCounter.NextValue();
                int valCPU = (int)CpuCounter.NextValue();
                int valMem = (int)MemCounter.NextValue();
                //netPerc.Value = Convert.ToDouble((8 * (sendSum + receiveSum) / bandwidth) * 100);
                cpuMonitor.Value = valCPU;
                ramMonitor.Value = 1.05 * valMem;
                ssdMonitor.Value = drivecounter;
                cpuPerc.Text = cpuMonitor.Value.ToString() + "%";
                ramPerc.Text = ramMonitor.Value.ToString() + "%";
                ssdPerc.Text = ssdMonitor.Value.ToString() + "%";
                await Task.Delay(3000);
            }
        }

        /// <summary>
        /// Закрытие окна MonitoringPC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
