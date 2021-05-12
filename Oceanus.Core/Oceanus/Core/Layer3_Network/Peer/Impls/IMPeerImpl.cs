using Google.Protobuf;
using LitJson;
using Oceanus.Core.Errors;
using Oceanus.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Oceanus.Core.Network
{
    internal class LoginRequest
    {
        public string userId;
        public string deviceId;
        public int terminal;
        public bool active;
        public string jwtToken;
    }
    internal class IMPeerImpl : IMPeer
    {
        readonly string TAG = typeof(IMPeer).Name;
        internal string mToken;
        internal string mHost;
        internal int mPort;
        internal string mUserId;
        internal string mDeviceId;
        internal int mTerminal;
        internal string mLoginUrl;
        internal string mJwtToken;
        internal string mPrefix;

        internal bool mActiveLogin = false;
        private IMChannel mChannel;
        public static readonly int STATUS_NONE = 1;
        public static readonly int STATUS_CONNECTING = 5;
        public static readonly int STATUS_CONNECTED = 10;
        public static readonly int STATUS_DISCONNECTED = 100;
        public static readonly int STATUS_DESTROYED = 1000;
        internal AtomicInt mStatus;
        internal AtomicInt retryCounter;
        protected object mLock = new object();
        private List<int> mShutdownErrorCodes;

        public event OnPeerConnected OnPeerConnectedEvents;
        public event OnPeerDisconnected OnPeerDisconnectedEvents;
        //public event OnIMResultReceived OnIMResultReceivedEvents;
        public event OnPeerReceivedMessage OnPeerReceivedMessageEvents;
        public event OnPeerReceivedData OnPeerReceivedDataEvents;
        public event OnPeerShuttedDown OnPeerShuttedDownEvents;

        public IMPeerImpl(string userId, string deviceId, int terminal, string prefix, List<int> shutdownErrorCodes = null)
        {
            ValidateUtils.CheckNotNull(userId);
            ValidateUtils.CheckEqualsAny(terminal, IMConstants.TERMINAL_ANDROID, IMConstants.TERMINAL_IOS);

            mShutdownErrorCodes = shutdownErrorCodes;
            mUserId = userId;
            mDeviceId = deviceId;
            mTerminal = terminal;
            mPrefix = prefix;
            mStatus = new AtomicInt(STATUS_NONE);
            retryCounter = new AtomicInt(-1);
            //IncomingInvocation incomingInvocation = new global::IncomingInvocation();
        }

        private void StartThread()
        {
            ThreadStart threadStart = new ThreadStart(Run);
            Thread thread = new Thread(threadStart);
            thread.IsBackground = true;
            thread.Start();
        }

        private void Run()
        {
            while(mStatus.Get() != STATUS_DESTROYED)
            {
                if(mStatus.Get() == STATUS_CONNECTED)
                {
                    retryCounter.Set(-1);
                    try
                    {
                        OceanusLogger.info(TAG, mPrefix + ": Connected, will wait {0} seconds to check again", IMConstants.CONFIG_CHANNEL_CONNECTED_MAX_WAIT_SECNODS);
                        Monitor.Enter(mLock);
                        Monitor.Wait(mLock, TimeSpan.FromSeconds(IMConstants.CONFIG_CHANNEL_CONNECTED_MAX_WAIT_SECNODS));
                        OceanusLogger.info(TAG, mPrefix + ": Connected, wakeup to check again");
                    }
                    finally
                    {
                        Monitor.Exit(mLock);
                    }
                    continue;
                }
                try
                {
                    mStatus.Set(STATUS_CONNECTING);
                    retryCounter.Increment();
                    Connect();

                    OceanusLogger.info(TAG, mPrefix + ": Connected, waiting identity result {0} seconds, retryCounter {1}", IMConstants.CONFIG_CHANNEL_IDENTITY_RESULT_TIMEOUT_SECNODS, retryCounter.Get());
                    try
                    {
                        Monitor.Enter(mLock);
                        Monitor.Wait(mLock, TimeSpan.FromSeconds(IMConstants.CONFIG_CHANNEL_IDENTITY_RESULT_TIMEOUT_SECNODS));
                    }
                    finally
                    {
                        Monitor.Exit(mLock);
                    }
                }
                catch (Exception e)
                {
                    OceanusLogger.error(TAG, mPrefix + ": Connect failed, " + e.Message + " will retry " + retryCounter.Get());
                    OceanusLogger.info(TAG, mPrefix + ": Sleep {0} seconds...", IMConstants.CONFIG_CHANNEL_ERROR_RETRY_SECNODS);
                    Thread.Sleep(TimeSpan.FromSeconds(IMConstants.CONFIG_CHANNEL_ERROR_RETRY_SECNODS)); 
                    OceanusLogger.info(TAG, mPrefix + ": Sleep is finished");
                }
            }
            OceanusLogger.info(TAG, "IMPeer {0} is destroyed", mPrefix);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start(string loginUrl, string jwtToken)
        {
            ValidateUtils.CheckNotNull(loginUrl);
            if (mStatus.CompareAndSet(STATUS_NONE, STATUS_CONNECTING))
            {
                mActiveLogin = true;
                mLoginUrl = loginUrl;
                mJwtToken = jwtToken;
                StartThread();
            }
            else
            {
                OceanusLogger.warn(TAG, mPrefix + ": IMClient start on loginUrl {2} illegally, expecting status {0} but actual is {1}", STATUS_NONE, mStatus.Get(), loginUrl);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start(string host, int port, string token)
        {
            ValidateUtils.CheckAllNotNull(host, token);

            if (mStatus.CompareAndSet(STATUS_NONE, STATUS_CONNECTING))
            {
                mHost = host;
                mPort = port;
                mToken = token;
                StartThread();
            }
            else
            {
                OceanusLogger.warn(TAG, mPrefix + ": IMClient start on host {2} illegally, expecting status {0} but actual is {1}", STATUS_NONE, mStatus.Get(), host);
            }
            
        }

        public void Stop()
        {
            mStatus.Set(STATUS_DESTROYED);
            if (mChannel != null)
            {
                mChannel.Close();
            }
        }

        private void Connect()
        {
            if (mLoginUrl != null)
            {
                LoginToConnectServer(mLoginUrl);
            }
            else if (mHost != null && mToken != null)
            {
                ConnectToServer(mHost, mPort, mToken);
            }
            else
            {
                OceanusLogger.error(TAG, mPrefix + ": Connect failed because parameters are illegal, mLoginUrl {0} mHost {1}, mPort {2} mToken {3}", mLoginUrl, mHost, mPort, mToken);
            }
        }

        private void LoginToConnectServer(string loginUrl)
        {
            HttpWebRequest HttpWebRequest = (HttpWebRequest)WebRequest.Create(loginUrl);
            HttpWebRequest.Method = "POST";
            HttpWebRequest.ReadWriteTimeout = 30000;
            LoginRequest loginRequest = new LoginRequest
            {
                userId = mUserId,
                deviceId = mDeviceId,
                terminal = mTerminal,
                active = mActiveLogin, 
                jwtToken = mJwtToken
            };
            string json = JsonMapper.ToJson(loginRequest);
            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest.ContentType = "application/json";
            using (var stream = HttpWebRequest.GetRequestStream())
            {
                stream.Write(body, 0, body.Length);
            }
            ;
            
            try
            {
                using (HttpWebResponse webresp = (HttpWebResponse)HttpWebRequest.GetResponse())
                {
                    if (webresp.StatusCode != HttpStatusCode.OK)
                    {
                        throw new CoreException(ErrorCodes.ERROR_NETWORK_LOGIN_FAILED, mPrefix + ": Login status code failed, " + (int)webresp.StatusCode);
                    }
                    string responseStr = SafeUtils.StreamToString(webresp.GetResponseStream(), Encoding.UTF8);
                    if (responseStr == null || responseStr.Length == 0)
                        throw new CoreException(ErrorCodes.ERROR_NETWORK_LOGIN_FAILED, mPrefix + ": Login response is empty");
                    ResponseData<IMLoginInfo> responseData;
                    try
                    {
                        responseData = JsonMapper.ToObject<ResponseData<IMLoginInfo>>(responseStr);
                    }
                    catch (Exception e)
                    {
                        throw new CoreException(ErrorCodes.ERROR_JSON_PARSE_FAILED, mPrefix + ": Parse login responseStr json failed, " + e.Message + ":" + responseStr);
                    }
                    if (responseData.code != 1)
                    {
                        throw new CoreException(responseData.code, mPrefix + ": Server error, code " + responseData.code + " message " + responseData.message);
                    }
                    OceanusLogger.info(TAG, mPrefix + ": LoginToConnectServer on url {0}, host {1}, port {2}, token {3}", loginUrl, responseData.data.host, responseData.data.port, responseData.data.token);
                    ConnectToServer(responseData.data.host, responseData.data.port, responseData.data.token);
                }
            }
            catch (WebException e)
            {
                //请求失败
                throw new CoreException(ErrorCodes.ERROR_NETWORK_LOGIN_FAILED, mPrefix + ": Login failed, " + e.Message);
            }
        }
        private void ConnectToServer(string host, int port, string token)
        {
            OceanusLogger.info(TAG, mPrefix + ": ConnectToServer host " + host + " port " + port);
            if (mChannel != null)
            {
                mChannel.Close();
                mChannel = null;
            }
            mChannel = new WebsocketChannel(mPrefix);
            mChannel.RegisterChannelStatusDelegate((IMChannel channel, int status, int code) =>
            {
                switch(status)
                {
                    case IMConstants.CHANNEL_STATUS_CONNECTED:
                        SafeUtils.SafeCallback(mPrefix + ": Peer connected",
                            () => HandleOnPeerConnected(channel)); 
                        break;
                    case IMConstants.CHANNEL_STATUS_DISCONNECTED:
                        SafeUtils.SafeCallback(mPrefix + ": Peer disconnected",
                           () => HandleOnPeerDisconnected(channel, code));
                        break;
                }
            });
            mChannel.RegisterDataDelegate((string content, string contentType, string id, long time) =>
            {
                OnPeerReceivedDataEvents(new IMData()
                {
                    Content = content, 
                    ContentType = contentType, 
                    Id = id, 
                    Time = time, 
                });
            });
            mChannel.RegisterMessageDelegate((string content, string contentType, string id, string userId, string groupId, long time) =>
            {
                OnPeerReceivedMessageEvents(new IMMessage()
                {
                    Content = content,
                    ContentType = contentType,
                    Id = id, 
                    UserId = userId, 
                    GroupId = groupId, 
                    Time = time,
                }) ;
            });
            mChannel.Connect(host, port, token);
            //Task task = Task.Run(() => { 

            //});
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void HandleOnPeerConnected(IMChannel chanel)
        {
            //if (mOnPeerConnectedMethod != null)
            //    mOnPeerConnectedMethod();
            if(chanel == mChannel)
            {
                if(mStatus.Get() != STATUS_CONNECTED)
                {
                    if(mActiveLogin)
                        mActiveLogin = false;
                    mStatus.Set(STATUS_CONNECTED);
                    SafeUtils.SafeCallback(mPrefix + ": HandleOnPeerConnected call OnPeerConnectedEvents, connected", () =>
                    {
                        OnPeerConnectedEvents();
                    });
                }
                else
                {
                    OceanusLogger.warn(TAG, mPrefix + ": HandleOnPeerConnected illegal, already connected");
                }
            } else
            {
                OceanusLogger.warn(TAG, mPrefix + ": Old channel HandleOnPeerConnected " + chanel + " new " + mChannel);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void HandleOnPeerDisconnected(IMChannel channel, int code)
        {
            //if (mOnPeerDisconnectedMethod != null)
            //    mOnPeerDisconnectedMethod(code);
            if(channel == mChannel)
            {
                bool needWakeup = false;
                if(mShutdownErrorCodes != null && mShutdownErrorCodes.Contains(code))
                {
                    mStatus.Set(STATUS_DESTROYED);
                    SafeUtils.SafeCallback(mPrefix + ": HandleOnPeerDisconnected call OnPeerShuttedDownEvents code " + code + " will NOT reconnect", () =>
                    {
                        OnPeerShuttedDownEvents(code);
                    });
                    needWakeup = true;
                } else if (mStatus.Get() != STATUS_DISCONNECTED)
                {
                    mStatus.Set(STATUS_DISCONNECTED);
                    SafeUtils.SafeCallback(mPrefix + ": HandleOnPeerDisconnected call OnPeerDisconnectedEvents code " + code, () =>
                    {
                        OnPeerDisconnectedEvents(code);
                    });
                    needWakeup = true;
                }
                else
                {
                    OceanusLogger.warn(TAG, mPrefix + ": HandleOnPeerDisconnected illegal, disconnected already " + " code " + code);
                }
                if(needWakeup)
                {
                    try
                    {
                        Monitor.Enter(mLock);
                        Monitor.PulseAll(mLock);
                    }
                    finally
                    {
                        Monitor.Exit(mLock);
                    }
                }
            } else
            {
                OceanusLogger.warn(TAG, mPrefix + ": Old channel HandleOnPeerDisconnected " + channel + " new " + mChannel + " code " + code);
            }
            
           
        }
        public void Send(object content, string contentType, OnIMResultReceived onIMResultReceivedMethod, int sendTimeoutSeconds)
        {
            if(mChannel == null || mChannel.Status() != IMConstants.CHANNEL_STATUS_CONNECTED)
            {
                onIMResultReceivedMethod(new IMResult()
                {
                    //ForId = Guid.NewGuid().ToString("N"),
                    Code = ErrorCodes.ERROR_NETWORK_DISCONNECTED,
                    Description = "Channel disconnected",
                    Time = SafeUtils.CurrentTimeMillis(),
                });
                return;
                //throw new CoreException(ErrorCodes.ERROR_NETWORK_DISCONNECTED, "Channel disconnected");
            }
            
            IMResultAction resultAction = new IMResultAction()
            {
                Content = content,
                ContentType = contentType,
                OnIMResultReceivedMethod = onIMResultReceivedMethod,
                Id = Guid.NewGuid().ToString("N"),
                SendTimeoutSeconds = sendTimeoutSeconds,
            };

            mChannel.Send(resultAction);
        }

        //public void RegisterPeerConnectedDelegate(OnPeerConnected onPeerConnectedMethod)
        //{
        //    mOnPeerConnectedMethod = onPeerConnectedMethod;
        //}

        //public void RegisterPeerDisconnectedDelegate(OnPeerDisconnected onPeerDisconnectedMethod)
        //{
        //    mOnPeerDisconnectedMethod = onPeerDisconnectedMethod;
        //}
    }
}
