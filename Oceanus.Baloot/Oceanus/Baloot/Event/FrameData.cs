using Oceanus.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus.Baloot
{
    public class FrameData
    {
        /// <summary>
        /// 
        /// </summary>
        public string service { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string group { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? state { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<int> target { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? rMod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? rModInx { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? kCK { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? mul { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? mulInx { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? lMul { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? duel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? comCard { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? deaInx { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? lpIndex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<int?> winInxs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Ani ani { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> inxs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Input input { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, Player> players { get; set; }

        public void CopyValues(FrameData source, BalootGameManager balootGameManager)
        {
            if (source == null)
                return;
            IEnumerable<PropertyInfo> properties = typeof(FrameData).GetProperties().Where(prop => prop.CanRead && prop.CanWrite);
            Dictionary<string, FieldChangedEvent> fieldChangedEvents = new Dictionary<string, FieldChangedEvent>();
            foreach (PropertyInfo prop in properties)
            {
                var value = prop.GetValue(source, null);
                var targetValue = prop.GetValue(this, null);
                if (value != null && (targetValue == null || !targetValue.Equals(value)))
                {
                    if (!prop.Name.Equals("players") && !value.Equals(targetValue)) 
                    {
                        FieldChangedEvent fieldChangedEvent = new FieldChangedEvent();
                        fieldChangedEvent.OldValue = targetValue;
                        fieldChangedEvent.NewValue = value;
                        fieldChangedEvent.Field = prop.Name;
                        
                        prop.SetValue(this, value, null);
                        fieldChangedEvents.TryAdd(prop.Name, fieldChangedEvent);
                    }
                }
            }
            if(fieldChangedEvents.Count() > 0)
            {
                balootGameManager.CallOnBalootFrameDataChanged(fieldChangedEvents);
            }
            if(source.players != null)
            {
                if (this.players == null)
                    this.players = new Dictionary<string, Player>();
                foreach (string playerId in source.players.Keys)
                {
                    Player player = this.players.GetValueOrDefault(playerId);
                    if(player != null)
                    {
                        Player sourcePlayer = source.players.GetValueOrDefault(playerId);
                        if(sourcePlayer != null)
                            CopyValues(player, sourcePlayer, balootGameManager);
                    } else
                    {
                        player = source.players.GetValueOrDefault(playerId);
                        if(player != null)
                        {
                            this.players.TryAdd(playerId, player);
                            balootGameManager.CallOnBalootPlayerAdded(player);
                        }
                    }
                   
                }

            }
        }

        public void CopyValues(Player target, Player source, BalootGameManager balootGameManager)
        {
            IEnumerable<PropertyInfo> properties = typeof(Player).GetProperties().Where(prop => prop.CanRead && prop.CanWrite);
            Dictionary<string, FieldChangedEvent> fieldChangedEvents = new Dictionary<string, FieldChangedEvent>();

            foreach (PropertyInfo prop in properties)
            {
                var value = prop.GetValue(source, null);
                var targetValue = prop.GetValue(target, null);
                if (value != null && (targetValue == null || !targetValue.Equals(value)))
                {
                    FieldChangedEvent fieldChangedEvent = new FieldChangedEvent();
                    fieldChangedEvent.OldValue = targetValue;
                    fieldChangedEvent.NewValue = value;
                    fieldChangedEvent.Field = prop.Name;

                    prop.SetValue(this, value, null);
                    fieldChangedEvents.TryAdd(prop.Name, fieldChangedEvent);
                }
            }
            balootGameManager.CallOnBalootPlayerChanged(target.id, fieldChangedEvents);
        }
    }
    public class Ani
    {
        /// <summary>
        /// 
        /// </summary>
        public long start { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long duration { get; set; }
    }

    public class Input
    {
        /// <summary>
        /// 
        /// </summary>
        public long start { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long duration { get; set; }
    }

    public class Cards
    {
        /// <summary>
        /// 
        /// </summary>
        public List<int> cards { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int sawa { get; set; }
    }

    public class Discards
    {
        /// <summary>
        /// 
        /// </summary>
        public List<int> cards { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<int> ani { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<SCardsItem> sCards { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<String, int> sCount { get; set; }
    }

    public class SCardsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public List<int> cards { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int point { get; set; }
    }

    public class Player
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Cards cards { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Discards discards { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<SCardsItem> sCards { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pointCards { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int cardScore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int sScore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pointsNow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pointsTotal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int fScore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int bScore { get; set; }
    }
}
