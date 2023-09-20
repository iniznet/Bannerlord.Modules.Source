using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Chat
{
	// Token: 0x0200015A RID: 346
	public class ChatLogItemWidget : Widget
	{
		// Token: 0x060011C4 RID: 4548 RVA: 0x00030F17 File Offset: 0x0002F117
		public ChatLogItemWidget(UIContext context)
			: base(context)
		{
			this._fullyInsideAction = new Action<Widget>(this.UpdateWidgetFullyInside);
		}

		// Token: 0x060011C5 RID: 4549 RVA: 0x00030F50 File Offset: 0x0002F150
		private void UpdateWidgetFullyInside(Widget widget)
		{
			widget.DoNotRenderIfNotFullyInsideScissor = false;
		}

		// Token: 0x060011C6 RID: 4550 RVA: 0x00030F59 File Offset: 0x0002F159
		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			base.ApplyActionOnAllChildren(this._fullyInsideAction);
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x00030F70 File Offset: 0x0002F170
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

		// Token: 0x060011C8 RID: 4552 RVA: 0x000310A8 File Offset: 0x0002F2A8
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

		// Token: 0x060011C9 RID: 4553 RVA: 0x0003115C File Offset: 0x0002F35C
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

		// Token: 0x17000644 RID: 1604
		// (get) Token: 0x060011CA RID: 4554 RVA: 0x000311DE File Offset: 0x0002F3DE
		// (set) Token: 0x060011CB RID: 4555 RVA: 0x000311E6 File Offset: 0x0002F3E6
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

		// Token: 0x17000645 RID: 1605
		// (get) Token: 0x060011CC RID: 4556 RVA: 0x000311F8 File Offset: 0x0002F3F8
		// (set) Token: 0x060011CD RID: 4557 RVA: 0x00031200 File Offset: 0x0002F400
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

		// Token: 0x17000646 RID: 1606
		// (get) Token: 0x060011CE RID: 4558 RVA: 0x00031212 File Offset: 0x0002F412
		// (set) Token: 0x060011CF RID: 4559 RVA: 0x0003121A File Offset: 0x0002F41A
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

		// Token: 0x17000647 RID: 1607
		// (get) Token: 0x060011D0 RID: 4560 RVA: 0x00031238 File Offset: 0x0002F438
		// (set) Token: 0x060011D1 RID: 4561 RVA: 0x00031240 File Offset: 0x0002F440
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

		// Token: 0x0400081A RID: 2074
		private int _defaultMarginLeftPerIndent = 20;

		// Token: 0x0400081B RID: 2075
		private string _detailOpeningTag = "<Detail>";

		// Token: 0x0400081C RID: 2076
		private string _detailClosingTag = "</Detail>";

		// Token: 0x0400081D RID: 2077
		private Action<Widget> _fullyInsideAction;

		// Token: 0x0400081E RID: 2078
		private ChatLogWidget _chatLogWidget;

		// Token: 0x0400081F RID: 2079
		private string _chatLine;

		// Token: 0x04000820 RID: 2080
		private RichTextWidget _oneLineTextWidget;

		// Token: 0x04000821 RID: 2081
		private ChatCollapsableListPanel _collapsableWidget;

		// Token: 0x020001A0 RID: 416
		public struct ChatMultiLineElement
		{
			// Token: 0x06001332 RID: 4914 RVA: 0x000345CB File Offset: 0x000327CB
			public ChatMultiLineElement(string line, int identModifier)
			{
				this.Line = line;
				this.IdentModifier = identModifier;
			}

			// Token: 0x0400093A RID: 2362
			public string Line;

			// Token: 0x0400093B RID: 2363
			public int IdentModifier;
		}
	}
}
