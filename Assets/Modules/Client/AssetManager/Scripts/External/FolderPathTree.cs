using UnityEngine;
using System;

namespace Modules.Client.AssetManager.External
{
	[Serializable]
	public class FolderPathTree
	{
		[TextArea(1, 1), SerializeField]
		public string Path;
		[TextArea(10, 10), SerializeField]
		public string Tree;
	}
}