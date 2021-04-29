using System;

namespace Oceanus.Test
{
    using Oceanus.Core.Network;
    using Oceanus.Core.Utils;
    using System.Collections;
    using System.Threading.Tasks;

    class User
    {
        public string name
        {
            get; set;
        }
        public string id
        {
            get; set;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            IMPeer mClient = IMPeerBuilder.Builder().WithUserId("xiaocai").Build();
            mClient.OnPeerConnectedEvents += () =>
            {
                User user = new User()
                {
                    id = "234",
                    name = "dfafd"
                };
                mClient.Send(user, "user", (IMResult result) => {
                    Logger.info("aaa", "result " + result);
                }, 8);

            };
            mClient.OnPeerDisconnectedEvents += (int code) =>
            {
                Logger.info("aaa", "disconnected");
            };
            mClient.OnPeerReceivedDataEvents += (IMData data) =>
            {
                Logger.info("aaa", "Data received contentType {0} id {1} time {2} content {3}", data.ContentType, data.Id, data.Time, data.GetContent<string>());
            };
            mClient.OnPeerReceivedMessageEvents += (IMMessage message) =>
            {
                Logger.info("aaa", "Message received contentType {0} id {1} time {2} userId {3} groupId {4} content {5}", message.ContentType, message.Id, message.Time, message.UserId, message.GroupId, message.GetContent<string>());
            };
            mClient.Start("aplomb.acucom.net", 2443,
                "eyJhbGciOiJIUzI1NiJ9.eyJleHAiOjE2MTk4MDM5NDUsInVzZXIiOiJ7XCJnXCI6XCJncm91cDFcIixcImlkXCI6XCJ0ZXN0X18wXCIsXCJyaWRcIjpcIjYwOGE3YmQ4Y2ZiMmI5ZmQyNzhhOGFjYlwiLFwic1wiOlwiNDk2NTg3NDkyXCIsXCJzdlwiOlwiZ3dnYW1lMjFcIn0iLCJpYXQiOjE2MTk3MDM5NDV9.HMyrS06iK_IUJY3MM4j7p-d9NutJIFm8copXu_GtcC0");



            Console.ReadKey();
        }
    }
}
