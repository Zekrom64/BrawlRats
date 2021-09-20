using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlRats.Util {

	/// <summary>
	/// Extension methods for arrays.
	/// </summary>
	public static class ArrayExtensions {

		/// <summary>
		/// Enumerates each item in the array using the <see cref="Array.ForEach{T}(T[], Action{T})"/> method.
		/// </summary>
		/// <typeparam name="T">Array element type</typeparam>
		/// <param name="array">Array to enumerate</param>
		/// <param name="fn">Enumeration function</param>
		public static void ForEach<T>(this T[] array, Action<T> fn) => Array.ForEach(array, fn);

	}
}
