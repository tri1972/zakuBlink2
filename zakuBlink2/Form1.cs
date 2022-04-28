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
    public partial class ZakuBlinkForm : Form
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

        NotifyIcon notifyIcon;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ZakuBlinkForm()
        {  
            InitializeComponent();
            // タスクバーに表示しない
            this.ShowInTaskbar = false;
            setComponents();

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
        /// <summary>
        /// 主にタスクトレイに常駐させるよう実行
        /// 参考
        /// https://www.fenet.jp/dotnet/column/environment/4527/
        /// </summary>
        private void setComponents()
        {
            notifyIcon = new NotifyIcon();
            // アイコンの設定
            notifyIcon.Icon = new Icon(@"..\icon\Star_24682.ico");
            // アイコンを表示する
            notifyIcon.Visible = true;
            // アイコンにマウスポインタを合わせたときのテキスト
            notifyIcon.Text = "NortyFyTest";

            // コンテキストメニュー
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem toolStripMenuItemDebug = new ToolStripMenuItem();
            toolStripMenuItemDebug.Text = " & Debug";
            toolStripMenuItemDebug.Click += ToolStripMenuItem_Click_Debug;
            contextMenuStrip.Items.Add(toolStripMenuItemDebug);

            ToolStripMenuItem toolStripMenuItemExit = new ToolStripMenuItem();
            toolStripMenuItemExit.Text = " & 終了";
            toolStripMenuItemExit.Click += ToolStripMenuItem_Click_Exit;
            contextMenuStrip.Items.Add(toolStripMenuItemExit);
            
            notifyIcon.ContextMenuStrip = contextMenuStrip;

            // NotifyIconのクリックイベント
            notifyIcon.Click += NotifyIcon_Click;
        }
        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            // Formの表示/非表示を反転
            this.Visible = !this.Visible;
        }

        private void ToolStripMenuItem_Click_Debug(object sender, EventArgs e)
        {
            var debugForm = new Debug_Form(this.serialPort);
            debugForm.ShowDialog();
        }

        private void ToolStripMenuItem_Click_Exit(object sender, EventArgs e)
        {
            // アプリケーションの終了
            Application.Exit();
        }

        private void ZakuBlink_Load(object sender, EventArgs e)
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

    }
}
