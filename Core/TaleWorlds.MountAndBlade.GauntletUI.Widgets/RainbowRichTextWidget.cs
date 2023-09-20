using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class RainbowRichTextWidget : RichTextWidget
	{
		public RainbowRichTextWidget(UIContext context)
			: base(context)
		{
		}

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

		private Color targetColor = Color.White;
	}
}
