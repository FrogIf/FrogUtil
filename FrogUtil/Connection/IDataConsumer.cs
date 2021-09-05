using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frog.Util.Connection
{
    public interface IDataConsumer
    {
        void Consume(byte[] bytes);
    }
}
