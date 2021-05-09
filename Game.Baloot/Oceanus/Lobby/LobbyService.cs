﻿using Oceanus.Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus
{
    //public delegate void OnPeerConnected();
    //public delegate void OnPeerDisconnected(int code);
    //public delegate void OnIMResultReceived(IMResult result);

    //public delegate void OnPeerReceivedMessage(IMMessage message);
    //public delegate void OnPeerReceivedData(IMData data);

    public delegate void OnNetworkResultReceived<T>(NetworkResult<T> result);
    //public delegate void OnIMResultReceived(IMResult result);
    public delegate void OnNetworkEventReceived(NetworkEvent networkEvent);

    public interface LobbyService
    {
        event OnNetworkEventReceived OnNetworkEventReceivedEvents;
        /**
         * <summary>
         *  游客登录接口
         *  并采集用户手机基本信息
         *  返回的LoginResult需要客户端持久化存储， 自动进入游戏时， 可以直接通过LoginResult里的信息直接start， 接入Gateway服务器
         * </summary>
         * <param name="deviceId">设备的唯一ID</param>
         * <param name="model">手机型号</param>
         * <param name="network">手机此时的网络， 例如联通4G还是5G或者是Wifi</param>
         * <param name="onResultReceived">获得返回数据的回调代理</param>
         * <param name="os">手机操作系统的版本</param>
         */
        void LoginGuest(string deviceId, string model, string os, string network, OnNetworkResultReceived<LoginResult> onResultReceived);
        /**
         * <summary>
         *  通过已知的userId和jwtToken登录Gateway服务器， 获得Gateway服务器的信息之后， 直接建立Websocket通道
         * </summary>
         * <param name="deviceId">设备唯一ID</param>
         * <param name="jwtToken">登录Gateway所需令牌</param>
         * <param name="terminal">设备类型， 例如Android还是iOS</param>
         * <param name="userId">用户ID</param>
         */
        void Start(string userId, string deviceId, int terminal, string jwtToken);

        /**
         * <summary>
         *  获得单太的PlayerGameStatusManager， 需要主动注册事件监听PlayerGameStatus的变化
         * </summary>
         * 
         */
        /**
         * <summary>
         *  开始匹配
         * </summary>
         * 
         * <param name="service">游戏的服务名字</param>
         * <param name="group">游戏的场次名字</param>
         */
        void StartMatching(string service, string group, OnIMResultReceived onResultReceived);
        PlayerGameStatusManager GetPlayerGameStatusManager();
        /**
         * <summary>
         *  此方法必须要在网络事件收到Connected之后才能调用， 否则会报错
         *  领取登录奖励接口， 领取成功后， Result会返回Code == 1， 但是不会有金币变化的数量。 
         *  成功之后会由PlayerGameStatusEvent立即推送通知金币变化
         * </summary>
         * 
         */
        void TakeCheckInReward(int dayNumber, OnIMResultReceived onResultReceived);
        /**
         * <summary>
         *  领取破产保护
         *  根据收到破产保护推送事件， 显示对话框
         * </summary>
         * 
         */
        void TakeBankruptProtection(OnIMResultReceived onResultReceived);
    }
}