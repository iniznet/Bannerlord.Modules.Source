using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapConversation
{
	// Token: 0x02000106 RID: 262
	public class MapConversationScreenButtonWidget : ButtonWidget
	{
		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x06000D8A RID: 3466 RVA: 0x00025FF5 File Offset: 0x000241F5
		// (set) Token: 0x06000D8B RID: 3467 RVA: 0x00025FFD File Offset: 0x000241FD
		public Widget ConversationParent { get; set; }

		// Token: 0x06000D8C RID: 3468 RVA: 0x00026006 File Offset: 0x00024206
		public MapConversationScreenButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06000D8D RID: 3469 RVA: 0x0002600F File Offset: 0x0002420F
		// (set) Token: 0x06000D8E RID: 3470 RVA: 0x00026017 File Offset: 0x00024217
		public bool IsBarterActive
		{
			get
			{
				return this._isBarterActive;
			}
			set
			{
				if (this._isBarterActive != value)
				{
					this._isBarterActive = value;
					this.ConversationParent.IsVisible = !this.IsBarterActive;
				}
			}
		}

		// Token: 0x04000640 RID: 1600
		private bool _isBarterActive;
	}
}
