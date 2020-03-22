using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Nzb.DataModel;

namespace Nzb.System
{
#pragma warning disable CA1304, CA1305
    public class LogManager
    {
        private Lazy<ILog> _logInfo = new Lazy<ILog>(() => log4net.LogManager.GetLogger("Info"));
        private Lazy<ILog> _logDebug = new Lazy<ILog>(() => log4net.LogManager.GetLogger("Debug"));
        private Lazy<ILog> _logError = new Lazy<ILog>(() => log4net.LogManager.GetLogger("Error"));
        private Lazy<ILog> _logWarn = new Lazy<ILog>(() => log4net.LogManager.GetLogger("Warn"));
        private ILog LogInfo { get { return this._logInfo.Value; } }
        private ILog LogError { get { return this._logError.Value; } }
        private ILog LogWarn { get { return this._logWarn.Value; } }
        private ILog LogDebug { get { return this._logDebug.Value; } }

        private static Lazy<LogManager> _current = new Lazy<LogManager>(() => new LogManager());
        public  static LogManager Current { get { return LogManager._current.Value; } }

        static LogManager()
        {
        }

        private LogManager()
        {

        }

        public void Info(string message, params object[] parms)
        {
            this.Info(string.Format(message, parms));
        }

        public void Info(string message)
        {
            this.LogInfo.Info(message);
        }


        public void Debug(string message, params object[] parms)
        {
            this.Debug(string.Format(message, parms));
        }

        public void Debug(string message)
        {
            this.LogDebug.Debug(message);
        }


        public void Warn(string message, params object[] parms)
        {
            this.Warn(string.Format(message, parms));
        }

        public void Warn(string message)
        {
            this.LogWarn.Warn(message);
        }

        public void Error(string message, params object[] parms)
        {
            this.Error(string.Format(message, parms));
        }

        public void Error(string message)
        {
            this.LogError.Error(message);
        }

        public void Error(Exception ex)
        {
            this.LogError.Error(ex.Message, ex);
        }
    }
}
