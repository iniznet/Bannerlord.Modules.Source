using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.BarterSystem.Barterables
{
	// Token: 0x0200041A RID: 1050
	public class NoAttackBarterable : Barterable
	{
		// Token: 0x17000D24 RID: 3364
		// (get) Token: 0x06003E13 RID: 15891 RVA: 0x00128C34 File Offset: 0x00126E34
		public override string StringID
		{
			get
			{
				return "no_attack_barterable";
			}
		}

		// Token: 0x06003E14 RID: 15892 RVA: 0x00128C3B File Offset: 0x00126E3B
		public NoAttackBarterable(Hero originalOwner, Hero otherHero, PartyBase ownerParty, PartyBase otherParty, CampaignTime duration)
			: base(originalOwner, ownerParty)
		{
			this._otherFaction = otherParty.MapFaction;
			this._duration = duration;
			this._otherHero = otherHero;
			this._otherParty = otherParty;
		}

		// Token: 0x17000D25 RID: 3365
		// (get) Token: 0x06003E15 RID: 15893 RVA: 0x00128C6C File Offset: 0x00126E6C
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

		// Token: 0x06003E16 RID: 15894 RVA: 0x00128CD4 File Offset: 0x00126ED4
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

		// Token: 0x06003E17 RID: 15895 RVA: 0x00128D34 File Offset: 0x00126F34
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

		// Token: 0x06003E18 RID: 15896 RVA: 0x00128DB7 File Offset: 0x00126FB7
		public override ImageIdentifier GetVisualIdentifier()
		{
			return null;
		}

		// Token: 0x040012A0 RID: 4768
		private readonly IFaction _otherFaction;

		// Token: 0x040012A1 RID: 4769
		private readonly CampaignTime _duration;

		// Token: 0x040012A2 RID: 4770
		private readonly Hero _otherHero;

		// Token: 0x040012A3 RID: 4771
		private readonly PartyBase _otherParty;
	}
}
