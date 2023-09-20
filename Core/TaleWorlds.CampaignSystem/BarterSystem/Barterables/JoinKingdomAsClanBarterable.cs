using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.BarterSystem.Barterables
{
	// Token: 0x02000416 RID: 1046
	public class JoinKingdomAsClanBarterable : Barterable
	{
		// Token: 0x17000D1C RID: 3356
		// (get) Token: 0x06003DE5 RID: 15845 RVA: 0x00127E4D File Offset: 0x0012604D
		public override string StringID
		{
			get
			{
				return "join_faction_barterable";
			}
		}

		// Token: 0x06003DE6 RID: 15846 RVA: 0x00127E54 File Offset: 0x00126054
		public JoinKingdomAsClanBarterable(Hero owner, Kingdom targetKingdom, bool isDefecting = false)
			: base(owner, null)
		{
			this.TargetKingdom = targetKingdom;
			this.IsDefecting = isDefecting;
		}

		// Token: 0x17000D1D RID: 3357
		// (get) Token: 0x06003DE7 RID: 15847 RVA: 0x00127E6C File Offset: 0x0012606C
		public override TextObject Name
		{
			get
			{
				TextObject textObject = new TextObject("{=8Az4q2wp}Join {FACTION}", null);
				textObject.SetTextVariable("FACTION", this.TargetKingdom.Name);
				return textObject;
			}
		}

		// Token: 0x06003DE8 RID: 15848 RVA: 0x00127E90 File Offset: 0x00126090
		public override int GetUnitValueForFaction(IFaction factionForEvaluation)
		{
			float num = -1000000f;
			if (factionForEvaluation == base.OriginalOwner.Clan)
			{
				num = Campaign.Current.Models.DiplomacyModel.GetScoreOfClanToJoinKingdom(base.OriginalOwner.Clan, this.TargetKingdom);
				if (base.OriginalOwner.Clan.Kingdom != null)
				{
					int valueForFaction = new LeaveKingdomAsClanBarterable(base.OriginalOwner, base.OriginalParty).GetValueForFaction(factionForEvaluation);
					if (!this.TargetKingdom.IsAtWarWith(base.OriginalOwner.Clan.Kingdom))
					{
						float num2 = base.OriginalOwner.Clan.CalculateTotalSettlementValueForFaction(base.OriginalOwner.Clan.Kingdom);
						num -= num2 * ((this.TargetKingdom.Leader == Hero.MainHero) ? 0.5f : 1f);
					}
					num += (float)valueForFaction;
				}
			}
			else if (factionForEvaluation.MapFaction == this.TargetKingdom)
			{
				num = Campaign.Current.Models.DiplomacyModel.GetScoreOfKingdomToGetClan(this.TargetKingdom, base.OriginalOwner.Clan);
			}
			if (this.TargetKingdom == Clan.PlayerClan.Kingdom && Hero.MainHero.GetPerkValue(DefaultPerks.Trade.SilverTongue))
			{
				num += num * DefaultPerks.Trade.SilverTongue.PrimaryBonus;
			}
			return (int)num;
		}

		// Token: 0x06003DE9 RID: 15849 RVA: 0x00127FDD File Offset: 0x001261DD
		public override void CheckBarterLink(Barterable linkedBarterable)
		{
		}

		// Token: 0x06003DEA RID: 15850 RVA: 0x00127FE0 File Offset: 0x001261E0
		public override bool IsCompatible(Barterable barterable)
		{
			LeaveKingdomAsClanBarterable leaveKingdomAsClanBarterable = barterable as LeaveKingdomAsClanBarterable;
			return leaveKingdomAsClanBarterable == null || leaveKingdomAsClanBarterable.OriginalOwner.MapFaction != this.TargetKingdom;
		}

		// Token: 0x06003DEB RID: 15851 RVA: 0x0012800F File Offset: 0x0012620F
		public override ImageIdentifier GetVisualIdentifier()
		{
			return new ImageIdentifier(BannerCode.CreateFrom(this.TargetKingdom.Banner), false);
		}

		// Token: 0x06003DEC RID: 15852 RVA: 0x00128027 File Offset: 0x00126227
		public override string GetEncyclopediaLink()
		{
			return this.TargetKingdom.EncyclopediaLink;
		}

		// Token: 0x06003DED RID: 15853 RVA: 0x00128034 File Offset: 0x00126234
		public override void Apply()
		{
			if (this.TargetKingdom != null && this.TargetKingdom != null && this.TargetKingdom.Leader == Hero.MainHero)
			{
				int valueForFaction = base.GetValueForFaction(base.OriginalOwner.Clan);
				int num = ((valueForFaction < 0) ? (20 - valueForFaction / 20000) : 20);
				ChangeRelationAction.ApplyPlayerRelation(base.OriginalOwner.Clan.Leader, num, true, true);
				if (base.OriginalOwner.Clan.MapFaction != null)
				{
					ChangeRelationAction.ApplyRelationChangeBetweenHeroes(base.OriginalOwner.Clan.Leader, base.OriginalOwner.Clan.MapFaction.Leader, -100, true);
				}
			}
			if (PlayerEncounter.Current != null && PlayerEncounter.Current.PlayerSide == BattleSideEnum.Defender && PlayerSiege.PlayerSiegeEvent != null && PlayerSiege.PlayerSide == BattleSideEnum.Attacker)
			{
				PlayerEncounter.Current.SetPlayerSiegeInterruptedByEnemyDefection();
			}
			if (base.OriginalOwner.Clan.Kingdom != null)
			{
				if (base.OriginalOwner.Clan.Kingdom != null && this.TargetKingdom != null && base.OriginalOwner.Clan.Kingdom.IsAtWarWith(this.TargetKingdom))
				{
					ChangeKingdomAction.ApplyByLeaveWithRebellionAgainstKingdom(base.OriginalOwner.Clan, true);
				}
				else
				{
					ChangeKingdomAction.ApplyByLeaveKingdom(base.OriginalOwner.Clan, true);
				}
			}
			if (this.IsDefecting)
			{
				ChangeKingdomAction.ApplyByJoinToKingdomByDefection(base.OriginalOwner.Clan, this.TargetKingdom, true);
				return;
			}
			ChangeKingdomAction.ApplyByJoinToKingdom(base.OriginalOwner.Clan, this.TargetKingdom, true);
		}

		// Token: 0x06003DEE RID: 15854 RVA: 0x001281B5 File Offset: 0x001263B5
		internal static void AutoGeneratedStaticCollectObjectsJoinKingdomAsClanBarterable(object o, List<object> collectedObjects)
		{
			((JoinKingdomAsClanBarterable)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06003DEF RID: 15855 RVA: 0x001281C3 File Offset: 0x001263C3
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x0400129B RID: 4763
		public readonly Kingdom TargetKingdom;

		// Token: 0x0400129C RID: 4764
		public readonly bool IsDefecting;
	}
}
