using LitJson;
using Oceanus.Core.Errors;
using Oceanus.Core.Network;
using Oceanus.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus.Baloot
{
    class BalootGameServiceImpl : BalootGameService
    {
        private const string TAG = nameof(BalootGameServiceImpl);
        public event OnNetworkEventReceived OnNetworkEventReceivedEvents;

        private IMPeer mIMPeer; 
        private AtomicInt ConnectStatus = new AtomicInt(OceanusFactory.CONNECT_STATUS_DISCONNECTED);
        private BalootGameManager mBalootGameManager;

        public void ASHKAL_CONFIRM(int model, OnIMResultReceived onResultReceived = null)
        {
            callActionCheck();
            Hashtable table = new Hashtable();
            table.Add("mode", model);
            //Logger.info(TAG, "ASHKAL_CONFIRM content " + JsonMapper.ToJson(table));
            mIMPeer.Send(table, "AC", onResultReceived);
        }

        public void DOUBLE_LOCK(int lock1, OnIMResultReceived onResultReceived = null)
        {
            callActionCheck();
            Hashtable table = new Hashtable();
            table.Add("lock", lock1);
            //Logger.info(TAG, "ASHKAL_CONFIRM content " + JsonMapper.ToJson(table));
            mIMPeer.Send(table, "DL", onResultReceived);
        }

        public void FIRST_MODEL(int model, OnIMResultReceived onResultReceived = null)
        {
            callActionCheck();
            Hashtable table = new Hashtable();
            table.Add("model", model);
            mIMPeer.Send(table, "FM", onResultReceived);
        }

        public void HOKOMConfirm(int model, OnIMResultReceived onResultReceived = null)
        {
            callActionCheck();
            Hashtable table = new Hashtable();
            table.Add("model", model);
            mIMPeer.Send(table, "HC", onResultReceived);
        }

        public void HOKOM_DOUBLE(int mul, int duel, OnIMResultReceived onResultReceived = null)
        {
            callActionCheck();
            Hashtable table = new Hashtable();
            table.Add("mul", mul);
            table.Add("duel", duel);
            mIMPeer.Send(table, "HD", onResultReceived);
        }

        public void HOKOM_KING(int kind, OnIMResultReceived onResultReceived = null)
        {
            callActionCheck();
            Hashtable table = new Hashtable();
            table.Add("kind", kind);
            mIMPeer.Send(table, "HK", onResultReceived);
        }

        public void HOKOM_SUN(int model, OnIMResultReceived onResultReceived = null)
        {
            callActionCheck();
            Hashtable table = new Hashtable();
            table.Add("model", model);
            mIMPeer.Send(table, "HS", onResultReceived);
        }

        public void PLAYING(int cId, Dictionary<string, int> sCount, int sawa, OnIMResultReceived onResultReceived = null)
        {
            callActionCheck();
            Hashtable table = new Hashtable();
            table.Add("cId", cId);
            table.Add("sCount", sCount);
            table.Add("sawa", sawa);
            mIMPeer.Send(table, "P", onResultReceived);
        }

        public void SECOND_MODEL(int model, OnIMResultReceived onResultReceived = null)
        {
            callActionCheck();
            Hashtable table = new Hashtable();
            table.Add("model", model);
            mIMPeer.Send(table, "SM", onResultReceived);
        }

        public void SUN_DOUBLE(int mul, OnIMResultReceived onResultReceived = null)
        {
            callActionCheck();
            Hashtable table = new Hashtable();
            table.Add("mul", mul);
            mIMPeer.Send(table, "SD", onResultReceived);
        }
        public BalootGameManager GetBalootGameManager()
        {
            return mBalootGameManager;
        }
        public void Start(string userId, string deviceId, int terminal, string host, int port, string jwtToken)
        {
            ValidateUtils.CheckAllNotNull(userId, deviceId, jwtToken);
            ValidateUtils.CheckEqualsAny(terminal, OceanusFactory.TERMINAL_ANDROID, OceanusFactory.TERMINAL_IOS);

            if (mIMPeer != null)
            {
                throw new CoreException(ErrorCodes.ERROR_BALOOT_SERVICE_STARTED_ALREADY, "BalootGameService is started already, can not start again.");
            }
            lock (this)
            {
                if (mIMPeer == null)
                {
                    mBalootGameManager = new BalootGameManagerImpl();
                    IMPeerBuilder builder = IMPeerBuilder.Builder().
                    //AsAndroid().
                    WithUserId(userId).
                    withPrefix("BalootServer").
                    WithDeviceId(deviceId);
                    switch (terminal)
                    {
                        case OceanusFactory.TERMINAL_ANDROID:
                            builder.AsAndroid();
                            break;
                        case OceanusFactory.TERMINAL_IOS:
                            builder.AsIOS();
                            break;
                    }
                    mIMPeer = builder.Build();
                    mIMPeer.OnPeerConnectedEvents += () =>
                    {
                        //User user = new User()
                        //{
                        //    id = "234",
                        //    name = "dfafd"
                        //};
                        //mClient.Send(user, "user", (IMResult result) => {
                        //    Logger.info("aaa", "result " + result);
                        //}, 8);
                        ConnectStatus.Set(OceanusFactory.CONNECT_STATUS_CONNECTED);

                        SafeUtils.SafeCallback("Baloot Connected", () =>
                        {
                            NetworkEvent networkEvent = new ConnectedEvent();
                            OnNetworkEventReceivedEvents(networkEvent);
                        });

                    };
                    mIMPeer.OnPeerDisconnectedEvents += (int code) =>
                    {
                        ConnectStatus.Set(OceanusFactory.CONNECT_STATUS_DISCONNECTED);

                        SafeUtils.SafeCallback("Baloot Disconnected", () =>
                        {
                            NetworkEvent networkEvent = new DisconnectedEvent();
                            OnNetworkEventReceivedEvents(networkEvent);
                        });
                    };
                    mIMPeer.OnPeerReceivedDataEvents += (IMData data) =>
                    {
                        //Logger.info(TAG, "Data received contentType {0} id {1} time {2} content {3}", data.ContentType, data.Id, data.Time, data.GetContent<string>());
                        SafeUtils.SafeCallback("Baloot data received, type " + data.ContentType, () =>
                        {
                            NetworkEvent theNetworkEvent = OceanusFactory.GetInstance().ConvertToNetworkEvent(data);
                            if (theNetworkEvent != null)
                            {
                                switch(theNetworkEvent.Type)
                                {
                                    case NetworkEvent.TYPE_BALOOT_ALL_FRAME_DATA:
                                        mBalootGameManager.InitAllFrameData(((AllFrameDataEvent)theNetworkEvent).frameData);
                                        break;
                                    case NetworkEvent.TYPE_BALOOT_UPDATE_FRAME_DATA:
                                        mBalootGameManager.UpdateFrameData(((UpdateFrameDataEvent)theNetworkEvent).frameData);
                                        break;
                                    default:
                                        OnNetworkEventReceivedEvents(theNetworkEvent);
                                        break;
                                }
                            }
                            else
                            {
                                Logger.error(TAG, "Unknown IMData received, " + data.ContentType + ". Will be ignored");
                            }
                        });
                    };
                    mIMPeer.OnPeerReceivedMessageEvents += (IMMessage message) =>
                    {
                        //Logger.info(TAG, "Message received contentType {0} id {1} time {2} userId {3} groupId {4} content {5}", message.ContentType, message.Id, message.Time, message.UserId, message.GroupId, message.GetContent<string>());
                        SafeUtils.SafeCallback("Baloot message received, type " + message.ContentType, () =>
                        {
                            NetworkEvent theNetworkEvent = OceanusFactory.GetInstance().ConvertToNetworkEvent(message);
                            if (theNetworkEvent != null)
                            {
                                OnNetworkEventReceivedEvents(theNetworkEvent);
                            }
                            else
                            {
                                Logger.error(TAG, "Unknown IMMessage received, " + message.ContentType + ". Will be ignored");
                            }

                        });
                    };
                    ////接入大厅服务器
                    //mIMPeer.Start(Settings.LoginGatewayUrl, jwtToken);
                    mIMPeer.Start(host, port, jwtToken);
                    Logger.info(TAG, "Start connecting baloot server {0} port {1}", host, port);
                }
            }
        }

        public void Stop()
        {
            if(mIMPeer != null)
            {
                mIMPeer.Stop();
                mIMPeer = null;
            }
        }

        private void callActionCheck()
        {
            if (ConnectStatus.Get() != OceanusFactory.CONNECT_STATUS_CONNECTED)
                throw new CoreException(ErrorCodes.ERROR_IMPEER_ROOM_NOT_CONNECTED, "IMPeer not connected, can not invoke any action now. ");
        }
    }
}
