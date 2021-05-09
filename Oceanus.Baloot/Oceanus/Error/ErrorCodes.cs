using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    class ErrorCodes
    {
        public static readonly int START = 1000;
        public static readonly int ERROR_UNKNOWN = START + 1;
        public static readonly int ERROR_NETWORK_LOGIN_FAILED = START + 2;
        public static readonly int ERROR_JSON_PARSE_FAILED = START + 3;
        public static readonly int ERROR_LOBBY_SERVICE_STARTED_ALREADY = START + 4;
        public static readonly int ERROR_IMPEER_NOT_CONNECTED = START + 5;
        public static readonly int ERROR_BALOOT_SERVICE_STARTED_ALREADY = START + 6;
        public static readonly int ERROR_IMPEER_ROOM_NOT_CONNECTED = START + 7;

    }
}
