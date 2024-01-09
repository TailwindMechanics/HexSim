using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Modules.Server.NeuroNavigation.External
{
	public class NeuroNav
	{
		public List<Vector3> FindPath(Vector3 start, Vector3 target)
		{
			var result = new List<Vector3>
			{
				start,
				target
			};
			return result;
		}
	}
}