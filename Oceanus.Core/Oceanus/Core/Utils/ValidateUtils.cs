using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceanus.Core.Utils
{
    class ValidateUtils
    {
        public static void checkNotNull(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Illegal argument");
        }
        public static void checkAllNotNull(params object[] objs)
        {
            foreach(object obj in objs)
            {
                if (obj == null)
                    throw new ArgumentNullException("Illegal arguments");
            }
        }
    }
}
