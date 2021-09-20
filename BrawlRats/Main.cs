using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrawlRats.Graphics;

namespace BrawlRats {

	public static class GameMain {

		public static bool Running { get; set; } = true;

		public static void Main(string[] args) {
			Display.Init();

			while(Running) {
				Display.RunEvents();
			}

			Display.Deinit();
		}

	}
}
