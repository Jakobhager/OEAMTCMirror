using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OEAMTCMirror
{
    public class LogWriter
    {
        private static string _filename = "log.txt";
        private StreamWriter _w = File.AppendText(_filename);

        public void Log(string line)
        {

            string logline = DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + ": " + line + "\r\n";

            //_w.Write("{0} {1}", DateTime.Now.ToLongTimeString(),
            //    DateTime.Now.ToLongDateString());
            //_w.Write("  :");
            //_w.Write("  :{0}\r\n", line);

            _w.Write(logline);
        }
    }


}
