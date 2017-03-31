using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Mechsoft.ADServices.Helpers
{
    internal class Logger
    {
        private readonly string localDataPath;
        private readonly string localCustomPath;
        private readonly string logDirectory;
        private readonly string logfileDirectory;

        public Logger()
        {
            localDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            localCustomPath = "Mechsoft.ADServices";
            logDirectory = Path.Combine(localDataPath, localCustomPath);
            logfileDirectory = Path.Combine(logDirectory, "logfile.txt");

            ValidateLogFile();
        }

        public void Write(string Message, LogType Type)
        {
            using (var logWriter = new StreamWriter(logfileDirectory))
            {
                logWriter.WriteLine(string.Format("{0}-{1}-{2}: {3}", DateTime.Now.ToString(), Type.ToString(), "Message", Message));
                logWriter.Close();
            }
        }

        public void Write(Exception Ex, LogType Type = LogType.Error)
        {

            using (var logWriter = new StreamWriter(logfileDirectory))
            {
                logWriter.WriteLine(string.Format("{0}-{1}-{2}: {3}", DateTime.Now.ToString(), Type.ToString(), "Message", Ex.ToString()));
                logWriter.Close();
            }
        }

        private void ValidateLogFile()
        {

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var backupPath = Path.Combine(logDirectory, string.Format("logfile-{0}-backup.txt", DateTime.Now.ToShortDateString()));
            if (!File.Exists(logfileDirectory))
            {
                File.Create(logfileDirectory);
            }
            else
            {
                var fileSize = new FileInfo(logfileDirectory).Length;
                if (fileSize > 10000000)
                {
                    File.Move(logfileDirectory, backupPath);
                    File.Create(logfileDirectory);
                }
            }

        }
    }

    internal enum LogType
    {
        Info,
        Warning,
        Error
    }
}
