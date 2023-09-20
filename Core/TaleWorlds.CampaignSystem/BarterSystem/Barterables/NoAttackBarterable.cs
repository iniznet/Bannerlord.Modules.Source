using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.BarterSystem.Barterables
{
	public class NoAttackBarterable : Barterable
	{
		public override string StringID
		{
			get
			{
				return "no_attack_barterable";
			}
		}

		public NoAttackBarterable(Hero originalOwner, Hero otherHero, PartyBase ownerParty, PartyBase otherParty, CampaignTime duration)
			: base(originalOwner, ownerParty)
		{
			this._otherFaction = otherParty.MapFaction;
			this._duration = duration;
			this._otherHero = otherHero;
			this._otherParty = otherParty;
		}

		public override TextObject Name
		{
			get
			{
				TextObject textObject = new TextObject("{=Y3lGJT8H}{PARTY} Won't attack {FACTION} for {DURATION} {?DURATION>1}days{?}day{\\?}.", null);
				textObject.SetTextVariable("PARTY", base.OriginalParty.Name);
				textObject.SetTextVariable("FACTION", this._otherFaction.Name);
				textObject.SetTextVariable("DURATION", this._duration.ToDays.ToString());
				return textObject;
			}
		}

		public override void Apply()
		{
			if (base.OriginalParty == MobileParty.MainParty.Party)
			{
				if (this._otherFaction.NotAttackableByPlayerUntilTime.IsPast)
				{
					this._otherFaction.NotAttackableByPlayerUntilTime = CampaignTime.Now;
				}
				this._otherFaction.NotAttackableByPlayerUntilTime += this._duration;
			}
		}

		public override int GetUnitValueForFaction(IFaction faction)
		{
			int num = 0;
			float militaryValueOfParty = Campaign.Current.Models.ValuationModel.GetMilitaryValueOfParty(base.OriginalParty.MobileParty);
			if (faction.MapFaction == this._otherFaction.MapFaction && faction.MapFaction.IsAtWarWith(base.OriginalParty.MapFaction))
			{
				num = (int)(militaryValueOfParty * 0.1f);
			}
			else if (faction.MapFaction == base.OriginalParty.MapFaction)
			{
				num = -(int)(militaryValueOfParty * 0.1f);
			}
			return num;
		}

		public override ImageIdentifier GetVisualIdentifier()
		{
			return null;
		}

		private readonly IFaction _otherFaction;

		private readonly CampaignTime _duration;

		private readonly Hero _otherHero;

		private readonly PartyBase _otherParty;
	}
}
