using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000031 RID: 49
	public class RainbowRichTextWidget : RichTextWidget
	{
		// Token: 0x060002B5 RID: 693 RVA: 0x00008F86 File Offset: 0x00007186
		public RainbowRichTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00008F9C File Offset: 0x0000719C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			base.Brush.FontColor = Color.Lerp(base.ReadOnlyBrush.FontColor, this.targetColor, dt);
			if (base.Brush.FontColor.ToVec3().Distance(this.targetColor.ToVec3()) < 1f)
			{
				Random random = new Random();
				this.targetColor = Color.FromVector3(new Vector3((float)random.Next(255), (float)random.Next(255), (float)random.Next(255)));
			}
		}

		// Token: 0x04000117 RID: 279
		private Color targetColor = Color.White;
	}
}
