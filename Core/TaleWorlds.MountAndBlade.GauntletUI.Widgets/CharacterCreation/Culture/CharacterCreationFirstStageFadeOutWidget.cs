using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterCreation.Culture
{
	// Token: 0x0200016B RID: 363
	public class CharacterCreationFirstStageFadeOutWidget : Widget
	{
		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x060012A2 RID: 4770 RVA: 0x00033727 File Offset: 0x00031927
		// (set) Token: 0x060012A3 RID: 4771 RVA: 0x0003372F File Offset: 0x0003192F
		public float StayTime { get; set; } = 1.5f;

		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x060012A4 RID: 4772 RVA: 0x00033738 File Offset: 0x00031938
		// (set) Token: 0x060012A5 RID: 4773 RVA: 0x00033740 File Offset: 0x00031940
		public float FadeOutTime { get; set; } = 1.5f;

		// Token: 0x060012A6 RID: 4774 RVA: 0x00033749 File Offset: 0x00031949
		public CharacterCreationFirstStageFadeOutWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060012A7 RID: 4775 RVA: 0x00033768 File Offset: 0x00031968
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._totalTime < this.StayTime)
			{
				this.SetGlobalAlphaRecursively(1f);
				base.IsEnabled = true;
			}
			else if (this._totalTime > this.StayTime && this._totalTime < this.StayTime + this.FadeOutTime)
			{
				float num = Mathf.Lerp(1f, 0f, (this._totalTime - this.StayTime) / this.FadeOutTime);
				this.SetGlobalAlphaRecursively(num);
				base.IsEnabled = num > 0.2f;
			}
			else
			{
				this.SetGlobalAlphaRecursively(0f);
				base.IsEnabled = false;
			}
			this._totalTime += dt;
		}

		// Token: 0x0400088A RID: 2186
		private float _totalTime;
	}
}
