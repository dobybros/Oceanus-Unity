
using Oceanus.Core.Utils;
using System.Threading.Tasks;

namespace Oceanus.Core.Network
{
    public delegate void OnPeerConnected();
    public delegate void OnPeerDisconnected(int code);
    public delegate void OnIMResultReceived(IMResult result);

    public delegate void OnPeerReceivedMessage(IMMessage message);
    public delegate void OnPeerReceivedData(IMData data);

    public interface IMPeer
    {
        event OnPeerConnected OnPeerConnectedEvents;
        event OnPeerDisconnected OnPeerDisconnectedEvents;
        event OnPeerReceivedMessage OnPeerReceivedMessageEvents;
        event OnPeerReceivedData OnPeerReceivedDataEvents;
        //event OnIMResultReceived OnIMResultReceivedEvents;

        void Start(string loginUrl, string jwtToken);
        void Start(string host, int port, string token);
        void Stop();
        /**
         * <summary>
         * If channel is disconnected, send failed will be return immediately. 
         * If channel is connected, when send time exceeded sendTimeoutSeconds, the timeout error will be returned. 
         * </summary>
         * 
         * <param name="content">any content, will be serialized with json format</param>
         * <param name="contentType">content type of the content</param>
         * <param name="id">any id, used for find corresponding IMResult. Also server will use the id to defend repeated message sending.</param>
         * <param name="sendTimeoutSeconds">send timeout seconds, only available when channel is connected</param>
         */
        void Send(object content, string contentType, OnIMResultReceived onIMResultReceivedMethod = null, int sendTimeoutSeconds = 0);
    }
    public class IMPeerBuilder
    {
        private string mUserId;
        private string mDeviceId;
        private int mTermianl;
        public static IMPeerBuilder Builder()
        {
            return new IMPeerBuilder();
        }
        public IMPeerBuilder WithUserId(string userId)
        {
            this.mUserId = userId;
            return this;
        }
        public IMPeerBuilder WithDeviceId(string deviceId)
        {
            this.mDeviceId = deviceId;
            return this;
        }
        public IMPeerBuilder AsAndroid()
        {
            mTermianl = IMConstants.TERMINAL_ANDROID;
            return this;
        }
        public IMPeerBuilder AsIOS()
        {
            mTermianl = IMConstants.TERMINAL_IOS;
            return this;
        }
        public IMPeer Build()
        {
            ValidateUtils.CheckAllNotNull(mUserId, mDeviceId);
            ValidateUtils.CheckEqualsAny(mTermianl, IMConstants.TERMINAL_ANDROID, IMConstants.TERMINAL_IOS);

            return (IMPeer) new IMPeerImpl(mUserId, mDeviceId, mTermianl);
        }
    }
    
}
