using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frog.Util.Connection
{
    public abstract class AbstractConnection : IConnection
    {
        /// <summary>
        /// 数据消费者
        /// </summary>
        private List<IDataConsumer> consumers = new List<IDataConsumer>();

        public AbstractConnection(List<IDataConsumer> consumers)
        {
            foreach(IDataConsumer consumer in consumers)
            {
                this.consumers.Add(consumer);
            }
        }
        
        public abstract void Close(CloseSuccessCallback closeSuccessCallback);
        public abstract string[] GetAvailableTargets();
        public abstract bool IsOpen();
        public abstract bool Open(IConfiguration config);
        public abstract bool SendData(byte[] bytes);
        protected void OnReceive(byte[] bytes)
        {
            if(consumers != null)
            {
                Parallel.ForEach(consumers, consumer => consumer.Consume(bytes));
            }
        }
    }
}
