using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    class PlayerGameStatusManagerImpl : PlayerGameStatusManager
    {
        public event OnPlayerGameStatusEventReceived OnPlayerGameStatusEventReceivedEvents;

        public PlayerGameStatus GetPlayerGameStatus()
        {
            throw new NotImplementedException();
        }
    }
}
