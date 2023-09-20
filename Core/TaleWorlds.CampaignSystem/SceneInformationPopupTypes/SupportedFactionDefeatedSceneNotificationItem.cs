using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000C4 RID: 196
	public class SupportedFactionDefeatedSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x06001257 RID: 4695 RVA: 0x00054242 File Offset: 0x00052442
		public Kingdom Faction { get; }

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x06001258 RID: 4696 RVA: 0x0005424A File Offset: 0x0005244A
		public bool PlayerWantsRestore { get; }

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06001259 RID: 4697 RVA: 0x00054252 File Offset: 0x00052452
		public override string SceneID
		{
			get
			{
				return "scn_supported_faction_defeated_notification";
			}
		}

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x0600125A RID: 4698 RVA: 0x0005425C File Offset: 0x0005245C
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("FORMAL_NAME", CampaignSceneNotificationHelper.GetFormalNameForKingdom(this.Faction));
				GameTexts.SetVariable("PLAYER_WANTS_RESTORE", this.PlayerWantsRestore ? 1 : 0);
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				return GameTexts.FindText("str_supported_faction_defeated", null);
			}
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x000542CC File Offset: 0x000524CC
		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner>
			{
				this.Faction.Banner,
				this.Faction.Banner
			};
		}

		// Token: 0x0600125C RID: 4700 RVA: 0x000542F5 File Offset: 0x000524F5
		public SupportedFactionDefeatedSceneNotificationItem(Kingdom faction, bool playerWantsRestore)
		{
			this.Faction = faction;
			this.PlayerWantsRestore = playerWantsRestore;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x0400067B RID: 1659
		private readonly CampaignTime _creationCampaignTime;
	}
}
