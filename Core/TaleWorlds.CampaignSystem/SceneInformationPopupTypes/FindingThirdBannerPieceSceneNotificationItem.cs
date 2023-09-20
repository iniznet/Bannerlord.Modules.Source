using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000B6 RID: 182
	public class FindingThirdBannerPieceSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x060011EA RID: 4586 RVA: 0x0005200B File Offset: 0x0005020B
		public override string SceneID
		{
			get
			{
				return "scn_third_banner_piece_notification";
			}
		}

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x060011EB RID: 4587 RVA: 0x00052012 File Offset: 0x00050212
		public override bool IsAffirmativeOptionShown { get; }

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x060011EC RID: 4588 RVA: 0x0005201C File Offset: 0x0005021C
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				return GameTexts.FindText("str_third_banner_piece_found", null);
			}
		}

		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x060011ED RID: 4589 RVA: 0x00052061 File Offset: 0x00050261
		public override TextObject AffirmativeTitleText
		{
			get
			{
				return GameTexts.FindText("str_third_banner_piece_found_assembled", null);
			}
		}

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x060011EE RID: 4590 RVA: 0x0005206E File Offset: 0x0005026E
		public override TextObject AffirmativeText
		{
			get
			{
				return new TextObject("{=6mgapvxb}Assemble", null);
			}
		}

		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x060011EF RID: 4591 RVA: 0x0005207B File Offset: 0x0005027B
		public override TextObject AffirmativeDescriptionText
		{
			get
			{
				return new TextObject("{=IRLB42FY}Assemble the dragon banner!", null);
			}
		}

		// Token: 0x060011F0 RID: 4592 RVA: 0x00052088 File Offset: 0x00050288
		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner> { Hero.MainHero.ClanBanner };
		}

		// Token: 0x060011F1 RID: 4593 RVA: 0x0005209F File Offset: 0x0005029F
		public FindingThirdBannerPieceSceneNotificationItem()
		{
			this.IsAffirmativeOptionShown = true;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x04000641 RID: 1601
		private readonly CampaignTime _creationCampaignTime;
	}
}
