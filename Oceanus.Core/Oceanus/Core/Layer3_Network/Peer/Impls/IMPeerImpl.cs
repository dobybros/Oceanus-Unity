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
using System.Text;
using System.Threading.Tasks;

namespace Oceanus.Core.Network
{
    internal class IMPeerImpl : IMPeer
    {
        readonly string TAG = typeof(IMPeer).Name;
        internal string mToken;
        internal string mHost;
        internal int mPort;
        internal string mUserId;
        internal string mLoginUrl;
        private IMChannel mChannel;
        public static readonly int STATUS_NONE = 1;
        public static readonly int STATUS_CONNECTING = 5;
        public static readonly int STATUS_CONNECTED = 10;
        public static readonly int STATUS_DISCONNECTED = 100;
        public static readonly int STATUS_DESTROYED = 1000;
        internal AtomicInt mStatus;


        //private OnPeerConnected mOnPeerConnectedMethod;
        //private OnPeerDisconnected mOnPeerDisconnectedMethod;

        public event OnPeerConnected OnPeerConnectedEvents;
        public event OnPeerDisconnected OnPeerDisconnectedEvents;
        //public event OnIMResultReceived OnIMResultReceivedEvents;
        public event OnPeerReceivedMessage OnPeerReceivedMessageEvents;
        public event OnPeerReceivedData OnPeerReceivedDataEvents;

        public IMPeerImpl(string userId)
        {
            mUserId = userId;
            mStatus = new AtomicInt(STATUS_NONE);
            //IncomingInvocation incomingInvocation = new global::IncomingInvocation();
        }

        public string test()
        {
            //WebRequest a;
            //WebSocketServer w;
            //CancellationTokenSource a;
            IncomingInvocation incomingInvocation = new IncomingInvocation();
            incomingInvocation.Service = "my service";
            incomingInvocation.Class = "my class";
            incomingInvocation.Method = "method1";

            MemoryStream stream = new MemoryStream();
            incomingInvocation.WriteTo(stream);
            byte[] newData = incomingInvocation.ToByteArray();
            byte[] data = stream.ToArray();

            IncomingInvocation newIncomingInvocation = IncomingInvocation.Parser.ParseFrom(data);
            Hashtable map = new Hashtable();
            map.Add("a", "b");

            string json = JsonMapper.ToJson(map);
            return json;
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public void Start(string loginUrl)
        {
            ValidateUtils.checkNotNull(loginUrl);
            if (mStatus.CompareAndSet(STATUS_NONE, STATUS_CONNECTING))
            {
                mLoginUrl = loginUrl;
                Connect();
            }
            else
            {
                Logger.warn(TAG, "IMClient start on loginUrl {2} illegally, expecting status {0} but actual is {1}", STATUS_NONE, mStatus.Get(), loginUrl);
            }
        }
        public void Start(string host, int port, string token)
        {
            ValidateUtils.checkAllNotNull(host, token);

            if (mStatus.CompareAndSet(STATUS_NONE, STATUS_CONNECTING))
            {
                mHost = host;
                mPort = port;
                mToken = token;
                Connect();
            }
            else
            {
                Logger.warn(TAG, "IMClient start on host {2} illegally, expecting status {0} but actual is {1}", STATUS_NONE, mStatus.Get(), host);
            }
            
        }

        public void Stop()
        {

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
                Logger.error(TAG, "Connect failed because parameters are illegal, mLoginUrl {0} mHost {1}, mPort {2} mToken {3}", mLoginUrl, mHost, mPort, mToken);
            }
        }

        private void LoginToConnectServer(string loginUrl)
        {
            string host = null, token = null;
            int port = 123;
            ConnectToServer(host, port, token);
        }
        private void ConnectToServer(string host, int port, string token)
        {
            if (mChannel != null)
            {
                mChannel.Close();
                mChannel = null;
            }
            mChannel = new WebsocketChannel();
            mChannel.RegisterChannelStatusDelegate((int status, int code) =>
            {
                switch(status)
                {
                    case IMConstants.CHANNEL_STATUS_CONNECTED:
                        SafeUtils.SafeCallback("Peer connected",
                            () => HandleOnPeerConnected()); 
                        break;
                    case IMConstants.CHANNEL_STATUS_DISCONNECTED:
                        SafeUtils.SafeCallback("Peer disconnected",
                           () => HandleOnPeerDisconnected(code));
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

        private void HandleOnPeerConnected()
        {
            //if (mOnPeerConnectedMethod != null)
            //    mOnPeerConnectedMethod();
            OnPeerConnectedEvents();
        }

        private void HandleOnPeerDisconnected(int code)
        {
            //if (mOnPeerDisconnectedMethod != null)
            //    mOnPeerDisconnectedMethod(code);
            OnPeerDisconnectedEvents(code);
        }

        public void RegisterIMMessageObserver<T>(string contentType, IMMessageObserver<T> messageObserver)
        {
            throw new System.NotImplementedException();
        }

        public void UnRegisterIMMessageObserver<T>(string contentType, IMMessageObserver<T> messageObserver)
        {
            throw new System.NotImplementedException();
        }

        public void UnRegisterIMMessageObserver<T>(string contentType)
        {
            throw new System.NotImplementedException();
        }

        public void RegisterIMDataObserver<T>(string contentType, IMDataObserver<T> dataObserver)
        {
            throw new System.NotImplementedException();
        }

        public void UnRegisterIMDataObserver<T>(string contentType, IMDataObserver<T> dataObserver)
        {
            throw new System.NotImplementedException();
        }

        public void UnRegisterIMDataObserver<T>(string contentType)
        {
            throw new System.NotImplementedException();
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
