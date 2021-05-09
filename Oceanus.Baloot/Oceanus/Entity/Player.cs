using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public class Player
    {
        public static readonly int GENDER_MALE = 1;
        public static readonly int GENDER_FEMALE = 2;

        public string userId
        {
            get; set;
        }
        public string name
        {
            get; set;
        }
        public string icon
        {
            get; set;
        }
        public int gender
        {
            get; set;
        }
    }
}
