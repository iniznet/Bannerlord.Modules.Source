using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000010 RID: 16
	public class ContainerPageControlButtonListWidget : ContainerPageControlWidget
	{
		// Token: 0x06000095 RID: 149 RVA: 0x00003672 File Offset: 0x00001872
		public ContainerPageControlButtonListWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00003686 File Offset: 0x00001886
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.RefreshButtonList();
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00003698 File Offset: 0x00001898
		private void RefreshButtonList()
		{
			if (!this._buttonsInitialized)
			{
				this._pageButtonsList = new List<ButtonWidget>();
				if (base.HasChild(this.PageButtonTemplate))
				{
					base.RemoveChild(this.PageButtonTemplate);
				}
				base.RemoveAllChildren();
				if (this.PageButtonItemsListPanel != null && this.PageButtonTemplate != null && this.EmptyButtonBrush != null && this.FullButtonBrush != null)
				{
					this.PageButtonItemsListPanel.RemoveAllChildren();
					if (base.PageCount == 1)
					{
						base.IsVisible = false;
					}
					else
					{
						for (int i = 0; i < base.PageCount; i++)
						{
							ButtonWidget buttonWidget = new ButtonWidget(base.Context);
							this.PageButtonItemsListPanel.AddChild(buttonWidget);
							buttonWidget.Brush = this.PageButtonTemplate.ReadOnlyBrush;
							buttonWidget.DoNotAcceptEvents = false;
							buttonWidget.SuggestedHeight = this.PageButtonTemplate.SuggestedHeight;
							buttonWidget.SuggestedWidth = this.PageButtonTemplate.SuggestedWidth;
							buttonWidget.MarginLeft = this.PageButtonTemplate.MarginLeft;
							buttonWidget.MarginRight = this.PageButtonTemplate.MarginRight;
							buttonWidget.DoNotPassEventsToChildren = this.PageButtonTemplate.DoNotPassEventsToChildren;
							buttonWidget.ClickEventHandlers.Add(new Action<Widget>(this.OnPageSelection));
							this._pageButtonsList.Add(buttonWidget);
						}
						this.UpdatePageButtonBrushes();
					}
					this._buttonsInitialized = true;
				}
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000037F3 File Offset: 0x000019F3
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this._buttonsInitialized = false;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00003802 File Offset: 0x00001A02
		protected override void OnContainerItemsUpdated()
		{
			base.OnContainerItemsUpdated();
			this.UpdatePageButtonBrushes();
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00003810 File Offset: 0x00001A10
		private void OnPageSelection(Widget stageButton)
		{
			int num = this._pageButtonsList.IndexOf(stageButton as ButtonWidget);
			base.GoToPage(num);
			this.UpdatePageButtonBrushes();
		}

		// Token: 0x0600009B RID: 155 RVA: 0x0000383C File Offset: 0x00001A3C
		private void UpdatePageButtonBrushes()
		{
			if (this._pageButtonsList.Count < base.PageCount)
			{
				return;
			}
			for (int i = 0; i < base.PageCount; i++)
			{
				if (i == this._currentPageIndex)
				{
					this._pageButtonsList[i].Brush = base.EventManager.Context.Brushes.First((Brush b) => b.Name == this.FullButtonBrush);
				}
				else
				{
					this._pageButtonsList[i].Brush = base.EventManager.Context.Brushes.First((Brush b) => b.Name == this.EmptyButtonBrush);
				}
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600009C RID: 156 RVA: 0x000038DD File Offset: 0x00001ADD
		// (set) Token: 0x0600009D RID: 157 RVA: 0x000038E5 File Offset: 0x00001AE5
		[Editor(false)]
		public ButtonWidget PageButtonTemplate
		{
			get
			{
				return this._pageButtonTemplate;
			}
			set
			{
				if (value != this._pageButtonTemplate)
				{
					this._pageButtonTemplate = value;
					base.OnPropertyChanged<ButtonWidget>(value, "PageButtonTemplate");
				}
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00003903 File Offset: 0x00001B03
		// (set) Token: 0x0600009F RID: 159 RVA: 0x0000390B File Offset: 0x00001B0B
		[Editor(false)]
		public string FullButtonBrush
		{
			get
			{
				return this._fullButtonBrush;
			}
			set
			{
				if (this._fullButtonBrush != value)
				{
					this._fullButtonBrush = value;
					base.OnPropertyChanged<string>(value, "FullButtonBrush");
				}
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x0000392E File Offset: 0x00001B2E
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x00003936 File Offset: 0x00001B36
		[Editor(false)]
		public string EmptyButtonBrush
		{
			get
			{
				return this._emptyButtonBrush;
			}
			set
			{
				if (this._emptyButtonBrush != value)
				{
					this._emptyButtonBrush = value;
					base.OnPropertyChanged<string>(value, "EmptyButtonBrush");
				}
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x00003959 File Offset: 0x00001B59
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x00003961 File Offset: 0x00001B61
		[Editor(false)]
		public ListPanel PageButtonItemsListPanel
		{
			get
			{
				return this._pageButtonItemsListPanel;
			}
			set
			{
				if (value != this._pageButtonItemsListPanel)
				{
					this._pageButtonItemsListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "PageButtonItemsListPanel");
				}
			}
		}

		// Token: 0x0400004A RID: 74
		private List<ButtonWidget> _pageButtonsList = new List<ButtonWidget>();

		// Token: 0x0400004B RID: 75
		private bool _buttonsInitialized;

		// Token: 0x0400004C RID: 76
		private ButtonWidget _pageButtonTemplate;

		// Token: 0x0400004D RID: 77
		private ListPanel _pageButtonItemsListPanel;

		// Token: 0x0400004E RID: 78
		private string _fullButtonBrush;

		// Token: 0x0400004F RID: 79
		private string _emptyButtonBrush;
	}
}
