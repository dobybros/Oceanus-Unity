using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceanus.Core.Utils
{
    class ValidateUtils
    {
        public static void CheckNotNull(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Illegal argument");
        }
        public static void CheckAllNotNull(params object[] objs)
        {
            foreach(object obj in objs)
            {
                if (obj == null)
                    throw new ArgumentNullException("Illegal arguments");
            }
        }

        public static void CheckEqualsAny(object obj, params object[] anyValues)
        {
            if (obj == null || anyValues == null || anyValues.Length == 0)
                throw new ArgumentException("Illegal arguments");

            bool hit = false;
            for(int i = 0; i < anyValues.Length; i++)
            {
                if(obj.Equals(anyValues[i]))
                {
                    hit = true;
                    break;
                }
            }
            if (!hit)
                throw new ArgumentException("Illegal arguments");
        }
    }
}
