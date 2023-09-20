using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Conversation
{
	// Token: 0x0200014B RID: 331
	public class ConversationAnswersContainerWidget : Widget
	{
		// Token: 0x0600114C RID: 4428 RVA: 0x0002FDD9 File Offset: 0x0002DFD9
		public ConversationAnswersContainerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x0002FDE2 File Offset: 0x0002DFE2
		protected override void OnLateUpdate(float dt)
		{
			this.UpdateHeight();
			base.OnLateUpdate(dt);
			this.UpdateHeight();
		}

		// Token: 0x0600114E RID: 4430 RVA: 0x0002FDF7 File Offset: 0x0002DFF7
		protected override void OnUpdate(float dt)
		{
			this.UpdateHeight();
			base.OnUpdate(dt);
			this.UpdateHeight();
		}

		// Token: 0x0600114F RID: 4431 RVA: 0x0002FE0C File Offset: 0x0002E00C
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

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x06001150 RID: 4432 RVA: 0x0002FE5B File Offset: 0x0002E05B
		// (set) Token: 0x06001151 RID: 4433 RVA: 0x0002FE63 File Offset: 0x0002E063
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

		// Token: 0x040007EF RID: 2031
		private Widget _answerContainerWidget;
	}
}
