﻿using Frog.Util.Log;
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
        private static readonly ILogger logger = LoggerFactory.GetLogger("SerialPortOperator");

        // 实际串口对象
        private readonly SerialPort serialPort = new SerialPort();

        // 接收消息处理器
        private List<ReceiveDataHandler> handlers = new List<ReceiveDataHandler>();

        // 标记串口正准备关闭
        private volatile bool Closing = false;

        // 标记串口正在接收数据
        private volatile bool Receiving = false;

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
        public enum SendDataType
        {
            CHAR,
            HEX
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="content">发送内容</param>
        /// <param name="sendDataType">发送类型</param>
        /// <returns>是否发送成功: true - 成功; false - 失败</returns>
        public bool SendData(string content, SendDataType sendDataType = SendDataType.CHAR)
        {
            if (content == null)
            {
                logger.warn("send content is null, can't send.");
                return false;
            }
            if (!serialPort.IsOpen)
            {
                logger.warn("serial port is not open, can't send data.");
                return false;
            }

            bool result = false;
            switch (sendDataType)
            {
                case SendDataType.CHAR:
                    serialPort.Write(content);
                    result = true;
                    break;
                case SendDataType.HEX:
                    List<byte> hexDataSend = new List<byte>();
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
                    logger.warn("unrecognizable data type : " + sendDataType);
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
            byte[] buf = new byte[size];
            for (int i = 0; i < size; i++)
            {
                buf[i] = (byte)serialPort.ReadByte();
            }

            // 并行执行所有监听
            Parallel.ForEach(handlers, (handler) => {
                handler.Handle(buf);
            });

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
            void Handle(string message);

            /// <summary>
            /// 串口接收消息之后, 回调方法, 传入字节数组
            /// </summary>
            /// <param name="buf">串口接收到的字节数组</param>
            void Handle(byte[] buf);
        }

        public delegate void CloseSuccessCallback();

        /// <summary>
        /// 安全关闭串口
        /// </summary>
        public void CloseSerialPort(CloseSuccessCallback closeSuccessCallback)
        {
            if (serialPort.IsOpen)
            {
                logger.info("serial port prepare close.");
                Closing = true;  // 标记当前串口准备关闭
                while (Receiving)   // 自旋, 等待接收完成后继续执行
                {
                    logger.info("serial port closing...");
                    Application.DoEvents(); // 让出CPU控制权
                }
                serialPort.Close();
                closeSuccessCallback();
                Closing = false;
                Receiving = false;
                logger.info("serial port has been closed.");
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
                logger.warn("serial port has bean opened, can't set baud rate.");
                return false;
            }
            else
            {
                serialPort.BaudRate = baudRate;
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
                logger.warn("serial port has bean opened, can't set port name.");
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
                logger.warn("serial port has bean opened, can't be open again.");
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
            logger.info("serial port has open on : {}", serialPort.PortName);
            return true;
        }

    }
}