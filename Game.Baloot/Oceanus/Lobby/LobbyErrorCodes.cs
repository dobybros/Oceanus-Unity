using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public class LobbyErrorCodes
    {
        /// <summary>
        /// 匹配服务器不可用
        /// </summary>
        public const int ERROR_START_MATCHING_NO_LEADER_ELECTED = 2050;
        /// <summary>
        /// 并发修改匹配状态， 需要重试， 可以是自动重试也可以是玩家重试
        /// </summary>
        public const int ERROR_START_MATCHING_MODIFY_CONCURRENTLY = 2048;
        /// <summary>
        /// 匹配过程中， 玩家状态发生了变化
        /// </summary>
        public const int ERROR_START_MATCHING_PLAYER_STATUS_CHANGED = 2053;
        /// <summary>
        /// 玩家已经在匹配中
        /// </summary>
        public const int ERROR_START_MATCHING_BUT_PLAYER_IN_MATCHING = 2045;
        /// <summary>
        /// 玩家已经在游戏中
        /// </summary>
        public const int ERROR_START_MATCHING_BUT_PLAYER_IN_GAME = 2044;
        /// <summary>
        /// 玩家状态出现未知错误
        /// </summary>
        public const int ERROR_START_MATCHING_BUT_PLAYER_UNKNOWN = 2046;
        /// <summary>
        /// 该玩法和场次没有服务器在处理， 需要稍等或者联系管理员
        /// </summary>
        public const int ERROR_START_MATCHING_NO_SERVER_ASSIGNED = 2049;
    }
}
