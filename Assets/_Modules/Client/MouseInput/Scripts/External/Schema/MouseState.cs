using System;


namespace Modules.Client.MouseInput.External.Schema
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