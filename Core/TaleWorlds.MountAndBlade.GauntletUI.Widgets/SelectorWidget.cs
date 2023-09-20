using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000035 RID: 53
	public class SelectorWidget : Widget
	{
		// Token: 0x060002FB RID: 763 RVA: 0x00009B37 File Offset: 0x00007D37
		public SelectorWidget(UIContext context)
			: base(context)
		{
			this._listSelectionHandler = new Action<Widget>(this.OnSelectionChanged);
			this._listItemRemovedHandler = new Action<Widget, Widget>(this.OnListChanged);
			this._listItemAddedHandler = new Action<Widget, Widget>(this.OnListChanged);
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00009B76 File Offset: 0x00007D76
		public void OnListChanged(Widget widget)
		{
			this.RefreshSelectedItem();
		}

		// Token: 0x060002FD RID: 765 RVA: 0x00009B7E File Offset: 0x00007D7E
		public void OnListChanged(Widget parentWidget, Widget addedWidget)
		{
			this.RefreshSelectedItem();
		}

		// Token: 0x060002FE RID: 766 RVA: 0x00009B86 File Offset: 0x00007D86
		public void OnSelectionChanged(Widget widget)
		{
			this.CurrentSelectedIndex = this.ListPanelValue;
			this.RefreshSelectedItem();
			base.OnPropertyChanged(this.CurrentSelectedIndex, "CurrentSelectedIndex");
		}

		// Token: 0x060002FF RID: 767 RVA: 0x00009BAB File Offset: 0x00007DAB
		private void RefreshSelectedItem()
		{
			this.ListPanelValue = this.CurrentSelectedIndex;
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000300 RID: 768 RVA: 0x00009BB9 File Offset: 0x00007DB9
		// (set) Token: 0x06000301 RID: 769 RVA: 0x00009BD0 File Offset: 0x00007DD0
		[Editor(false)]
		public int ListPanelValue
		{
			get
			{
				if (this.Container != null)
				{
					return this.Container.IntValue;
				}
				return -1;
			}
			set
			{
				if (this.Container != null && this.Container.IntValue != value)
				{
					this.Container.IntValue = value;
				}
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000302 RID: 770 RVA: 0x00009BF4 File Offset: 0x00007DF4
		// (set) Token: 0x06000303 RID: 771 RVA: 0x00009BFC File Offset: 0x00007DFC
		[Editor(false)]
		public int CurrentSelectedIndex
		{
			get
			{
				return this._currentSelectedIndex;
			}
			set
			{
				if (this._currentSelectedIndex != value && value >= 0)
				{
					this._currentSelectedIndex = value;
					this.RefreshSelectedItem();
				}
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000304 RID: 772 RVA: 0x00009C18 File Offset: 0x00007E18
		// (set) Token: 0x06000305 RID: 773 RVA: 0x00009C20 File Offset: 0x00007E20
		[Editor(false)]
		public Container Container
		{
			get
			{
				return this._container;
			}
			set
			{
				if (this._container != null)
				{
					this._container.SelectEventHandlers.Remove(this._listSelectionHandler);
					this._container.ItemAddEventHandlers.Remove(this._listItemAddedHandler);
					this._container.ItemRemoveEventHandlers.Remove(this._listItemRemovedHandler);
				}
				this._container = value;
				if (this._container != null)
				{
					this._container.SelectEventHandlers.Add(this._listSelectionHandler);
					this._container.ItemAddEventHandlers.Add(this._listItemAddedHandler);
					this._container.ItemRemoveEventHandlers.Add(this._listItemRemovedHandler);
				}
				this.RefreshSelectedItem();
			}
		}

		// Token: 0x0400013B RID: 315
		private int _currentSelectedIndex;

		// Token: 0x0400013C RID: 316
		private Action<Widget> _listSelectionHandler;

		// Token: 0x0400013D RID: 317
		private Action<Widget, Widget> _listItemRemovedHandler;

		// Token: 0x0400013E RID: 318
		private Action<Widget, Widget> _listItemAddedHandler;

		// Token: 0x0400013F RID: 319
		private Container _container;
	}
}
