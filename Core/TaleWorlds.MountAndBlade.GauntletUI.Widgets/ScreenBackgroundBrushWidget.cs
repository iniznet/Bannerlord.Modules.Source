using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000034 RID: 52
	public class ScreenBackgroundBrushWidget : BrushWidget
	{
		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060002E4 RID: 740 RVA: 0x0000988C File Offset: 0x00007A8C
		// (set) Token: 0x060002E5 RID: 741 RVA: 0x00009894 File Offset: 0x00007A94
		public bool IsParticleVisible { get; set; }

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060002E6 RID: 742 RVA: 0x0000989D File Offset: 0x00007A9D
		// (set) Token: 0x060002E7 RID: 743 RVA: 0x000098A5 File Offset: 0x00007AA5
		public bool IsSmokeVisible { get; set; }

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060002E8 RID: 744 RVA: 0x000098AE File Offset: 0x00007AAE
		// (set) Token: 0x060002E9 RID: 745 RVA: 0x000098B6 File Offset: 0x00007AB6
		public bool IsFullscreenImageEnabled { get; set; }

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060002EA RID: 746 RVA: 0x000098BF File Offset: 0x00007ABF
		// (set) Token: 0x060002EB RID: 747 RVA: 0x000098C7 File Offset: 0x00007AC7
		public bool AnimEnabled { get; set; }

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060002EC RID: 748 RVA: 0x000098D0 File Offset: 0x00007AD0
		// (set) Token: 0x060002ED RID: 749 RVA: 0x000098D8 File Offset: 0x00007AD8
		public Widget ParticleWidget1 { get; set; }

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060002EE RID: 750 RVA: 0x000098E1 File Offset: 0x00007AE1
		// (set) Token: 0x060002EF RID: 751 RVA: 0x000098E9 File Offset: 0x00007AE9
		public Widget ParticleWidget2 { get; set; }

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060002F0 RID: 752 RVA: 0x000098F2 File Offset: 0x00007AF2
		// (set) Token: 0x060002F1 RID: 753 RVA: 0x000098FA File Offset: 0x00007AFA
		public Widget SmokeWidget1 { get; set; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060002F2 RID: 754 RVA: 0x00009903 File Offset: 0x00007B03
		// (set) Token: 0x060002F3 RID: 755 RVA: 0x0000990B File Offset: 0x00007B0B
		public Widget SmokeWidget2 { get; set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060002F4 RID: 756 RVA: 0x00009914 File Offset: 0x00007B14
		// (set) Token: 0x060002F5 RID: 757 RVA: 0x0000991C File Offset: 0x00007B1C
		public float SmokeSpeedModifier { get; set; } = 1f;

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060002F6 RID: 758 RVA: 0x00009925 File Offset: 0x00007B25
		// (set) Token: 0x060002F7 RID: 759 RVA: 0x0000992D File Offset: 0x00007B2D
		public float ParticleSpeedModifier { get; set; } = 1f;

		// Token: 0x060002F8 RID: 760 RVA: 0x00009936 File Offset: 0x00007B36
		public ScreenBackgroundBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0000995C File Offset: 0x00007B5C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._firstFrame)
			{
				this.UpdateBackgroundImage();
				this._firstFrame = false;
			}
			this.ParticleWidget1.IsVisible = this.IsParticleVisible;
			this.ParticleWidget2.IsVisible = this.IsParticleVisible;
			this.SmokeWidget1.IsVisible = this.IsSmokeVisible;
			this.SmokeWidget2.IsVisible = this.IsSmokeVisible;
			if (this.AnimEnabled)
			{
				if (this.IsParticleVisible)
				{
					this.ParticleWidget1.PositionXOffset = this._totalParticleXOffset;
					this.ParticleWidget2.PositionXOffset = this.ParticleWidget1.PositionXOffset + this.ParticleWidget1.SuggestedWidth;
					this._totalParticleXOffset -= dt * 10f * this.ParticleSpeedModifier;
					if (Math.Abs(this._totalParticleXOffset) >= this.ParticleWidget1.SuggestedWidth)
					{
						this._totalParticleXOffset = 0f;
					}
				}
				if (this.IsSmokeVisible)
				{
					this.SmokeWidget1.PositionXOffset = this._totalSmokeXOffset;
					this.SmokeWidget2.PositionXOffset = this.SmokeWidget1.PositionXOffset - this.SmokeWidget1.SuggestedWidth;
					if (Math.Abs(this._totalSmokeXOffset) >= this.SmokeWidget1.SuggestedWidth)
					{
						this._totalSmokeXOffset = 0f;
					}
					this._totalSmokeXOffset += dt * 10f * this.SmokeSpeedModifier;
				}
			}
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00009AC8 File Offset: 0x00007CC8
		private void UpdateBackgroundImage()
		{
			if (this.IsFullscreenImageEnabled)
			{
				int num = base.Context.UIRandom.Next(base.Brush.Styles.Count);
				base.Brush.Sprite = base.ReadOnlyBrush.Styles.ElementAt(num).Layers[0].Sprite;
				return;
			}
			base.Brush.Sprite = null;
		}

		// Token: 0x04000138 RID: 312
		private bool _firstFrame = true;

		// Token: 0x04000139 RID: 313
		private float _totalSmokeXOffset;

		// Token: 0x0400013A RID: 314
		private float _totalParticleXOffset;
	}
}
