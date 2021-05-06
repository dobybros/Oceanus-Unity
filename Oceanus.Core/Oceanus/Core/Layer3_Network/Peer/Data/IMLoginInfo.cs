using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceanus.Core.Network
{
    public class IMLoginInfo
    {
        public string host
        {
            set; get;
        }
        public int port
        {
            get; set;
        }
        public string token
        {
            get; set;
        }
        public string server
        {
            get; set;
        }
    }
}
