using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class MissionMultiplayerFFAClient : MissionMultiplayerGameModeBaseClient
	{
		public override bool IsGameModeUsingGold
		{
			get
			{
				return false;
			}
		}

		public override bool IsGameModeTactical
		{
			get
			{
				return false;
			}
		}

		public override bool IsGameModeUsingRoundCountdown
		{
			get
			{
				return false;
			}
		}

		public override MissionLobbyComponent.MultiplayerGameType GameType
		{
			get
			{
				return MissionLobbyComponent.MultiplayerGameType.FreeForAll;
			}
		}

		public override int GetGoldAmount()
		{
			return 0;
		}

		public override void OnGoldAmountChangedForRepresentative(MissionRepresentativeBase representative, int goldAmount)
		{
		}

		public override void AfterStart()
		{
			base.Mission.SetMissionMode(MissionMode.Battle, true);
		}
	}
}
