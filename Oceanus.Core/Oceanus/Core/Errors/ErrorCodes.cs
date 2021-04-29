using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceanus.Core.Errors
{
    class ErrorCodes
    {
        private const int START = 80000;
        public const int ERROR_NETWORK_DISCONNECTED = START + 1;
        public const int ERROR_MESSAGE_START_SENDING_ALREADY = START + 2;
        public const int ERROR_NETWORK_SEND_FAILED = START + 3;
        public const int ERROR_NETWORK_CLOSED = START + 4;
    }
}
