using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.BarterSystem.Barterables
{
	// Token: 0x0200041B RID: 1051
	public class PeaceBarterable : Barterable
	{
		// Token: 0x17000D26 RID: 3366
		// (get) Token: 0x06003E19 RID: 15897 RVA: 0x00128DBA File Offset: 0x00126FBA
		public CampaignTime Duration { get; }

		// Token: 0x17000D27 RID: 3367
		// (get) Token: 0x06003E1A RID: 15898 RVA: 0x00128DC2 File Offset: 0x00126FC2
		public override string StringID
		{
			get
			{
				return "peace_barterable";
			}
		}

		// Token: 0x06003E1B RID: 15899 RVA: 0x00128DC9 File Offset: 0x00126FC9
		public PeaceBarterable(Hero owner, IFaction peaceOfferingFaction, IFaction offeredFaction, CampaignTime duration)
			: base(owner, null)
		{
			this.Duration = duration;
			this.PeaceOfferingFaction = peaceOfferingFaction;
			this.OfferedFaction = offeredFaction;
		}

		// Token: 0x06003E1C RID: 15900 RVA: 0x00128DE9 File Offset: 0x00126FE9
		public PeaceBarterable(IFaction peaceOfferingFaction, IFaction offeredFaction, CampaignTime duration)
			: base(peaceOfferingFaction.Leader, null)
		{
			this.Duration = duration;
			this.PeaceOfferingFaction = peaceOfferingFaction;
			this.OfferedFaction = offeredFaction;
		}

		// Token: 0x17000D28 RID: 3368
		// (get) Token: 0x06003E1D RID: 15901 RVA: 0x00128E0D File Offset: 0x0012700D
		public override TextObject Name
		{
			get
			{
				TextObject textObject = new TextObject("{=R0bJS0pn}Make peace with the {OTHER_FACTION}", null);
				textObject.SetTextVariable("OTHER_FACTION", this.OfferedFaction.InformalName);
				return textObject;
			}
		}

		// Token: 0x06003E1E RID: 15902 RVA: 0x00128E34 File Offset: 0x00127034
		public override int GetUnitValueForFaction(IFaction factionToEvaluateFor)
		{
			float num = 0f;
			IFaction faction = this.OfferedFaction;
			IFaction faction2 = this.PeaceOfferingFaction;
			if (factionToEvaluateFor.MapFaction == faction)
			{
				IFaction faction3 = faction;
				faction = faction2;
				faction2 = faction3;
			}
			if (faction == null || faction2 == null)
			{
				return 0;
			}
			TextObject textObject;
			num = (float)((int)Campaign.Current.Models.DiplomacyModel.GetScoreOfDeclaringPeace(faction2, faction, factionToEvaluateFor, out textObject));
			if (factionToEvaluateFor.IsKingdomFaction)
			{
				float num2 = 0f;
				int num3 = 0;
				foreach (Clan clan in ((Kingdom)factionToEvaluateFor).Clans)
				{
					float num4 = ((clan.Leader != null) ? ((clan.Leader.Gold < 50000) ? (1f + 0.5f * ((50000f - (float)clan.Leader.Gold) / 50000f)) : ((clan.Leader.Gold > 200000) ? MathF.Max(0.66f, MathF.Pow(200000f / (float)clan.Leader.Gold, 0.4f)) : 1f)) : 1f);
					num2 += num4;
					num3++;
				}
				float num5 = (num2 + 1f) / ((float)num3 + 1f);
				num /= num5;
			}
			return (int)num;
		}

		// Token: 0x06003E1F RID: 15903 RVA: 0x00128F9C File Offset: 0x0012719C
		public override bool IsCompatible(Barterable barterable)
		{
			PeaceBarterable peaceBarterable = barterable as PeaceBarterable;
			return peaceBarterable == null || peaceBarterable.OfferedFaction != base.OriginalOwner.MapFaction;
		}

		// Token: 0x06003E20 RID: 15904 RVA: 0x00128FCB File Offset: 0x001271CB
		public override ImageIdentifier GetVisualIdentifier()
		{
			return null;
		}

		// Token: 0x06003E21 RID: 15905 RVA: 0x00128FCE File Offset: 0x001271CE
		public override string GetEncyclopediaLink()
		{
			return base.OriginalOwner.MapFaction.EncyclopediaLink;
		}

		// Token: 0x06003E22 RID: 15906 RVA: 0x00128FE0 File Offset: 0x001271E0
		public override void Apply()
		{
			if (this.PeaceOfferingFaction.MapFaction.IsAtWarWith(this.OfferedFaction))
			{
				MakePeaceAction.Apply(this.PeaceOfferingFaction.MapFaction, this.OfferedFaction, 0);
				if (PlayerEncounter.Current != null && Hero.OneToOneConversationHero == base.OriginalOwner)
				{
					PlayerEncounter.LeaveEncounter = true;
					PartyBase originalParty = base.OriginalParty;
					bool flag;
					if (originalParty == null)
					{
						flag = null != null;
					}
					else
					{
						MobileParty mobileParty = originalParty.MobileParty;
						flag = ((mobileParty != null) ? mobileParty.Ai.AiBehaviorPartyBase : null) != null;
					}
					if (flag)
					{
						LocatableSearchData<MobileParty> locatableSearchData = Campaign.Current.MobilePartyLocator.StartFindingLocatablesAroundPosition(MobileParty.MainParty.Position2D, 5f);
						for (MobileParty mobileParty2 = Campaign.Current.MobilePartyLocator.FindNextLocatable(ref locatableSearchData); mobileParty2 != null; mobileParty2 = Campaign.Current.MobilePartyLocator.FindNextLocatable(ref locatableSearchData))
						{
							if (!mobileParty2.IsMainParty && mobileParty2.MapFaction == base.OriginalOwner.MapFaction && (mobileParty2.TargetParty == MobileParty.MainParty || mobileParty2.Ai.AiBehaviorPartyBase == PartyBase.MainParty))
							{
								mobileParty2.Ai.SetMoveModeHold();
							}
						}
						if (base.OriginalParty.MobileParty.Army != null && MobileParty.MainParty.Army != base.OriginalParty.MobileParty.Army)
						{
							base.OriginalParty.MobileParty.Army.LeaderParty.Ai.SetMoveModeHold();
						}
					}
				}
			}
		}

		// Token: 0x06003E23 RID: 15907 RVA: 0x00129141 File Offset: 0x00127341
		internal static void AutoGeneratedStaticCollectObjectsPeaceBarterable(object o, List<object> collectedObjects)
		{
			((PeaceBarterable)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06003E24 RID: 15908 RVA: 0x0012914F File Offset: 0x0012734F
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x040012A5 RID: 4773
		public readonly IFaction PeaceOfferingFaction;

		// Token: 0x040012A6 RID: 4774
		public readonly IFaction OfferedFaction;
	}
}
