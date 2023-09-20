using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x0200039D RID: 925
	public interface IEducationLogic
	{
		// Token: 0x06003730 RID: 14128
		void Finalize(Hero child, List<string> chosenOptions);

		// Token: 0x06003731 RID: 14129
		void GetOptionProperties(Hero child, string optionKey, List<string> previousChoices, out TextObject optionTitle, out TextObject description, out TextObject effect, out ValueTuple<CharacterAttribute, int>[] attributes, out ValueTuple<SkillObject, int>[] skills, out ValueTuple<SkillObject, int>[] focusPoints, out EducationCampaignBehavior.EducationCharacterProperties[] characterProperties);

		// Token: 0x06003732 RID: 14130
		void GetPageProperties(Hero child, List<string> previousChoices, out TextObject title, out TextObject description, out TextObject instruction, out EducationCampaignBehavior.EducationCharacterProperties[] defaultProperties, out string[] availableOptions);

		// Token: 0x06003733 RID: 14131
		void GetStageProperties(Hero child, out int pageCount);

		// Token: 0x06003734 RID: 14132
		bool IsValidEducationNotification(EducationMapNotification educationMapNotification);
	}
}
