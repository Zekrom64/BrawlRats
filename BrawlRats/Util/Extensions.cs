using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlRats.Util {
	public static class ArrayExtensions {

		public static void ForEach<T>(this T[] array, Action<T> fn) => Array.ForEach(array, fn);

	}
}
