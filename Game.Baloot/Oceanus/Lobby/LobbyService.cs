using Oceanus.Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{

    public delegate void OnNetworkResultReceived<T>(NetworkResult<T> result);
    public delegate void OnNetworkEventReceived(NetworkEvent networkEvent);

    public interface LobbyService
    {
        event OnNetworkEventReceived OnNetworkEventReceivedEvents;
        
        /// <summary>
        /// 游客登录接口
        /// 并采集用户手机基本信息
        /// 返回的LoginResult需要客户端持久化存储， 自动进入游戏时， 可以直接通过LoginResult里的信息直接start， 接入Gateway服务器
        /// </summary>
        /// <param name="deviceId">设备的唯一ID</param>
        /// <param name="model">手机型号</param>
        /// <param name="os">手机操作系统的版本</param>
        /// <param name="network">手机此时的网络， 例如联通4G还是5G或者是Wifi</param>
        /// <param name="onResultReceived"></param>
        void LoginGuest(string deviceId, string model, string os, string network, OnNetworkResultReceived<LoginResult> onResultReceived = null);
        
        /// <summary>
        /// 通过已知的userId和jwtToken登录Gateway服务器， 获得Gateway服务器的信息之后， 直接建立Websocket通道
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="deviceId">设备唯一ID</param>
        /// <param name="terminal">设备类型， 例如Android还是iOS</param>
        /// <param name="jwtToken">登录Gateway所需令牌</param>
        void Start(string userId, string deviceId, int terminal, string jwtToken);
        /// <summary>
        /// 断掉当前通道， 再次Start的时候会重新建连
        /// </summary>
        void Stop();

        /// <summary>
        /// 开始匹配
        /// </summary>
        /// <param name="service">游戏的服务名字</param>
        /// <param name="group">游戏的场次名字</param>
        /// <param name="onResultReceived"></param>
        void StartMatching(string service, string group, OnIMResultReceived onResultReceived = null);
        /// <summary>
        /// 取消匹配
        /// </summary>
        void CancelMatching(OnIMResultReceived onResultReceived = null);
        /// <summary>
        /// 获得单太的PlayerGameStatusManager， 需要主动注册事件监听PlayerGameStatus的变化
        /// </summary>
        /// <returns></returns>
        PlayerGameStatusManager GetPlayerGameStatusManager();
        
        /// <summary>
        /// 此方法必须要在网络事件收到Connected之后才能调用， 否则会报错
        /// 领取登录奖励接口， 领取成功后， Result会返回Code == 1， 但是不会有金币变化的数量。 
        /// 成功之后会由PlayerGameStatusEvent立即推送通知金币变化
        /// </summary>
        /// <param name="dayNumber"></param>
        /// <param name="onResultReceived"></param>
        void TakeCheckInReward(int dayNumber, OnIMResultReceived onResultReceived = null);
        
        /// <summary>
        /// 领取破产保护
        /// 根据收到破产保护推送事件， 显示对话框
        /// </summary>
        /// <param name="onResultReceived"></param>
        void TakeBankruptProtection(OnIMResultReceived onResultReceived = null);
    }
}
