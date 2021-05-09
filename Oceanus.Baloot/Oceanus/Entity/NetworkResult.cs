using Oceanus.Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public class NetworkResult<T> : IMResult
    {
        private T t;
        public NetworkResult() { }
        public NetworkResult(T t) {
            this.t = t;
        }

        public T GetContent() 
        {
            if (t != null)
                return t;
            return GetContent<T>();
        }

    }
}
