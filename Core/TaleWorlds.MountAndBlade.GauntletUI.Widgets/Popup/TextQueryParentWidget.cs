using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Popup
{
	// Token: 0x02000056 RID: 86
	public class TextQueryParentWidget : Widget
	{
		// Token: 0x0600047F RID: 1151 RVA: 0x0000E005 File Offset: 0x0000C205
		public TextQueryParentWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x0000E00E File Offset: 0x0000C20E
		private void FocusOnTextQuery()
		{
			base.EventManager.SetWidgetFocused(this.TextInputWidget, true);
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x0000E022 File Offset: 0x0000C222
		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			if (this.TextInputWidget != null)
			{
				this.FocusOnTextQuery();
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000482 RID: 1154 RVA: 0x0000E038 File Offset: 0x0000C238
		// (set) Token: 0x06000483 RID: 1155 RVA: 0x0000E040 File Offset: 0x0000C240
		[Editor(false)]
		public EditableTextWidget TextInputWidget
		{
			get
			{
				return this._editableTextWidget;
			}
			set
			{
				if (value != this._editableTextWidget)
				{
					this._editableTextWidget = value;
					this.FocusOnTextQuery();
				}
			}
		}

		// Token: 0x040001F7 RID: 503
		private EditableTextWidget _editableTextWidget;
	}
}
