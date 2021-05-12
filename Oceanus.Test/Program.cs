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
            IMPeer mClient = IMPeerBuilder.Builder().
                AsAndroid().
                WithUserId("60921c89cfb2b901a6f54b24").
                WithDeviceId("android").
                Build();
            mClient.OnPeerConnectedEvents += () =>
            {
                User user = new User()
                {
                    id = "234",
                    name = "dfafd"
                };
                mClient.Send(user, "user", (IMResult result) => {
                    OceanusLogger.info("aaa", "result " + result);
                }, 8);

            };
            mClient.OnPeerDisconnectedEvents += (int code) =>
            {
                OceanusLogger.info("aaa", "disconnected");
            };
            mClient.OnPeerReceivedDataEvents += (IMData data) =>
            {
                OceanusLogger.info("aaa", "Data received contentType {0} id {1} time {2} content {3}", data.ContentType, data.Id, data.Time, data.GetContent<string>());
            };
            mClient.OnPeerReceivedMessageEvents += (IMMessage message) =>
            {
                OceanusLogger.info("aaa", "Message received contentType {0} id {1} time {2} userId {3} groupId {4} content {5}", message.ContentType, message.Id, message.Time, message.UserId, message.GroupId, message.GetContent<string>());
            };
            //接入房间服务器
            //mClient.Start("aplomb.acucom.net", 2443,
            //    "eyJhbGciOiJIUzI1NiJ9.eyJleHAiOjE2MTk4NDk4NTksInVzZXIiOiJ7XCJnXCI6XCJncm91cDFcIixcImlkXCI6XCJ0ZXN0X18wXCIsXCJyaWRcIjpcIjYwOGI2YmUzY2ZiMmI5ZmQyNzhhOGFlNlwiLFwic1wiOlwiNDk2NTg3NDkyXCIsXCJzdlwiOlwiZ3dnYW1lMjFcIn0iLCJpYXQiOjE2MTk3NDk4NTl9.DGo7vHKVKmsa1zkLSDg0yJ-w1yIABasELRA8u3kkvN4");
            //接入大厅服务器
            mClient.Start("https://aplomb.acucom.net/rest/goldcentral/gateway/login", 
                "eyJhbGciOiJIUzI1NiJ9.eyJ0aW1lIjoxNjIwMjg0ODEyNzM3LCJ1c2VySWQiOiI2MDkyMWM4OWNmYjJiOTAxYTZmNTRiMjQiLCJpbmFjdGl2ZURheXMiOjcsImlhdCI6MTYyMDI4NDgxMn0.lPrxSrb7B4NnNP3mf0sr9NxaTi9gzWF9YtuE0tYteQg");


            Console.ReadKey();
        }
    }
}
