using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceanus.Core.Network
{
    class IMConstants
    {
        public const byte TYPE_IDENTITY = 1;
        public const byte TYPE_INCOMINGDATA = 10;
        public const byte TYPE_INCOMINGMESSAGE = 30;
        public const byte TYPE_INCOMINGINVOCATION = 50;
        public const byte TYPE_OUTGOINGDATA = 20;
        public const byte TYPE_OUTGOINGMESSAGE = 40;
        public const byte TYPE_PING = 111;
        public const byte TYPE_RESULT = 100;

        public const byte CHANNEL_STATUS_INIT = 1;
        public const byte CHANNEL_STATUS_CONNECTING = 5;
        public const byte CHANNEL_STATUS_CONNECTED = 10;
        public const byte CHANNEL_STATUS_DISCONNECTED = 100;
    }
}
