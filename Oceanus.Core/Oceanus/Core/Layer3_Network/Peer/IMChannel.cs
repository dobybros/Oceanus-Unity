using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceanus.Core.Network
{
    internal delegate void OnReceivedMessage(string content, string contentType, string id, string userId, string groupId, long time);
    internal delegate void OnReceivedData(string content, string contentType, string id, long time);
    internal delegate void OnChannelStatus(IMChannel chanel, int status, int code);
    /**
     * <summary>
     * IMChannel defines behaviors of a channel. 
     * The Channel can be implemented by TCP, Websocket, UDP, etc.
     * 
     * IMChannel is one time lifecircle. Will be used once only. When reconnect, another IMChannel instance will be created. 
     * In this way, the coding can be simplified and less bugs. 
     * </summary>
     * 
     */
    interface IMChannel
    {
        /**
         * <summary>
         * Connect to Server with host, port and token
         * </summary>
         * 
         */
        void Connect(string host, int port, string token);
        /**
         * <summary>
         * send message to server
         * </summary>
         * 
         * <param name="contentType">Content type of the content</param>
         * <param name="content">Any content, will be serialized to json string before sending</param>
         * <param name="id">Optional field, if id is given, the Result data will be send back with forId field equal the given id. </param>
         * <param name="sendTimeoutSeconds">send timeout seconds when send a message, if timeout, the channel will be re-established to send again</param>
         */
        void Send(IMResultAction resultAction);

        void RegisterMessageDelegate(OnReceivedMessage onReceivedMessageMethod);
        void RegisterDataDelegate(OnReceivedData onReceivedDataMethod);

        void RegisterChannelStatusDelegate(OnChannelStatus onChannelStatusMethod);
        /**
         * Close channel
         */
        void Close();

        int Status();
    }

}
