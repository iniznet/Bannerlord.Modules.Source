using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000316 RID: 790
	public abstract class MPPerkCondition<T> : MPPerkCondition where T : MissionMultiplayerGameModeBase
	{
		// Token: 0x17000793 RID: 1939
		// (get) Token: 0x06002A8F RID: 10895 RVA: 0x000A5A64 File Offset: 0x000A3C64
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

		// Token: 0x06002A90 RID: 10896 RVA: 0x000A5A8C File Offset: 0x000A3C8C
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
