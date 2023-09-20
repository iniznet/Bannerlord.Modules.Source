using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	// Token: 0x020000CC RID: 204
	public class MPPerkVM : ViewModel
	{
		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x0600131A RID: 4890 RVA: 0x0003E9CA File Offset: 0x0003CBCA
		// (set) Token: 0x0600131B RID: 4891 RVA: 0x0003E9D2 File Offset: 0x0003CBD2
		public int PerkIndex { get; private set; }

		// Token: 0x0600131C RID: 4892 RVA: 0x0003E9DB File Offset: 0x0003CBDB
		public MPPerkVM(Action<MPPerkVM> onSelectPerk, IReadOnlyPerkObject perk, bool isSelectable, int perkIndex)
		{
			this.Perk = perk;
			this.PerkIndex = perkIndex;
			this._onSelectPerk = onSelectPerk;
			this.IconType = perk.IconId;
			this.IsSelectable = isSelectable;
			this.RefreshValues();
		}

		// Token: 0x0600131D RID: 4893 RVA: 0x0003EA14 File Offset: 0x0003CC14
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.Perk.Name.ToString();
			this.Description = this.Perk.Description.ToString();
			GameTexts.SetVariable("newline", "\n");
			this.Hint = new HintViewModel(this.Perk.Description, null);
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x0003EA79 File Offset: 0x0003CC79
		public void ExecuteSelectPerk()
		{
			this._onSelectPerk(this);
		}

		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x0600131F RID: 4895 RVA: 0x0003EA87 File Offset: 0x0003CC87
		// (set) Token: 0x06001320 RID: 4896 RVA: 0x0003EA8F File Offset: 0x0003CC8F
		[DataSourceProperty]
		public string IconType
		{
			get
			{
				return this._iconType;
			}
			set
			{
				if (value != this._iconType)
				{
					this._iconType = value;
					base.OnPropertyChangedWithValue<string>(value, "IconType");
				}
			}
		}

		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x06001321 RID: 4897 RVA: 0x0003EAB2 File Offset: 0x0003CCB2
		// (set) Token: 0x06001322 RID: 4898 RVA: 0x0003EABA File Offset: 0x0003CCBA
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
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x06001323 RID: 4899 RVA: 0x0003EAD8 File Offset: 0x0003CCD8
		// (set) Token: 0x06001324 RID: 4900 RVA: 0x0003EAE0 File Offset: 0x0003CCE0
		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		// Token: 0x1700064F RID: 1615
		// (get) Token: 0x06001325 RID: 4901 RVA: 0x0003EB03 File Offset: 0x0003CD03
		// (set) Token: 0x06001326 RID: 4902 RVA: 0x0003EB0B File Offset: 0x0003CD0B
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x06001327 RID: 4903 RVA: 0x0003EB2E File Offset: 0x0003CD2E
		// (set) Token: 0x06001328 RID: 4904 RVA: 0x0003EB36 File Offset: 0x0003CD36
		[DataSourceProperty]
		public bool IsSelectable
		{
			get
			{
				return this._isSelectable;
			}
			set
			{
				if (value != this._isSelectable)
				{
					this._isSelectable = value;
					base.OnPropertyChangedWithValue(value, "IsSelectable");
				}
			}
		}

		// Token: 0x0400092C RID: 2348
		public readonly IReadOnlyPerkObject Perk;

		// Token: 0x0400092D RID: 2349
		private readonly Action<MPPerkVM> _onSelectPerk;

		// Token: 0x0400092F RID: 2351
		private string _iconType;

		// Token: 0x04000930 RID: 2352
		private string _name;

		// Token: 0x04000931 RID: 2353
		private string _description;

		// Token: 0x04000932 RID: 2354
		private bool _isSelectable;

		// Token: 0x04000933 RID: 2355
		private HintViewModel _hint;
	}
}
