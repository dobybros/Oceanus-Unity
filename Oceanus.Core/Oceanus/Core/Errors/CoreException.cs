using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceanus.Core.Errors
{
    public class CoreException : Exception
    {
        public int Code
        {
            get; set;
        }

        public CoreException(int code) : base()
        {
            this.Code = code;
        }

        public CoreException(int code, string message) : base(message)
        {
            this.Code = code;
        }

        public CoreException(int code, string message, Exception e) : base(message, e)
        {
            this.Code = code;
        }
    }
}
