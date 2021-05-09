using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public delegate void OnPlayerGameStatusEventReceived(PlayerGameStatusEvent playerGameStatusEvent);

    public class PlayerGameStatusEvent
    {
        public static readonly int TYPE_INITED = 1;
        public static readonly int TYPE_BALANCE_CHANGED = 10;
        public static readonly int TYPE_PLAYER_CHANGED = 20;
        public static readonly int TYPE_GAMES_CHANGED = 30;
        public static readonly int TYPE_ACTIVITIES_CHANGED = 40;
        public static readonly int TYPE_GAMEINFO_CHANGED = 100; //key是gameId
        public static readonly int TYPE_ACTIVITY_CHANGED = 110; //key是activityId
        public int Type
        {
            set; get;
        }
        public string Key
        {
            set; get;
        }
    }
    /**
     * <summary>
     *  负责管理PlayerGameStatus的获取与动态更新
     * </summary>
     */
    public interface PlayerGameStatusManager
    {
        event OnPlayerGameStatusEventReceived OnPlayerGameStatusEventReceivedEvents;
        /**
         * 获取实时的PlayerGameStatus对象
         */
        PlayerGameStatus GetPlayerGameStatus();
    }
}
