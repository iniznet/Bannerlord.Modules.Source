using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby
{
	public class MPLobbyCosmeticSigilItemVM : MPLobbySigilItemVM
	{
		public MPLobbyCosmeticSigilItemVM(int iconID, int rarity, int cost, string cosmeticID)
			: base(iconID, null)
		{
			this.Rarity = rarity;
			this.Cost = cost;
			this.CosmeticID = cosmeticID;
		}

		public static void SetOnSelectionCallback(Action<MPLobbyCosmeticSigilItemVM> onSelection)
		{
			MPLobbyCosmeticSigilItemVM._onSelection = onSelection;
		}

		public static void ResetOnSelectionCallback()
		{
			MPLobbyCosmeticSigilItemVM._onSelection = null;
		}

		public static void SetOnObtainRequestedCallback(Action<MPLobbyCosmeticSigilItemVM> onObtainRequested)
		{
			MPLobbyCosmeticSigilItemVM._onObtainRequested = onObtainRequested;
		}

		public static void ResetOnObtainRequestedCallback()
		{
			MPLobbyCosmeticSigilItemVM._onObtainRequested = null;
		}

		private void ExecuteSelection()
		{
			if (this.IsUnlocked)
			{
				Action<MPLobbyCosmeticSigilItemVM> onSelection = MPLobbyCosmeticSigilItemVM._onSelection;
				if (onSelection == null)
				{
					return;
				}
				onSelection(this);
				return;
			}
			else
			{
				Action<MPLobbyCosmeticSigilItemVM> onObtainRequested = MPLobbyCosmeticSigilItemVM._onObtainRequested;
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

		private static Action<MPLobbyCosmeticSigilItemVM> _onSelection;

		private static Action<MPLobbyCosmeticSigilItemVM> _onObtainRequested;

		private bool _isUnlocked;

		private bool _isUsed;

		private int _rarity;

		private int _cost;
	}
}
