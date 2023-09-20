using System;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	// Token: 0x020000F9 RID: 249
	public class MPOptionsVM : OptionsVM
	{
		// Token: 0x060015F8 RID: 5624 RVA: 0x000469FF File Offset: 0x00044BFF
		public MPOptionsVM(bool autoHandleClose, Action onChangeBrightnessRequest, Action onChangeExposureRequest, Action<KeyOptionVM> onKeybindRequest)
			: base(autoHandleClose, OptionsVM.OptionsMode.Multiplayer, onKeybindRequest, onChangeBrightnessRequest, onChangeExposureRequest)
		{
			this.RefreshValues();
		}

		// Token: 0x060015F9 RID: 5625 RVA: 0x00046A35 File Offset: 0x00044C35
		public MPOptionsVM(Action onClose, Action<KeyOptionVM> onKeybindRequest)
			: base(OptionsVM.OptionsMode.Multiplayer, onClose, onKeybindRequest, null, null)
		{
			this.RefreshValues();
		}

		// Token: 0x060015FA RID: 5626 RVA: 0x00046A6A File Offset: 0x00044C6A
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ApplyText = new TextObject("{=BAaS5Dkc}Apply", null).ToString();
			this.RevertText = new TextObject("{=Npqlj5Ln}Revert Changes", null).ToString();
		}

		// Token: 0x060015FB RID: 5627 RVA: 0x00046A9E File Offset: 0x00044C9E
		public new void ExecuteCancel()
		{
			base.ExecuteCancel();
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x00046AA8 File Offset: 0x00044CA8
		public void ExecuteApply()
		{
			bool flag = base.IsOptionsChanged();
			base.OnDone();
			foreach (GenericOptionDataVM genericOptionDataVM in this._groupedCategories.SelectMany((GroupedOptionCategoryVM c) => c.AllOptions).Concat(this._performanceOptionCategory.AllOptions))
			{
				genericOptionDataVM.ApplyValue();
			}
			InformationManager.DisplayMessage(new InformationMessage(flag ? this._changesAppliedTextObject.ToString() : this._noChangesMadeTextObject.ToString()));
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x00046B58 File Offset: 0x00044D58
		public void ForceCancel()
		{
			base.HandleCancel(false);
		}

		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x060015FE RID: 5630 RVA: 0x00046B61 File Offset: 0x00044D61
		// (set) Token: 0x060015FF RID: 5631 RVA: 0x00046B69 File Offset: 0x00044D69
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x06001600 RID: 5632 RVA: 0x00046B87 File Offset: 0x00044D87
		// (set) Token: 0x06001601 RID: 5633 RVA: 0x00046B8F File Offset: 0x00044D8F
		[DataSourceProperty]
		public string ApplyText
		{
			get
			{
				return this._applyText;
			}
			set
			{
				if (value != this._applyText)
				{
					this._applyText = value;
					base.OnPropertyChangedWithValue<string>(value, "ApplyText");
				}
			}
		}

		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x06001602 RID: 5634 RVA: 0x00046BB2 File Offset: 0x00044DB2
		// (set) Token: 0x06001603 RID: 5635 RVA: 0x00046BBA File Offset: 0x00044DBA
		[DataSourceProperty]
		public string RevertText
		{
			get
			{
				return this._revertText;
			}
			set
			{
				if (value != this._revertText)
				{
					this._revertText = value;
					base.OnPropertyChangedWithValue<string>(value, "RevertText");
				}
			}
		}

		// Token: 0x04000A76 RID: 2678
		private TextObject _changesAppliedTextObject = new TextObject("{=SfsnlbyK}Changes applied.", null);

		// Token: 0x04000A77 RID: 2679
		private TextObject _noChangesMadeTextObject = new TextObject("{=jS5rrX8M}There are no changes to apply.", null);

		// Token: 0x04000A78 RID: 2680
		private bool _isEnabled;

		// Token: 0x04000A79 RID: 2681
		private string _applyText;

		// Token: 0x04000A7A RID: 2682
		private string _revertText;
	}
}
