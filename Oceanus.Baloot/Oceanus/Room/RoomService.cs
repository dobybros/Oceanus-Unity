using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    public interface RoomService
    {
        /**
        * <summary>
        *  通过匹配成功的推送事件获得房间服务器的host和port， 以及jwtToken之后可以进入房间
        *  
        *  在房间结束之后需要调用Stop方法， 释放通道资源
        * </summary>
        * <param name="deviceId">设备唯一ID</param>
        * <param name="jwtToken">登录房间服务器所需令牌</param>
        * <param name="terminal">设备类型， 例如Android还是iOS</param>
        * <param name="userId">用户ID</param>
        * <param name="host">房间服务器的Host</param>
        * <param name="port">房间服务器的Port</param>
        */
        void Start(string userId, string deviceId, int terminal, string host, int port, string jwtToken);
        /**
         * <summary>
         *  释放资源
         * </summary>
         * 
         */
        void Stop();
    }
}
