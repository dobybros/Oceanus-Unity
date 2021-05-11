using NetWork.Oceanus.Baloot;
using Oceanus.Core.Network;
using Oceanus.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = Oceanus.Core.Utils.Logger;

namespace NetWork.Oceanus
{
    //class UnityLogger : LoggerListener
    //{
    //    private string mPrefix = "@+@";
    //    public void error(string message)
    //    {
    //        Debug.LogError(mPrefix + message);
    //    }

    //    public void fatal(string message)
    //    {
    //        Debug.LogError(mPrefix + message);
    //    }

    //    public void info(string message)
    //    {
    //        Debug.Log(mPrefix + message);
    //    }

    //    public void warn(string message)
    //    {
    //        Debug.LogWarning(mPrefix + message);
    //    }
    //}
    public class OceanusFactory
    {
        private static readonly string TAG = typeof(OceanusFactory).Name;

        private static OceanusFactory Instance;

        public const int TERMINAL_ANDROID = 100;
        public const int TERMINAL_IOS = 110;

        /// <summary>
        /// 断连之后会自动重连
        /// </summary>
        public const int CONNECT_STATUS_DISCONNECTED = -1;
        /// <summary>
        /// 断连之后， 不会在自动重连
        /// </summary>
        public const int CONNECT_STATUS_SHUTTEDDOWN = -100;
        /// <summary>
        /// 已经连接上了
        /// </summary>
        public const int CONNECT_STATUS_CONNECTED = 1;
        private RoomService mRoomService;
        private LobbyService mLobbyService;

        public static OceanusFactory GetInstance()
        {
            if(Instance == null)
            {
                lock(typeof(OceanusFactory))
                {
                    if(Instance == null)
                    {
                        //Logger.LoggerListener = new UnityLogger();
                        Instance = new OceanusFactory();
                    }
                }
            }
            return Instance;
        }

        public NetworkEvent ConvertToNetworkEvent(IMData data)
        {
            switch(data.ContentType)
            {
                case MatchingResultEvent.CONTENT_TYPE:
                    return data.GetContent<MatchingResultEvent>();
                case AllFrameDataEvent.CONTENT_TYPE:
                    FrameData frameData = data.GetContent<FrameData>();
                    AllFrameDataEvent allFrameDataEvent = new AllFrameDataEvent();
                    allFrameDataEvent.frameData = frameData;
                    return allFrameDataEvent;
                case UpdateFrameDataEvent.CONTENT_TYPE:
                    FrameData updateFrameData = data.GetContent<FrameData>();
                    UpdateFrameDataEvent updateFrameDataEvent = new UpdateFrameDataEvent();
                    updateFrameDataEvent.frameData = updateFrameData;
                    return updateFrameDataEvent;
                    
            }
            return null;
        }

        public LobbyService GetLobbyService()
        {

            if(mLobbyService == null)
            {
                lock(this)
                {
                    if(mLobbyService == null)
                    {
                        mLobbyService = new LobbyServiceImpl();
                    }
                }
                
            }
            return mLobbyService;
        }

        public BalootGameService CreateBalootGameService()
        {
            return CreateRoomService<BalootGameServiceImpl>();
        }
        /**
         * <summary>
         *  同时只能创建一个RoomService， 如果创建第二个RoomService， 会自动关闭上一个RoomService。
         * </summary>
         */
        private T CreateRoomService<T>() 
            where T : RoomService, new()
        {
            if(mRoomService != null)
            {
                try
                {
                    mRoomService.Stop();
                } catch(Exception e)
                {
                    Logger.error(TAG, "Old roomService stop failed, {0}", e.Message);
                }
                
                mRoomService = null;
            }
            mRoomService = new T();
            return (T)mRoomService;
        }
    }
}
