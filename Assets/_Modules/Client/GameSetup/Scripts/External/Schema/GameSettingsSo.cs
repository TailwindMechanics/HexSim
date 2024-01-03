using UnityEngine;

using Modules.Shared.HexMap.External.Schema;


namespace Modules.Client.GameSetup.External.Schema
{
	[CreateAssetMenu(fileName = "new _gameSettings", menuName = "Modules/Gameplay/Settings")]
	public class GameSettingsSo : ScriptableObject
	{
		public GameSettingsVo Vo => settings;
		[SerializeField] GameSettingsVo settings = new();

		[SerializeField] string teamName;
		[SerializeField] ActorVo actorToAdd;

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