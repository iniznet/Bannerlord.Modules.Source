using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Siege
{
	// Token: 0x020000FD RID: 253
	public class MapSiegePOIBrushWidget : BrushWidget
	{
		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06000D07 RID: 3335 RVA: 0x000248B4 File Offset: 0x00022AB4
		private Color _fullColor
		{
			get
			{
				return new Color(0.2784314f, 0.9882353f, 0.44313726f, 1f);
			}
		}

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06000D08 RID: 3336 RVA: 0x000248CF File Offset: 0x00022ACF
		private Color _emptyColor
		{
			get
			{
				return new Color(0.9882353f, 0.2784314f, 0.2784314f, 1f);
			}
		}

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06000D09 RID: 3337 RVA: 0x000248EA File Offset: 0x00022AEA
		// (set) Token: 0x06000D0A RID: 3338 RVA: 0x000248F2 File Offset: 0x00022AF2
		public SliderWidget Slider { get; set; }

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06000D0B RID: 3339 RVA: 0x000248FB File Offset: 0x00022AFB
		// (set) Token: 0x06000D0C RID: 3340 RVA: 0x00024903 File Offset: 0x00022B03
		public Brush ConstructionBrush { get; set; }

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06000D0D RID: 3341 RVA: 0x0002490C File Offset: 0x00022B0C
		// (set) Token: 0x06000D0E RID: 3342 RVA: 0x00024914 File Offset: 0x00022B14
		public Brush NormalBrush { get; set; }

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x06000D0F RID: 3343 RVA: 0x0002491D File Offset: 0x00022B1D
		// (set) Token: 0x06000D10 RID: 3344 RVA: 0x00024925 File Offset: 0x00022B25
		public Vec2 ScreenPosition { get; set; }

		// Token: 0x06000D11 RID: 3345 RVA: 0x0002492E File Offset: 0x00022B2E
		public MapSiegePOIBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000D12 RID: 3346 RVA: 0x00024948 File Offset: 0x00022B48
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			base.ScaledPositionXOffset = this.ScreenPosition.x - base.Size.X / 2f;
			base.ScaledPositionYOffset = this.ScreenPosition.y;
			float num = (float)(this.IsInVisibleRange ? 1 : 0);
			float num2 = MathF.Lerp(base.ReadOnlyBrush.GlobalAlphaFactor, num, dt * 10f, 1E-05f);
			this.SetGlobalAlphaRecursively(num2);
			base.IsEnabled = false;
			if (this._animState == MapSiegePOIBrushWidget.AnimState.Start)
			{
				this._tickCount++;
				if (this._tickCount > 5)
				{
					this._animState = MapSiegePOIBrushWidget.AnimState.Starting;
				}
			}
			else if (this._animState == MapSiegePOIBrushWidget.AnimState.Starting)
			{
				(this.Slider.Filler as BrushWidget).BrushRenderer.RestartAnimation();
				if (this.QueueIndex == 0)
				{
					this.HammerAnimWidget.BrushRenderer.RestartAnimation();
				}
				this._animState = MapSiegePOIBrushWidget.AnimState.Playing;
			}
			if (!this._isBrushChanged)
			{
				(this.Slider.Filler as BrushWidget).Brush = (this.IsConstructing ? this.ConstructionBrush : this.NormalBrush);
				this._animState = MapSiegePOIBrushWidget.AnimState.Start;
				this._isBrushChanged = true;
			}
			if (!this.IsConstructing)
			{
				this.UpdateColorOfSlider();
			}
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x00024A84 File Offset: 0x00022C84
		protected override void OnMousePressed()
		{
			base.OnMousePressed();
			this.IsPOISelected = true;
			base.EventFired("OnSelection", Array.Empty<object>());
		}

		// Token: 0x06000D14 RID: 3348 RVA: 0x00024AA3 File Offset: 0x00022CA3
		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
		}

		// Token: 0x06000D15 RID: 3349 RVA: 0x00024AAB File Offset: 0x00022CAB
		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
		}

		// Token: 0x06000D16 RID: 3350 RVA: 0x00024AB4 File Offset: 0x00022CB4
		private void SetMachineTypeIcon(int machineType)
		{
			string text = "SPGeneral\\MapSiege\\";
			switch (machineType)
			{
			case 0:
				text += "wall";
				break;
			case 1:
				text += "broken_wall";
				break;
			case 2:
				text += "ballista";
				break;
			case 3:
				text += "trebuchet";
				break;
			case 4:
				text += "ladder";
				break;
			case 5:
				text += "ram";
				break;
			case 6:
				text += "tower";
				break;
			case 7:
				text += "mangonel";
				break;
			default:
				text += "fallback";
				break;
			}
			this.MachineTypeIconWidget.Sprite = base.Context.SpriteData.GetSprite(text);
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x00024B88 File Offset: 0x00022D88
		private void UpdateColorOfSlider()
		{
			(this.Slider.Filler as BrushWidget).Brush.Color = Color.Lerp(this._emptyColor, this._fullColor, this.Slider.ValueFloat / this.Slider.MaxValueFloat);
		}

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x06000D18 RID: 3352 RVA: 0x00024BD7 File Offset: 0x00022DD7
		// (set) Token: 0x06000D19 RID: 3353 RVA: 0x00024BDF File Offset: 0x00022DDF
		public MapSiegeConstructionControllerWidget ConstructionControllerWidget
		{
			get
			{
				return this._constructionControllerWidget;
			}
			set
			{
				if (this._constructionControllerWidget != value)
				{
					this._constructionControllerWidget = value;
				}
			}
		}

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x06000D1A RID: 3354 RVA: 0x00024BF1 File Offset: 0x00022DF1
		// (set) Token: 0x06000D1B RID: 3355 RVA: 0x00024BF9 File Offset: 0x00022DF9
		public bool IsPlayerSidePOI
		{
			get
			{
				return this._isPlayerSidePOI;
			}
			set
			{
				if (this._isPlayerSidePOI != value)
				{
					this._isPlayerSidePOI = value;
				}
			}
		}

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06000D1C RID: 3356 RVA: 0x00024C0B File Offset: 0x00022E0B
		// (set) Token: 0x06000D1D RID: 3357 RVA: 0x00024C13 File Offset: 0x00022E13
		public bool IsInVisibleRange
		{
			get
			{
				return this._isInVisibleRange;
			}
			set
			{
				if (this._isInVisibleRange != value)
				{
					this._isInVisibleRange = value;
				}
			}
		}

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x06000D1E RID: 3358 RVA: 0x00024C25 File Offset: 0x00022E25
		// (set) Token: 0x06000D1F RID: 3359 RVA: 0x00024C2D File Offset: 0x00022E2D
		public bool IsPOISelected
		{
			get
			{
				return this._isPOISelected;
			}
			set
			{
				if (this._isPOISelected != value)
				{
					this._isPOISelected = value;
					this.ConstructionControllerWidget.SetCurrentPOIWidget(value ? this : null);
				}
			}
		}

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x06000D20 RID: 3360 RVA: 0x00024C51 File Offset: 0x00022E51
		// (set) Token: 0x06000D21 RID: 3361 RVA: 0x00024C59 File Offset: 0x00022E59
		public bool IsConstructing
		{
			get
			{
				return this._isConstructing;
			}
			set
			{
				if (this._isConstructing != value)
				{
					this._isConstructing = value;
					this._isBrushChanged = false;
					this._animState = MapSiegePOIBrushWidget.AnimState.Idle;
				}
			}
		}

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06000D22 RID: 3362 RVA: 0x00024C79 File Offset: 0x00022E79
		// (set) Token: 0x06000D23 RID: 3363 RVA: 0x00024C81 File Offset: 0x00022E81
		public int MachineType
		{
			get
			{
				return this._machineType;
			}
			set
			{
				if (this._machineType != value)
				{
					this._machineType = value;
					this.SetMachineTypeIcon(value);
				}
			}
		}

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06000D24 RID: 3364 RVA: 0x00024C9A File Offset: 0x00022E9A
		// (set) Token: 0x06000D25 RID: 3365 RVA: 0x00024CA2 File Offset: 0x00022EA2
		public int QueueIndex
		{
			get
			{
				return this._queueIndex;
			}
			set
			{
				if (this._queueIndex != value)
				{
					this._queueIndex = value;
					this._animState = MapSiegePOIBrushWidget.AnimState.Start;
					this._tickCount = 0;
				}
			}
		}

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x06000D26 RID: 3366 RVA: 0x00024CC2 File Offset: 0x00022EC2
		// (set) Token: 0x06000D27 RID: 3367 RVA: 0x00024CCA File Offset: 0x00022ECA
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

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x06000D28 RID: 3368 RVA: 0x00024CDC File Offset: 0x00022EDC
		// (set) Token: 0x06000D29 RID: 3369 RVA: 0x00024CE4 File Offset: 0x00022EE4
		public BrushWidget HammerAnimWidget
		{
			get
			{
				return this._hammerAnimWidget;
			}
			set
			{
				if (this._hammerAnimWidget != value)
				{
					this._hammerAnimWidget = value;
				}
			}
		}

		// Token: 0x0400060B RID: 1547
		private MapSiegePOIBrushWidget.AnimState _animState;

		// Token: 0x04000610 RID: 1552
		private bool _isBrushChanged;

		// Token: 0x04000611 RID: 1553
		private int _tickCount;

		// Token: 0x04000612 RID: 1554
		private bool _isConstructing;

		// Token: 0x04000613 RID: 1555
		private bool _isPlayerSidePOI;

		// Token: 0x04000614 RID: 1556
		private bool _isInVisibleRange;

		// Token: 0x04000615 RID: 1557
		private bool _isPOISelected;

		// Token: 0x04000616 RID: 1558
		private BrushWidget _hammerAnimWidget;

		// Token: 0x04000617 RID: 1559
		private Widget _machineTypeIconWidget;

		// Token: 0x04000618 RID: 1560
		private int _machineType = -1;

		// Token: 0x04000619 RID: 1561
		private int _queueIndex = -1;

		// Token: 0x0400061A RID: 1562
		private MapSiegeConstructionControllerWidget _constructionControllerWidget;

		// Token: 0x02000196 RID: 406
		public enum AnimState
		{
			// Token: 0x04000911 RID: 2321
			Idle,
			// Token: 0x04000912 RID: 2322
			Start,
			// Token: 0x04000913 RID: 2323
			Starting,
			// Token: 0x04000914 RID: 2324
			Playing
		}
	}
}
