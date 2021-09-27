using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlRats.Content.Scene {

	public abstract class BaseScene {

		public Physics Physics { get; } = new();

	}

}
