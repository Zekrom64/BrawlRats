using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrawlRats.Graphics;
using BrawlRats.Util;

namespace BrawlRats {

	public static class GameMain {

		/// <summary>
		/// If the game is running.
		/// </summary>
		public static bool Running { get; set; } = true;

		public static void Main(string[] args) {
			Log.Print($"Startup @{DateTime.Now}");
			// Initialize the display
			Display.Init();

			// Make the window visible
			Display.Window.Visible = true;

			// While running
			while(Running) {
				// Run event handling
				Display.RunEvents();
			}

			// Deinitialize the display
			Display.Deinit();
		}

	}
}
