using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlRats.Graphics {

	public enum ZLayer {
		Background = -30,
		BackgroundClutter = -20,
		Items = -10,
		Characters = 0,
		Effects = 10,
		ForegroundClutter = 20,
		Overlay = 30
	}

	public class RenderBuffer {

	}

}
