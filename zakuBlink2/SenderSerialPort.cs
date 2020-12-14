using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zakuBlink2
{
    public static class SenderSerialPort
    {

        private static float beforeValueReadCounterDiskReadByte;
        private static float beforeValueReadCounterDiskWriteByte;


        public static void InitOpenSerialPort(SerialPort serialPort,string portName)
        {
            serialPort.BaudRate = 9600;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Handshake = Handshake.None;
            serialPort.PortName = portName;
        }

        public static void SendByteData(byte sendData, SerialPort serialPort)
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
                throw err;
            }
            catch (System.InvalidOperationException err)
            {
                throw err;
            }

        }
        /// <summary>
        /// ザクヘッドにシリアルデータを送るスレッドメソッド
        /// </summary>
        public static void RunSendZakuHead(SerialPort serialPort, PerformanceCounter readCounterDiskReadByte, PerformanceCounter readCounterDiskWriteByte)
        {
            byte[] outputBlinkMonoEye = new byte[2];
            byte[] outputMoveMonoEye = new byte[2];
            outputBlinkMonoEye[0] = 0x00;
            long prescalerCounter = 0;
            float currentReadCounterDiskReadByte = 0;
            float currentReadCounterDiskWriteByte = 0;

            while (true)
            {
                if (serialPort.IsOpen)
                {
                    currentReadCounterDiskReadByte = readCounterDiskReadByte.NextValue();
                    currentReadCounterDiskWriteByte = readCounterDiskWriteByte.NextValue();
                    if (prescalerCounter % 10 == 0)//0.1秒ごとにアクセス
                    {
                        if (beforeValueReadCounterDiskReadByte != currentReadCounterDiskReadByte
                            || beforeValueReadCounterDiskWriteByte != currentReadCounterDiskWriteByte)
                        {
                            outputBlinkMonoEye[0] = 0xff;
                            beforeValueReadCounterDiskReadByte = currentReadCounterDiskReadByte;
                            beforeValueReadCounterDiskWriteByte = currentReadCounterDiskWriteByte;
                        }
                        else
                        {
                            outputBlinkMonoEye[0] = 0x00;
                        }
                        outputBlinkMonoEye[1] = (byte)'\n';
                        serialPort.Write(outputBlinkMonoEye, 0, 2);
                        outputMoveMonoEye[0] = (byte)(90.0 + (130.0 - 90.0) / 100000000.0 * (currentReadCounterDiskReadByte - currentReadCounterDiskWriteByte));
                        outputMoveMonoEye[1] = (byte)'\n';
                        System.Threading.Thread.Sleep(100);
                        serialPort.Write(outputMoveMonoEye, 0, 2);
                    }

                }
                //0.01秒待機する
                System.Threading.Thread.Sleep(10);
                ++prescalerCounter;
            }
        }
    }
}
