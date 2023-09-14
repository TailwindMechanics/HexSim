using Modules.HexMap.External.DataObjects;

namespace Modules.HexTiles.Internal
{
	public class HexTile
	{
		public Hex2 Coords { get; }

		public HexTile (Hex2 coords)
		{
			Coords = coords;
		}
	}
}