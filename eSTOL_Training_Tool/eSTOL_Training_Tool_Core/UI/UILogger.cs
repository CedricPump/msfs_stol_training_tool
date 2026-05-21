using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STOL_Training_Tool_Core.Core;

namespace STOL_Training_Tool_Core.UI
{
    internal class UILogger
    {
        private static UILogger instance = null;
        private FormUI form;

        public UILogger(FormUI form)
        {
            this.form = form;
            instance = this;
        }

        public static UILogger GetInstance()
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(form), "FormUI instance must be provided for the first time.");
            }
            return instance;
        }

        public void Log(string message = "", bool debug = false)
        {
            if (!Config.GetInstance().debug && debug) return;
            if (form != null)
            {
                form.appendResult(message);
            }
        }

        public static void LogInfo(string message)
        {
            UILogger.GetInstance().Log(message, false);
        }

        public static void LogDebug(string message)
        {
            UILogger.GetInstance().Log(message, true);
        }

    }
}
