#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

using Shared.HexMap.External.Schema;


namespace Client.Hex2Editor.Editor
{
	[CustomPropertyDrawer(typeof(Hex2))]
	public class Hex2Drawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height), label);

			position.x += EditorGUIUtility.labelWidth;
			position.width -= EditorGUIUtility.labelWidth;
			position.width /= 2;

			var neProp = property.FindPropertyRelative("ne");
			var seProp = property.FindPropertyRelative("se");

			EditorGUIUtility.labelWidth = 22;
			EditorGUI.PropertyField(position, neProp, new GUIContent(" Ne "));

			position.x += position.width;
			EditorGUI.PropertyField(position, seProp, new GUIContent(" Se "));
			EditorGUIUtility.labelWidth = EditorGUIUtility.labelWidth;
			EditorGUI.EndProperty();
		}
	}
}

#endif