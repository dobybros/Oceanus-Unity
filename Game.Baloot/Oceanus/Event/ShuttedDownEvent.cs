using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public class ShuttedDownEvent : NetworkEvent
    {
        public ShuttedDownEvent() : base(TYPE_SHUTTDDOWN)
        {

        }
        public int Code
        {
            get; set;
        }
    }
}
