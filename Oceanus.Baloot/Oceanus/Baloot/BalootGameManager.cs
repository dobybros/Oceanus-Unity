using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus.Baloot
{
    public delegate void OnBalootFrameDataInited(FrameData frameData);
    public delegate void OnBalootPlayerAdded(Player player);
    public delegate void OnBalootFrameDataChanged(Dictionary<string, FieldChangedEvent> fieldChangedList);
    public delegate void OnBalootPlayerChanged(string playerId, Dictionary<string, FieldChangedEvent> fieldChangedList);
    public class FieldChangedEvent
    {
        //public static readonly int TYPE_MAIN = 1;
        //public static readonly int TYPE_PLAYER = 10;

        //public int Type
        //{
        //    set; get;
        //}
        public string Field
        {
            set; get;
        }
        public object OldValue
        {
            set; get;
        }
        public object NewValue
        {
            set; get;
        }
    }
    public interface BalootGameManager
    {
        event OnBalootFrameDataChanged OnBalootFrameDataChangedEvents;
        event OnBalootPlayerChanged OnBalootPlayerChangedEvents;
        event OnBalootFrameDataInited OnBalootFrameDataInitedEvents;
        event OnBalootPlayerAdded OnBalootPlayerAddedEvents;
        FrameData GetFrameData();
        void InitAllFrameData(FrameData frameData);
        void UpdateFrameData(FrameData frameData);

        void CallOnBalootFrameDataChanged(Dictionary<string, FieldChangedEvent> fieldChangedEvents);
        void CallOnBalootPlayerChanged(string playerId, Dictionary<string, FieldChangedEvent> fieldChangedEvents);
        void CallOnBalootPlayerAdded(Player player);
        void CallOnFrameDataInited(FrameData frameData);
    }
    
}
