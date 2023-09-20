using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Conversation
{
	// Token: 0x02000150 RID: 336
	public class ConversationScreenButtonWidget : ButtonWidget
	{
		// Token: 0x0600116D RID: 4461 RVA: 0x00030146 File Offset: 0x0002E346
		public ConversationScreenButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x0003015C File Offset: 0x0002E35C
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.AnswerList != null && this.ContinueButton != null)
			{
				this.ContinueButton.IsVisible = this.AnswerList.ChildCount == 0;
				this.ContinueButton.IsEnabled = this.AnswerList.ChildCount == 0;
			}
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x000301B4 File Offset: 0x0002E3B4
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			foreach (ConversationOptionListPanel conversationOptionListPanel in this._newlyAddedItems)
			{
				conversationOptionListPanel.OptionButtonWidget.ClickEventHandlers.Add(new Action<Widget>(this.OnOptionSelection));
			}
			this._newlyAddedItems.Clear();
			ListPanel answerList = this.AnswerList;
			if (answerList != null && answerList.ChildCount > 0 && this.AnswerList.GetChild(this.AnswerList.ChildCount - 1) != null)
			{
				this.AnswerList.GetChild(this.AnswerList.ChildCount - 1).MarginBottom = 5f;
			}
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x00030280 File Offset: 0x0002E480
		private void OnOptionSelection(Widget obj)
		{
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x00030284 File Offset: 0x0002E484
		private void OnOptionRemoved(Widget obj, Widget child)
		{
			ConversationOptionListPanel conversationOptionListPanel;
			if ((conversationOptionListPanel = obj as ConversationOptionListPanel) != null)
			{
				conversationOptionListPanel.OptionButtonWidget.ClickEventHandlers.Remove(new Action<Widget>(this.OnOptionSelection));
			}
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x000302B8 File Offset: 0x0002E4B8
		private void OnNewOptionAdded(Widget parent, Widget child)
		{
			this._newlyAddedItems.Add(child as ConversationOptionListPanel);
		}

		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x06001173 RID: 4467 RVA: 0x000302CB File Offset: 0x0002E4CB
		// (set) Token: 0x06001174 RID: 4468 RVA: 0x000302D4 File Offset: 0x0002E4D4
		[Editor(false)]
		public ListPanel AnswerList
		{
			get
			{
				return this._answerList;
			}
			set
			{
				if (value != this._answerList)
				{
					if (value != null)
					{
						value.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnNewOptionAdded));
						value.ItemRemoveEventHandlers.Add(new Action<Widget, Widget>(this.OnOptionRemoved));
					}
					if (this._answerList != null)
					{
						value.ItemAddEventHandlers.Remove(new Action<Widget, Widget>(this.OnNewOptionAdded));
						value.ItemRemoveEventHandlers.Remove(new Action<Widget, Widget>(this.OnOptionRemoved));
					}
					this._answerList = value;
					base.OnPropertyChanged<ListPanel>(value, "AnswerList");
				}
			}
		}

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x06001175 RID: 4469 RVA: 0x00030366 File Offset: 0x0002E566
		// (set) Token: 0x06001176 RID: 4470 RVA: 0x0003036E File Offset: 0x0002E56E
		[Editor(false)]
		public ButtonWidget ContinueButton
		{
			get
			{
				return this._continueButton;
			}
			set
			{
				if (value != this._continueButton)
				{
					this._continueButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ContinueButton");
				}
			}
		}

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06001177 RID: 4471 RVA: 0x0003038C File Offset: 0x0002E58C
		// (set) Token: 0x06001178 RID: 4472 RVA: 0x00030394 File Offset: 0x0002E594
		[Editor(false)]
		public bool IsPersuasionActive
		{
			get
			{
				return this._isPersuasionActive;
			}
			set
			{
				if (value != this._isPersuasionActive)
				{
					this._isPersuasionActive = value;
					base.OnPropertyChanged(value, "IsPersuasionActive");
				}
			}
		}

		// Token: 0x040007FB RID: 2043
		private List<ConversationOptionListPanel> _newlyAddedItems = new List<ConversationOptionListPanel>();

		// Token: 0x040007FC RID: 2044
		private ListPanel _answerList;

		// Token: 0x040007FD RID: 2045
		private ButtonWidget _continueButton;

		// Token: 0x040007FE RID: 2046
		private bool _isPersuasionActive;
	}
}
