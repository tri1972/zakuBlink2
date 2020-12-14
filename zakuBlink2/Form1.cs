using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace zakuBlink2
{
    public partial class Form1 : Form
    {
        private int currentCompoBoxSerialIndex;

        private float readCounterValue;
        public  float ReadCounterValue
        {
            get { return readCounterValue; }
            set { readCounterValue = value;
                this.label1.Text = readCounterValue.ToString();
            }
        }


        /// <summary>
        /// Disk Read Bytes/sec データ
        /// </summary>
        private PerformanceCounter readCounterDiskReadByte;
        /// <summary>
        /// Disk Write Bytes/sec データ
        /// </summary>
        private PerformanceCounter readCounterDiskWriteByte;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            Task taskPerformanceMonitor = Task.Run(() =>
            {
                RunPerformanceMonitor();
            });

            Task taskSendZakuHead = Task.Run(() =>
            {
                SenderSerialPort.RunSendZakuHead(this.serialPort,this.readCounterDiskReadByte,this.readCounterDiskWriteByte);
            });
            this.readCounterDiskReadByte 
                = GetterPerformanceMonitor.GetInstancePerformanceMonitorDiskAccess("Disk Read Bytes/sec");
            this.readCounterDiskWriteByte 
                = GetterPerformanceMonitor.GetInstancePerformanceMonitorDiskAccess("Disk Write Bytes/sec");
        }



        private void Form1_Load(object sender, EventArgs e)
        {

            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                portComboBox.Items.Add(port);
            }
            if (portComboBox.Items.Count > 0)
                portComboBox.SelectedIndex = 0;
        }
        /// <summary>
        /// パフォーマンスモニター値取得ランナブル
        /// </summary>
        private void RunPerformanceMonitor()
        {
            while (true)
            {
                var readValueDiskReadByteData = readCounterDiskReadByte.NextValue();
                var readValueDiskWriteByteData = readCounterDiskWriteByte.NextValue();

                if (readValueDiskReadByteData != 0)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)(() => 
                        { 
                            label1.Text = readValueDiskReadByteData.ToString();
                            label4.Text = readValueDiskWriteByteData.ToString();

                        }));
                    }
                }
                //1秒待機する
                System.Threading.Thread.Sleep(1000);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write(textBox1.Text + "\n");
            }
        }

        private void portComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.currentCompoBoxSerialIndex=this.portComboBox.SelectedIndex;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte outputData= (byte)this.numericUpDown1.Value;
            if (0 <= outputData && outputData <= 180)
            {
                SenderSerialPort.SendByteData(outputData,this.serialPort);
            }
            else
            {
                MessageBox.Show("数値データは30～150しか有効ではありません");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                SenderSerialPort.InitOpenSerialPort(this.serialPort, portComboBox.Items[this.currentCompoBoxSerialIndex].ToString());
                serialPort.Open();
            }
            catch (System.UnauthorizedAccessException err)
            {
                MessageBox.Show(err.Message);
            }
            catch (System.InvalidOperationException err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        delegate void SetTextCallback(string text);
        private void Response(string text)
        {
            if (textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Response);
                Invoke(d, new object[] { text });
            }
            else
            {
                textBox2.AppendText(text + "\n");
            }
        }
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string str = serialPort.ReadLine();
            Response(str);
        }
    }
}
