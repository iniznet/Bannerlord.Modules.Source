using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	// Token: 0x020000CB RID: 203
	public class MissionSiegeEngineMarkerWidget : Widget
	{
		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06000A50 RID: 2640 RVA: 0x0001D3E5 File Offset: 0x0001B5E5
		// (set) Token: 0x06000A51 RID: 2641 RVA: 0x0001D3ED File Offset: 0x0001B5ED
		public SliderWidget Slider { get; set; }

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06000A52 RID: 2642 RVA: 0x0001D3F6 File Offset: 0x0001B5F6
		// (set) Token: 0x06000A53 RID: 2643 RVA: 0x0001D3FE File Offset: 0x0001B5FE
		public BrushWidget MachineIconParent { get; set; }

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06000A54 RID: 2644 RVA: 0x0001D407 File Offset: 0x0001B607
		// (set) Token: 0x06000A55 RID: 2645 RVA: 0x0001D40F File Offset: 0x0001B60F
		public Brush EnemyBrush { get; set; }

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06000A56 RID: 2646 RVA: 0x0001D418 File Offset: 0x0001B618
		// (set) Token: 0x06000A57 RID: 2647 RVA: 0x0001D420 File Offset: 0x0001B620
		public Brush AllyBrush { get; set; }

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06000A58 RID: 2648 RVA: 0x0001D429 File Offset: 0x0001B629
		// (set) Token: 0x06000A59 RID: 2649 RVA: 0x0001D431 File Offset: 0x0001B631
		public Vec2 ScreenPosition { get; set; }

		// Token: 0x06000A5A RID: 2650 RVA: 0x0001D43C File Offset: 0x0001B63C
		public MissionSiegeEngineMarkerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000A5B RID: 2651 RVA: 0x0001D490 File Offset: 0x0001B690
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			base.ScaledPositionXOffset = this.ScreenPosition.x - base.Size.X / 2f;
			base.ScaledPositionYOffset = this.ScreenPosition.y;
			float num = (this.IsActive ? 0.65f : 0f);
			float num2 = MathF.Lerp(base.AlphaFactor, num, dt * 10f, 1E-05f);
			this.SetGlobalAlphaRecursively(num2);
			if (!this._isBrushChanged)
			{
				this.MachineIconParent.Brush = (this.IsEnemy ? this.EnemyBrush : this.AllyBrush);
				this._isBrushChanged = true;
			}
			this.UpdateColorOfSlider();
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x0001D544 File Offset: 0x0001B744
		private void SetMachineTypeIcon(string machineType)
		{
			string text = "SPGeneral\\MapSiege\\" + machineType;
			this.MachineTypeIconWidget.Sprite = base.Context.SpriteData.GetSprite(text);
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x0001D57C File Offset: 0x0001B77C
		private void UpdateColorOfSlider()
		{
			(this.Slider.Filler as BrushWidget).Brush.Color = Color.Lerp(this._emptyColor, this._fullColor, this.Slider.ValueFloat / this.Slider.MaxValueFloat);
		}

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06000A5E RID: 2654 RVA: 0x0001D5CB File Offset: 0x0001B7CB
		// (set) Token: 0x06000A5F RID: 2655 RVA: 0x0001D5D3 File Offset: 0x0001B7D3
		public bool IsEnemy
		{
			get
			{
				return this._isEnemy;
			}
			set
			{
				if (this._isEnemy != value)
				{
					this._isEnemy = value;
				}
			}
		}

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06000A60 RID: 2656 RVA: 0x0001D5E5 File Offset: 0x0001B7E5
		// (set) Token: 0x06000A61 RID: 2657 RVA: 0x0001D5ED File Offset: 0x0001B7ED
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (this._isActive != value)
				{
					this._isActive = value;
				}
			}
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06000A62 RID: 2658 RVA: 0x0001D5FF File Offset: 0x0001B7FF
		// (set) Token: 0x06000A63 RID: 2659 RVA: 0x0001D607 File Offset: 0x0001B807
		public string EngineType
		{
			get
			{
				return this._engineType;
			}
			set
			{
				if (this._engineType != value)
				{
					this._engineType = value;
					this.SetMachineTypeIcon(value);
				}
			}
		}

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06000A64 RID: 2660 RVA: 0x0001D625 File Offset: 0x0001B825
		// (set) Token: 0x06000A65 RID: 2661 RVA: 0x0001D62D File Offset: 0x0001B82D
		public Widget MachineTypeIconWidget
		{
			get
			{
				return this._machineTypeIconWidget;
			}
			set
			{
				if (this._machineTypeIconWidget != value)
				{
					this._machineTypeIconWidget = value;
				}
			}
		}

		// Token: 0x040004B2 RID: 1202
		private Color _fullColor = new Color(0.2784314f, 0.9882353f, 0.44313726f, 1f);

		// Token: 0x040004B3 RID: 1203
		private Color _emptyColor = new Color(0.9882353f, 0.2784314f, 0.2784314f, 1f);

		// Token: 0x040004B9 RID: 1209
		private bool _isBrushChanged;

		// Token: 0x040004BA RID: 1210
		private bool _isEnemy;

		// Token: 0x040004BB RID: 1211
		private bool _isActive;

		// Token: 0x040004BC RID: 1212
		private Widget _machineTypeIconWidget;

		// Token: 0x040004BD RID: 1213
		private string _engineType;
	}
}
