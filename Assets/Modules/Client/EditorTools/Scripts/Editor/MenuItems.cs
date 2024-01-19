using System.Linq;
using UnityEditor;

using Modules.Client.Utilities.External;

namespace Modules.Client.EditorTools.Editor
{
	internal static class MenuItems
	{
        [MenuItem("Modules/Collapse Inspector Components &c")] // hotkey = alt_c
        static void CollapseAllInspectorComponents()
        {
	        var tracker = ActiveEditorTracker.sharedTracker;
	        for (var i = 0; i < tracker.activeEditors.Length; i++)
	        {
		        tracker.SetVisible(i, 0);
		        tracker.activeEditors[i].Repaint();
	        }
        }

        [MenuItem("Modules/Save All %&s")] // ctr_alt_s
		static void SaveAllAndClear()
			=> EditorSave.SaveAllAndClear();

		[MenuItem("Modules/Clear Console %&c")] // ctrl_alt_c
		static void ClearConsole()
			=> EditorSave.ClearConsole();

		[MenuItem("Modules/Reset Position And Rotation %&r")] // ctrl_alt_r
		static void ZeroOutTransform()
			=> Selection.gameObjects.ToList().ForEach(selected => selected.transform.ZeroOutLocalPositionAndRotation());
	}
}