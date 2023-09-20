using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	public class PlayerMoveTroopEvent : EventBase
	{
		public CharacterObject Troop { get; private set; }

		public int Amount { get; private set; }

		public bool IsPrisoner { get; private set; }

		public PartyScreenLogic.PartyRosterSide FromSide { get; private set; }

		public PartyScreenLogic.PartyRosterSide ToSide { get; private set; }

		public PlayerMoveTroopEvent(CharacterObject troop, PartyScreenLogic.PartyRosterSide fromSide, PartyScreenLogic.PartyRosterSide toSide, int amount, bool isPrisoner)
		{
			this.Troop = troop;
			this.FromSide = fromSide;
			this.ToSide = toSide;
			this.IsPrisoner = isPrisoner;
			this.Amount = amount;
		}
	}
}
