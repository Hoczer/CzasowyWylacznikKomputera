using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace WylacznikKomputera
{
    public partial class Form1 : Form
    {
        System.Timers.Timer timer;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            dateTimePicker1.Value = DateTime.Today.AddHours(16);
        }
        delegate void UpdateLabel(Label status, string wartosc);
        void UptadeDataLabel(Label status, string wartosc)
        {
            status.Text = wartosc;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            DateTime userTime = dateTimePicker1.Value;
            TimeSpan pozostalyCzas = userTime - DateTime.Now;
            UpdateLabel update = UptadeDataLabel;
            if (StatusLabel.InvokeRequired)
            {
                Invoke(update, StatusLabel, "Odliczam... "+pozostalyCzas.Hours+":"+pozostalyCzas.Minutes+":"+pozostalyCzas.Seconds);
            }
            if (currentTime.Hour == userTime.Hour && currentTime.Minute == userTime.Minute && currentTime.Second == userTime.Second)
            {
                timer.Stop();
                try
                {
                    //UpdateLabel update = UptadeDataLabel;
                    if (StatusLabel.InvokeRequired)
                    {
                        Invoke(update, StatusLabel, "Wyłączam...");
                    }
                    WylaczKomputer();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        void WylaczKomputer()
        {
            ManagementBaseObject mboShutdown = null;
            ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
            mcWin32.Get();

            // You can't shutdown without security privileges
            mcWin32.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject mboShutdownParams =
                     mcWin32.GetMethodParameters("Win32Shutdown");

            // Flag 1 means we want to shut down the system. Use "2" to reboot.
            mboShutdownParams["Flags"] = "1";
            mboShutdownParams["Reserved"] = "0";
            foreach (ManagementObject manObj in mcWin32.GetInstances())
            {
                mboShutdown = manObj.InvokeMethod("Win32Shutdown",
                                               mboShutdownParams, null);
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            timer.Start();
            StatusLabel.Text = "Odliczam...";
            StatusLabel.ForeColor = Color.Orange;
            pictureBox1.Enabled = true;

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            timer.Stop();
            StatusLabel.Text = "Zatrzymano...";
            StatusLabel.ForeColor = Color.White;
            pictureBox1.Enabled = false;
        }
    }
}
