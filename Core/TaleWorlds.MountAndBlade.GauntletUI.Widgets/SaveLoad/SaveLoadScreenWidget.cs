using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.SaveLoad
{
	// Token: 0x02000051 RID: 81
	public class SaveLoadScreenWidget : Widget
	{
		// Token: 0x06000443 RID: 1091 RVA: 0x0000D870 File Offset: 0x0000BA70
		public SaveLoadScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000444 RID: 1092 RVA: 0x0000D879 File Offset: 0x0000BA79
		public void SetCurrentSaveTuple(SavedGameTupleButtonWidget tuple)
		{
			if (tuple != null)
			{
				this.LoadButton.IsVisible = true;
				this._currentSelectedTuple = tuple;
				return;
			}
			this.LoadButton.IsEnabled = false;
			this._currentSelectedTuple = null;
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000445 RID: 1093 RVA: 0x0000D8A5 File Offset: 0x0000BAA5
		// (set) Token: 0x06000446 RID: 1094 RVA: 0x0000D8AD File Offset: 0x0000BAAD
		[Editor(false)]
		public ButtonWidget LoadButton
		{
			get
			{
				return this._loadButton;
			}
			set
			{
				if (this._loadButton != value)
				{
					this._loadButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "LoadButton");
				}
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000447 RID: 1095 RVA: 0x0000D8CB File Offset: 0x0000BACB
		// (set) Token: 0x06000448 RID: 1096 RVA: 0x0000D8D3 File Offset: 0x0000BAD3
		[Editor(false)]
		public bool IsSaving
		{
			get
			{
				return this._isSaving;
			}
			set
			{
				if (this._isSaving != value)
				{
					this._isSaving = value;
					base.OnPropertyChanged(value, "IsSaving");
				}
			}
		}

		// Token: 0x040001D9 RID: 473
		private SavedGameTupleButtonWidget _currentSelectedTuple;

		// Token: 0x040001DA RID: 474
		private ButtonWidget _loadButton;

		// Token: 0x040001DB RID: 475
		private bool _isSaving;
	}
}
