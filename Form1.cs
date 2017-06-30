using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace sterowanie_arduino
{
    public partial class Form1 : Form
    {
        private StringBuilder command;
        private SerialPort ArduinoPort;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            On_disconnect(); 

            command = new StringBuilder();
            command.Append("#00000000000000#");

            ArduinoPort = new SerialPort();
        }

        private void On_disconnect()
        {
            BoxName.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            BoxName.Items.AddRange(ports);

            BoxControl.Enabled = false;
            ButPolaczenie.Text = "CONNECT";
            BoxName.Enabled = true;
            BoxSpeed.Enabled = true;
            StatusLabel.ForeColor = Color.Red;
            StatusLabel.Text = "NOT CONNECTED WITH ARDUINO!";
            TimerRead.Stop();
        }

        private void On_connect()
        {
            BoxControl.Enabled = true;
            ButPolaczenie.Text = "DISCONNECT";
            BoxName.Enabled = false;
            BoxSpeed.Enabled = false;
            StatusLabel.ForeColor = Color.Green;
            StatusLabel.Text = "CONNECTED WITH ARDUINO - " + ArduinoPort.PortName;

            TimerRead.Interval = 50;
            TimerRead.Start();
        }

        private void ButPolaczenie_Click(object sender, EventArgs e)
        {
            if (ButPolaczenie.Text == "CONNECT")
            {
                if (BoxSpeed.Text != "" && BoxName.Text != "")
                {
                    try
                    {
                        ArduinoPort.PortName = BoxName.Text;
                        ArduinoPort.BaudRate = Convert.ToInt32(BoxSpeed.Text);
                        ArduinoPort.DtrEnable = true;
                        ArduinoPort.Open();
                        On_connect();

                        ArduinoPort.Write(command.ToString());
                    
                    }
                    catch(Exception exp1)
                    {
                        MessageBox.Show(exp1.Message);
                    }
                }
            }
            else if(ButPolaczenie.Text == "DISCONNECT")
            {
                if(ArduinoPort.IsOpen)
                {
                    ArduinoPort.Close();
                    On_disconnect();
                }
            }
        }

        private void Change_state(int i, Button now_button, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                if(command[i] == '0' || command[i] == '1')
                {
                    command.Remove(i, 1);
                    command.Insert(i, 'I');
                    now_button.BackColor = Color.Blue;
                }
                else if(command[i] == 'I')
                {
                    command.Remove(i, 1);
                    command.Insert(i, '0');
                    now_button.BackColor = Color.Red;
                }
            }
            else if(e.Button == MouseButtons.Left && command[i] != 'I') 
            {
                if (command[i] == '1')
                {
                    command.Remove(i, 1);
                    command.Insert(i, '0');
                    now_button.BackColor = Color.Red;
                }
                else if(command[i] == '0')
                {
                    command.Remove(i, 1);
                    command.Insert(i, '1');
                    now_button.BackColor = Color.Green;
                }
            }

            ArduinoPort.Write(command.ToString());
        }

        private void buttonMouseUp(object sender, MouseEventArgs e)
        {
            Button now_button = sender as Button;

            int number = Convert.ToInt32(now_button.Tag);

            Change_state(number + 1, now_button, e);
        }

        private void TimerRead_Tick(object sender, EventArgs e)
        {
            if (!ArduinoPort.IsOpen)
                On_disconnect();

            else if (ArduinoPort.BytesToRead > 0)
            {
                String com_read;

                com_read = ArduinoPort.ReadLine();
                Process(com_read);
            }
        }

        private void Process(string com_read)
        {
            TextInputs.Clear();

            for (int i = 0; i < com_read.Length ; i++)
            {
                if(com_read[i] == '1')
                {
                    TextInputs.Text += ("D" + (i-1) + "  ->  TRUE \r\n");
                }
                else if(com_read[i] == '0')
                {
                    TextInputs.Text += ("D" + (i-1) + "  ->  FALSE \r\n");
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(ArduinoPort.IsOpen)
                ArduinoPort.Close();
        }
    }
}
