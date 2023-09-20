using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000B3 RID: 179
	public class ProEmpireConspiracyBeginsSceneNotificationItem : EmpireConspiracySupportsSceneNotificationItemBase
	{
		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x060011DE RID: 4574 RVA: 0x00051EA4 File Offset: 0x000500A4
		public override TextObject TitleText
		{
			get
			{
				TextObject textObject = GameTexts.FindText("str_empire_conspiracy_supports_proempire", null);
				textObject.SetTextVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(CampaignTime.Now));
				textObject.SetTextVariable("YEAR", CampaignTime.Now.GetYear);
				return textObject;
			}
		}

		// Token: 0x060011DF RID: 4575 RVA: 0x00051EEB File Offset: 0x000500EB
		public ProEmpireConspiracyBeginsSceneNotificationItem(Hero kingHero)
			: base(kingHero)
		{
		}
	}
}
