using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BrawlRats.Util;

namespace BrawlRats.WinMain {

	public static class WinMain {

		public static void Main(string[] args) {
			// Catch any exceptions, log them, and show a message box to the confused user
			try {
				GameMain.Main(args);
			} catch (Exception e) {
				Log.Print("Uncaught exception:\n" + e);
				MessageBox.Show("Uncaught exception: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

	}
}
