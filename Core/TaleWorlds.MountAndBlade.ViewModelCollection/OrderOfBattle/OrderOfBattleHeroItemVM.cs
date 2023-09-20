using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	public class OrderOfBattleHeroItemVM : ViewModel
	{
		public ItemObject BannerOfHero { get; private set; }

		public bool IsAssignedBeforePlayer { get; private set; }

		public Formation InitialFormation { get; private set; }

		public OrderOfBattleFormationItemVM InitialFormationItem { get; private set; }

		public OrderOfBattleFormationItemVM CurrentAssignedFormationItem
		{
			get
			{
				return this._currentAssignedFormationItem;
			}
			set
			{
				if (value != this._currentAssignedFormationItem)
				{
					this._currentAssignedFormationItem = value;
					if (this._currentAssignedFormationItem == null)
					{
						this.OnAssignmentRemoved();
					}
					this.IsAssignedToAFormation = this._currentAssignedFormationItem != null;
					this.IsLeadingAFormation = this._currentAssignedFormationItem != null && this._currentAssignedFormationItem.Formation.Captain == this.Agent;
					this.OnAssignedFormationChanged();
				}
			}
		}

		public OrderOfBattleHeroItemVM()
		{
			this.IsDisabled = true;
			this.RefreshValues();
		}

		public OrderOfBattleHeroItemVM(Agent agent)
		{
			this.Agent = agent;
			this.BannerOfHero = agent.FormationBanner;
			this.IsDisabled = !Mission.Current.PlayerTeam.IsPlayerGeneral && !agent.IsMainAgent;
			this.IsShown = true;
			this.IsMainHero = this.Agent.IsMainAgent;
			this.ImageIdentifier = new ImageIdentifierVM(CharacterCode.CreateFrom(this.Agent.Character));
			this.Tooltip = new BasicTooltipViewModel(() => this.GetCommanderTooltip());
			this.RefreshValues();
		}

		public void SetInitialFormation(OrderOfBattleFormationItemVM formation)
		{
			if (this.InitialFormationItem != null)
			{
				Debug.FailedAssert("Initial formation for hero is already set", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\OrderOfBattle\\OrderOfBattleHeroItemVM.cs", "SetInitialFormation", 76);
			}
			if (formation != null)
			{
				this.InitialFormationItem = formation;
				this.InitialFormation = formation.Formation;
			}
		}

		public override void RefreshValues()
		{
			Func<Agent, List<TooltipProperty>> getAgentTooltip = OrderOfBattleHeroItemVM.GetAgentTooltip;
			this._cachedTooltipProperties = ((getAgentTooltip != null) ? getAgentTooltip(this.Agent) : null);
		}

		private List<TooltipProperty> GetCommanderTooltip()
		{
			return this._cachedTooltipProperties;
		}

		public void OnAssignmentRemoved()
		{
			if (this.CurrentAssignedFormationItem != null)
			{
				this.CurrentAssignedFormationItem.Formation.Refresh();
			}
			if (this.InitialFormation != null)
			{
				this.Agent.Formation = this.InitialFormation;
				this.InitialFormation.Refresh();
				this.Agent.Team.DetachmentManager.RemoveScoresOfAgentFromDetachments(this.Agent);
			}
		}

		public void RefreshInformation()
		{
			if (this.Agent != null)
			{
				this.ImageIdentifier = new ImageIdentifierVM(CharacterCode.CreateFrom(this.Agent.Character));
				return;
			}
			this.ImageIdentifier = new ImageIdentifierVM(CharacterCode.CreateEmpty());
		}

		private void OnAssignedFormationChanged()
		{
			Action<OrderOfBattleHeroItemVM> onHeroAssignedFormationChanged = OrderOfBattleHeroItemVM.OnHeroAssignedFormationChanged;
			if (onHeroAssignedFormationChanged != null)
			{
				onHeroAssignedFormationChanged(this);
			}
			this.RefreshAssignmentInfo();
		}

		public void RefreshAssignmentInfo()
		{
			if (!this.IsLeadingAFormation)
			{
				this.HasMismatchedAssignment = false;
				return;
			}
			DeploymentFormationClass orderOfBattleClass = this.CurrentAssignedFormationItem.GetOrderOfBattleClass();
			if (this.Agent.HasMount)
			{
				if (orderOfBattleClass == DeploymentFormationClass.Infantry || orderOfBattleClass == DeploymentFormationClass.Ranged || orderOfBattleClass == DeploymentFormationClass.InfantryAndRanged)
				{
					this.HasMismatchedAssignment = true;
					this.MismatchedAssignmentDescriptionText = this._mismatchMountedText.ToString();
					return;
				}
			}
			else if (orderOfBattleClass == DeploymentFormationClass.Cavalry || orderOfBattleClass == DeploymentFormationClass.HorseArcher || orderOfBattleClass == DeploymentFormationClass.CavalryAndHorseArcher)
			{
				this.HasMismatchedAssignment = true;
				this.MismatchedAssignmentDescriptionText = this._mismatchDismountedText.ToString();
			}
		}

		public void SetIsPreAssigned(bool isPreAssigned)
		{
			this.IsAssignedBeforePlayer = isPreAssigned;
		}

		private void ExecuteSelection()
		{
			Action<OrderOfBattleHeroItemVM> onHeroSelection = OrderOfBattleHeroItemVM.OnHeroSelection;
			if (onHeroSelection == null)
			{
				return;
			}
			onHeroSelection(this);
		}

		private void ExecuteBeginAssignment()
		{
			Action<OrderOfBattleHeroItemVM> onHeroAssignmentBegin = OrderOfBattleHeroItemVM.OnHeroAssignmentBegin;
			if (onHeroAssignmentBegin == null)
			{
				return;
			}
			onHeroAssignmentBegin(this);
		}

		private void ExecuteEndAssignment()
		{
			Action<OrderOfBattleHeroItemVM> onHeroAssignmentEnd = OrderOfBattleHeroItemVM.OnHeroAssignmentEnd;
			if (onHeroAssignmentEnd == null)
			{
				return;
			}
			onHeroAssignmentEnd(this);
		}

		[DataSourceProperty]
		public string MismatchedAssignmentDescriptionText
		{
			get
			{
				return this._mismatchedAssignmentDescriptionText;
			}
			set
			{
				if (value != this._mismatchedAssignmentDescriptionText)
				{
					this._mismatchedAssignmentDescriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "MismatchedAssignmentDescriptionText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAssignedToAFormation
		{
			get
			{
				return this._isAssignedToAFormation;
			}
			set
			{
				if (value != this._isAssignedToAFormation)
				{
					this._isAssignedToAFormation = value;
					base.OnPropertyChangedWithValue(value, "IsAssignedToAFormation");
				}
			}
		}

		[DataSourceProperty]
		public bool IsLeadingAFormation
		{
			get
			{
				return this._isLeadingAFormation;
			}
			set
			{
				if (value != this._isLeadingAFormation)
				{
					this._isLeadingAFormation = value;
					base.OnPropertyChangedWithValue(value, "IsLeadingAFormation");
				}
			}
		}

		[DataSourceProperty]
		public bool HasMismatchedAssignment
		{
			get
			{
				return this._hasMismatchedAssignment;
			}
			set
			{
				if (value != this._hasMismatchedAssignment)
				{
					this._hasMismatchedAssignment = value;
					base.OnPropertyChangedWithValue(value, "HasMismatchedAssignment");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				if (value != this._isDisabled)
				{
					this._isDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsDisabled");
				}
			}
		}

		public bool IsShown
		{
			get
			{
				return this._isShown;
			}
			set
			{
				if (value != this._isShown)
				{
					this._isShown = value;
					base.OnPropertyChangedWithValue(value, "IsShown");
				}
			}
		}

		public bool IsMainHero
		{
			get
			{
				return this._isMainHero;
			}
			set
			{
				if (value != this._isMainHero)
				{
					this._isMainHero = value;
					base.OnPropertyChangedWithValue(value, "IsMainHero");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM ImageIdentifier
		{
			get
			{
				return this._imageIdentifier;
			}
			set
			{
				if (value != this._imageIdentifier)
				{
					this._imageIdentifier = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ImageIdentifier");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel Tooltip
		{
			get
			{
				return this._tooltip;
			}
			set
			{
				if (value != this._tooltip)
				{
					this._tooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Tooltip");
				}
			}
		}

		[DataSourceProperty]
		public bool IsHighlightActive
		{
			get
			{
				return this._isHighlightActive;
			}
			set
			{
				if (value != this._isHighlightActive)
				{
					this._isHighlightActive = value;
					base.OnPropertyChangedWithValue(value, "IsHighlightActive");
				}
			}
		}

		private readonly TextObject _mismatchMountedText = new TextObject("{=jR237DSU}Commander is mounted!", null);

		private readonly TextObject _mismatchDismountedText = new TextObject("{=i0esOr2l}Commander is not mounted!", null);

		public static Action<OrderOfBattleHeroItemVM> OnHeroSelection;

		public static Action<OrderOfBattleHeroItemVM> OnHeroAssignedFormationChanged;

		public static Func<Agent, List<TooltipProperty>> GetAgentTooltip;

		public static Action<OrderOfBattleHeroItemVM> OnHeroAssignmentBegin;

		public static Action<OrderOfBattleHeroItemVM> OnHeroAssignmentEnd;

		public readonly Agent Agent;

		private List<TooltipProperty> _cachedTooltipProperties;

		private OrderOfBattleFormationItemVM _currentAssignedFormationItem;

		private string _mismatchedAssignmentDescriptionText;

		private bool _isAssignedToAFormation;

		private bool _isLeadingAFormation;

		private bool _hasMismatchedAssignment;

		private bool _isSelected;

		private bool _isDisabled;

		private bool _isShown;

		private bool _isMainHero;

		private ImageIdentifierVM _imageIdentifier;

		private BasicTooltipViewModel _tooltip;

		private bool _isHighlightActive;
	}
}
