using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	// Token: 0x0200005A RID: 90
	public class MPLobbyCosmeticSigilItemVM : MPLobbySigilItemVM
	{
		// Token: 0x060007A1 RID: 1953 RVA: 0x0001DFF6 File Offset: 0x0001C1F6
		public MPLobbyCosmeticSigilItemVM(int iconID, Action<MPLobbyCosmeticSigilItemVM> onSelection, Action<MPLobbyCosmeticSigilItemVM> onObtainRequested, int rarity, int cost, string cosmeticID)
			: base(iconID, null)
		{
			this._onSelection = onSelection;
			this._onObtainRequested = onObtainRequested;
			this.Rarity = rarity;
			this.Cost = cost;
			this.CosmeticID = cosmeticID;
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x0001E026 File Offset: 0x0001C226
		private void ExecuteSelection()
		{
			if (this.IsUnlocked)
			{
				Action<MPLobbyCosmeticSigilItemVM> onSelection = this._onSelection;
				if (onSelection == null)
				{
					return;
				}
				onSelection(this);
				return;
			}
			else
			{
				Action<MPLobbyCosmeticSigilItemVM> onObtainRequested = this._onObtainRequested;
				if (onObtainRequested == null)
				{
					return;
				}
				onObtainRequested(this);
				return;
			}
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x060007A3 RID: 1955 RVA: 0x0001E053 File Offset: 0x0001C253
		// (set) Token: 0x060007A4 RID: 1956 RVA: 0x0001E05B File Offset: 0x0001C25B
		[DataSourceProperty]
		public bool IsUnlocked
		{
			get
			{
				return this._isUnlocked;
			}
			set
			{
				if (value != this._isUnlocked)
				{
					this._isUnlocked = value;
					base.OnPropertyChangedWithValue(value, "IsUnlocked");
				}
			}
		}

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x060007A5 RID: 1957 RVA: 0x0001E079 File Offset: 0x0001C279
		// (set) Token: 0x060007A6 RID: 1958 RVA: 0x0001E081 File Offset: 0x0001C281
		[DataSourceProperty]
		public bool IsUsed
		{
			get
			{
				return this._isUsed;
			}
			set
			{
				if (value != this._isUsed)
				{
					this._isUsed = value;
					base.OnPropertyChangedWithValue(value, "IsUsed");
				}
			}
		}

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x060007A7 RID: 1959 RVA: 0x0001E09F File Offset: 0x0001C29F
		// (set) Token: 0x060007A8 RID: 1960 RVA: 0x0001E0A7 File Offset: 0x0001C2A7
		[DataSourceProperty]
		public int Rarity
		{
			get
			{
				return this._rarity;
			}
			set
			{
				if (value != this._rarity)
				{
					this._rarity = value;
					base.OnPropertyChangedWithValue(value, "Rarity");
				}
			}
		}

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x060007A9 RID: 1961 RVA: 0x0001E0C5 File Offset: 0x0001C2C5
		// (set) Token: 0x060007AA RID: 1962 RVA: 0x0001E0CD File Offset: 0x0001C2CD
		[DataSourceProperty]
		public int Cost
		{
			get
			{
				return this._cost;
			}
			set
			{
				if (value != this._cost)
				{
					this._cost = value;
					base.OnPropertyChangedWithValue(value, "Cost");
				}
			}
		}

		// Token: 0x040003E1 RID: 993
		public readonly string CosmeticID;

		// Token: 0x040003E2 RID: 994
		private Action<MPLobbyCosmeticSigilItemVM> _onSelection;

		// Token: 0x040003E3 RID: 995
		private Action<MPLobbyCosmeticSigilItemVM> _onObtainRequested;

		// Token: 0x040003E4 RID: 996
		private bool _isUnlocked;

		// Token: 0x040003E5 RID: 997
		private bool _isUsed;

		// Token: 0x040003E6 RID: 998
		private int _rarity;

		// Token: 0x040003E7 RID: 999
		private int _cost;
	}
}
