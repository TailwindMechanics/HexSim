using System.Linq;


namespace Modules.Client.Utilities.External
{
	public static class StringExtensions
	{
		public static float ToSeedFloat(this string str)
		{
			var seed = str.Aggregate<char, long>(0, (current, c) => (current << 5) - current + c);
			const int prime = 1000000007;
			return (float)(seed % prime) / prime;
		}
	}
}