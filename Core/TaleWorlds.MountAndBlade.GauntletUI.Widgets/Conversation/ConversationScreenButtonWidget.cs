using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Conversation
{
	public class ConversationScreenButtonWidget : ButtonWidget
	{
		public ConversationScreenButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.AnswerList != null && this.ContinueButton != null)
			{
				this.ContinueButton.IsVisible = this.AnswerList.ChildCount == 0;
				this.ContinueButton.IsEnabled = this.AnswerList.ChildCount == 0;
			}
		}

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

		private void OnOptionSelection(Widget obj)
		{
		}

		private void OnOptionRemoved(Widget obj, Widget child)
		{
			ConversationOptionListPanel conversationOptionListPanel;
			if ((conversationOptionListPanel = obj as ConversationOptionListPanel) != null)
			{
				conversationOptionListPanel.OptionButtonWidget.ClickEventHandlers.Remove(new Action<Widget>(this.OnOptionSelection));
			}
		}

		private void OnNewOptionAdded(Widget parent, Widget child)
		{
			this._newlyAddedItems.Add(child as ConversationOptionListPanel);
		}

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

		private List<ConversationOptionListPanel> _newlyAddedItems = new List<ConversationOptionListPanel>();

		private ListPanel _answerList;

		private ButtonWidget _continueButton;

		private bool _isPersuasionActive;
	}
}
