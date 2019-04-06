using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Launchpad;

namespace LaunchpadVisualizer
{
    public partial class Form1 : Form
    {
        private IReadOnlyList<MidiDevice> devices = null;
        public LaunchpadDevice connectingDevice = null;
        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FormClosing += (object a, FormClosingEventArgs b) => connectingDevice?.Stop();
            RefreshDeviceList();
        }
        
        /// <summary>
        /// Connects specific launchpad or auto connect or only refresh list
        /// </summary>
        /// <param name="id">Midi device Id. leave empty to only refresh device list</param>
        private void ConnectLaunchpad(string id = "")
        {
            if(id == "" || devices == null)
            {
                devices = Launchpad.Engines.Winmm.WinmmMidiDevices.GetLaunchpads();
            }
            if(devices.Count == 0)
            {
                MessageBox.Show("Launchpad not found. Please try again", "Error", MessageBoxButtons.OK);
                return;
            }
            // disconnect if necessary
            if(connectingDevice != null && connectingDevice.Id != id && id != "")
            {
                connectingDevice.Stop();
                connectingDevice = null;
            }
            // if device not yet connected for whatever reason
            if(connectingDevice == null)
            {
                if(id == "")
                {
                    connectingDevice = new LaunchpadDevice(devices[devices.Count - 1]);
                }
                else
                {
                    foreach (var device in devices)
                    {
                        if (device.Id == id)
                        {
                            connectingDevice = new LaunchpadDevice(device);
                            break;
                        }
                    }
                }
                if(connectingDevice != null)
                {
                    connectingDevice.Start(140,0);
                    
                }
                else
                {
                    MessageBox.Show("Selected device not found", "Error", MessageBoxButtons.OK);
                }
            }
        }

        private void buttonRefreshDevice_Click(object sender, EventArgs e)
        {
            RefreshDeviceList();
        }

        private void RefreshDeviceList()
        {
            comboDeviceList.Text = "";
            comboDeviceList.Items.Clear();
            ConnectLaunchpad();
            foreach (var device in devices)
            {
                comboDeviceList.Items.Add(new ComboBoxItem(device.Name, device.Id));
            }
            if (connectingDevice != null)
            {
                comboDeviceList.Text = connectingDevice.Id;
            }
        }


        private void comboDeviceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConnectLaunchpad((string)((ComboBoxItem)comboDeviceList.SelectedItem).Value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connectingDevice.SetFlash(4, 4, 5, 18);
        }
    }

    public class ComboBoxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }
        public ComboBoxItem(string text, object value)
        {
            Text = text;
            Value = value;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
