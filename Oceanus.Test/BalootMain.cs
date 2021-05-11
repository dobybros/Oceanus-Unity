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
            bool matchingOnce = false;
            string deviceId = "aplomb's android deviceId2";
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
                        switch (networkEvent.Type)
                        {
                            case NetworkEvent.TYPE_CONNECTED:
                                Logger.info("Test", "networkEvent TYPE_CONNECTED content " + networkEvent);
                                if(!matchingOnce)
                                {
                                    matchingOnce = true;
                                    lobbyService.StartMatching("gwgamebaloot", "group1", (IMResult result1) =>
                                    {
                                        switch (result1.Code)
                                        {
                                            case 1:
                                                break;
                                            case LobbyErrorCodes.ERROR_START_MATCHING_BUT_PLAYER_IN_MATCHING:
                                                lobbyService.CancelMatching((IMResult result2) =>
                                                {
                                                    Logger.info("Test", "Cancel matching result code " + result2.Code);
                                                });
                                                break;
                                        }
                                        Logger.info("Test", "StartMatching result " + JsonMapper.ToJson(result1));
                                    });
                                }
                                
                                break;
                            case NetworkEvent.TYPE_DISCONNECTED:
                                Logger.info("Test", "networkEvent TYPE_DISCONNECTED content " + networkEvent);
                                break;
                            case NetworkEvent.TYPE_SHUTTDDOWN:
                                Logger.info("Test", "networkEvent TYPE_SHUTTDDOWN content " + networkEvent);
                                break;
                            case NetworkEvent.TYPE_LOBBY_MATCHING_RESULT:
                                Logger.info("Test", "networkEvent TYPE_LOBBY_MATCHING_RESULT content " + networkEvent);
                                MatchingResultEvent matchingResultEvent = (MatchingResultEvent)networkEvent;
                                //messageReceivedEvent.Message.GetContent<>();

                                BalootGameService balootGameService = OceanusFactory.GetInstance().CreateBalootGameService();

                                balootGameService.Start(loginResult.player.userId, deviceId, OceanusFactory.TERMINAL_ANDROID, matchingResultEvent.host, matchingResultEvent.port, matchingResultEvent.token);
                                balootGameService.OnNetworkEventReceivedEvents += (NetworkEvent NetworkEvent) =>
                                {
                                    switch (networkEvent.Type)
                                    {
                                        case NetworkEvent.TYPE_CONNECTED:
                                            Logger.info("Test", "balootGameService connected");
                                            break;
                                        case NetworkEvent.TYPE_DISCONNECTED:
                                            Logger.info("Test", "balootGameService disconnected");
                                            break;
                                        case NetworkEvent.TYPE_SHUTTDDOWN:
                                            Logger.info("Test", "balootGameService shutted down");
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
