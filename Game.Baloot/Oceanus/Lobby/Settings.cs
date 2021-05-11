using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public class Settings
    {
        public static string Host = "https://aplomb.acucom.net";
        public static readonly string LoginUrl = Host + "/rest/goldcentral/login";
        public static readonly string LoginGatewayUrl = Host + "/rest/goldcentral/gateway/login";
    }
}
