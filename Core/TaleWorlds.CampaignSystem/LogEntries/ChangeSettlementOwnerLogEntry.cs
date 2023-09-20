﻿using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	// Token: 0x020002D0 RID: 720
	public class ChangeSettlementOwnerLogEntry : LogEntry, IEncyclopediaLog, IWarLog
	{
		// Token: 0x060029E4 RID: 10724 RVA: 0x000B2A99 File Offset: 0x000B0C99
		internal static void AutoGeneratedStaticCollectObjectsChangeSettlementOwnerLogEntry(object o, List<object> collectedObjects)
		{
			((ChangeSettlementOwnerLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x060029E5 RID: 10725 RVA: 0x000B2AA7 File Offset: 0x000B0CA7
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.Settlement);
			collectedObjects.Add(this.PreviousClan);
			collectedObjects.Add(this.NewClan);
		}

		// Token: 0x060029E6 RID: 10726 RVA: 0x000B2AD4 File Offset: 0x000B0CD4
		internal static object AutoGeneratedGetMemberValueSettlement(object o)
		{
			return ((ChangeSettlementOwnerLogEntry)o).Settlement;
		}

		// Token: 0x060029E7 RID: 10727 RVA: 0x000B2AE1 File Offset: 0x000B0CE1
		internal static object AutoGeneratedGetMemberValuePreviousClan(object o)
		{
			return ((ChangeSettlementOwnerLogEntry)o).PreviousClan;
		}

		// Token: 0x060029E8 RID: 10728 RVA: 0x000B2AEE File Offset: 0x000B0CEE
		internal static object AutoGeneratedGetMemberValueNewClan(object o)
		{
			return ((ChangeSettlementOwnerLogEntry)o).NewClan;
		}

		// Token: 0x060029E9 RID: 10729 RVA: 0x000B2AFB File Offset: 0x000B0CFB
		internal static object AutoGeneratedGetMemberValue_bySiege(object o)
		{
			return ((ChangeSettlementOwnerLogEntry)o)._bySiege;
		}

		// Token: 0x060029EA RID: 10730 RVA: 0x000B2B0D File Offset: 0x000B0D0D
		public ChangeSettlementOwnerLogEntry(Settlement settlement, Hero newOwner, Hero previousOwner, bool bySiege)
		{
			this.Settlement = settlement;
			this.PreviousClan = previousOwner.Clan;
			this.NewClan = newOwner.Clan;
			this._bySiege = bySiege;
		}

		// Token: 0x060029EB RID: 10731 RVA: 0x000B2B3C File Offset: 0x000B0D3C
		public override ImportanceEnum GetImportanceForClan(Clan clan)
		{
			if (this.PreviousClan == this.NewClan)
			{
				return ImportanceEnum.Zero;
			}
			if (this.NewClan != clan)
			{
				return ImportanceEnum.Important;
			}
			return ImportanceEnum.VeryImportant;
		}

		// Token: 0x060029EC RID: 10732 RVA: 0x000B2B5C File Offset: 0x000B0D5C
		public bool IsRelatedToWar(StanceLink stance, out IFaction effector, out IFaction effected)
		{
			IFaction faction = stance.Faction1;
			IFaction faction2 = stance.Faction2;
			effector = this.NewClan.MapFaction;
			effected = this.PreviousClan.MapFaction;
			return (this.NewClan.MapFaction == faction && this.PreviousClan.MapFaction == faction2) || (this.NewClan.MapFaction == faction2 && this.PreviousClan.MapFaction == faction);
		}

		// Token: 0x060029ED RID: 10733 RVA: 0x000B2BCD File Offset: 0x000B0DCD
		public override void GetConversationScoreAndComment(Hero talkTroop, bool findString, out string comment, out ImportanceEnum score)
		{
			score = ImportanceEnum.Zero;
			comment = "";
			if (this._bySiege && (this.NewClan == Clan.PlayerClan || this.PreviousClan == Clan.PlayerClan))
			{
				score = ImportanceEnum.Important;
				if (findString)
				{
					comment = "str_comment_changeownerofsettlement_you_captured_castle";
				}
			}
		}

		// Token: 0x060029EE RID: 10734 RVA: 0x000B2C0C File Offset: 0x000B0E0C
		public override int GetAsRumor(Settlement talkSettlement, ref TextObject comment)
		{
			int num = 0;
			Settlement settlement = this.Settlement;
			float num2;
			if (this.NewClan.IsBanditFaction && settlement.IsHideout && Campaign.Current.Models.MapDistanceModel.GetDistance(talkSettlement, settlement, 60f, out num2))
			{
				comment = new TextObject("{=MXGtQ6YV}I hear {.%}{BANDIT_NAME}{.%} have moved into the old {HIDEOUT_NAME} near here. Travellers better watch themselves.", null);
				comment.SetTextVariable("BANDIT_NAME", this.NewClan.Name);
				comment.SetTextVariable("HIDEOUT_NAME", settlement.Name);
				return 4;
			}
			if (this._bySiege && this.PreviousClan == talkSettlement.MapFaction)
			{
				comment = new TextObject("{=UMn2QMIk}Did you hear {ENEMY_NAME} took {FORTRESS_NAME} by storm? Do you think they'll come here next?", null);
				comment.SetTextVariable("ENEMY_NAME", FactionHelper.GetTermUsedByOtherFaction(this.NewClan, talkSettlement.MapFaction, false));
				comment.SetTextVariable("FORTRESS_NAME", settlement.Name);
				return 10;
			}
			return num;
		}

		// Token: 0x060029EF RID: 10735 RVA: 0x000B2CE9 File Offset: 0x000B0EE9
		public override string ToString()
		{
			return this.GetEncyclopediaText().ToString();
		}

		// Token: 0x060029F0 RID: 10736 RVA: 0x000B2CF6 File Offset: 0x000B0EF6
		public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
		{
			return (obj == this.Settlement || obj == this.NewClan || obj == this.NewClan.Leader) && !this._bySiege;
		}

		// Token: 0x060029F1 RID: 10737 RVA: 0x000B2D34 File Offset: 0x000B0F34
		public TextObject GetEncyclopediaText()
		{
			TextObject textObject = GameTexts.FindText("str_settlement_owner_changed_news", null);
			textObject.SetTextVariable("SETTLEMENT", this.Settlement.IsHideout ? this.Settlement.Name : this.Settlement.EncyclopediaLinkWithName);
			if (this.NewClan == null && this.Settlement.IsHideout)
			{
				return GameTexts.FindText("str_hideout_owner_changed_news", null);
			}
			StringHelpers.SetCharacterProperties("LORD", this.NewClan.Leader.CharacterObject, textObject, false);
			return textObject;
		}

		// Token: 0x04000CAF RID: 3247
		[SaveableField(80)]
		public readonly Settlement Settlement;

		// Token: 0x04000CB0 RID: 3248
		[SaveableField(81)]
		public readonly Clan PreviousClan;

		// Token: 0x04000CB1 RID: 3249
		[SaveableField(82)]
		public readonly Clan NewClan;

		// Token: 0x04000CB2 RID: 3250
		[SaveableField(83)]
		private readonly bool _bySiege;
	}
}
