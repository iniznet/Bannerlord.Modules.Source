using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	public class MPLobbyCosmeticSigilItemVM : MPLobbySigilItemVM
	{
		public MPLobbyCosmeticSigilItemVM(int iconID, Action<MPLobbyCosmeticSigilItemVM> onSelection, Action<MPLobbyCosmeticSigilItemVM> onObtainRequested, int rarity, int cost, string cosmeticID)
			: base(iconID, null)
		{
			this._onSelection = onSelection;
			this._onObtainRequested = onObtainRequested;
			this.Rarity = rarity;
			this.Cost = cost;
			this.CosmeticID = cosmeticID;
		}

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

		public readonly string CosmeticID;

		private Action<MPLobbyCosmeticSigilItemVM> _onSelection;

		private Action<MPLobbyCosmeticSigilItemVM> _onObtainRequested;

		private bool _isUnlocked;

		private bool _isUsed;

		private int _rarity;

		private int _cost;
	}
}
