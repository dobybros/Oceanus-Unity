using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public class Action
    {
        public Action(string contentType)
        {
            ContentType = contentType;
        }
        public string ContentType
        {
            get;
        }
    }
}
