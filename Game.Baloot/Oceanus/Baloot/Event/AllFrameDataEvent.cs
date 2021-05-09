using NetWork.Oceanus;
using NetWork.Oceanus.Baloot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus.Baloot
{
    class AllFrameDataEvent : NetworkEvent
    {
        public const string CONTENT_TYPE = "all";
        public AllFrameDataEvent() : base(TYPE_BALOOT_ALL_FRAME_DATA)
        {

        }
        public FrameData frameData
        {
            set; get;
        }
    }
}
