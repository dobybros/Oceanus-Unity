using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceanus.Core.Network
{
    public class ResponseData<T>
    {
        public int code
        {
            get; set;
        }
        public string message
        {
            get; set;
        }
        public T data
        {
            get; set;
        }
    }
}
