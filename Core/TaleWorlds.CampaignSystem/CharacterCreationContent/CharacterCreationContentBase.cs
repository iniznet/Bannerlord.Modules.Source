using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	// Token: 0x020001DC RID: 476
	public abstract class CharacterCreationContentBase
	{
		// Token: 0x17000736 RID: 1846
		// (get) Token: 0x06001BF8 RID: 7160 RVA: 0x0007E022 File Offset: 0x0007C222
		public static CharacterCreationContentBase Instance
		{
			get
			{
				CharacterCreationState characterCreationState = GameStateManager.Current.ActiveState as CharacterCreationState;
				if (characterCreationState == null)
				{
					return null;
				}
				return characterCreationState.CurrentCharacterCreationContent;
			}
		}

		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x06001BF9 RID: 7161 RVA: 0x0007E03E File Offset: 0x0007C23E
		protected virtual int ChildhoodAge
		{
			get
			{
				return 7;
			}
		}

		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x06001BFA RID: 7162 RVA: 0x0007E041 File Offset: 0x0007C241
		protected virtual int EducationAge
		{
			get
			{
				return 12;
			}
		}

		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x06001BFB RID: 7163 RVA: 0x0007E045 File Offset: 0x0007C245
		protected virtual int YouthAge
		{
			get
			{
				return 17;
			}
		}

		// Token: 0x1700073A RID: 1850
		// (get) Token: 0x06001BFC RID: 7164 RVA: 0x0007E049 File Offset: 0x0007C249
		protected virtual int AccomplishmentAge
		{
			get
			{
				return 20;
			}
		}

		// Token: 0x1700073B RID: 1851
		// (get) Token: 0x06001BFD RID: 7165 RVA: 0x0007E04D File Offset: 0x0007C24D
		protected virtual int FocusToAdd
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700073C RID: 1852
		// (get) Token: 0x06001BFE RID: 7166 RVA: 0x0007E050 File Offset: 0x0007C250
		protected virtual int SkillLevelToAdd
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x1700073D RID: 1853
		// (get) Token: 0x06001BFF RID: 7167 RVA: 0x0007E054 File Offset: 0x0007C254
		protected virtual int AttributeLevelToAdd
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700073E RID: 1854
		// (get) Token: 0x06001C00 RID: 7168 RVA: 0x0007E057 File Offset: 0x0007C257
		public virtual int FocusToAddByCulture
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x06001C01 RID: 7169 RVA: 0x0007E05A File Offset: 0x0007C25A
		public virtual int SkillLevelToAddByCulture
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x17000740 RID: 1856
		// (get) Token: 0x06001C02 RID: 7170 RVA: 0x0007E05E File Offset: 0x0007C25E
		// (set) Token: 0x06001C03 RID: 7171 RVA: 0x0007E066 File Offset: 0x0007C266
		protected int SelectedParentType { get; set; } = 1;

		// Token: 0x17000741 RID: 1857
		// (get) Token: 0x06001C04 RID: 7172 RVA: 0x0007E06F File Offset: 0x0007C26F
		// (set) Token: 0x06001C05 RID: 7173 RVA: 0x0007E077 File Offset: 0x0007C277
		protected int SelectedTitleType { get; set; }

		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x06001C06 RID: 7174
		public abstract TextObject ReviewPageDescription { get; }

		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x06001C07 RID: 7175 RVA: 0x0007E080 File Offset: 0x0007C280
		// (set) Token: 0x06001C08 RID: 7176 RVA: 0x0007E088 File Offset: 0x0007C288
		protected FaceGenChar MotherFacegenCharacter { get; set; }

		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x06001C09 RID: 7177 RVA: 0x0007E091 File Offset: 0x0007C291
		// (set) Token: 0x06001C0A RID: 7178 RVA: 0x0007E099 File Offset: 0x0007C299
		protected FaceGenChar FatherFacegenCharacter { get; set; }

		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x06001C0B RID: 7179 RVA: 0x0007E0A2 File Offset: 0x0007C2A2
		// (set) Token: 0x06001C0C RID: 7180 RVA: 0x0007E0AA File Offset: 0x0007C2AA
		protected BodyProperties PlayerBodyProperties { get; set; }

		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x06001C0D RID: 7181 RVA: 0x0007E0B3 File Offset: 0x0007C2B3
		// (set) Token: 0x06001C0E RID: 7182 RVA: 0x0007E0BB File Offset: 0x0007C2BB
		protected Equipment PlayerStartEquipment { get; set; }

		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x06001C0F RID: 7183 RVA: 0x0007E0C4 File Offset: 0x0007C2C4
		// (set) Token: 0x06001C10 RID: 7184 RVA: 0x0007E0CC File Offset: 0x0007C2CC
		protected Equipment PlayerCivilianEquipment { get; set; }

		// Token: 0x17000748 RID: 1864
		// (get) Token: 0x06001C11 RID: 7185
		public abstract IEnumerable<Type> CharacterCreationStages { get; }

		// Token: 0x06001C12 RID: 7186 RVA: 0x0007E0D5 File Offset: 0x0007C2D5
		public void Initialize(CharacterCreation characterCreation)
		{
			this.OnInitialized(characterCreation);
			this.SetMainHeroInitialStats();
		}

		// Token: 0x06001C13 RID: 7187 RVA: 0x0007E0E4 File Offset: 0x0007C2E4
		protected virtual void OnInitialized(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001C14 RID: 7188 RVA: 0x0007E0E8 File Offset: 0x0007C2E8
		public void ApplySkillAndAttributeEffects(List<SkillObject> skills, int focusToAdd, int skillLevelToAdd, CharacterAttribute attribute, int attributeLevelToAdd, List<TraitObject> traits = null, int traitLevelToAdd = 0, int renownToAdd = 0, int goldToAdd = 0, int unspentFocusPoints = 0, int unspentAttributePoints = 0)
		{
			foreach (SkillObject skillObject in skills)
			{
				Hero.MainHero.HeroDeveloper.AddFocus(skillObject, focusToAdd, false);
				if (Hero.MainHero.GetSkillValue(skillObject) == 1)
				{
					Hero.MainHero.HeroDeveloper.ChangeSkillLevel(skillObject, skillLevelToAdd - 1, false);
				}
				else
				{
					Hero.MainHero.HeroDeveloper.ChangeSkillLevel(skillObject, skillLevelToAdd, false);
				}
			}
			Hero.MainHero.HeroDeveloper.UnspentFocusPoints += unspentFocusPoints;
			Hero.MainHero.HeroDeveloper.UnspentAttributePoints += unspentAttributePoints;
			if (attribute != null)
			{
				Hero.MainHero.HeroDeveloper.AddAttribute(attribute, attributeLevelToAdd, false);
			}
			if (traits != null && traitLevelToAdd > 0 && traits.Count > 0)
			{
				foreach (TraitObject traitObject in traits)
				{
					Hero.MainHero.SetTraitLevel(traitObject, Hero.MainHero.GetTraitLevel(traitObject) + traitLevelToAdd);
				}
			}
			if (renownToAdd > 0)
			{
				GainRenownAction.Apply(Hero.MainHero, (float)renownToAdd, true);
			}
			if (goldToAdd > 0)
			{
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, goldToAdd, true);
			}
			Hero.MainHero.HeroDeveloper.SetInitialLevel(1);
		}

		// Token: 0x06001C15 RID: 7189 RVA: 0x0007E258 File Offset: 0x0007C458
		public void SetPlayerBanner(Banner banner)
		{
			this._banner = banner;
		}

		// Token: 0x06001C16 RID: 7190 RVA: 0x0007E261 File Offset: 0x0007C461
		public Banner GetCurrentPlayerBanner()
		{
			return this._banner;
		}

		// Token: 0x06001C17 RID: 7191 RVA: 0x0007E269 File Offset: 0x0007C469
		public void SetSelectedCulture(CultureObject culture, CharacterCreation characterCreation)
		{
			this._culture = culture;
			characterCreation.ResetMenuOptions();
			this.OnCultureSelected();
		}

		// Token: 0x06001C18 RID: 7192 RVA: 0x0007E280 File Offset: 0x0007C480
		public void ApplyCulture(CharacterCreation characterCreation)
		{
			CharacterObject.PlayerCharacter.Culture = CharacterCreationContentBase.Instance._culture;
			Clan.PlayerClan.Culture = CharacterCreationContentBase.Instance._culture;
			Clan.PlayerClan.UpdateHomeSettlement(null);
			Hero.MainHero.BornSettlement = Clan.PlayerClan.HomeSettlement;
			this.OnApplyCulture();
		}

		// Token: 0x06001C19 RID: 7193 RVA: 0x0007E2DA File Offset: 0x0007C4DA
		public IEnumerable<CultureObject> GetCultures()
		{
			foreach (CultureObject cultureObject in MBObjectManager.Instance.GetObjectTypeList<CultureObject>())
			{
				if (cultureObject.IsMainCulture)
				{
					yield return cultureObject;
				}
			}
			List<CultureObject>.Enumerator enumerator = default(List<CultureObject>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06001C1A RID: 7194 RVA: 0x0007E2E3 File Offset: 0x0007C4E3
		public CultureObject GetSelectedCulture()
		{
			return this._culture;
		}

		// Token: 0x06001C1B RID: 7195 RVA: 0x0007E2EB File Offset: 0x0007C4EB
		public virtual int GetSelectedParentType()
		{
			return 0;
		}

		// Token: 0x06001C1C RID: 7196 RVA: 0x0007E2EE File Offset: 0x0007C4EE
		protected virtual void OnApplyCulture()
		{
		}

		// Token: 0x06001C1D RID: 7197 RVA: 0x0007E2F0 File Offset: 0x0007C4F0
		protected virtual void OnCultureSelected()
		{
		}

		// Token: 0x06001C1E RID: 7198 RVA: 0x0007E2F2 File Offset: 0x0007C4F2
		public virtual void OnCharacterCreationFinalized()
		{
		}

		// Token: 0x06001C1F RID: 7199 RVA: 0x0007E2F4 File Offset: 0x0007C4F4
		private void SetMainHeroInitialStats()
		{
			Hero.MainHero.HeroDeveloper.ClearHero();
			Hero.MainHero.HitPoints = 100;
			foreach (SkillObject skillObject in Skills.All)
			{
				Hero.MainHero.HeroDeveloper.InitializeSkillXp(skillObject);
			}
			foreach (CharacterAttribute characterAttribute in Attributes.All)
			{
				Hero.MainHero.HeroDeveloper.AddAttribute(characterAttribute, 2, false);
			}
		}

		// Token: 0x040008E5 RID: 2277
		private CultureObject _culture;

		// Token: 0x040008E6 RID: 2278
		private Banner _banner;
	}
}
