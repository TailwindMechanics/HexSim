using System.Collections.Generic;
using System;

namespace Modules.Server.NeuroNavigation.External
{
	[Serializable]
	public class NeuroPath
	{
		public List<NeuroNode> Path { get; }
		public NeuroPath(List<NeuroNode> path)
			=> Path = path;
	}
}