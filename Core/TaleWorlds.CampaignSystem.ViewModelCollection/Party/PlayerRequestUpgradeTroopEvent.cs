using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	public class PlayerRequestUpgradeTroopEvent : EventBase
	{
		public CharacterObject SourceTroop { get; private set; }

		public CharacterObject TargetTroop { get; private set; }

		public int Number { get; private set; }

		public PlayerRequestUpgradeTroopEvent(CharacterObject sourceTroop, CharacterObject targetTroop, int num)
		{
			this.SourceTroop = sourceTroop;
			this.TargetTroop = targetTroop;
			this.Number = num;
		}
	}
}
