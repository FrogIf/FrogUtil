using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frog.Util.Connection
{
    public class EthernetOperator : AbstractConnection
    {
        public EthernetOperator(List<IDataConsumer> consumers) : base(consumers)
        {

        }

        public override void Close(CloseSuccessCallback closeSuccessCallback)
        {
            
        }

        public override string[] GetAvailableTargets()
        {
            throw new NotImplementedException();
        }

        public override bool IsOpen()
        {
            throw new NotImplementedException();
        }

        public override bool Open(IConfiguration config)
        {
            throw new NotImplementedException();
        }

        public override bool SendData(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
