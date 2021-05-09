using NetWork.Oceanus;
using NetWork.Oceanus.Baloot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus.Baloot
{
    class UpdateFrameDataEvent : NetworkEvent
    {
        public const string CONTENT_TYPE = "upd";
        public UpdateFrameDataEvent() : base(TYPE_BALOOT_UPDATE_FRAME_DATA)
        {

        }
        public FrameData frameData
        {
            set; get;
        }
    }
}
