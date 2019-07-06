using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace AppKiller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll")]
        public static extern int FindWindow(string ClassName, string WindowName);
        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\AppKiller.txt";
        public string GetAllApps()
        {
            string allApp="";
            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "")
                    { }
                    else if (line != "")
                    { allApp += line + System.Environment.NewLine ; }
                }
            }
            return allApp;
        }

        public void UpdateListBox()
        {
            listBox1.Items.Clear();
            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "")
                    { }
                    else if (line != "")
                    { listBox1.Items.Add(line); }
                }
            }
        }

        private void searchApp_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                    listBox1.Items.Add(p.ProcessName);
            }
        }

        private void showApps_Click(object sender, EventArgs e)
        {
            UpdateListBox();
        }

        private void deleteApp_Click(object sender, EventArgs e)
        {
            listBox1.Items.Remove(listBox1.SelectedItem);
            string text = "";
            foreach (var item in listBox1.Items)
            {
                text += item.ToString() +Environment.NewLine;
            }
            File.WriteAllText(path, text);
        }

        private void selectApp_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") { 
            string selectedAppName = listBox1.SelectedItem.ToString();
            string allApp = GetAllApps();
            File.WriteAllText(path, allApp + selectedAppName);
                //UpdateListBox();
            }
            else
            {
                string selectedAppName = textBox1.Text;
                string allApp = GetAllApps();
                File.WriteAllText(path, allApp + selectedAppName);
            }
            textBox1.Clear();
            UpdateListBox();
        }
        private void ServiceKillerFunc(string serviceName)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);
                service.Stop();
                var timeout = new TimeSpan(0, 0, 5); // 5seconds
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

            }
            catch (Exception exc)
            {
                // MessageBox.Show("k"+exc.Message);
            }
        }
        private void AppKillerFunc(string appName)
        {
            try
            {
                foreach (System.Diagnostics.Process myProc in System.Diagnostics.Process.GetProcesses())
                {
                    if (myProc.ProcessName == appName)
                    {
                        myProc.Kill();
                    }
                }
            }
            catch(Exception exc)
            {
                // MessageBox.Show("q"+exc.Message);
            }

        }

        private void killApp_Click(object sender, EventArgs e)
        {
            try
            {
                int HWND = FindWindow(null, "My Window");//window title

                SendMessage(HWND, WM_SYSCOMMAND, SC_CLOSE, 0);
                if (checkBox1.Checked == false)
                {
                    string selectedAppName = listBox1.SelectedItem.ToString();
                    AppKillerFunc(selectedAppName);
                    ServiceKillerFunc(selectedAppName);
                }
                else
                {
                    for (int i = 0; i <= (listBox1.Items.Count-1); i++)
                    {
                            string selectedAppName = listBox1.Items[i].ToString();
                            AppKillerFunc(selectedAppName);
                            ServiceKillerFunc(selectedAppName);
                    }
                }
                
            }
            catch (Exception exc)
            {
                //MessageBox.Show("Hata:" + exc.Message);
            }

            UpdateListBox();
        }
       

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
        private void notifyIcon1_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
          
        }

        private void killToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = true;
            killApp_Click(null, null);
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon1.BalloonTipTitle = "Info";
            notifyIcon1.BalloonTipText = "Double click to maximize.";
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Visible = true;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            UpdateListBox();
           this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            ServiceController[] oaServiceController = ServiceController.GetServices(Environment.MachineName);
            foreach (ServiceController sc in oaServiceController)
                listBox1.Items.Add(sc.DisplayName);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            notifyIcon1.BalloonTipTitle = "Info";
            notifyIcon1.BalloonTipText = "Double click to maximize.";
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Visible = true;
            listBox1.Items.Clear();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
            listBox1.Items.Clear();
        }
        
    }
}
