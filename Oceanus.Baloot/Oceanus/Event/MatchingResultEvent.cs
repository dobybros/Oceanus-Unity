using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public class MatchingResultEvent : NetworkEvent
    {
        public const string CONTENT_TYPE = "matchResult";
        public MatchingResultEvent() : base(TYPE_LOBBY_MATCHING_RESULT)
        {

        }
        
        public string id
        {
            set; get;
        }
        public string token
        {
            set; get;
        }
        public int code
        {
            set; get;
        }
        public string server
        {
            set; get;
        }
        public string host
        {
            set; get;
        }
        public int port
        {
            set; get;
        }
    }
}
