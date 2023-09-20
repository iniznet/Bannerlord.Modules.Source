using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000B2 RID: 178
	public class AntiEmpireConspiracyBeginsSceneNotificationItem : EmpireConspiracySupportsSceneNotificationItemBase
	{
		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x060011DC RID: 4572 RVA: 0x00051DE8 File Offset: 0x0004FFE8
		public override TextObject TitleText
		{
			get
			{
				List<TextObject> list = new List<TextObject>();
				foreach (Kingdom kingdom in this._antiEmpireFactions)
				{
					list.Add(kingdom.InformalName);
				}
				TextObject textObject = GameTexts.FindText("str_empire_conspiracy_supports_antiempire", null);
				textObject.SetTextVariable("FACTION_NAMES", GameTexts.GameTextHelper.MergeTextObjectsWithComma(list, true));
				textObject.SetTextVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(CampaignTime.Now));
				textObject.SetTextVariable("YEAR", CampaignTime.Now.GetYear);
				return textObject;
			}
		}

		// Token: 0x060011DD RID: 4573 RVA: 0x00051E94 File Offset: 0x00050094
		public AntiEmpireConspiracyBeginsSceneNotificationItem(Hero kingHero, List<Kingdom> antiEmpireFactions)
			: base(kingHero)
		{
			this._antiEmpireFactions = antiEmpireFactions;
		}

		// Token: 0x0400063A RID: 1594
		private readonly List<Kingdom> _antiEmpireFactions;
	}
}
