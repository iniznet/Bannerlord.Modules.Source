using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapConversation
{
	public class MapConversationScreenButtonWidget : ButtonWidget
	{
		public Widget ConversationParent { get; set; }

		public MapConversationScreenButtonWidget(UIContext context)
			: base(context)
		{
		}

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

		private bool _isBarterActive;
	}
}
