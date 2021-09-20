using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlRats.Util {

	public static class Log {

		private static Stream logFile;
		private static StreamWriter logFileWriter;

		static Log() {
			logFile = new FileStream("log.txt", FileMode.OpenOrCreate, FileAccess.Write);
			logFileWriter = new StreamWriter(logFile, Encoding.UTF8);
			AppDomain.CurrentDomain.ProcessExit += (e, a) => logFile.Dispose();
		}

		private static void LogLine(string text) {
			logFileWriter?.WriteLine(text);
			Console.WriteLine(text);
		}

		private static void LogText(string text) {
			text.Split('\n').ForEach(LogLine);
			logFile.FlushAsync();
		}

		public static void Print(object o) => LogText(o.ToString());

	}
}
