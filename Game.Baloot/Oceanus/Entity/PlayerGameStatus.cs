using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public class PlayerGameStatus
    {
        public int? balance
        {
            get; set;
        }
        public Player player
        {
            get; set;
        }
        public List<string> games
        {
            get; set;
        }
        public Dictionary<string, GameInfo> gameMap
        {
            get; set;
        }
        public List<string> activities
        {
            set; get;
        }
        public Dictionary<string, Activity> activityMap
        {
            set; get;
        }
    }
}
