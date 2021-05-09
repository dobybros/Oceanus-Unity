using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public class ConnectedEvent : NetworkEvent 
    {
        public ConnectedEvent():base(TYPE_CONNECTED)
        {

        }
    }
}
