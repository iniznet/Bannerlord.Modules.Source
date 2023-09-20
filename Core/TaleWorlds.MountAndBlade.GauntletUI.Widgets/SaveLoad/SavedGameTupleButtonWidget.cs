using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.SaveLoad
{
	// Token: 0x0200004E RID: 78
	public class SavedGameTupleButtonWidget : ButtonWidget
	{
		// Token: 0x0600042E RID: 1070 RVA: 0x0000D695 File Offset: 0x0000B895
		public SavedGameTupleButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x0000D69E File Offset: 0x0000B89E
		protected override void OnClick()
		{
			base.OnClick();
			if (this.ScreenWidget != null)
			{
				this.ScreenWidget.SetCurrentSaveTuple(this);
			}
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x0000D6BA File Offset: 0x0000B8BA
		private void OnSaveDeletion(Widget widget)
		{
			this.ScreenWidget.SetCurrentSaveTuple(null);
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000431 RID: 1073 RVA: 0x0000D6C8 File Offset: 0x0000B8C8
		// (set) Token: 0x06000432 RID: 1074 RVA: 0x0000D6D0 File Offset: 0x0000B8D0
		[Editor(false)]
		public SaveLoadScreenWidget ScreenWidget
		{
			get
			{
				return this._screenWidget;
			}
			set
			{
				if (this._screenWidget != value)
				{
					this._screenWidget = value;
					base.OnPropertyChanged<SaveLoadScreenWidget>(value, "ScreenWidget");
				}
			}
		}

		// Token: 0x040001D3 RID: 467
		private SaveLoadScreenWidget _screenWidget;
	}
}
