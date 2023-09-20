using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard
{
	// Token: 0x0200008A RID: 138
	public class MultiplayerScoreboardStripedBackgroundWidget : MultiplayerScoreboardStatsListPanel
	{
		// Token: 0x06000756 RID: 1878 RVA: 0x00015AD0 File Offset: 0x00013CD0
		public MultiplayerScoreboardStripedBackgroundWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x00015ADC File Offset: 0x00013CDC
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			if (base.ChildCount % 2 == 1)
			{
				child.Sprite = base.Context.SpriteData.GetSprite("BlankWhiteSquare_9");
				child.Color = Color.ConvertStringToColor("#000000FF");
				child.AlphaFactor = 0.2f;
			}
		}
	}
}
