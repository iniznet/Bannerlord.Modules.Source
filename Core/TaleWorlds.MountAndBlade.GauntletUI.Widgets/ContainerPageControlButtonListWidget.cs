using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ContainerPageControlButtonListWidget : ContainerPageControlWidget
	{
		public ContainerPageControlButtonListWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.RefreshButtonList();
		}

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

		protected override void OnInitialized()
		{
			base.OnInitialized();
			this._buttonsInitialized = false;
		}

		protected override void OnContainerItemsUpdated()
		{
			base.OnContainerItemsUpdated();
			this.UpdatePageButtonBrushes();
		}

		private void OnPageSelection(Widget stageButton)
		{
			int num = this._pageButtonsList.IndexOf(stageButton as ButtonWidget);
			base.GoToPage(num);
			this.UpdatePageButtonBrushes();
		}

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

		private List<ButtonWidget> _pageButtonsList = new List<ButtonWidget>();

		private bool _buttonsInitialized;

		private ButtonWidget _pageButtonTemplate;

		private ListPanel _pageButtonItemsListPanel;

		private string _fullButtonBrush;

		private string _emptyButtonBrush;
	}
}
