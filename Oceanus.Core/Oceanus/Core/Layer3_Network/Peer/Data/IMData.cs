using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceanus.Core.Network 
{ 
    public class IMData
    {
        internal string Content
        {
            get; set;
        }
        public string ContentType
        {
            get; set;
        }
        public string Id
        {
            get; set;
        }

        public long Time
        {
            get; set;
        }

        internal object mActualContent;
        public T GetContent<T>()
        {
            if (mActualContent == null)
            {
                if(typeof(string).Equals(typeof(T))) {
                    mActualContent = Content;
                } else
                {
                    mActualContent = JsonMapper.ToObject<T>(new JsonReader(Content));
                    Content = null;
                }
                return (T)mActualContent;
            }
            else
            {
                return (T)mActualContent;
            }
        }
    }
}
