using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public class DisconnectedEvent : NetworkEvent
    {
        public DisconnectedEvent() : base(TYPE_DISCONNECTED)
        {

        }
        public int Code
        {
            get; set;
        }
    }
}
