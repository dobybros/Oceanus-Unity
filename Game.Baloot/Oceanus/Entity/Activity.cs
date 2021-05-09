using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public class Activity
    {
        public string id
        {
            set; get;
        }
        public long? startTime
        {
            set; get;
        }
        public long? endTime
        {
            set; get;
        }

        public string info
        {
            set; get;
        }
    }
}
