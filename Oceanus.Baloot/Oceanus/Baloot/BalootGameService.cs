using Oceanus.Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus.Baloot
{
    public delegate void OnNetworkEventReceived(NetworkEvent networkEvent);
    public class BalootConstants
    {
        public static readonly int MODEL_PASS = -1;
        public static readonly int MODEL_SUN = 0;
        public static readonly int MODEL_ASHKAL = 1;
        public static readonly int MODEL_HOKOM = 2;

        public static readonly int MUL_NONE = 1;
        public static readonly int MUL_TWO = 2;
        public static readonly int MUL_THREE = 3;
        public static readonly int MUL_FOUR = 4;

        //duel:1：决斗，0：不决斗（不到决斗的时候不传）
        public static readonly int DUAL_YES = 1;
        public static readonly int DUAL_NO = 0;

        //lock:是否锁定。1锁定；0不锁定
        public static readonly int LOCK_YES = 1;
        public static readonly int LOCK_NO = 0;

        //kind：40黑桃；30红桃；20梅花；10方块
        public static readonly int KIND_SPADES = 40;
        public static readonly int KIND_HEARTS = 30;
        public static readonly int KIND_CLUBS = 20;
        public static readonly int KIND_DIAMONDS = 10;

        //sawa:1表示我要SAWA, 0是不能SAWA。（不能SAWA可以不传）
        public static readonly int SAWA_YES = 1;
        public static readonly int SAWA_NO = 0;
    }
    public interface BalootGameService : RoomService
    {
        event OnNetworkEventReceived OnNetworkEventReceivedEvents;

        /**
         * <summary>
         *  获取单太的BalootGameManager对象， 注册事件监听。
         * </summary>
         * 
         */
        BalootGameManager GetBalootGameManager();
        /**
         * body：
            {
            "contentType": "FM",
            "content" : {
                    "model":-1
                },
            "id": ""//唯一id
            }
            state：
            STATE_FIRST_MODEL
            参数说明：
            contentType:动作，我要选模式
            model：选了啥模式。-1:PASS;0:SUN;1:ASHKAL;2:HOKOM;
         * 
         */
        void FIRST_MODEL(int model, OnIMResultReceived onResultReceived = null);
        /**
         * 
         * body
            {
            "contentType": "HC",
            "content" : {
                    "model":-1
                },
            "id": ""//唯一id
            }
            state
            STATE_HOKOM_CONFIRM
            参数说明
            contentType:动作，有人选择的HOKOM\SECOND_HOKOM
            其他人反对他，我要选PASS\SUN\ASHKAL
            没人反对，我自己确认下HOKOM\SECOND_HOKOM\SUN
            model：选了啥模式。
            -1:PASS;
            0:SUN;
            1:ASHKAL;
            2:HOKOM;
            3:SECOND_HOKOM
         * 
         */
        void HOKOMConfirm(int model, OnIMResultReceived onResultReceived = null);
        /**
         * 
         * body
            {
            "contentType": "HS",
            "content" : {
                    "model":-1
                },
            "id": ""//唯一id
            }
            state
            STATE_HOKOM_SUN
            参数说明
            contentType:动作，有人反对王牌，王牌优先选择SUN\PASS
            model：选了啥模式。
            -1:PASS;
            0:SUN;
         */
        void HOKOM_SUN(int model, OnIMResultReceived onResultReceived = null);
        /**
         * body
            {
            "contentType": "SD",
            "content" : {
                    "mul":1
                },
            "id": ""//唯一id
            }
            state
            STATE_SUN_DOUBLE
            参数说明
            contentType:动作，敌方选择SUN\ASHKAL，我们要翻倍
            mul:翻几倍，1：不翻倍；2：翻两倍
         * 
         */
        void SUN_DOUBLE(int mul, OnIMResultReceived onResultReceived = null);

        /**
         * 
         * body
            {
            "contentType": "HD",
            "content" : {
                    "mul":1,
                    "duel":1
                },
            "id": ""//唯一id
            }
            state
            STATE_HOKOM_DOUBLE
            参数说明
            contentType:动作，HOKOM\SECOND_HOKOM，循环翻倍，直至决斗
            mul:翻几倍，1：不翻倍；2：翻两倍；3：翻三倍；4：翻四倍（决斗的时侯不传）
            duel:1：决斗，0：不决斗（不到决斗的时候不传）
         */
        void HOKOM_DOUBLE(int mul, int duel, OnIMResultReceived onResultReceived = null);

        /**
         * body
            {
            "contentType": "DL",
            "content" : {
                    "lock":1,
                },
            "id": ""//唯一id
            }
            state
            STATE_DOUBLE_LOCK
            参数说明
            contentType:动作，2\4倍的人可以选择锁定
            lock:是否锁定。1锁定；0不锁定
         * 
         */
        void DOUBLE_LOCK(int lock1, OnIMResultReceived onResultReceived = null);
        /**
         * 
         * body
            {
            "contentType": "AC",
            "content" : {
                    "model":1,
                },
            "id": ""//唯一id
            }
            state
            STATE_ASHKAL_CONFIRM
            参数说明
            contentType:动作，有人选了ASHKAL,上家选择PASS\SUN
            model：选了啥模式。
            -1:PASS;
            0:SUN;
         * 
         */
        void ASHKAL_CONFIRM(int model, OnIMResultReceived onResultReceived = null);

        /**
         * body
            {
            "contentType": "SM",
            "content" : {
                    "model":1,
                },
            "id": ""//唯一id
            }
            state
            STATE_SECOND_MODEL
            参数说明
            contentType:动作，我要选模式
            model：选了啥模式。-1:PASS;0:SUN;1:ASHKAL;2:HOKOM;
         * 
         */
        void SECOND_MODEL(int model, OnIMResultReceived onResultReceived = null);
        /**
         * body
            {
            "contentType": "HK",
            "content" : {
                    "kind":10,
                },
            "id": ""//唯一id
            }
            state
            STATE_HOKOM_KING
            参数说明
            contentType:动作，我要选花色
            kind：40黑桃；30红桃；20梅花；10方块
         * 
         */
        void HOKOM_KING(int kind, OnIMResultReceived onResultReceived = null);
        /**
         * 
         * body
            {
            "contentType": "P",
            "content" : {
                    "cId":1012,
                    "sCount":{"1":2,"2":1}//SIRA:两个；50:一个
                    "sawa":1,
                },
            "id": ""//唯一id
            }
            state
            STATE_PLAYING
            参数说明
            contentType:动作，我要出牌
            cId：花色+点数的牌ID， 例如1001方块A,3013红桃K
            sCount:特殊牌型和数量（玩家本局第一次出牌，才可能有）
            sawa:1表示我要SAWA, 0是不能SAWA。（不能SAWA可以不传）
         */
        void PLAYING(int cId, Dictionary<String, int> sCount, int sawa, OnIMResultReceived onResultReceived = null);
    }
}
