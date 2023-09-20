using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x0200003A RID: 58
	public class MPCultureItemVM : ViewModel
	{
		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000517 RID: 1303 RVA: 0x000165E0 File Offset: 0x000147E0
		// (set) Token: 0x06000518 RID: 1304 RVA: 0x000165E8 File Offset: 0x000147E8
		public BasicCultureObject Culture { get; private set; }

		// Token: 0x06000519 RID: 1305 RVA: 0x000165F1 File Offset: 0x000147F1
		public MPCultureItemVM(string cultureCode, Action<MPCultureItemVM> onSelection)
		{
			this._onSelection = onSelection;
			this.CultureCode = cultureCode;
			this.Culture = MBObjectManager.Instance.GetObject<BasicCultureObject>(cultureCode);
			this.RefreshValues();
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x0001661E File Offset: 0x0001481E
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Hint = new HintViewModel(this.Culture.Name, null);
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x0001663D File Offset: 0x0001483D
		private void ExecuteSelection()
		{
			this._onSelection(this);
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x0600051C RID: 1308 RVA: 0x0001664B File Offset: 0x0001484B
		// (set) Token: 0x0600051D RID: 1309 RVA: 0x00016653 File Offset: 0x00014853
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChanged("IsSelected");
				}
			}
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x0600051E RID: 1310 RVA: 0x00016670 File Offset: 0x00014870
		// (set) Token: 0x0600051F RID: 1311 RVA: 0x00016678 File Offset: 0x00014878
		[DataSourceProperty]
		public string CultureCode
		{
			get
			{
				return this._cultureCode;
			}
			set
			{
				if (value != this._cultureCode)
				{
					this._cultureCode = value;
					base.OnPropertyChanged("CultureCode");
				}
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06000520 RID: 1312 RVA: 0x0001669A File Offset: 0x0001489A
		// (set) Token: 0x06000521 RID: 1313 RVA: 0x000166A2 File Offset: 0x000148A2
		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChanged("Hint");
				}
			}
		}

		// Token: 0x04000292 RID: 658
		private Action<MPCultureItemVM> _onSelection;

		// Token: 0x04000294 RID: 660
		private bool _isSelected;

		// Token: 0x04000295 RID: 661
		private string _cultureCode;

		// Token: 0x04000296 RID: 662
		private HintViewModel _hint;
	}
}
