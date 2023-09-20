using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Armory
{
	public class MPArmoryCosmeticItemVM : ViewModel
	{
		public string UnequipText { get; private set; }

		public MPArmoryCosmeticItemVM(CosmeticsManager.CosmeticElement cosmetic, string cosmeticID, Action<MPArmoryCosmeticItemVM> onEquipped, Action<MPArmoryCosmeticItemVM> onPurchaseRequested, Action<MPArmoryCosmeticItemVM> onPreviewed)
		{
			this.Cosmetic = cosmetic;
			this.CosmeticID = cosmeticID;
			this._onEquipped = onEquipped;
			this._onPurchaseRequested = onPurchaseRequested;
			this._onPreviewed = onPreviewed;
			ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(cosmetic.Id);
			this.EquipmentElement = new EquipmentElement(@object, null, null, false);
			this.Icon = new ImageIdentifierVM(@object, "");
			this.Cost = cosmetic.Cost;
			this.Rarity = (int)cosmetic.Rarity;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.EquipmentElement.Item.Name.ToString();
			this.OwnedText = new TextObject("{=B5bcj3pC}Owned", null).ToString();
			this.PreviewText = new TextObject("{=un7poy9x}Preview", null).ToString();
			this.UnequipText = new TextObject("{=QndVFTbx}Unequip", null).ToString();
			this.IsUnlockedUpdated();
		}

		public override void OnFinalize()
		{
			InputKeyItemVM actionKey = this.ActionKey;
			if (actionKey != null)
			{
				actionKey.OnFinalize();
			}
			InputKeyItemVM previewKey = this.PreviewKey;
			if (previewKey == null)
			{
				return;
			}
			previewKey.OnFinalize();
		}

		private void ExecuteAction()
		{
			if (this.IsUnlocked)
			{
				Action<MPArmoryCosmeticItemVM> onEquipped = this._onEquipped;
				if (onEquipped != null)
				{
					onEquipped(this);
				}
				SoundEvent.PlaySound2D("event:/ui/inventory/helmet");
				return;
			}
			this._onPurchaseRequested(this);
			SoundEvent.PlaySound2D("event:/ui/multiplayer/click_item");
		}

		private void ExecutePreview()
		{
			this._onPreviewed(this);
		}

		private void ExecuteEnableActions()
		{
			this.AreActionsEnabled = true;
		}

		private void ExecuteDisableActions()
		{
			this.AreActionsEnabled = false;
		}

		public void IsUnlockedUpdated()
		{
			this.ActionText = (this.IsUnlocked ? new TextObject("{=DKqLY1aJ}Equip", null).ToString() : new TextObject("{=i2mNBaxE}Obtain", null).ToString());
		}

		public void RefreshKeyBindings(HotKey actionKey, HotKey previewKey)
		{
			this.ActionKey = InputKeyItemVM.CreateFromHotKey(actionKey, false);
			this.PreviewKey = InputKeyItemVM.CreateFromHotKey(previewKey, false);
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
					this.IsUnlockedUpdated();
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
		public bool AreActionsEnabled
		{
			get
			{
				return this._areActionsEnabled;
			}
			set
			{
				if (value != this._areActionsEnabled)
				{
					this._areActionsEnabled = value;
					base.OnPropertyChangedWithValue(value, "AreActionsEnabled");
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

		[DataSourceProperty]
		public string OwnedText
		{
			get
			{
				return this._ownedText;
			}
			set
			{
				if (value != this._ownedText)
				{
					this._ownedText = value;
					base.OnPropertyChangedWithValue<string>(value, "OwnedText");
				}
			}
		}

		[DataSourceProperty]
		public string ActionText
		{
			get
			{
				return this._actionText;
			}
			set
			{
				if (value != this._actionText)
				{
					this._actionText = value;
					base.OnPropertyChangedWithValue<string>(value, "ActionText");
				}
			}
		}

		[DataSourceProperty]
		public string PreviewText
		{
			get
			{
				return this._previewText;
			}
			set
			{
				if (value != this._previewText)
				{
					this._previewText = value;
					base.OnPropertyChangedWithValue<string>(value, "PreviewText");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM Icon
		{
			get
			{
				return this._icon;
			}
			set
			{
				if (value != this._icon)
				{
					this._icon = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Icon");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM ActionKey
		{
			get
			{
				return this._actionKey;
			}
			set
			{
				if (value != this._actionKey)
				{
					this._actionKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ActionKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM PreviewKey
		{
			get
			{
				return this._previewKey;
			}
			set
			{
				if (value != this._previewKey)
				{
					this._previewKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "PreviewKey");
				}
			}
		}

		public readonly CosmeticsManager.CosmeticElement Cosmetic;

		public readonly EquipmentElement EquipmentElement;

		public readonly string CosmeticID;

		private readonly Action<MPArmoryCosmeticItemVM> _onEquipped;

		private readonly Action<MPArmoryCosmeticItemVM> _onPurchaseRequested;

		private readonly Action<MPArmoryCosmeticItemVM> _onPreviewed;

		private bool _isUnlocked;

		private bool _isUsed;

		private bool _areActionsEnabled;

		private int _cost;

		private int _rarity;

		private string _name;

		private string _ownedText;

		private string _actionText;

		private string _previewText;

		private ImageIdentifierVM _icon;

		private InputKeyItemVM _actionKey;

		private InputKeyItemVM _previewKey;
	}
}
