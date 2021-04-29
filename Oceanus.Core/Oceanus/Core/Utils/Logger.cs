using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Oceanus.Core.Utils
{
    public interface LoggerListener
    {
        void info(string message);
        void warn(string message);
        void error(string message);
        void fatal(string message);
    }
    public class Logger
    {
        public static LoggerListener LoggerListener
        {
            set;
            get;
        }
        public static string Format(string message, params object[] args)
        {
            if (args == null)
                return message;
            for(int i = 0; i < args.Length; i++)
            {
                message = message.Replace("{" + i + "}", args[i].ToString());
            }
            return message;
        }
        private static string PrintStringFormat(string type, string tag, string message, params object[] args)
        {
            message = Logger.Format(message, args);
            return string.Format("{0}#Thread {1}#{2:yyyy/MM/dd HH:mm:ss.fff}#{3}:: {4}", type, Thread.CurrentThread.ManagedThreadId, DateTime.Now, tag, message);
        }

        public static void error(string tag, string message, params object[] args)
        {
            if(LoggerListener == null)
            {
                Console.WriteLine(Logger.PrintStringFormat("ERROR", tag, message, args)); 
            } else
            {
                LoggerListener.error(Logger.PrintStringFormat("ERROR", tag, message, args));
            }
        }

        public static void fatal(string tag, string message, params object[] args)
        {
            if (LoggerListener == null)
            {
                Console.WriteLine(Logger.PrintStringFormat("FATAL", tag, message, args));
            }
            else
            {
                LoggerListener.error(Logger.PrintStringFormat("FATAL", tag, message, args));
            }
        }

        public static void info(string tag, string message, params object[] args)
        {
            if (LoggerListener == null)
            {
                Console.WriteLine(Logger.PrintStringFormat("INFO", tag, message, args));
            }
            else
            {
                LoggerListener.error(Logger.PrintStringFormat("INFO", tag, message, args));
            }
        }

        public static void warn(string tag, string message, params object[] args)
        {
            if (LoggerListener == null)
            {
                Console.WriteLine(Logger.PrintStringFormat("WARN", tag, message, args));
            }
            else
            {
                LoggerListener.error(Logger.PrintStringFormat("WARN", tag, message, args));
            }
        }
    }
}
