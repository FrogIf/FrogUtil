﻿using System.Collections.Generic;
using System.Threading;

namespace Frog.Util.Collection
{
    /// <summary>
    /// 阻塞队列
    /// </summary>
    public class BlockingQueue<T>
    {
        #region Fields & Properties
        //队列名称
        private string m_name;
        //队列最大长度
        private readonly int m_maxSize;
        //FIFO队列
        private Queue<T> m_queue;
        //是否运行中
        private bool m_isRunning;
        //入队手动复位事件
        private ManualResetEvent m_enqueueWait;
        //出队手动复位事件
        private ManualResetEvent m_dequeueWait;
        /// <summary>
        /// 队列长度
        /// </summary>
        public int Count => m_queue.Count;
        #endregion

        #region Ctor
        public BlockingQueue(int maxSize, string name = "BlockingQueue", bool isRunning = false)
        {
            m_maxSize = maxSize;
            m_name = name;
            m_queue = new Queue<T>(m_maxSize);
            m_isRunning = isRunning;
            m_enqueueWait = new ManualResetEvent(false); // 无信号，入队waitOne阻塞
            m_dequeueWait = new ManualResetEvent(false); // 无信号, 出队waitOne阻塞
        }
        #endregion

        #region Public Method

        /// <summary>
        /// 开启阻塞队列
        /// </summary>
        public void Open()
        {
            m_isRunning = true;
        }

        /// <summary>
        /// 关闭阻塞队列
        /// </summary>
        public void Close()
        {
            // 停止队列
            m_isRunning = false;
            // 发送信号，通知出队阻塞waitOne可继续执行，可进行出队操作
            m_dequeueWait.Set();
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            if (!m_isRunning)
            {
                return;
            }

            while (true)
            {
                lock (m_queue)
                {
                    // 如果队列未满，继续入队
                    if (m_queue.Count < m_maxSize)
                    {
                        m_queue.Enqueue(item);
                        // 置为无信号
                        m_enqueueWait.Reset();
                        // 发送信号，通知出队阻塞waitOne可继续执行，可进行出队操作
                        m_dequeueWait.Set();
                        break;
                    }
                }
                // 如果队列已满，则阻塞队列，停止入队，等待信号
                m_enqueueWait.WaitOne();
            }
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Dequeue(ref T item)
        {
            while (true)
            {
                if (!m_isRunning)
                {
                    lock (m_queue) return false;
                }
                lock (m_queue)
                {
                    // 如果队列有数据，则执行出队
                    if (m_queue.Count > 0)
                    {
                        item = m_queue.Dequeue();
                        // 置为无信号
                        m_dequeueWait.Reset();
                        // 发送信号，通知入队阻塞waitOne可继续执行，可进行入队操作
                        m_enqueueWait.Set();
                        return true;
                    }
                }
                // 如果队列无数据，则阻塞队列，停止出队，等待信号
                m_dequeueWait.WaitOne();
            }
        }
        #endregion

        public bool IsRunning
        {
            get
            {
                return m_isRunning;
            }
        }
    }
}
