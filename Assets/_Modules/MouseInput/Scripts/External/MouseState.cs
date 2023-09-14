using System;

namespace Modules.MouseInput.External
{
	[Serializable]
	public enum MouseState
	{
		None,
		Down,
		ClickCooldown,
		Held,
		Up,
		Click
	}
}