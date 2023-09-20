using System;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper
{
	public class PerkVM : ViewModel
	{
		private bool _hasAlternativeAndSelected
		{
			get
			{
				return this.AlternativeType != 0 && this._getIsPerkSelected(this.Perk.AlternativePerk);
			}
		}

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

		public void ExecuteShowPerkConcept()
		{
			if (this._perkConceptObj != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this._perkConceptObj.EncyclopediaLink);
				return;
			}
			Debug.FailedAssert("Couldn't find Perks encyclopedia page", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\CharacterDeveloper\\PerkVM.cs", "ExecuteShowPerkConcept", 151);
		}

		public void ExecuteStartSelection()
		{
			if (this._isAvailable && !this._getIsPerkSelected(this.Perk) && !this._hasAlternativeAndSelected && this._getIsPreviousPerkSelected(this.Perk))
			{
				this._onStartSelection(this);
			}
		}

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

		public readonly PerkObject Perk;

		private readonly Action<PerkVM> _onStartSelection;

		private readonly Action<PerkVM> _onSelectionOver;

		private readonly Func<PerkObject, bool> _getIsPerkSelected;

		private readonly Func<PerkObject, bool> _getIsPreviousPerkSelected;

		private readonly bool _isAvailable;

		private readonly Concept _perkConceptObj;

		private bool _isInSelection;

		private PerkVM.PerkStates _currentState = PerkVM.PerkStates.None;

		private string _levelText;

		private string _perkId;

		private string _backgroundImage;

		private BasicTooltipViewModel _hint;

		private int _level;

		private int _alternativeType;

		private int _perkState = -1;

		private bool _isTutorialHighlightEnabled;

		public enum PerkStates
		{
			None = -1,
			NotEarned,
			EarnedButNotSelected,
			InSelection,
			EarnedAndActive,
			EarnedAndNotActive,
			EarnedPreviousPerkNotSelected
		}

		public enum PerkAlternativeType
		{
			NoAlternative,
			FirstAlternative,
			SecondAlternative
		}
	}
}
