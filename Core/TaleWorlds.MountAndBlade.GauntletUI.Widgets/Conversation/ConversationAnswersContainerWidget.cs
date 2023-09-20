using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Conversation
{
	public class ConversationAnswersContainerWidget : Widget
	{
		public ConversationAnswersContainerWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			this.UpdateHeight();
			base.OnLateUpdate(dt);
			this.UpdateHeight();
		}

		protected override void OnUpdate(float dt)
		{
			this.UpdateHeight();
			base.OnUpdate(dt);
			this.UpdateHeight();
		}

		private void UpdateHeight()
		{
			if (this.AnswerContainerWidget.Size.Y >= base.Size.Y)
			{
				base.HeightSizePolicy = SizePolicy.Fixed;
				base.ScaledSuggestedHeight = this.AnswerContainerWidget.Size.Y;
				return;
			}
			base.HeightSizePolicy = SizePolicy.CoverChildren;
		}

		[Editor(false)]
		public Widget AnswerContainerWidget
		{
			get
			{
				return this._answerContainerWidget;
			}
			set
			{
				if (value != this._answerContainerWidget)
				{
					this._answerContainerWidget = value;
					base.OnPropertyChanged<Widget>(value, "AnswerContainerWidget");
				}
			}
		}

		private Widget _answerContainerWidget;
	}
}
