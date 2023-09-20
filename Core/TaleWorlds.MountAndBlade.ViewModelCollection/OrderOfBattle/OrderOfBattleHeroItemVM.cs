using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	// Token: 0x0200002F RID: 47
	public class OrderOfBattleHeroItemVM : ViewModel
	{
		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000390 RID: 912 RVA: 0x0000FC84 File Offset: 0x0000DE84
		// (set) Token: 0x06000391 RID: 913 RVA: 0x0000FC8C File Offset: 0x0000DE8C
		public ItemObject BannerOfHero { get; private set; }

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000392 RID: 914 RVA: 0x0000FC95 File Offset: 0x0000DE95
		// (set) Token: 0x06000393 RID: 915 RVA: 0x0000FC9D File Offset: 0x0000DE9D
		public bool IsAssignedBeforePlayer { get; private set; }

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06000394 RID: 916 RVA: 0x0000FCA6 File Offset: 0x0000DEA6
		// (set) Token: 0x06000395 RID: 917 RVA: 0x0000FCAE File Offset: 0x0000DEAE
		public Formation InitialFormation { get; private set; }

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06000396 RID: 918 RVA: 0x0000FCB7 File Offset: 0x0000DEB7
		// (set) Token: 0x06000397 RID: 919 RVA: 0x0000FCBF File Offset: 0x0000DEBF
		public OrderOfBattleFormationItemVM InitialFormationItem { get; private set; }

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06000398 RID: 920 RVA: 0x0000FCC8 File Offset: 0x0000DEC8
		// (set) Token: 0x06000399 RID: 921 RVA: 0x0000FCD0 File Offset: 0x0000DED0
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

		// Token: 0x0600039A RID: 922 RVA: 0x0000FD39 File Offset: 0x0000DF39
		public OrderOfBattleHeroItemVM()
		{
			this.IsDisabled = true;
			this.RefreshValues();
		}

		// Token: 0x0600039B RID: 923 RVA: 0x0000FD70 File Offset: 0x0000DF70
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

		// Token: 0x0600039C RID: 924 RVA: 0x0000FE2B File Offset: 0x0000E02B
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

		// Token: 0x0600039D RID: 925 RVA: 0x0000FE61 File Offset: 0x0000E061
		public override void RefreshValues()
		{
			Func<Agent, List<TooltipProperty>> getAgentTooltip = OrderOfBattleHeroItemVM.GetAgentTooltip;
			this._cachedTooltipProperties = ((getAgentTooltip != null) ? getAgentTooltip(this.Agent) : null);
		}

		// Token: 0x0600039E RID: 926 RVA: 0x0000FE80 File Offset: 0x0000E080
		private List<TooltipProperty> GetCommanderTooltip()
		{
			return this._cachedTooltipProperties;
		}

		// Token: 0x0600039F RID: 927 RVA: 0x0000FE88 File Offset: 0x0000E088
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

		// Token: 0x060003A0 RID: 928 RVA: 0x0000FEEC File Offset: 0x0000E0EC
		public void RefreshInformation()
		{
			if (this.Agent != null)
			{
				this.ImageIdentifier = new ImageIdentifierVM(CharacterCode.CreateFrom(this.Agent.Character));
				return;
			}
			this.ImageIdentifier = new ImageIdentifierVM(CharacterCode.CreateEmpty());
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x0000FF22 File Offset: 0x0000E122
		private void OnAssignedFormationChanged()
		{
			Action<OrderOfBattleHeroItemVM> onHeroAssignedFormationChanged = OrderOfBattleHeroItemVM.OnHeroAssignedFormationChanged;
			if (onHeroAssignedFormationChanged != null)
			{
				onHeroAssignedFormationChanged(this);
			}
			this.RefreshAssignmentInfo();
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x0000FF3C File Offset: 0x0000E13C
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

		// Token: 0x060003A3 RID: 931 RVA: 0x0000FFBB File Offset: 0x0000E1BB
		public void SetIsPreAssigned(bool isPreAssigned)
		{
			this.IsAssignedBeforePlayer = isPreAssigned;
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x0000FFC4 File Offset: 0x0000E1C4
		private void ExecuteSelection()
		{
			Action<OrderOfBattleHeroItemVM> onHeroSelection = OrderOfBattleHeroItemVM.OnHeroSelection;
			if (onHeroSelection == null)
			{
				return;
			}
			onHeroSelection(this);
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x0000FFD6 File Offset: 0x0000E1D6
		private void ExecuteBeginAssignment()
		{
			Action<OrderOfBattleHeroItemVM> onHeroAssignmentBegin = OrderOfBattleHeroItemVM.OnHeroAssignmentBegin;
			if (onHeroAssignmentBegin == null)
			{
				return;
			}
			onHeroAssignmentBegin(this);
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x0000FFE8 File Offset: 0x0000E1E8
		private void ExecuteEndAssignment()
		{
			Action<OrderOfBattleHeroItemVM> onHeroAssignmentEnd = OrderOfBattleHeroItemVM.OnHeroAssignmentEnd;
			if (onHeroAssignmentEnd == null)
			{
				return;
			}
			onHeroAssignmentEnd(this);
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x060003A7 RID: 935 RVA: 0x0000FFFA File Offset: 0x0000E1FA
		// (set) Token: 0x060003A8 RID: 936 RVA: 0x00010002 File Offset: 0x0000E202
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

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x060003A9 RID: 937 RVA: 0x00010025 File Offset: 0x0000E225
		// (set) Token: 0x060003AA RID: 938 RVA: 0x0001002D File Offset: 0x0000E22D
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

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x060003AB RID: 939 RVA: 0x0001004B File Offset: 0x0000E24B
		// (set) Token: 0x060003AC RID: 940 RVA: 0x00010053 File Offset: 0x0000E253
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

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x060003AD RID: 941 RVA: 0x00010071 File Offset: 0x0000E271
		// (set) Token: 0x060003AE RID: 942 RVA: 0x00010079 File Offset: 0x0000E279
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

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x060003AF RID: 943 RVA: 0x00010097 File Offset: 0x0000E297
		// (set) Token: 0x060003B0 RID: 944 RVA: 0x0001009F File Offset: 0x0000E29F
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

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x060003B1 RID: 945 RVA: 0x000100BD File Offset: 0x0000E2BD
		// (set) Token: 0x060003B2 RID: 946 RVA: 0x000100C5 File Offset: 0x0000E2C5
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

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x060003B3 RID: 947 RVA: 0x000100E3 File Offset: 0x0000E2E3
		// (set) Token: 0x060003B4 RID: 948 RVA: 0x000100EB File Offset: 0x0000E2EB
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

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x060003B5 RID: 949 RVA: 0x00010109 File Offset: 0x0000E309
		// (set) Token: 0x060003B6 RID: 950 RVA: 0x00010111 File Offset: 0x0000E311
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

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x060003B7 RID: 951 RVA: 0x0001012F File Offset: 0x0000E32F
		// (set) Token: 0x060003B8 RID: 952 RVA: 0x00010137 File Offset: 0x0000E337
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

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060003B9 RID: 953 RVA: 0x00010155 File Offset: 0x0000E355
		// (set) Token: 0x060003BA RID: 954 RVA: 0x0001015D File Offset: 0x0000E35D
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

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x060003BB RID: 955 RVA: 0x0001017B File Offset: 0x0000E37B
		// (set) Token: 0x060003BC RID: 956 RVA: 0x00010183 File Offset: 0x0000E383
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

		// Token: 0x040001D7 RID: 471
		private readonly TextObject _mismatchMountedText = new TextObject("{=jR237DSU}Commander is mounted!", null);

		// Token: 0x040001D8 RID: 472
		private readonly TextObject _mismatchDismountedText = new TextObject("{=i0esOr2l}Commander is not mounted!", null);

		// Token: 0x040001D9 RID: 473
		public static Action<OrderOfBattleHeroItemVM> OnHeroSelection;

		// Token: 0x040001DA RID: 474
		public static Action<OrderOfBattleHeroItemVM> OnHeroAssignedFormationChanged;

		// Token: 0x040001DB RID: 475
		public static Func<Agent, List<TooltipProperty>> GetAgentTooltip;

		// Token: 0x040001DC RID: 476
		public static Action<OrderOfBattleHeroItemVM> OnHeroAssignmentBegin;

		// Token: 0x040001DD RID: 477
		public static Action<OrderOfBattleHeroItemVM> OnHeroAssignmentEnd;

		// Token: 0x040001DE RID: 478
		public readonly Agent Agent;

		// Token: 0x040001E3 RID: 483
		private List<TooltipProperty> _cachedTooltipProperties;

		// Token: 0x040001E4 RID: 484
		private OrderOfBattleFormationItemVM _currentAssignedFormationItem;

		// Token: 0x040001E5 RID: 485
		private string _mismatchedAssignmentDescriptionText;

		// Token: 0x040001E6 RID: 486
		private bool _isAssignedToAFormation;

		// Token: 0x040001E7 RID: 487
		private bool _isLeadingAFormation;

		// Token: 0x040001E8 RID: 488
		private bool _hasMismatchedAssignment;

		// Token: 0x040001E9 RID: 489
		private bool _isSelected;

		// Token: 0x040001EA RID: 490
		private bool _isDisabled;

		// Token: 0x040001EB RID: 491
		private bool _isShown;

		// Token: 0x040001EC RID: 492
		private bool _isMainHero;

		// Token: 0x040001ED RID: 493
		private ImageIdentifierVM _imageIdentifier;

		// Token: 0x040001EE RID: 494
		private BasicTooltipViewModel _tooltip;

		// Token: 0x040001EF RID: 495
		private bool _isHighlightActive;
	}
}
