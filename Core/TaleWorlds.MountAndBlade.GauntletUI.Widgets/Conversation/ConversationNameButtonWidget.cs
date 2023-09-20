using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Conversation
{
	public class ConversationNameButtonWidget : ButtonWidget
	{
		public ConversationNameButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
			this.RelationBarContainer.IsVisible = this.IsRelationEnabled;
		}

		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
			this.RelationBarContainer.IsVisible = false;
		}

		[Editor(false)]
		public bool IsRelationEnabled
		{
			get
			{
				return this._isRelationEnabled;
			}
			set
			{
				if (value != this._isRelationEnabled)
				{
					this._isRelationEnabled = value;
					base.OnPropertyChanged(value, "IsRelationEnabled");
				}
			}
		}

		[Editor(false)]
		public Widget RelationBarContainer
		{
			get
			{
				return this._relationBarContainer;
			}
			set
			{
				if (value != this._relationBarContainer)
				{
					this._relationBarContainer = value;
					base.OnPropertyChanged<Widget>(value, "RelationBarContainer");
				}
			}
		}

		private bool _isRelationEnabled;

		private Widget _relationBarContainer;
	}
}
