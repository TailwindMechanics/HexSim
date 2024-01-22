using UnityEditor.Animations;
using UnityEngine;
using System;

namespace Modules.Client.AssetManager.Editor.Schema
{
	[Serializable]
	public class AnimatorDependencies
	{
		public Animator Animator;
		public AnimatorController Controller;
		public Avatar Avatar;

		public AnimatorDependencies(Animator animator, AnimatorController controller, Avatar avatar)
		{
			Animator = animator;
			Controller = controller;
			Avatar = avatar;
		}
	}
}