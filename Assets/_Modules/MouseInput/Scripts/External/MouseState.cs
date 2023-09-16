using System;

namespace Modules.MouseInput.External
{
	[Serializable]
	public enum MouseState
	{
		None,
		Down,
		Held,
		Up,
		Click
	}
}