using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.InitialMenu
{
	// Token: 0x020000D6 RID: 214
	public class InitialMenuOptionVM : ViewModel
	{
		// Token: 0x060013E5 RID: 5093 RVA: 0x00041670 File Offset: 0x0003F870
		public InitialMenuOptionVM(InitialStateOption initialStateOption)
		{
			this.InitialStateOption = initialStateOption;
			this.DisabledHint = new HintViewModel(initialStateOption.IsDisabledAndReason().Item2, null);
		}

		// Token: 0x060013E6 RID: 5094 RVA: 0x0004169C File Offset: 0x0003F89C
		public void ExecuteAction()
		{
			InitialState initialState = GameStateManager.Current.ActiveState as InitialState;
			if (initialState != null)
			{
				initialState.OnExecutedInitialStateOption(this.InitialStateOption);
				this.InitialStateOption.DoAction();
			}
		}

		// Token: 0x060013E7 RID: 5095 RVA: 0x000416D3 File Offset: 0x0003F8D3
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DisabledHint.HintText = this.InitialStateOption.IsDisabledAndReason().Item2;
		}

		// Token: 0x1700068D RID: 1677
		// (get) Token: 0x060013E8 RID: 5096 RVA: 0x000416FB File Offset: 0x0003F8FB
		// (set) Token: 0x060013E9 RID: 5097 RVA: 0x00041703 File Offset: 0x0003F903
		[DataSourceProperty]
		public HintViewModel DisabledHint
		{
			get
			{
				return this._disabledHint;
			}
			set
			{
				if (value != this._disabledHint)
				{
					this._disabledHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DisabledHint");
				}
			}
		}

		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x060013EA RID: 5098 RVA: 0x00041721 File Offset: 0x0003F921
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this.InitialStateOption.Name.ToString();
			}
		}

		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x060013EB RID: 5099 RVA: 0x00041733 File Offset: 0x0003F933
		[DataSourceProperty]
		public bool IsDisabled
		{
			get
			{
				return this.InitialStateOption.IsDisabledAndReason().Item1;
			}
		}

		// Token: 0x0400098C RID: 2444
		public readonly InitialStateOption InitialStateOption;

		// Token: 0x0400098D RID: 2445
		private HintViewModel _disabledHint;
	}
}
