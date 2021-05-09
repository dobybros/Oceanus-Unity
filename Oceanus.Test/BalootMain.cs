using System;

namespace Oceanus.Test
{
    using LitJson;
    using NetWork.Oceanus;
    using NetWork.Oceanus.Baloot;
    using Oceanus.Core.Network;
    using Oceanus.Core.Utils;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    class BalootMain
    {
        static void Main(string[] args)
        {
            run();
            Console.ReadKey();
        }

        public static void run()
        {
            string deviceId = "aplomb's android deviceId";
            LobbyService lobbyService = OceanusFactory.GetInstance().GetLobbyService();
            lobbyService.LoginGuest(deviceId, "Huawei", "android 4.4", "China Unicom 5G", (NetworkResult<LoginResult> result) =>
            {
                if (result.Code == 1)
                {
                    LoginResult loginResult = result.GetContent();
                    Logger.info("Test", "login guest successfully, result " + loginResult);
                    lobbyService.Start(loginResult.player.userId, deviceId, OceanusFactory.TERMINAL_ANDROID, loginResult.jwtToken);
                    lobbyService.OnNetworkEventReceivedEvents += (NetworkEvent networkEvent) =>
                    {
                        Logger.info("Test", "networkEvent " + networkEvent.Type + " content " + networkEvent);
                        switch (networkEvent.Type)
                        {
                            case NetworkEvent.TYPE_CONNECTED:
                                lobbyService.StartMatching("gwgamebaloot", "group1", (IMResult result1) =>
                                {
                                    if (result1.Code == 1)
                                    {

                                    }
                                    Logger.info("Test", "StartMatching result " + JsonMapper.ToJson(result1));
                                });
                                break;
                            case NetworkEvent.TYPE_DISCONNECTED:
                                break;
                            case NetworkEvent.TYPE_LOBBY_MATCHING_RESULT:
                                MatchingResultEvent matchingResultEvent = (MatchingResultEvent)networkEvent;
                                //messageReceivedEvent.Message.GetContent<>();
                                Logger.info("Test", "matchingResultEvent " + matchingResultEvent);

                                BalootGameService balootGameService = OceanusFactory.GetInstance().GetBalootGameService();

                                balootGameService.Start(loginResult.player.userId, deviceId, OceanusFactory.TERMINAL_ANDROID, matchingResultEvent.host, matchingResultEvent.port, matchingResultEvent.token);
                                balootGameService.OnNetworkEventReceivedEvents += (NetworkEvent NetworkEvent) =>
                                {
                                    switch (networkEvent.Type)
                                    {
                                        case NetworkEvent.TYPE_CONNECTED:

                                            break;
                                        case NetworkEvent.TYPE_DISCONNECTED:
                                            break;

                                    };
                                };
                                BalootGameManager balootGameManager = balootGameService.GetBalootGameManager();
                                balootGameManager.OnBalootPlayerChangedEvents += (string playerId, Dictionary<string, FieldChangedEvent> fieldChangedEvents) =>
                                {
                                    Logger.info("Test", "OnBalootPlayerChangedEvents playerId {0}, fieldChangedEvents {1}", playerId, fieldChangedEvents);
                                };
                                balootGameManager.OnBalootFrameDataChangedEvents += (Dictionary<string, FieldChangedEvent> fieldChangedEvents) =>
                                {
                                    FieldChangedEvent stateEvent = fieldChangedEvents.GetValueOrDefault("state");
                                    if (stateEvent != null)
                                    {
                                        Logger.info("Test", "state changed from " + stateEvent.OldValue + " to " + stateEvent.NewValue);
                                        //balootGameService.ASHKAL_CONFIRM(1, (IMResult result) =>
                                        //{
                                        //    Logger.info("Test", "ASHKAL_CONFIRM result " + result.Code);
                                        //});
                                    }
                                    Logger.info("Test", "OnBalootFrameDataChangedEvents fieldChangedEvents {0}", fieldChangedEvents);
                                };
                                balootGameManager.OnBalootFrameDataInitedEvents += (FrameData frameData) =>
                                {
                                    Logger.info("Test", "OnBalootFrameDataInitedEvents {0}", frameData);
                                };
                                balootGameManager.OnBalootPlayerAddedEvents += (NetWork.Oceanus.Baloot.Player player) =>
                                {
                                    Logger.info("Test", "OnBalootPlayerAddedEvents {0}", player);
                                };
                                break;
                            default:
                                Logger.error("Test", "Illegal NetworkEvent type {0} received, will be ignored. ", networkEvent.Type);
                                break;
                        }
                    };
                }
            });
        }
    }
}
