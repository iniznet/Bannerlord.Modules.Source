using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public abstract class MPPerkCondition<T> : MPPerkCondition where T : MissionMultiplayerGameModeBase
	{
		protected T GameModeInstance
		{
			get
			{
				Mission mission = Mission.Current;
				if (mission == null)
				{
					return default(T);
				}
				return mission.GetMissionBehavior<T>();
			}
		}

		protected override bool IsGameModesValid(List<string> gameModes)
		{
			if (typeof(MissionMultiplayerFlagDomination).IsAssignableFrom(typeof(T)))
			{
				string text = MissionLobbyComponent.MultiplayerGameType.Skirmish.ToString();
				string text2 = MissionLobbyComponent.MultiplayerGameType.Captain.ToString();
				foreach (string text3 in gameModes)
				{
					if (!text3.Equals(text, StringComparison.InvariantCultureIgnoreCase) && !text3.Equals(text2, StringComparison.InvariantCultureIgnoreCase))
					{
						return false;
					}
				}
				return true;
			}
			if (typeof(MissionMultiplayerTeamDeathmatch).IsAssignableFrom(typeof(T)))
			{
				string text4 = MissionLobbyComponent.MultiplayerGameType.TeamDeathmatch.ToString();
				using (List<string>.Enumerator enumerator = gameModes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.Equals(text4, StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
				}
				return true;
			}
			if (typeof(MissionMultiplayerSiege).IsAssignableFrom(typeof(T)))
			{
				string text5 = MissionLobbyComponent.MultiplayerGameType.Siege.ToString();
				using (List<string>.Enumerator enumerator = gameModes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.Equals(text5, StringComparison.InvariantCultureIgnoreCase))
						{
							return false;
						}
					}
				}
				return true;
			}
			Debug.FailedAssert("Not implemented game mode check", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\MPPerkCondition.cs", "IsGameModesValid", 133);
			return false;
		}
	}
}
