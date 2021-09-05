using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;

namespace Frog.Util.Connection
{
    /// <summary>
    /// 安全串口
    /// </summary>
    public class SerialPortConnection : AbstractConnection
    {

        // 实际串口对象
        private readonly SerialPort serialPort = new SerialPort();

        // 标记串口正准备关闭
        private volatile bool Closing = false;

        // 标记串口正在接收数据
        private volatile bool Receiving = false;

        public SerialPortConnection(List<IDataConsumer> consumers) : base(consumers)
        {
            // 增加串口接收数据监听
            this.serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceiveHandler);
        }

        public override bool SendData(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return false;
            }
            if (!serialPort.IsOpen)
            {
                return false;
            }

            serialPort.Write(data, 0, data.Length);
            return true;
        }

        public override bool Open(IConfiguration config)
        {
            if (serialPort.IsOpen)
            {
                return false;
            }

            if(!(config is SerialPortConfiguration))
            {
                throw new ArgumentException("config type is not right, except SerialPortConfiguration, but " + (config == null ? "null" : config.GetType().ToString()));
            }

            SerialPortConfiguration serialPortConfiguration = (SerialPortConfiguration)config;
            serialPort.PortName = config.Target();
            serialPort.StopBits = serialPortConfiguration.StopBitVal;
            serialPort.DataBits = serialPortConfiguration.DataBits;
            serialPort.BaudRate = serialPortConfiguration.BaudRate;

            try
            {
                serialPort.Open();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public override void Close(CloseSuccessCallback closeSuccessCallback)
        {
            if (serialPort.IsOpen)
            {
                Closing = true;  // 标记当前串口准备关闭
                while (Receiving)   // 自旋, 等待接收完成后继续执行
                {
                    Application.DoEvents(); // 让出CPU控制权
                }
                serialPort.Close();
                closeSuccessCallback();
                Closing = false;
                Receiving = false;
            }
        }

        /// <summary>
        /// 接收数据回调
        /// </summary>
        /// <param name="sender">数据发送器</param>
        /// <param name="e">事件</param>
        private void DataReceiveHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (Closing) { return; }

            Receiving = true;

            int size = serialPort.BytesToRead;
            byte[] buf = new byte[size];
            serialPort.Read(buf, 0, size);
            this.OnReceive(buf);

            Receiving = false;
        }

        public override bool IsOpen()
        {
            return serialPort.IsOpen;
        }

        public override string[] GetAvailableTargets()
        {
            return SerialPort.GetPortNames();
        }

        /// <summary>
        /// 串口连接配置
        /// </summary>
        public class SerialPortConfiguration : IConfiguration
        {
            /// <summary>
            /// 串口号
            /// </summary>
            private string portName;

            /// <summary>
            /// 数据位
            /// </summary>
            private int dataBits;

            /// <summary>
            /// 波特率
            /// </summary>
            private int baudRate;

            /// <summary>
            /// 停止位
            /// </summary>
            private StopBits stopBitVal;

            public string PortName
            {
                get { return this.portName; }
                set { this.portName = value; }
            }

            public int DataBits
            {
                get { return this.dataBits; }
                set { this.dataBits = value; }
            }

            public int BaudRate
            {
                get { return this.baudRate; }
                set { this.baudRate = value; }
            }

            public StopBits StopBitVal
            {
                get { return this.stopBitVal; }
                set { this.stopBitVal = value; }
            }

            public string Target()
            {
                return this.portName;
            }

            /// <summary>
            /// 停止位转换
            /// </summary>
            /// <param name="stopBitText"></param>
            /// <returns></returns>
            public static StopBits GetStopBits(string stopBitText)
            {
                switch (stopBitText)
                {
                    case "One":
                        return StopBits.One;
                    case "Two":
                        return StopBits.Two;
                    case "None":
                        return StopBits.None;
                    case "OnePointFive":
                        return StopBits.OnePointFive;
                    default:
                        throw new ArgumentException("Unrecognized stopBitText : " + stopBitText);
                }
            }
        }
    }
}
