using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	// Token: 0x02000353 RID: 851
	public interface IHeroDeveloper
	{
		// Token: 0x06002FD6 RID: 12246
		void SetInitialLevel(int i);

		// Token: 0x06002FD7 RID: 12247
		void AddSkillXp(SkillObject skill, float rawXp, bool isAffectedByFocusFactor = true, bool shouldNotify = true);

		// Token: 0x06002FD8 RID: 12248
		void CheckInitialLevel();

		// Token: 0x17000B88 RID: 2952
		// (get) Token: 0x06002FD9 RID: 12249
		// (set) Token: 0x06002FDA RID: 12250
		int UnspentFocusPoints { get; set; }

		// Token: 0x17000B89 RID: 2953
		// (get) Token: 0x06002FDB RID: 12251
		// (set) Token: 0x06002FDC RID: 12252
		int UnspentAttributePoints { get; set; }

		// Token: 0x06002FDD RID: 12253
		void ClearUnspentPoints();

		// Token: 0x06002FDE RID: 12254
		void AddFocus(SkillObject skill, int changeAmount, bool checkUnspentFocusPoints = true);

		// Token: 0x06002FDF RID: 12255
		void RemoveFocus(SkillObject skill, int changeAmount);

		// Token: 0x06002FE0 RID: 12256
		void DeriveSkillsFromTraits(bool isByNaturalGrowth = false, CharacterObject template = null);

		// Token: 0x06002FE1 RID: 12257
		void SetInitialSkillLevel(SkillObject skill, int newSkillValue);

		// Token: 0x06002FE2 RID: 12258
		void InitializeSkillXp(SkillObject skill);

		// Token: 0x06002FE3 RID: 12259
		void ClearHero();

		// Token: 0x06002FE4 RID: 12260
		void AddPerk(PerkObject perk);

		// Token: 0x06002FE5 RID: 12261
		float GetFocusFactor(SkillObject skill);

		// Token: 0x06002FE6 RID: 12262
		void AddAttribute(CharacterAttribute attribute, int changeAmount, bool checkUnspentPoints = true);

		// Token: 0x06002FE7 RID: 12263
		void RemoveAttribute(CharacterAttribute attrib, int changeAmount);

		// Token: 0x06002FE8 RID: 12264
		void ChangeSkillLevel(SkillObject skill, int changeAmount, bool shouldNotify = true);

		// Token: 0x06002FE9 RID: 12265
		int GetFocus(SkillObject skill);

		// Token: 0x06002FEA RID: 12266
		bool CanAddFocusToSkill(SkillObject skill);

		// Token: 0x06002FEB RID: 12267
		void AfterLoad();

		// Token: 0x06002FEC RID: 12268
		int GetTotalSkillPoints();

		// Token: 0x06002FED RID: 12269
		int GetXpRequiredForLevel(int level);

		// Token: 0x06002FEE RID: 12270
		MBReadOnlyList<PerkObject> GetOneAvailablePerkForEachPerkPair();

		// Token: 0x06002FEF RID: 12271
		int GetRequiredFocusPointsToAddFocus(SkillObject skill);

		// Token: 0x17000B8A RID: 2954
		// (get) Token: 0x06002FF0 RID: 12272
		int TotalXp { get; }

		// Token: 0x17000B8B RID: 2955
		// (get) Token: 0x06002FF1 RID: 12273
		Hero Hero { get; }

		// Token: 0x06002FF2 RID: 12274
		int GetSkillXpProgress(SkillObject skill);

		// Token: 0x06002FF3 RID: 12275
		bool GetPerkValue(PerkObject perk);
	}
}
