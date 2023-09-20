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
	public abstract class CharacterCreationContentBase
	{
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

		protected virtual int ChildhoodAge
		{
			get
			{
				return 7;
			}
		}

		protected virtual int EducationAge
		{
			get
			{
				return 12;
			}
		}

		protected virtual int YouthAge
		{
			get
			{
				return 17;
			}
		}

		protected virtual int AccomplishmentAge
		{
			get
			{
				return 20;
			}
		}

		protected virtual int FocusToAdd
		{
			get
			{
				return 1;
			}
		}

		protected virtual int SkillLevelToAdd
		{
			get
			{
				return 10;
			}
		}

		protected virtual int AttributeLevelToAdd
		{
			get
			{
				return 1;
			}
		}

		public virtual int FocusToAddByCulture
		{
			get
			{
				return 1;
			}
		}

		public virtual int SkillLevelToAddByCulture
		{
			get
			{
				return 10;
			}
		}

		protected int SelectedParentType { get; set; } = 1;

		protected int SelectedTitleType { get; set; }

		public abstract TextObject ReviewPageDescription { get; }

		protected FaceGenChar MotherFacegenCharacter { get; set; }

		protected FaceGenChar FatherFacegenCharacter { get; set; }

		protected BodyProperties PlayerBodyProperties { get; set; }

		protected Equipment PlayerStartEquipment { get; set; }

		protected Equipment PlayerCivilianEquipment { get; set; }

		public abstract IEnumerable<Type> CharacterCreationStages { get; }

		public void Initialize(CharacterCreation characterCreation)
		{
			this.OnInitialized(characterCreation);
			this.SetMainHeroInitialStats();
		}

		protected virtual void OnInitialized(CharacterCreation characterCreation)
		{
		}

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

		public void SetPlayerBanner(Banner banner)
		{
			this._banner = banner;
		}

		public Banner GetCurrentPlayerBanner()
		{
			return this._banner;
		}

		public void SetSelectedCulture(CultureObject culture, CharacterCreation characterCreation)
		{
			this._culture = culture;
			characterCreation.ResetMenuOptions();
			this.OnCultureSelected();
		}

		public void ApplyCulture(CharacterCreation characterCreation)
		{
			CharacterObject.PlayerCharacter.Culture = CharacterCreationContentBase.Instance._culture;
			Clan.PlayerClan.Culture = CharacterCreationContentBase.Instance._culture;
			Clan.PlayerClan.UpdateHomeSettlement(null);
			Hero.MainHero.BornSettlement = Clan.PlayerClan.HomeSettlement;
			this.OnApplyCulture();
		}

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

		public CultureObject GetSelectedCulture()
		{
			return this._culture;
		}

		public virtual int GetSelectedParentType()
		{
			return 0;
		}

		protected virtual void OnApplyCulture()
		{
		}

		protected virtual void OnCultureSelected()
		{
		}

		public virtual void OnCharacterCreationFinalized()
		{
		}

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

		private CultureObject _culture;

		private Banner _banner;
	}
}
