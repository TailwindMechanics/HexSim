using System.Collections.Generic;

namespace Modules.Client.Utilities.External
{
	public static class ListExtensions
	{
		public static T AddUnique<T>(this List<T> list, T item)
		{
			if (!list.Contains(item))
			{
				list.Add(item);
			}

			return list.Find(i => i.Equals(item));
		}
	}
}