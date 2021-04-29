using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceanus.Core.Network
{

    public class IMResult
    {
        public int Code
        {
            get; set;
        }
        public string Description
        {
            get; set;
        }
        public string ForId
        {
            get; set;
        }
        public long Time
        {
            get; set;
        }
        internal string Content
        {
            get; set;
        }

        private object mActualContent;

        public T GetContent<T>()
        {
            if(mActualContent == null)
            {
                mActualContent = JsonMapper.ToObject<T>(new JsonReader(Content));
                Content = null;
                return (T)mActualContent;
            } else
            {
                return (T)mActualContent;
            }
        }
    }
}
