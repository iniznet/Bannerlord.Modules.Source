using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	public class UpgradeTargetVM : ViewModel
	{
		public UpgradeTargetVM(int upgradeIndex, CharacterObject character, CharacterCode upgradeCharacterCode, Action<int, int> onUpgraded, Action<UpgradeTargetVM> onFocused)
		{
			this._upgradeIndex = upgradeIndex;
			this._originalCharacter = character;
			this._upgradeTarget = this._originalCharacter.UpgradeTargets[upgradeIndex];
			this._onUpgraded = onUpgraded;
			this._onFocused = onFocused;
			PerkObject perkObject;
			Campaign.Current.Models.PartyTroopUpgradeModel.DoesPartyHaveRequiredPerksForUpgrade(PartyBase.MainParty, this._originalCharacter, this._upgradeTarget, out perkObject);
			this.Requirements = new UpgradeRequirementsVM();
			this.Requirements.SetItemRequirement(this._upgradeTarget.UpgradeRequiresItemFromCategory);
			this.Requirements.SetPerkRequirement(perkObject);
			this.TroopImage = new ImageIdentifierVM(upgradeCharacterCode);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			UpgradeRequirementsVM requirements = this.Requirements;
			if (requirements == null)
			{
				return;
			}
			requirements.RefreshValues();
		}

		public void Refresh(int upgradableAmount, string hint, bool isAvailable, bool isInsufficient, bool itemRequirementsMet, bool perkRequirementsMet)
		{
			this.AvailableUpgrades = upgradableAmount;
			this.Hint = new HintViewModel(new TextObject("{=!}" + hint, null), null);
			this.IsAvailable = isAvailable;
			this.IsInsufficient = isInsufficient;
			UpgradeRequirementsVM requirements = this.Requirements;
			if (requirements == null)
			{
				return;
			}
			requirements.SetRequirementsMet(itemRequirementsMet, perkRequirementsMet);
		}

		public void ExecuteUpgradeEncyclopediaLink()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this._upgradeTarget.EncyclopediaLink);
		}

		public void ExecuteUpgrade()
		{
			if (this.IsAvailable && !this.IsInsufficient)
			{
				Action<int, int> onUpgraded = this._onUpgraded;
				if (onUpgraded == null)
				{
					return;
				}
				onUpgraded(this._upgradeIndex, this.AvailableUpgrades);
			}
		}

		public void ExecuteSetFocused()
		{
			if (this._upgradeTarget != null)
			{
				Action<UpgradeTargetVM> onFocused = this._onFocused;
				if (onFocused == null)
				{
					return;
				}
				onFocused(this);
			}
		}

		public void ExecuteSetUnfocused()
		{
			Action<UpgradeTargetVM> onFocused = this._onFocused;
			if (onFocused == null)
			{
				return;
			}
			onFocused(null);
		}

		[DataSourceProperty]
		public InputKeyItemVM PrimaryActionInputKey
		{
			get
			{
				return this._primaryActionInputKey;
			}
			set
			{
				if (value != this._primaryActionInputKey)
				{
					this._primaryActionInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "PrimaryActionInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM SecondaryActionInputKey
		{
			get
			{
				return this._secondaryActionInputKey;
			}
			set
			{
				if (value != this._secondaryActionInputKey)
				{
					this._secondaryActionInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "SecondaryActionInputKey");
				}
			}
		}

		[DataSourceProperty]
		public UpgradeRequirementsVM Requirements
		{
			get
			{
				return this._requirements;
			}
			set
			{
				if (value != this._requirements)
				{
					this._requirements = value;
					base.OnPropertyChangedWithValue<UpgradeRequirementsVM>(value, "Requirements");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM TroopImage
		{
			get
			{
				return this._troopImage;
			}
			set
			{
				if (value != this._troopImage)
				{
					this._troopImage = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "TroopImage");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		[DataSourceProperty]
		public int AvailableUpgrades
		{
			get
			{
				return this._availableUpgrades;
			}
			set
			{
				if (value != this._availableUpgrades)
				{
					this._availableUpgrades = value;
					base.OnPropertyChangedWithValue(value, "AvailableUpgrades");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAvailable
		{
			get
			{
				return this._isAvailable;
			}
			set
			{
				if (value != this._isAvailable)
				{
					this._isAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsAvailable");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInsufficient
		{
			get
			{
				return this._isInsufficient;
			}
			set
			{
				if (value != this._isInsufficient)
				{
					this._isInsufficient = value;
					base.OnPropertyChangedWithValue(value, "IsInsufficient");
				}
			}
		}

		[DataSourceProperty]
		public bool IsHighlighted
		{
			get
			{
				return this._isHighlighted;
			}
			set
			{
				if (value != this._isHighlighted)
				{
					this._isHighlighted = value;
					base.OnPropertyChangedWithValue(value, "IsHighlighted");
				}
			}
		}

		private CharacterObject _originalCharacter;

		private CharacterObject _upgradeTarget;

		private Action<int, int> _onUpgraded;

		private Action<UpgradeTargetVM> _onFocused;

		private int _upgradeIndex;

		private InputKeyItemVM _primaryActionInputKey;

		private InputKeyItemVM _secondaryActionInputKey;

		private UpgradeRequirementsVM _requirements;

		private ImageIdentifierVM _troopImage;

		private HintViewModel _hint;

		private int _availableUpgrades;

		private bool _isAvailable;

		private bool _isInsufficient;

		private bool _isHighlighted;
	}
}
