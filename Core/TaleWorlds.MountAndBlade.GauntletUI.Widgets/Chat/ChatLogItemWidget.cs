using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Chat
{
	public class ChatLogItemWidget : Widget
	{
		public ChatLogItemWidget(UIContext context)
			: base(context)
		{
			this._fullyInsideAction = new Action<Widget>(this.UpdateWidgetFullyInside);
		}

		private void UpdateWidgetFullyInside(Widget widget)
		{
			widget.DoNotRenderIfNotFullyInsideScissor = false;
		}

		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			base.ApplyActionOnAllChildren(this._fullyInsideAction);
		}

		private void PostMessage(string message)
		{
			if (message.IndexOf(this._detailOpeningTag, StringComparison.Ordinal) > 0)
			{
				foreach (ChatLogItemWidget.ChatMultiLineElement chatMultiLineElement in this.GetFormattedLinesFromMessage(message))
				{
					RichTextWidget richTextWidget = new RichTextWidget(base.Context)
					{
						Id = "FormattedLineRichTextWidget",
						WidthSizePolicy = SizePolicy.StretchToParent,
						HeightSizePolicy = SizePolicy.CoverChildren,
						Brush = this.OneLineTextWidget.ReadOnlyBrush,
						MarginTop = -2f,
						MarginBottom = -2f,
						IsEnabled = false,
						Text = chatMultiLineElement.Line,
						MarginLeft = (float)(chatMultiLineElement.IdentModifier * this._defaultMarginLeftPerIndent) * base._inverseScaleToUse,
						ClipContents = false,
						DoNotRenderIfNotFullyInsideScissor = false
					};
					this.CollapsableWidget.AddChild(richTextWidget);
				}
				this.CollapsableWidget.IsVisible = true;
				this.OneLineTextWidget.IsVisible = false;
				return;
			}
			this.OneLineTextWidget.Text = message;
			this.CollapsableWidget.IsVisible = false;
			this.OneLineTextWidget.IsVisible = true;
		}

		private List<ChatLogItemWidget.ChatMultiLineElement> GetFormattedLinesFromMessage(string message)
		{
			List<ChatLogItemWidget.ChatMultiLineElement> list = new List<ChatLogItemWidget.ChatMultiLineElement>();
			XmlDocument xmlDocument = new XmlDocument();
			int num = message.IndexOf(this._detailOpeningTag, StringComparison.Ordinal);
			string text = message.Substring(0, num);
			string text2 = message.Substring(num, message.Length - num);
			text2 = this._detailOpeningTag + text2 + this._detailClosingTag;
			list.Add(new ChatLogItemWidget.ChatMultiLineElement(text, 0));
			try
			{
				xmlDocument.LoadXml(text2);
				this.AddLinesFromXMLRecur(xmlDocument.FirstChild, ref list, 0);
			}
			catch (Exception ex)
			{
				Debug.FailedAssert("Couldn't parse chat log message: " + ex.Message, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Chat\\ChatLogItemWidget.cs", "GetFormattedLinesFromMessage", 111);
			}
			return list;
		}

		private void AddLinesFromXMLRecur(XmlNode currentNode, ref List<ChatLogItemWidget.ChatMultiLineElement> lineList, int currentIndentModifier)
		{
			if (currentNode.NodeType == XmlNodeType.Text)
			{
				lineList.Add(new ChatLogItemWidget.ChatMultiLineElement(currentNode.InnerText, currentIndentModifier));
				for (int i = 0; i < currentNode.ChildNodes.Count; i++)
				{
					this.AddLinesFromXMLRecur(currentNode.ChildNodes.Item(i), ref lineList, currentIndentModifier + 1);
				}
				return;
			}
			for (int j = 0; j < currentNode.ChildNodes.Count; j++)
			{
				this.AddLinesFromXMLRecur(currentNode.ChildNodes.Item(j), ref lineList, currentIndentModifier + 1);
			}
		}

		[Editor(false)]
		public RichTextWidget OneLineTextWidget
		{
			get
			{
				return this._oneLineTextWidget;
			}
			set
			{
				if (this._oneLineTextWidget != value)
				{
					this._oneLineTextWidget = value;
				}
			}
		}

		[Editor(false)]
		public ChatCollapsableListPanel CollapsableWidget
		{
			get
			{
				return this._collapsableWidget;
			}
			set
			{
				if (this._collapsableWidget != value)
				{
					this._collapsableWidget = value;
				}
			}
		}

		[Editor(false)]
		public string ChatLine
		{
			get
			{
				return this._chatLine;
			}
			set
			{
				if (this._chatLine != value)
				{
					this._chatLine = value;
					this.PostMessage(value);
				}
			}
		}

		[Editor(false)]
		public ChatLogWidget ChatLogWidget
		{
			get
			{
				return this._chatLogWidget;
			}
			set
			{
				if (this._chatLogWidget != value)
				{
					this._chatLogWidget = value;
				}
			}
		}

		private int _defaultMarginLeftPerIndent = 20;

		private string _detailOpeningTag = "<Detail>";

		private string _detailClosingTag = "</Detail>";

		private Action<Widget> _fullyInsideAction;

		private ChatLogWidget _chatLogWidget;

		private string _chatLine;

		private RichTextWidget _oneLineTextWidget;

		private ChatCollapsableListPanel _collapsableWidget;

		public struct ChatMultiLineElement
		{
			public ChatMultiLineElement(string line, int identModifier)
			{
				this.Line = line;
				this.IdentModifier = identModifier;
			}

			public string Line;

			public int IdentModifier;
		}
	}
}
