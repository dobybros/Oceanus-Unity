using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Oceanus.Test
{
    public class AssertAsyncHandler
    {
        private bool mIsFailed;
        private string mFailedMessage;

        public AssertAsyncHandler()
        {
            mIsFailed = true;
            mFailedMessage = "Timeout failed";
        }

        public void wait(int miliSeconds)
        {
            try
            {
                Monitor.Enter(this);
                Monitor.Wait(this, miliSeconds);
            }
            finally
            {
                Monitor.Exit(this);
            }
            

            Assert.IsFalse(mIsFailed, mFailedMessage);
        }

        public void pass()
        {
            mIsFailed = false;
            try
            {
                Monitor.Enter(this);
                Monitor.PulseAll(this);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
        public void failed(string message)
        {
            mIsFailed = true;
            mFailedMessage = message;
            try
            {
                Monitor.Enter(this);
                Monitor.PulseAll(this);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }
}
