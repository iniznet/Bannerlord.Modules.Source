using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200001D RID: 29
	public class GamepadCursorParentWidget : Widget
	{
		// Token: 0x06000157 RID: 343 RVA: 0x00005C0B File Offset: 0x00003E0B
		public GamepadCursorParentWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00005C14 File Offset: 0x00003E14
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.CenterWidget.SetGlobalAlphaRecursively(MathF.Lerp(this.CenterWidget.AlphaFactor, this.HasTarget ? 0.67f : 1f, 0.16f, 1E-05f));
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000159 RID: 345 RVA: 0x00005C61 File Offset: 0x00003E61
		// (set) Token: 0x0600015A RID: 346 RVA: 0x00005C69 File Offset: 0x00003E69
		public float XOffset
		{
			get
			{
				return this._xOffset;
			}
			set
			{
				if (value != this._xOffset)
				{
					this._xOffset = value;
					base.OnPropertyChanged(value, "XOffset");
					this.CenterWidget.ScaledPositionXOffset = value;
				}
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600015B RID: 347 RVA: 0x00005C93 File Offset: 0x00003E93
		// (set) Token: 0x0600015C RID: 348 RVA: 0x00005C9B File Offset: 0x00003E9B
		public float YOffset
		{
			get
			{
				return this._yOffset;
			}
			set
			{
				if (value != this._yOffset)
				{
					this._yOffset = value;
					base.OnPropertyChanged(value, "YOffset");
					this.CenterWidget.ScaledPositionYOffset = value;
				}
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600015D RID: 349 RVA: 0x00005CC5 File Offset: 0x00003EC5
		// (set) Token: 0x0600015E RID: 350 RVA: 0x00005CCD File Offset: 0x00003ECD
		public bool HasTarget
		{
			get
			{
				return this._hasTarget;
			}
			set
			{
				if (value != this._hasTarget)
				{
					this._hasTarget = value;
					base.OnPropertyChanged(value, "HasTarget");
				}
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600015F RID: 351 RVA: 0x00005CEB File Offset: 0x00003EEB
		// (set) Token: 0x06000160 RID: 352 RVA: 0x00005CF3 File Offset: 0x00003EF3
		public BrushWidget CenterWidget
		{
			get
			{
				return this._centerWidget;
			}
			set
			{
				if (value != this._centerWidget)
				{
					this._centerWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "CenterWidget");
				}
			}
		}

		// Token: 0x040000A4 RID: 164
		private float _xOffset;

		// Token: 0x040000A5 RID: 165
		private float _yOffset;

		// Token: 0x040000A6 RID: 166
		private bool _hasTarget;

		// Token: 0x040000A7 RID: 167
		private BrushWidget _centerWidget;
	}
}
