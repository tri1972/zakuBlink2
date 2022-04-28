using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zakuBlink2
{
    public partial class Debug_Form : Form
    {

        private SerialPort serialPort;
        public Debug_Form(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write(textBox1.Text + "\n");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            byte outputData = (byte)this.numericUpDown1.Value;
            if (0 <= outputData && outputData <= 180)
            {
                SenderSerialPort.SendByteData(outputData, this.serialPort);
            }
            else
            {
                MessageBox.Show("数値データは30～150しか有効ではありません");
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
