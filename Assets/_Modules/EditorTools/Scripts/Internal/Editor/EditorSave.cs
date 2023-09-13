using UnityEditor.SceneManagement;
using Random = UnityEngine.Random;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using Modules.Utilities.External;


namespace Modules.EditorTools.Internal.Editor
{
	internal static class EditorSave
	{
		internal static void SaveAllAndClear()
		{
			ClearConsole();
			SaveAll();
		}

		internal static void ClearConsole()
		{
			var assembly    = Assembly.GetAssembly(typeof(SceneView));
			var type        = assembly.GetType("UnityEditor.LogEntries");
			var method      = type.GetMethod("Clear");

			method?.Invoke(new object(), null);
		}

		static void SaveAll()
		{
			var logColor = Random.ColorHSV().ToHex();
			if (Application.isPlaying)
			{
				Debug.Log($"<color={logColor}><b>Cannot save during Play mode.</b></color>");
				return;
			}

			SaveScene();
			SaveProject();

			Debug.Log($"<color={logColor}><b>~~~Scene and project saved.</b></color>");
		}

		static void SaveScene ()	=> EditorSceneManager.SaveOpenScenes();
		static void SaveProject ()	=> AssetDatabase.SaveAssets();
	}
}