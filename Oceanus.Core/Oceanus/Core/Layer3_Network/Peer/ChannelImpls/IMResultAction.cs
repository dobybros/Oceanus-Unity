using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Oceanus.Core.Network
{
    class IMResultAction
    {
        internal object Content
        {
            get; set;
        }
        internal string ContentType
        {
            get; set;
        }
        internal string Id
        {
            get; set;
        }
        internal OnIMResultReceived OnIMResultReceivedMethod
        {
            get; set;
        }
        internal IMResult IMResult
        {
            get; set;
        }
        internal int SendTimeoutSeconds
        {
            get; set;
        }
        internal CancellationTokenSource CancellationTokenSource
        {
            get; set;
        }
    }
}
