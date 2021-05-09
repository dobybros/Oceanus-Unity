using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public class NetworkEvent
    {
        public const int TYPE_CONNECTED = 1;
        public const int TYPE_DISCONNECTED = -1;
        //public const int TYPE_DATA_RECEIVED = 10;
        //public const int TYPE_MESSAGE_RECEIVED = 100;
        public const int TYPE_LOBBY_MATCHING_RESULT = 101;
        public const int TYPE_BALOOT_ALL_FRAME_DATA = 201;
        public const int TYPE_BALOOT_UPDATE_FRAME_DATA = 202;
        public NetworkEvent(int type)
        {
            Type = type;
        }
        public int Type
        {
            set; get;
        }
    }
}
