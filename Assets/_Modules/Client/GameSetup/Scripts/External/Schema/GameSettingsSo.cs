using Modules.Shared.HexMap.External.Schema;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Modules.Client.GameSetup.External.Schema
{
	[CreateAssetMenu(fileName = "new _gameSettings", menuName = "Modules/Gameplay/Settings")]
	public class GameSettingsSo : ScriptableObject
	{
		public GameSettingsVo Vo => settings;
		[HideLabel, SerializeField] GameSettingsVo settings = new();

		[FoldoutGroup("Add groups"), SerializeField] string teamName;
		[FoldoutGroup("Add groups"), HideLabel, SerializeField] ActorVo actorToAdd;
		[FoldoutGroup("Add groups"), Button(ButtonSizes.Medium)]
		void AddAtCoords ()
		{
			var team = settings.Teams.Find(t => t.TeamName == teamName);
			var neighbours = Hex2.GetNeighbors(actorToAdd.Hex2Coords);
			neighbours.ForEach(coord =>
			{
				team.Actors.Add(new ActorVo(actorToAdd.So, actorToAdd.HitPoints, coord));
			});

			Debug.Log($"<color=orange><b>>>> Added: {neighbours.Count}</b></color>");
		}
	}
}