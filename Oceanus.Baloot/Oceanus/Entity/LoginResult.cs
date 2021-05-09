using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public class LoginResult
    {
        public string jwtToken
        {
            get; set;
        }
        public Player player
        {
            get; set;
        }
    }
}
