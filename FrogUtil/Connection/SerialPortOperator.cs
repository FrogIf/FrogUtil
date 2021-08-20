using Frog.Util.Common;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frog.Util.Connection
{
    /// <summary>
    /// 安全串口
    /// </summary>
    public class SerialPortOperator
    {

        // 实际串口对象
        private readonly SerialPort serialPort = new SerialPort();

        // 接收消息处理器
        private List<ReceiveDataHandler> handlers = new List<ReceiveDataHandler>();

        // 标记串口正准备关闭
        private volatile bool Closing = false;

        // 标记串口正在接收数据
        private volatile bool Receiving = false;

        private volatile DataType receiveDataType = DataType.CHAR;

        public SerialPortOperator(List<ReceiveDataHandler> handlers)
        {
            // 增加串口接收数据监听
            this.serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceiveHandler);
            foreach (ReceiveDataHandler handler in handlers)
            {
                this.handlers.Add(handler);
            }
        }

        /// <summary>
        /// 发送数据类型
        /// </summary>
        public enum DataType
        {
            CHAR,
            HEX
        }

        /// <summary>
        /// 发送二进制数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendData(byte[] data)
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

        /// <summary>
        /// 发送二进制数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendData(byte[] data, int offset, int count)
        {
            if (data == null || offset >= data.Length)
            {
                return false;
            }
            if (!serialPort.IsOpen)
            {
                return false;
            }

            serialPort.Write(data, offset, count);
            return true;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="content">发送内容</param>
        /// <param name="sendDataType">发送类型</param>
        /// <returns>是否发送成功: true - 成功; false - 失败</returns>
        public bool SendData(string content, DataType sendDataType = DataType.CHAR)
        {
            if (content == null)
            {
                return false;
            }
            if (!serialPort.IsOpen)
            {
                return false;
            }

            bool result = false;
            switch (sendDataType)
            {
                case DataType.CHAR:
                    serialPort.Write(content);
                    result = true;
                    break;
                case DataType.HEX:
                    List<byte> hexDataSend = new List<byte>();
                    content = content.Replace(" ", "");
                    int j = 0;
                    for (int i = 0; i < content.Length - 1;)
                    {
                        try
                        {
                            hexDataSend.Add(Convert.ToByte(content.Substring(i, 2), 16));
                        }
                        catch
                        {
                            i--;
                            j--;
                        }
                        j++;
                        i += 2;
                    }

                    serialPort.Write(hexDataSend.ToArray(), 0, hexDataSend.Count);
                    result = true;
                    break;
                default:
                    break;
            }

            return result;
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
            if (DataType.CHAR == receiveDataType)
            {
                string message = serialPort.ReadExisting();
                // 并行执行所有监听
                Parallel.ForEach(handlers, (handler) => {
                    handler.Handle(message, size);
                });
            }
            else
            {
                byte[] buf = new byte[size];
                for (int i = 0; i < size; i++)
                {
                    buf[i] = (byte)serialPort.ReadByte();
                }

                // 并行执行所有监听
                Parallel.ForEach(handlers, (handler) => {
                    handler.Handle(buf, size);
                });
            }

            Receiving = false;
        }

        /// <summary>
        /// 消息处理器
        /// </summary>
        public interface ReceiveDataHandler
        {
            /// <summary>
            /// 串口接收到消息之后, 会回调该方法
            /// </summary>
            /// <param name="message">串口接收到的消息</param>
            void Handle(string message, int receiveCount);

            /// <summary>
            /// 串口接收消息之后, 回调方法, 传入字节数组
            /// </summary>
            /// <param name="buf">串口接收到的字节数组</param>
            void Handle(byte[] buf, int receiveCount);
        }

        public delegate void CloseSuccessCallback();

        /// <summary>
        /// 安全关闭串口
        /// </summary>
        public void CloseSerialPort(CloseSuccessCallback closeSuccessCallback)
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

        public string[] GetAvailablePortName()
        {
            return SerialPort.GetPortNames();
        }


        /// <summary>
        /// 串口是否打开
        /// </summary>
        /// <returns>true - 打开; false - 未打开</returns>
        public bool IsOpen()
        {
            return serialPort.IsOpen;
        }

        /// <summary>
        /// 设置串口波特率
        /// </summary>
        /// <param name="baudRate"></param>
        public bool SetBaudRate(int baudRate)
        {
            if (serialPort.IsOpen)
            {
                return false;
            }
            else
            {
                serialPort.BaudRate = baudRate;
                return true;
            }
        }

        /// <summary>
        /// 设置停止位
        /// </summary>
        public bool SetStopBits(StopBits stopBits)
        {
            if (serialPort.IsOpen)
            {
                return false;
            }
            else
            {
                serialPort.StopBits = stopBits;
                return true;
            }
        }

        public bool SetDataBits(int dataBits)
        {
            if (serialPort.IsOpen)
            {
                return false;
            }
            else
            {
                serialPort.DataBits = dataBits;
                return true;
            }
        }

        /// <summary>
        /// 设置串口号
        /// </summary>
        /// <param name="portName"></param>
        public bool SetPortName(string portName)
        {
            if (serialPort.IsOpen)
            {
                return false;
            }
            else
            {
                serialPort.PortName = portName;
                return true;
            }
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <returns></returns>
        public bool OpenSerialPort()
        {
            if (serialPort.IsOpen)
            {
                return false;
            }

            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
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

        public DataType ReceiveDataType
        {
            get { return this.receiveDataType; }
            set { this.receiveDataType = value; }
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

        /// <summary>
        /// 校验hex数据是否正确
        /// </summary>
        /// <param name="message">待校验的原始数据</param>
        /// <returns>结果: true - 正确; false - 错误</returns>
        public static bool HexDataValid(string message, ref string tip)
        {
            if (message == null && StringUtil.IsBlank(message)) { return false; }
            message = message.Replace(" ", "");
            message = message.ToUpper();
            char[] chars = message.ToCharArray();
            if (chars.Length % 2 != 0)
            {
                tip = "HEX格式不正确, len : " + chars.Length;
                return false;
            }
            for (int i = 0; i < chars.Length; i++)
            {
                char ch = chars[i];
                if (ch < '0' || (ch > '9' && (ch < 'A' || ch > 'F')))
                {
                    tip = "HEX格式不正确, pos : " + i + ", char : " + ch;
                    return false;
                }
            }
            return true;
        }

    }
}
