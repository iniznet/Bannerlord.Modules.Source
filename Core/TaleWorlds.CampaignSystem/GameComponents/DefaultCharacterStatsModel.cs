using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x020000F9 RID: 249
	public class DefaultCharacterStatsModel : CharacterStatsModel
	{
		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x060014C1 RID: 5313 RVA: 0x0005D06E File Offset: 0x0005B26E
		public override int MaxCharacterTier
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x060014C2 RID: 5314 RVA: 0x0005D074 File Offset: 0x0005B274
		public override int GetTier(CharacterObject character)
		{
			if (character.IsHero)
			{
				return 0;
			}
			return MathF.Min(MathF.Max(MathF.Ceiling(((float)character.Level - 5f) / 5f), 0), Campaign.Current.Models.CharacterStatsModel.MaxCharacterTier);
		}

		// Token: 0x060014C3 RID: 5315 RVA: 0x0005D0C4 File Offset: 0x0005B2C4
		public override ExplainedNumber MaxHitpoints(CharacterObject character, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(100f, includeDescriptions, null);
			PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.Trainer, character, true, ref explainedNumber);
			PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.UnwaveringDefense, character, true, ref explainedNumber);
			PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.ThickHides, character, true, ref explainedNumber);
			PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Athletics.WellBuilt, character, true, ref explainedNumber);
			PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Medicine.PreventiveMedicine, character, true, ref explainedNumber);
			PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Medicine.DoctorsOath, character, false, ref explainedNumber);
			PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Medicine.FortitudeTonic, character, false, ref explainedNumber);
			if (character.IsHero && character.HeroObject.PartyBelongedTo != null && character.HeroObject.PartyBelongedTo.LeaderHero != character.HeroObject && character.HeroObject.PartyBelongedTo.HasPerk(DefaultPerks.Medicine.FortitudeTonic, false))
			{
				explainedNumber.Add(DefaultPerks.Medicine.FortitudeTonic.PrimaryBonus, DefaultPerks.Medicine.FortitudeTonic.Name, null);
			}
			if (character.GetPerkValue(DefaultPerks.Athletics.MightyBlow) && character.GetSkillValue(DefaultSkills.Athletics) > 250)
			{
				int num = character.GetSkillValue(DefaultSkills.Athletics) - 250;
				if (num > 0)
				{
					explainedNumber.Add((float)num, DefaultPerks.Athletics.MightyBlow.Name, null);
				}
			}
			return explainedNumber;
		}
	}
}
