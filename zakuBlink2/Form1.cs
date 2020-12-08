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

        private static float beforeValueReadCounterDiskReadByte;

        public Form1()
        {
            InitializeComponent();
            Task taskPerformanceMonitor = Task.Run(() =>
            {
                runPerformanceMonitor();
            });

            Task taskSendZakuHead = Task.Run(() =>
            {
                runSendZakuHead();
            });


            this.readCounterDiskReadByte = new PerformanceCounter();
            ((System.ComponentModel.ISupportInitialize)(this.readCounterDiskReadByte)).BeginInit();
            this.readCounterDiskReadByte.CategoryName = "LogicalDisk";
            this.readCounterDiskReadByte.CounterName = "Disk Read Bytes/sec";
            this.readCounterDiskReadByte.InstanceName = "_Total";
            ((System.ComponentModel.ISupportInitialize)(this.readCounterDiskReadByte)).EndInit();

            this.readCounterDiskWriteByte = new PerformanceCounter();
            ((System.ComponentModel.ISupportInitialize)(this.readCounterDiskWriteByte)).BeginInit();
            this.readCounterDiskWriteByte.CategoryName = "LogicalDisk";
            this.readCounterDiskWriteByte.CounterName = "Disk Write Bytes/sec";
            this.readCounterDiskWriteByte.InstanceName = "_Total";
            ((System.ComponentModel.ISupportInitialize)(this.readCounterDiskWriteByte)).EndInit();


        }


        private void initOpenSerialPort()
        {
            serialPort.BaudRate = 9600;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Handshake = Handshake.None;
            serialPort.PortName = portComboBox.Items[this.currentCompoBoxSerialIndex].ToString();
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

        private void runPerformanceMonitor()
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
                    //this.ReadCounterValue = readData;
                }
                //1秒待機する
                System.Threading.Thread.Sleep(1000);

            }

        }

        private void runSendZakuHead()
        {
            byte[] outputdata = new byte[2];
            outputdata[0] = 0x00;
            while (true)
            {
                if (serialPort.IsOpen)
                {

                    if (beforeValueReadCounterDiskReadByte != readCounterDiskReadByte.NextValue())
                    {
                        outputdata[0] = 0xff;
                        beforeValueReadCounterDiskReadByte = readCounterDiskReadByte.NextValue();
                    }
                    else
                    {
                        outputdata[0] = 0x00;
                    }
                    outputdata[1] = (byte)'\n';
                    serialPort.Write(outputdata, 0, 2);
                }
                //0.1秒待機する
                System.Threading.Thread.Sleep(100);

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
        private void sendByteData(byte sendData)
        {
            try
            {
                byte[] outputdata = new byte[2];
                if (serialPort.IsOpen)
                {
                    outputdata[0] = sendData;
                    outputdata[1] = (byte)'\n';
                    serialPort.Write(outputdata, 0, 2);
                }
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

        private void button2_Click(object sender, EventArgs e)
        {
            byte outputData= (byte)this.numericUpDown1.Value;
            if (0 <= outputData && outputData <= 180)
            {
                sendByteData(outputData);
            }
            else
            {
                MessageBox.Show("数値データは30～150しか有効ではありません");
            }
            /*
            try
            {
                byte[] outputdata = new byte[2];
                if (serialPort.IsOpen)
                {
                    outputdata[0] = 
                    outputdata[1] = (byte)'\n';
                    if (0 <= outputdata[0] && outputdata[0] <= 180)
                    {
                        serialPort.Write(outputdata, 0, 2);
                    }
                    else
                    {
                        MessageBox.Show("数値データは30～150しか有効ではありません");
                    }
                }
            }
            catch(System.UnauthorizedAccessException err)
            {
                MessageBox.Show(err.Message);
            }
            catch(System.InvalidOperationException err)
            {
                MessageBox.Show(err.Message);
            }*/
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                this.initOpenSerialPort();
                serialPort.Open();
                //serialPort.Close();
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
