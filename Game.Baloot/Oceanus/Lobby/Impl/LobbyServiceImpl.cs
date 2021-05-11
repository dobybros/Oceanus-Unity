using LitJson;
using Oceanus.Core.Errors;
using Oceanus.Core.Network;
using Oceanus.Core.Utils;
using System;
using System.Collections;
using System.Net;
using System.Text;
using System.Threading;

namespace NetWork.Oceanus
{
    internal class LoginRequest
    {
        public const string TYPE_GUEST = "guest";
        public string account;
        public string type;
        public string deviceId;
        public string model;
        public string os;
        public string network;
    }
    internal class LoginGatewayRequest
    {
        public string userId;
        public string deviceId;
        public int terminal;
        public bool active;
        public string jwtToken;
    }
    class LobbyServiceImpl : LobbyService
    {
        private static readonly string TAG = typeof(LobbyServiceImpl).Name;
        private PlayerGameStatusManager mPlayerGameStatusManager;
        public event OnNetworkEventReceived OnNetworkEventReceivedEvents;
        private IMPeer mIMPeer;
        
        private AtomicInt ConnectStatus = new AtomicInt(OceanusFactory.CONNECT_STATUS_DISCONNECTED);
        public PlayerGameStatusManager GetPlayerGameStatusManager()
        {
            if (mPlayerGameStatusManager == null)
            {
                lock (this)
                {
                    if (mPlayerGameStatusManager == null)
                    {
                        mPlayerGameStatusManager = new PlayerGameStatusManagerImpl();
                    }
                }

            }
            return mPlayerGameStatusManager;
        }

        private LoginResult LoginGuestOnThread(string deviceId, string model, string os, string network)
        {
            HttpWebRequest HttpWebRequest = (HttpWebRequest)WebRequest.Create(Settings.LoginUrl);
            HttpWebRequest.Method = "POST";
            HttpWebRequest.ReadWriteTimeout = 30000;

            LoginRequest loginRequest = new LoginRequest();
            loginRequest.account = deviceId;
            loginRequest.deviceId = deviceId;
            loginRequest.type = LoginRequest.TYPE_GUEST;
            loginRequest.model = model;
            loginRequest.network = network;
            loginRequest.os = os;
            
            string json = JsonMapper.ToJson(loginRequest);
            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest.ContentType = "application/json";
            using (var stream = HttpWebRequest.GetRequestStream())
            {
                stream.Write(body, 0, body.Length);
            };

            try
            {
                using (HttpWebResponse webresp = (HttpWebResponse)HttpWebRequest.GetResponse())
                {
                    if (webresp.StatusCode != HttpStatusCode.OK)
                    {
                        throw new CoreException(ErrorCodes.ERROR_NETWORK_LOGIN_FAILED, "Login status code failed, " + (int)webresp.StatusCode);
                    }
                    string responseStr = SafeUtils.StreamToString(webresp.GetResponseStream(), Encoding.UTF8);
                    if (responseStr == null || responseStr.Length == 0)
                        throw new CoreException(ErrorCodes.ERROR_NETWORK_LOGIN_FAILED, "Login response is empty");
                    ResponseData<LoginResult> responseData;
                    try
                    {
                        responseData = JsonMapper.ToObject<ResponseData<LoginResult>>(responseStr);
                    }
                    catch (Exception e)
                    {
                        throw new CoreException(ErrorCodes.ERROR_JSON_PARSE_FAILED, "Parse login responseStr json failed, " + e.Message + ":" + responseStr);
                    }
                    if (responseData.code != 1)
                    {
                        throw new CoreException(responseData.code, "Server error, code " + responseData.code + " message " + responseData.message);
                    }
                    return responseData.data;
                }
            }
            catch (WebException e)
            {
                //请求失败
                throw new CoreException(ErrorCodes.ERROR_NETWORK_LOGIN_FAILED, "Login failed, " + e.Message);
            }
        }
        public void LoginGuest(string deviceId, string model, string os, string network, OnNetworkResultReceived<LoginResult> onResultReceived)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
            {
                try{
                    LoginResult loginResult = LoginGuestOnThread(deviceId, model, os, network);
                    NetworkResult<LoginResult> networResult = new NetworkResult<LoginResult>(loginResult);
                    networResult.Code = 1;
                    onResultReceived.Invoke(networResult);
                } catch(Exception e)
                {
                    int code = ErrorCodes.ERROR_UNKNOWN;
                    if (e.GetType().Equals(typeof(CoreException))) {
                        CoreException coreException = (CoreException)e;
                        code = coreException.Code;
                    }
                    NetworkResult<LoginResult> networResult = new NetworkResult<LoginResult>();
                    networResult.Code = code;
                    networResult.Description = e.Message;
                    onResultReceived.Invoke(networResult);
                }
            }));
        }

        public void Start(string userId, string deviceId, int terminal, string jwtToken)
        {
            ValidateUtils.CheckAllNotNull(userId, deviceId, jwtToken);
            ValidateUtils.CheckEqualsAny(terminal, OceanusFactory.TERMINAL_ANDROID, OceanusFactory.TERMINAL_IOS);

            if(mIMPeer != null)
            {
                throw new CoreException(ErrorCodes.ERROR_LOBBY_SERVICE_STARTED_ALREADY, "LobbyService is started already, can not start again.");
            }
            lock (this)
            {
                if (mIMPeer == null)
                {
                    IMPeerBuilder builder = IMPeerBuilder.Builder().
                    //AsAndroid().
                    WithUserId(userId).
                    withPrefix("LobbyServer").
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
                    mIMPeer.OnPeerConnectedEvents += ConnectedHandler;
                    mIMPeer.OnPeerDisconnectedEvents += DisconnectedHandler;
                    mIMPeer.OnPeerReceivedDataEvents += ReceivedDataHandler;
                    mIMPeer.OnPeerReceivedMessageEvents += ReceivedMessageHandler;
                    mIMPeer.OnPeerShuttedDownEvents += ShuttedDownHandler;
                    //接入大厅服务器
                    mIMPeer.Start(Settings.LoginGatewayUrl, jwtToken);
                    Logger.info(TAG, "LobbyService started");
                }
            }
        }

        public void Stop()
        {
            if(mIMPeer != null)
            {
                lock(this)
                {
                    if(mIMPeer != null)
                    {
                        try
                        {
                            mIMPeer.OnPeerConnectedEvents -= ConnectedHandler;
                            mIMPeer.OnPeerDisconnectedEvents -= DisconnectedHandler;
                            mIMPeer.OnPeerReceivedDataEvents -= ReceivedDataHandler;
                            mIMPeer.OnPeerReceivedMessageEvents -= ReceivedMessageHandler;
                            mIMPeer.Stop();
                        } catch(Exception e)
                        {
                            Logger.error(TAG, "Stop imPeer failed, " + e.Message + " but the stop will be considered successfully");
                        } finally
                        {
                            mIMPeer = null;
                            Logger.info(TAG, "LobbyService stopped");
                        }
                    }
                }
            }
        }
        private void ShuttedDownHandler(int code)
        {
            Logger.info(TAG, "disconnected");
            ConnectStatus.Set(OceanusFactory.CONNECT_STATUS_SHUTTEDDOWN);

            SafeUtils.SafeCallback("Lobby Disconnected code " + code, () =>
            {
                ShuttedDownEvent networkEvent = new ShuttedDownEvent();
                networkEvent.Code = code;
                OnNetworkEventReceivedEvents(networkEvent);
            });
        }
        private void ReceivedMessageHandler(IMMessage message)
        {
            //Logger.info(TAG, "Message received contentType {0} id {1} time {2} userId {3} groupId {4} content {5}", message.ContentType, message.Id, message.Time, message.UserId, message.GroupId, message.GetContent<string>());
            SafeUtils.SafeCallback("Lobby message received, type " + message.ContentType, () =>
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
        }

        private void ReceivedDataHandler(IMData data)
        {
            Logger.info(TAG, "Data received contentType {0} id {1} time {2} content {3}", data.ContentType, data.Id, data.Time, data.GetContent<string>());
            SafeUtils.SafeCallback("Lobby data received, type " + data.ContentType, () =>
            {
                NetworkEvent theNetworkEvent = OceanusFactory.GetInstance().ConvertToNetworkEvent(data);
                if (theNetworkEvent != null)
                {
                    OnNetworkEventReceivedEvents(theNetworkEvent);
                }
                else
                {
                    Logger.error(TAG, "Unknown IMData received, " + data.ContentType + ". Will be ignored");
                }
            });
        }

        private void DisconnectedHandler(int code)
        {
            Logger.info(TAG, "disconnected");
            ConnectStatus.Set(OceanusFactory.CONNECT_STATUS_DISCONNECTED);

            SafeUtils.SafeCallback("Lobby Disconnected code " + code, () =>
            {
                DisconnectedEvent networkEvent = new DisconnectedEvent();
                networkEvent.Code = code;
                OnNetworkEventReceivedEvents(networkEvent);
            });
        }

        private void ConnectedHandler()
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

            SafeUtils.SafeCallback("Lobby Connected", () =>
            {
                NetworkEvent networkEvent = new ConnectedEvent();
                OnNetworkEventReceivedEvents(networkEvent);
            });
        }

        public void TakeBankruptProtection(OnIMResultReceived onResultReceived)
        {
            throw new NotImplementedException();
        }

        public void TakeCheckInReward(int dayNumber, OnIMResultReceived onResultReceived)
        {
            throw new NotImplementedException();
        }

        public void StartMatching(string service, string group, OnIMResultReceived onResultReceived)
        {
            callActionCheck();
            Hashtable map = new Hashtable();
            map["service"] = service;
            map["group"] = group;
            mIMPeer.Send(map, "startMatching", onResultReceived, 10);
        }
        public void CancelMatching(OnIMResultReceived onResultReceived)
        {
            callActionCheck();
            Hashtable map = new Hashtable();
            mIMPeer.Send(map, "cancelMatching", onResultReceived, 10);
        }
        private void callActionCheck()
        {
            if (ConnectStatus.Get() != OceanusFactory.CONNECT_STATUS_CONNECTED)
                throw new CoreException(ErrorCodes.ERROR_IMPEER_NOT_CONNECTED, "IMPeer not connected, can not invoke any action now. ");
        }

    }
}
