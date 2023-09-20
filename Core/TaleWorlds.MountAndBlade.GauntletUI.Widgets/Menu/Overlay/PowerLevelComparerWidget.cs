using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.Overlay
{
	// Token: 0x020000F5 RID: 245
	public class PowerLevelComparerWidget : Widget
	{
		// Token: 0x06000CC3 RID: 3267 RVA: 0x00023ADD File Offset: 0x00021CDD
		public PowerLevelComparerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000CC4 RID: 3268 RVA: 0x00023AE8 File Offset: 0x00021CE8
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

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x06000CC5 RID: 3269 RVA: 0x00023E43 File Offset: 0x00022043
		// (set) Token: 0x06000CC6 RID: 3270 RVA: 0x00023E4B File Offset: 0x0002204B
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

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x06000CC7 RID: 3271 RVA: 0x00023E69 File Offset: 0x00022069
		// (set) Token: 0x06000CC8 RID: 3272 RVA: 0x00023E71 File Offset: 0x00022071
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

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x06000CC9 RID: 3273 RVA: 0x00023E8F File Offset: 0x0002208F
		// (set) Token: 0x06000CCA RID: 3274 RVA: 0x00023E97 File Offset: 0x00022097
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

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06000CCB RID: 3275 RVA: 0x00023EBD File Offset: 0x000220BD
		// (set) Token: 0x06000CCC RID: 3276 RVA: 0x00023EC5 File Offset: 0x000220C5
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

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06000CCD RID: 3277 RVA: 0x00023EEB File Offset: 0x000220EB
		// (set) Token: 0x06000CCE RID: 3278 RVA: 0x00023EF3 File Offset: 0x000220F3
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

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x06000CCF RID: 3279 RVA: 0x00023F19 File Offset: 0x00022119
		// (set) Token: 0x06000CD0 RID: 3280 RVA: 0x00023F21 File Offset: 0x00022121
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

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x06000CD1 RID: 3281 RVA: 0x00023F47 File Offset: 0x00022147
		// (set) Token: 0x06000CD2 RID: 3282 RVA: 0x00023F4F File Offset: 0x0002214F
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

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x06000CD3 RID: 3283 RVA: 0x00023F6D File Offset: 0x0002216D
		// (set) Token: 0x06000CD4 RID: 3284 RVA: 0x00023F75 File Offset: 0x00022175
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

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x06000CD5 RID: 3285 RVA: 0x00023F93 File Offset: 0x00022193
		// (set) Token: 0x06000CD6 RID: 3286 RVA: 0x00023F9B File Offset: 0x0002219B
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

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x06000CD7 RID: 3287 RVA: 0x00023FB9 File Offset: 0x000221B9
		// (set) Token: 0x06000CD8 RID: 3288 RVA: 0x00023FC1 File Offset: 0x000221C1
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

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x06000CD9 RID: 3289 RVA: 0x00023FDF File Offset: 0x000221DF
		// (set) Token: 0x06000CDA RID: 3290 RVA: 0x00023FE7 File Offset: 0x000221E7
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

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06000CDB RID: 3291 RVA: 0x00024005 File Offset: 0x00022205
		// (set) Token: 0x06000CDC RID: 3292 RVA: 0x0002400D File Offset: 0x0002220D
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

		// Token: 0x040005E3 RID: 1507
		private Widget _centerSeperatorWidget;

		// Token: 0x040005E4 RID: 1508
		private bool _isCenterSeperatorEnabled;

		// Token: 0x040005E5 RID: 1509
		private float _centerSpace;

		// Token: 0x040005E6 RID: 1510
		private double _defenderPower;

		// Token: 0x040005E7 RID: 1511
		private double _attackerPower;

		// Token: 0x040005E8 RID: 1512
		private double _initialAttackerBattlePower;

		// Token: 0x040005E9 RID: 1513
		private double _initialDefenderBattlePower;

		// Token: 0x040005EA RID: 1514
		private Widget _defenderPowerWidget;

		// Token: 0x040005EB RID: 1515
		private Widget _attackerPowerWidget;

		// Token: 0x040005EC RID: 1516
		private ListPanel _powerListPanel;

		// Token: 0x040005ED RID: 1517
		private ListPanel _defenderPowerListPanel;

		// Token: 0x040005EE RID: 1518
		private ListPanel _attackerPowerListPanel;

		// Token: 0x040005EF RID: 1519
		private ContainerItemDescription _defenderSideInitialPowerLevelDescription;

		// Token: 0x040005F0 RID: 1520
		private ContainerItemDescription _attackerSideInitialPowerLevelDescription;

		// Token: 0x040005F1 RID: 1521
		private ContainerItemDescription _defenderSidePowerLevelDescription;

		// Token: 0x040005F2 RID: 1522
		private ContainerItemDescription _defenderSideEmptyPowerLevelDescription;

		// Token: 0x040005F3 RID: 1523
		private ContainerItemDescription _attackerSidePowerLevelDescription;

		// Token: 0x040005F4 RID: 1524
		private ContainerItemDescription _attackerSideEmptyPowerLevelDescription;
	}
}
