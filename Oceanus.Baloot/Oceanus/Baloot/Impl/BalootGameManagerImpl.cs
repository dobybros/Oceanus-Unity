using LitJson;
using Oceanus.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWork.Oceanus.Baloot
{
    class BalootGameManagerImpl : BalootGameManager
    {
        public event OnBalootFrameDataChanged OnBalootFrameDataChangedEvents;
        public event OnBalootPlayerChanged OnBalootPlayerChangedEvents;
        public event OnBalootFrameDataInited OnBalootFrameDataInitedEvents;
        public event OnBalootPlayerAdded OnBalootPlayerAddedEvents;

        private FrameData mFrameData;
        public FrameData GetFrameData()
        {
            throw new NotImplementedException();
        }

        public void InitAllFrameData(FrameData frameData)
        {
            mFrameData = frameData;
            this.CallOnFrameDataInited(frameData);
        }

        public void UpdateFrameData(FrameData frameData)
        {
            mFrameData.CopyValues(frameData, this);
        }

        public void CallOnBalootFrameDataChanged(Dictionary<string, FieldChangedEvent> fieldChangedEvents)
        {
            if (fieldChangedEvents == null || fieldChangedEvents.Count() <= 0)
                return;
            SafeUtils.SafeCallback("CallOnBalootFrameDataChanged " + fieldChangedEvents.Count(), () =>
            {
                OnBalootFrameDataChangedEvents(fieldChangedEvents);
            });
        }

        public void CallOnBalootPlayerChanged(string playerId, Dictionary<string, FieldChangedEvent> fieldChangedEvents)
        {
            if (playerId == null || fieldChangedEvents == null || fieldChangedEvents.Count() <= 0)
                return;
            SafeUtils.SafeCallback("CallOnBalootPlayerChanged playerId " + playerId + " field count " + fieldChangedEvents.Count(), () =>
            {
                OnBalootPlayerChangedEvents(playerId, fieldChangedEvents);
            });
        }

        public void CallOnBalootPlayerAdded(Player player)
        {
            if (player == null)
                return;
            SafeUtils.SafeCallback("CallOnBalootPlayerAdded " + player, () =>
            {
                OnBalootPlayerAddedEvents(player);
            });
        }

        public void CallOnFrameDataInited(FrameData frameData)
        {
            if (frameData == null)
                return;
            SafeUtils.SafeCallback("CallOnFrameDataInited " + frameData, () =>
            {
                OnBalootFrameDataInitedEvents(frameData);
            });
        }
    }
}
