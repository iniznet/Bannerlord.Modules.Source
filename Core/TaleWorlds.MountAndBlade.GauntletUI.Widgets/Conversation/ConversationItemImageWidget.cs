using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Conversation
{
	public class ConversationItemImageWidget : ImageWidget
	{
		public Brush NormalBrush { get; set; }

		public Brush SpecialBrush { get; set; }

		public bool IsSpecial { get; set; }

		public ConversationItemImageWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._isInitialized)
			{
				base.Brush = (this.IsSpecial ? this.SpecialBrush : this.NormalBrush);
				this._isInitialized = true;
			}
		}

		private bool _isInitialized;
	}
}
