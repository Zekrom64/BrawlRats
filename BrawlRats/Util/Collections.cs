using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlRats.Util {

	public class EmptyCollection<T> : IReadOnlyCollection<T> {

		public int Count => 0;

		public IEnumerator<T> GetEnumerator() => Enumerable.Empty<T>().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	}

	public static class Collections<T> {

		public static readonly EmptyCollection<T> Empty = new();

	}

}
