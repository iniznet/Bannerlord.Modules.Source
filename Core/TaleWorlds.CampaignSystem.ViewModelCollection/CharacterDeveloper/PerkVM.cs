using System;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper
{
	// Token: 0x0200011A RID: 282
	public class PerkVM : ViewModel
	{
		// Token: 0x1700095F RID: 2399
		// (get) Token: 0x06001B63 RID: 7011 RVA: 0x0006301E File Offset: 0x0006121E
		private bool _hasAlternativeAndSelected
		{
			get
			{
				return this.AlternativeType != 0 && this._getIsPerkSelected(this.Perk.AlternativePerk);
			}
		}

		// Token: 0x17000960 RID: 2400
		// (get) Token: 0x06001B64 RID: 7012 RVA: 0x00063040 File Offset: 0x00061240
		// (set) Token: 0x06001B65 RID: 7013 RVA: 0x00063048 File Offset: 0x00061248
		public PerkVM.PerkStates CurrentState
		{
			get
			{
				return this._currentState;
			}
			private set
			{
				if (value != this._currentState)
				{
					this._currentState = value;
					this.PerkState = (int)value;
				}
			}
		}

		// Token: 0x17000961 RID: 2401
		// (set) Token: 0x06001B66 RID: 7014 RVA: 0x00063061 File Offset: 0x00061261
		public bool IsInSelection
		{
			set
			{
				if (value != this._isInSelection)
				{
					this._isInSelection = value;
					this.RefreshState();
					if (!this._isInSelection)
					{
						this._onSelectionOver(this);
					}
				}
			}
		}

		// Token: 0x06001B67 RID: 7015 RVA: 0x00063090 File Offset: 0x00061290
		public PerkVM(PerkObject perk, bool isAvailable, PerkVM.PerkAlternativeType alternativeType, Action<PerkVM> onStartSelection, Action<PerkVM> onSelectionOver, Func<PerkObject, bool> getIsPerkSelected, Func<PerkObject, bool> getIsPreviousPerkSelected)
		{
			PerkVM <>4__this = this;
			this.AlternativeType = (int)alternativeType;
			this.Perk = perk;
			this._onStartSelection = onStartSelection;
			this._onSelectionOver = onSelectionOver;
			this._getIsPerkSelected = getIsPerkSelected;
			this._getIsPreviousPerkSelected = getIsPreviousPerkSelected;
			this._isAvailable = isAvailable;
			this.PerkId = "SPPerks\\" + perk.StringId;
			this.Level = (int)perk.RequiredSkillValue;
			this.LevelText = ((int)perk.RequiredSkillValue).ToString();
			this.Hint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPerkEffectText(perk, <>4__this._getIsPerkSelected(<>4__this.Perk)));
			this._perkConceptObj = Concept.All.SingleOrDefault((Concept c) => c.StringId == "str_game_objects_perks");
			this.RefreshState();
		}

		// Token: 0x06001B68 RID: 7016 RVA: 0x00063194 File Offset: 0x00061394
		public void RefreshState()
		{
			bool flag = this._getIsPerkSelected(this.Perk);
			if (!this._isAvailable)
			{
				this.CurrentState = PerkVM.PerkStates.NotEarned;
				return;
			}
			if (this._isInSelection)
			{
				this.CurrentState = PerkVM.PerkStates.InSelection;
				return;
			}
			if (flag)
			{
				this.CurrentState = PerkVM.PerkStates.EarnedAndActive;
				return;
			}
			if (this.Perk.AlternativePerk != null && this._getIsPerkSelected(this.Perk.AlternativePerk))
			{
				this.CurrentState = PerkVM.PerkStates.EarnedAndNotActive;
				return;
			}
			if (this._getIsPreviousPerkSelected(this.Perk))
			{
				this.CurrentState = PerkVM.PerkStates.EarnedButNotSelected;
				return;
			}
			this.CurrentState = PerkVM.PerkStates.EarnedPreviousPerkNotSelected;
		}

		// Token: 0x06001B69 RID: 7017 RVA: 0x0006322D File Offset: 0x0006142D
		public void ExecuteShowPerkConcept()
		{
			if (this._perkConceptObj != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this._perkConceptObj.EncyclopediaLink);
				return;
			}
			Debug.FailedAssert("Couldn't find Perks encyclopedia page", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\CharacterDeveloper\\PerkVM.cs", "ExecuteShowPerkConcept", 151);
		}

		// Token: 0x06001B6A RID: 7018 RVA: 0x0006326C File Offset: 0x0006146C
		public void ExecuteStartSelection()
		{
			if (this._isAvailable && !this._getIsPerkSelected(this.Perk) && !this._hasAlternativeAndSelected && this._getIsPreviousPerkSelected(this.Perk))
			{
				this._onStartSelection(this);
			}
		}

		// Token: 0x17000962 RID: 2402
		// (get) Token: 0x06001B6B RID: 7019 RVA: 0x000632BB File Offset: 0x000614BB
		// (set) Token: 0x06001B6C RID: 7020 RVA: 0x000632C3 File Offset: 0x000614C3
		[DataSourceProperty]
		public bool IsTutorialHighlightEnabled
		{
			get
			{
				return this._isTutorialHighlightEnabled;
			}
			set
			{
				if (value != this._isTutorialHighlightEnabled)
				{
					this._isTutorialHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsTutorialHighlightEnabled");
				}
			}
		}

		// Token: 0x17000963 RID: 2403
		// (get) Token: 0x06001B6D RID: 7021 RVA: 0x000632E1 File Offset: 0x000614E1
		// (set) Token: 0x06001B6E RID: 7022 RVA: 0x000632E9 File Offset: 0x000614E9
		[DataSourceProperty]
		public BasicTooltipViewModel Hint
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
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Hint");
				}
			}
		}

		// Token: 0x17000964 RID: 2404
		// (get) Token: 0x06001B6F RID: 7023 RVA: 0x00063307 File Offset: 0x00061507
		// (set) Token: 0x06001B70 RID: 7024 RVA: 0x0006330F File Offset: 0x0006150F
		[DataSourceProperty]
		public int Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if (value != this._level)
				{
					this._level = value;
					base.OnPropertyChangedWithValue(value, "Level");
				}
			}
		}

		// Token: 0x17000965 RID: 2405
		// (get) Token: 0x06001B71 RID: 7025 RVA: 0x0006332D File Offset: 0x0006152D
		// (set) Token: 0x06001B72 RID: 7026 RVA: 0x00063335 File Offset: 0x00061535
		[DataSourceProperty]
		public int PerkState
		{
			get
			{
				return this._perkState;
			}
			set
			{
				if (value != this._perkState)
				{
					this._perkState = value;
					base.OnPropertyChangedWithValue(value, "PerkState");
				}
			}
		}

		// Token: 0x17000966 RID: 2406
		// (get) Token: 0x06001B73 RID: 7027 RVA: 0x00063353 File Offset: 0x00061553
		// (set) Token: 0x06001B74 RID: 7028 RVA: 0x0006335B File Offset: 0x0006155B
		[DataSourceProperty]
		public int AlternativeType
		{
			get
			{
				return this._alternativeType;
			}
			set
			{
				if (value != this._alternativeType)
				{
					this._alternativeType = value;
					base.OnPropertyChangedWithValue(value, "AlternativeType");
				}
			}
		}

		// Token: 0x17000967 RID: 2407
		// (get) Token: 0x06001B75 RID: 7029 RVA: 0x00063379 File Offset: 0x00061579
		// (set) Token: 0x06001B76 RID: 7030 RVA: 0x00063381 File Offset: 0x00061581
		[DataSourceProperty]
		public string LevelText
		{
			get
			{
				return this._levelText;
			}
			set
			{
				if (value != this._levelText)
				{
					this._levelText = value;
					base.OnPropertyChangedWithValue<string>(value, "LevelText");
				}
			}
		}

		// Token: 0x17000968 RID: 2408
		// (get) Token: 0x06001B77 RID: 7031 RVA: 0x000633A4 File Offset: 0x000615A4
		// (set) Token: 0x06001B78 RID: 7032 RVA: 0x000633AC File Offset: 0x000615AC
		[DataSourceProperty]
		public string BackgroundImage
		{
			get
			{
				return this._backgroundImage;
			}
			set
			{
				if (value != this._backgroundImage)
				{
					this._backgroundImage = value;
					base.OnPropertyChangedWithValue<string>(value, "BackgroundImage");
				}
			}
		}

		// Token: 0x17000969 RID: 2409
		// (get) Token: 0x06001B79 RID: 7033 RVA: 0x000633CF File Offset: 0x000615CF
		// (set) Token: 0x06001B7A RID: 7034 RVA: 0x000633D7 File Offset: 0x000615D7
		[DataSourceProperty]
		public string PerkId
		{
			get
			{
				return this._perkId;
			}
			set
			{
				if (value != this._perkId)
				{
					this._perkId = value;
					base.OnPropertyChangedWithValue<string>(value, "PerkId");
				}
			}
		}

		// Token: 0x04000CED RID: 3309
		public readonly PerkObject Perk;

		// Token: 0x04000CEE RID: 3310
		private readonly Action<PerkVM> _onStartSelection;

		// Token: 0x04000CEF RID: 3311
		private readonly Action<PerkVM> _onSelectionOver;

		// Token: 0x04000CF0 RID: 3312
		private readonly Func<PerkObject, bool> _getIsPerkSelected;

		// Token: 0x04000CF1 RID: 3313
		private readonly Func<PerkObject, bool> _getIsPreviousPerkSelected;

		// Token: 0x04000CF2 RID: 3314
		private readonly bool _isAvailable;

		// Token: 0x04000CF3 RID: 3315
		private readonly Concept _perkConceptObj;

		// Token: 0x04000CF4 RID: 3316
		private bool _isInSelection;

		// Token: 0x04000CF5 RID: 3317
		private PerkVM.PerkStates _currentState = PerkVM.PerkStates.None;

		// Token: 0x04000CF6 RID: 3318
		private string _levelText;

		// Token: 0x04000CF7 RID: 3319
		private string _perkId;

		// Token: 0x04000CF8 RID: 3320
		private string _backgroundImage;

		// Token: 0x04000CF9 RID: 3321
		private BasicTooltipViewModel _hint;

		// Token: 0x04000CFA RID: 3322
		private int _level;

		// Token: 0x04000CFB RID: 3323
		private int _alternativeType;

		// Token: 0x04000CFC RID: 3324
		private int _perkState = -1;

		// Token: 0x04000CFD RID: 3325
		private bool _isTutorialHighlightEnabled;

		// Token: 0x02000263 RID: 611
		public enum PerkStates
		{
			// Token: 0x0400116E RID: 4462
			None = -1,
			// Token: 0x0400116F RID: 4463
			NotEarned,
			// Token: 0x04001170 RID: 4464
			EarnedButNotSelected,
			// Token: 0x04001171 RID: 4465
			InSelection,
			// Token: 0x04001172 RID: 4466
			EarnedAndActive,
			// Token: 0x04001173 RID: 4467
			EarnedAndNotActive,
			// Token: 0x04001174 RID: 4468
			EarnedPreviousPerkNotSelected
		}

		// Token: 0x02000264 RID: 612
		public enum PerkAlternativeType
		{
			// Token: 0x04001176 RID: 4470
			NoAlternative,
			// Token: 0x04001177 RID: 4471
			FirstAlternative,
			// Token: 0x04001178 RID: 4472
			SecondAlternative
		}
	}
}
