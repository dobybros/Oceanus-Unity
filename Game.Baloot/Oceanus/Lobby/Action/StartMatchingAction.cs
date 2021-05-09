using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public class StartMatchingAction : Action 
    {
        public string service
        {
            set; get;
        }
        public string group
        {
            set; get;
        }
        public StartMatchingAction() : base("startMatching")
        {
        }

    }
}
