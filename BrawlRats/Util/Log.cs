using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlRats.Util {

	/// <summary>
	/// Log manager.
	/// </summary>
	public static class Log {

		private static readonly Stream logFile;
		private static readonly StreamWriter logFileWriter;

		static Log() {
			logFile = new FileStream("log.txt", FileMode.Create, FileAccess.Write);
			logFileWriter = new StreamWriter(logFile, Encoding.UTF8);
			AppDomain.CurrentDomain.ProcessExit += (e, a) => {
				logFileWriter.Flush();
				logFile.Dispose();
			};
		}

		private static void LogLine(string text) {
			logFileWriter?.WriteLine(text);
			Console.WriteLine(text);
		}

		private static void LogText(string text) {
			foreach(var line in text.Split('\n')) LogLine(line);
			logFile.FlushAsync();
		}

		/// <summary>
		/// Prints a value to the log file, converted to a string using the
		/// <see cref="object.ToString()"/> method.
		/// </summary>
		/// <param name="o">Object to print</param>
		public static void Print(object? o) => LogText(o?.ToString() ?? "null");

	}
}
