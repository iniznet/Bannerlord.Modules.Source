using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.Overlay
{
	public class PowerLevelComparerWidget : Widget
	{
		public PowerLevelComparerWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			if (this.AttackerPowerWidget != null)
			{
				this.AttackerPowerWidget.AlphaFactor = 0.7f;
				this.AttackerPowerWidget.ValueFactor = -70f;
			}
			if (this.DefenderPowerWidget != null)
			{
				this.DefenderPowerWidget.AlphaFactor = 0.7f;
				this.DefenderPowerWidget.ValueFactor = -70f;
			}
			if (this._powerListPanel != null)
			{
				if (this._defenderSideInitialPowerLevelDescription == null)
				{
					this._defenderSideInitialPowerLevelDescription = new ContainerItemDescription();
					this._defenderSideInitialPowerLevelDescription.WidgetId = "DefenderSideInitialPowerLevel";
					this._powerListPanel.AddItemDescription(this._defenderSideInitialPowerLevelDescription);
				}
				if (this._attackerSideInitialPowerLevelDescription == null)
				{
					this._attackerSideInitialPowerLevelDescription = new ContainerItemDescription();
					this._attackerSideInitialPowerLevelDescription.WidgetId = "AttackerSideInitialPowerLevel";
					this._powerListPanel.AddItemDescription(this._attackerSideInitialPowerLevelDescription);
				}
			}
			if (this._defenderPowerListPanel != null)
			{
				if (this._defenderSidePowerLevelDescription == null)
				{
					this._defenderSidePowerLevelDescription = new ContainerItemDescription();
					this._defenderSidePowerLevelDescription.WidgetId = "DefenderSidePowerLevel";
					this._defenderPowerListPanel.AddItemDescription(this._defenderSidePowerLevelDescription);
				}
				if (this._defenderSideEmptyPowerLevelDescription == null)
				{
					this._defenderSideEmptyPowerLevelDescription = new ContainerItemDescription();
					this._defenderSideEmptyPowerLevelDescription.WidgetId = "DefenderSideEmptyPowerLevel";
					this._defenderPowerListPanel.AddItemDescription(this._defenderSideEmptyPowerLevelDescription);
				}
			}
			if (this._attackerPowerListPanel != null)
			{
				if (this._attackerSidePowerLevelDescription == null)
				{
					this._attackerSidePowerLevelDescription = new ContainerItemDescription();
					this._attackerSidePowerLevelDescription.WidgetId = "AttackerSidePowerLevel";
					this._attackerPowerListPanel.AddItemDescription(this._attackerSidePowerLevelDescription);
				}
				if (this._attackerSideEmptyPowerLevelDescription == null)
				{
					this._attackerSideEmptyPowerLevelDescription = new ContainerItemDescription();
					this._attackerSideEmptyPowerLevelDescription.WidgetId = "AttackerSideEmptyPowerLevel";
					this._attackerPowerListPanel.AddItemDescription(this._attackerSideEmptyPowerLevelDescription);
				}
			}
			if (this._defenderSideInitialPowerLevelDescription != null && this._attackerSideInitialPowerLevelDescription != null)
			{
				float num = (float)this.InitialDefenderBattlePower / (float)(this.InitialAttackerBattlePower + this.InitialDefenderBattlePower);
				float num2 = (float)this.InitialAttackerBattlePower / (float)(this.InitialAttackerBattlePower + this.InitialDefenderBattlePower);
				if (this._defenderSideInitialPowerLevelDescription.WidthStretchRatio != num || this._attackerSideInitialPowerLevelDescription.WidthStretchRatio != num2)
				{
					this._defenderSideInitialPowerLevelDescription.WidthStretchRatio = num;
					this._attackerSideInitialPowerLevelDescription.WidthStretchRatio = num2;
					base.SetMeasureAndLayoutDirty();
				}
			}
			if (this._defenderSidePowerLevelDescription != null && this._defenderSideEmptyPowerLevelDescription != null)
			{
				float num3 = 1f - (float)this.DefenderPower / (float)this.InitialDefenderBattlePower;
				float num4 = (float)this.DefenderPower / (float)this.InitialDefenderBattlePower;
				if (this._defenderSideEmptyPowerLevelDescription.WidthStretchRatio != num3 || this._defenderSidePowerLevelDescription.WidthStretchRatio != num4)
				{
					this._defenderSidePowerLevelDescription.WidthStretchRatio = num4;
					this._defenderSideEmptyPowerLevelDescription.WidthStretchRatio = num3;
					base.SetMeasureAndLayoutDirty();
				}
			}
			if (this._attackerSidePowerLevelDescription != null && this._attackerSideEmptyPowerLevelDescription != null)
			{
				float num5 = 1f - (float)this.AttackerPower / (float)this.InitialAttackerBattlePower;
				float num6 = (float)this.AttackerPower / (float)this.InitialAttackerBattlePower;
				if (this._attackerSidePowerLevelDescription.WidthStretchRatio != num6 || this._attackerSideEmptyPowerLevelDescription.WidthStretchRatio != num5)
				{
					this._attackerSidePowerLevelDescription.WidthStretchRatio = num6;
					this._attackerSideEmptyPowerLevelDescription.WidthStretchRatio = num5;
					base.SetMeasureAndLayoutDirty();
				}
			}
			if (this.IsCenterSeperatorEnabled && this.CenterSeperatorWidget != null)
			{
				this.CenterSeperatorWidget.ScaledPositionXOffset = this.AttackerPowerWidget.Size.X - (this.CenterSeperatorWidget.Size.X - this.CenterSpace) / 2f;
			}
			base.OnLateUpdate(dt);
		}

		[Editor(false)]
		public bool IsCenterSeperatorEnabled
		{
			get
			{
				return this._isCenterSeperatorEnabled;
			}
			set
			{
				if (this._isCenterSeperatorEnabled != value)
				{
					this._isCenterSeperatorEnabled = value;
					base.OnPropertyChanged(value, "IsCenterSeperatorEnabled");
				}
			}
		}

		[Editor(false)]
		public float CenterSpace
		{
			get
			{
				return this._centerSpace;
			}
			set
			{
				if (this._centerSpace != value)
				{
					this._centerSpace = value;
					base.OnPropertyChanged(value, "CenterSpace");
				}
			}
		}

		[Editor(false)]
		public double DefenderPower
		{
			get
			{
				return this._defenderPower;
			}
			set
			{
				if (this._defenderPower != value && !double.IsNaN(value))
				{
					this._defenderPower = value;
					base.OnPropertyChanged(value, "DefenderPower");
				}
			}
		}

		[Editor(false)]
		public double AttackerPower
		{
			get
			{
				return this._attackerPower;
			}
			set
			{
				if (this._attackerPower != value && !double.IsNaN(value))
				{
					this._attackerPower = value;
					base.OnPropertyChanged(value, "AttackerPower");
				}
			}
		}

		[Editor(false)]
		public double InitialAttackerBattlePower
		{
			get
			{
				return this._initialAttackerBattlePower;
			}
			set
			{
				if (this._initialAttackerBattlePower != value && !double.IsNaN(value))
				{
					this._initialAttackerBattlePower = value;
					base.OnPropertyChanged(value, "InitialAttackerBattlePower");
				}
			}
		}

		[Editor(false)]
		public double InitialDefenderBattlePower
		{
			get
			{
				return this._initialDefenderBattlePower;
			}
			set
			{
				if (this._initialDefenderBattlePower != value && !double.IsNaN(value))
				{
					this._initialDefenderBattlePower = value;
					base.OnPropertyChanged(value, "InitialDefenderBattlePower");
				}
			}
		}

		[Editor(false)]
		public Widget AttackerPowerWidget
		{
			get
			{
				return this._attackerPowerWidget;
			}
			set
			{
				if (this._attackerPowerWidget != value)
				{
					this._attackerPowerWidget = value;
					base.OnPropertyChanged<Widget>(value, "AttackerPowerWidget");
				}
			}
		}

		[Editor(false)]
		public Widget DefenderPowerWidget
		{
			get
			{
				return this._defenderPowerWidget;
			}
			set
			{
				if (this._defenderPowerWidget != value)
				{
					this._defenderPowerWidget = value;
					base.OnPropertyChanged<Widget>(value, "DefenderPowerWidget");
				}
			}
		}

		[Editor(false)]
		public ListPanel PowerListPanel
		{
			get
			{
				return this._powerListPanel;
			}
			set
			{
				if (this._powerListPanel != value)
				{
					this._powerListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "PowerListPanel");
				}
			}
		}

		[Editor(false)]
		public ListPanel AttackerPowerListPanel
		{
			get
			{
				return this._attackerPowerListPanel;
			}
			set
			{
				if (this._attackerPowerListPanel != value)
				{
					this._attackerPowerListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "AttackerPowerListPanel");
				}
			}
		}

		[Editor(false)]
		public ListPanel DefenderPowerListPanel
		{
			get
			{
				return this._defenderPowerListPanel;
			}
			set
			{
				if (this._defenderPowerListPanel != value)
				{
					this._defenderPowerListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "DefenderPowerListPanel");
				}
			}
		}

		[Editor(false)]
		public Widget CenterSeperatorWidget
		{
			get
			{
				return this._centerSeperatorWidget;
			}
			set
			{
				if (this._centerSeperatorWidget != value)
				{
					this._centerSeperatorWidget = value;
					base.OnPropertyChanged<Widget>(value, "CenterSeperatorWidget");
				}
			}
		}

		private Widget _centerSeperatorWidget;

		private bool _isCenterSeperatorEnabled;

		private float _centerSpace;

		private double _defenderPower;

		private double _attackerPower;

		private double _initialAttackerBattlePower;

		private double _initialDefenderBattlePower;

		private Widget _defenderPowerWidget;

		private Widget _attackerPowerWidget;

		private ListPanel _powerListPanel;

		private ListPanel _defenderPowerListPanel;

		private ListPanel _attackerPowerListPanel;

		private ContainerItemDescription _defenderSideInitialPowerLevelDescription;

		private ContainerItemDescription _attackerSideInitialPowerLevelDescription;

		private ContainerItemDescription _defenderSidePowerLevelDescription;

		private ContainerItemDescription _defenderSideEmptyPowerLevelDescription;

		private ContainerItemDescription _attackerSidePowerLevelDescription;

		private ContainerItemDescription _attackerSideEmptyPowerLevelDescription;
	}
}
