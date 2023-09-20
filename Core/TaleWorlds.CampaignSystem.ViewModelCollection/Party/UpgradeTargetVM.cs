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
	// Token: 0x0200002C RID: 44
	public class UpgradeTargetVM : ViewModel
	{
		// Token: 0x06000450 RID: 1104 RVA: 0x00017F04 File Offset: 0x00016104
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

		// Token: 0x06000451 RID: 1105 RVA: 0x00017FA9 File Offset: 0x000161A9
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

		// Token: 0x06000452 RID: 1106 RVA: 0x00017FC4 File Offset: 0x000161C4
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

		// Token: 0x06000453 RID: 1107 RVA: 0x00018018 File Offset: 0x00016218
		public void ExecuteUpgradeEncyclopediaLink()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this._upgradeTarget.EncyclopediaLink);
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x00018034 File Offset: 0x00016234
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

		// Token: 0x06000455 RID: 1109 RVA: 0x00018062 File Offset: 0x00016262
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

		// Token: 0x06000456 RID: 1110 RVA: 0x0001807D File Offset: 0x0001627D
		public void ExecuteSetUnfocused()
		{
			Action<UpgradeTargetVM> onFocused = this._onFocused;
			if (onFocused == null)
			{
				return;
			}
			onFocused(null);
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000457 RID: 1111 RVA: 0x00018090 File Offset: 0x00016290
		// (set) Token: 0x06000458 RID: 1112 RVA: 0x00018098 File Offset: 0x00016298
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

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000459 RID: 1113 RVA: 0x000180B6 File Offset: 0x000162B6
		// (set) Token: 0x0600045A RID: 1114 RVA: 0x000180BE File Offset: 0x000162BE
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

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x0600045B RID: 1115 RVA: 0x000180DC File Offset: 0x000162DC
		// (set) Token: 0x0600045C RID: 1116 RVA: 0x000180E4 File Offset: 0x000162E4
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

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x0600045D RID: 1117 RVA: 0x00018102 File Offset: 0x00016302
		// (set) Token: 0x0600045E RID: 1118 RVA: 0x0001810A File Offset: 0x0001630A
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

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x0600045F RID: 1119 RVA: 0x00018128 File Offset: 0x00016328
		// (set) Token: 0x06000460 RID: 1120 RVA: 0x00018130 File Offset: 0x00016330
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

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000461 RID: 1121 RVA: 0x0001814E File Offset: 0x0001634E
		// (set) Token: 0x06000462 RID: 1122 RVA: 0x00018156 File Offset: 0x00016356
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

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000463 RID: 1123 RVA: 0x00018174 File Offset: 0x00016374
		// (set) Token: 0x06000464 RID: 1124 RVA: 0x0001817C File Offset: 0x0001637C
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

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000465 RID: 1125 RVA: 0x0001819A File Offset: 0x0001639A
		// (set) Token: 0x06000466 RID: 1126 RVA: 0x000181A2 File Offset: 0x000163A2
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

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000467 RID: 1127 RVA: 0x000181C0 File Offset: 0x000163C0
		// (set) Token: 0x06000468 RID: 1128 RVA: 0x000181C8 File Offset: 0x000163C8
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

		// Token: 0x040001D8 RID: 472
		private CharacterObject _originalCharacter;

		// Token: 0x040001D9 RID: 473
		private CharacterObject _upgradeTarget;

		// Token: 0x040001DA RID: 474
		private Action<int, int> _onUpgraded;

		// Token: 0x040001DB RID: 475
		private Action<UpgradeTargetVM> _onFocused;

		// Token: 0x040001DC RID: 476
		private int _upgradeIndex;

		// Token: 0x040001DD RID: 477
		private InputKeyItemVM _primaryActionInputKey;

		// Token: 0x040001DE RID: 478
		private InputKeyItemVM _secondaryActionInputKey;

		// Token: 0x040001DF RID: 479
		private UpgradeRequirementsVM _requirements;

		// Token: 0x040001E0 RID: 480
		private ImageIdentifierVM _troopImage;

		// Token: 0x040001E1 RID: 481
		private HintViewModel _hint;

		// Token: 0x040001E2 RID: 482
		private int _availableUpgrades;

		// Token: 0x040001E3 RID: 483
		private bool _isAvailable;

		// Token: 0x040001E4 RID: 484
		private bool _isInsufficient;

		// Token: 0x040001E5 RID: 485
		private bool _isHighlighted;
	}
}
