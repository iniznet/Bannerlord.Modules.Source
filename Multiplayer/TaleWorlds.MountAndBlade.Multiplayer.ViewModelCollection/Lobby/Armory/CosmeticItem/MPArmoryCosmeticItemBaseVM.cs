using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticItem
{
	public abstract class MPArmoryCosmeticItemBaseVM : ViewModel
	{
		public static event Action<MPArmoryCosmeticItemBaseVM> OnEquipped;

		public static event Action<MPArmoryCosmeticItemBaseVM> OnPurchaseRequested;

		public static event Action<MPArmoryCosmeticItemBaseVM> OnPreviewed;

		public string UnequipText { get; private set; }

		public CosmeticsManager.CosmeticType CosmeticType { get; }

		public MPArmoryCosmeticItemBaseVM(CosmeticElement cosmetic, string cosmeticID, CosmeticsManager.CosmeticType cosmeticType)
		{
			this.Cosmetic = cosmetic;
			this.CosmeticID = cosmeticID;
			this.Cost = cosmetic.Cost;
			this.Rarity = cosmetic.Rarity;
			this.CosmeticType = cosmeticType;
			this.IsUnequippable = true;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.OwnedText = new TextObject("{=B5bcj3pC}Owned", null).ToString();
			this.UnequipText = new TextObject("{=QndVFTbx}Unequip", null).ToString();
			this.UpdatePreviewAndActionTexts();
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

		public void ExecuteAction()
		{
			if (!this.IsUnlocked)
			{
				MPArmoryCosmeticItemBaseVM.OnPurchaseRequested(this);
				return;
			}
			Action<MPArmoryCosmeticItemBaseVM> onEquipped = MPArmoryCosmeticItemBaseVM.OnEquipped;
			if (onEquipped == null)
			{
				return;
			}
			onEquipped(this);
		}

		public void ExecutePreview()
		{
			MPArmoryCosmeticItemBaseVM.OnPreviewed(this);
		}

		public void ExecuteEnableActions()
		{
			this.AreActionsEnabled = true;
		}

		public void ExecuteDisableActions()
		{
			this.AreActionsEnabled = false;
		}

		protected void UpdatePreviewAndActionTexts()
		{
			if (this.IsUnlocked)
			{
				if (this.IsUsed)
				{
					this.ActionText = (this.IsUnequippable ? this.UnequipText : string.Empty);
				}
				else
				{
					this.ActionText = new TextObject("{=DKqLY1aJ}Equip", null).ToString();
				}
			}
			else
			{
				this.ActionText = new TextObject("{=i2mNBaxE}Obtain", null).ToString();
			}
			this.PreviewText = new TextObject("{=un7poy9x}Preview", null).ToString();
		}

		public void RefreshKeyBindings(HotKey actionKey, HotKey previewKey)
		{
			if (this.IsUnlocked && this.IsUsed && !this.IsUnequippable)
			{
				this.ActionKey = InputKeyItemVM.CreateFromHotKey(null, false);
			}
			else
			{
				string groupId = actionKey.GroupId;
				InputKeyItemVM actionKey2 = this.ActionKey;
				string text;
				if (actionKey2 == null)
				{
					text = null;
				}
				else
				{
					HotKey hotKey = actionKey2.HotKey;
					text = ((hotKey != null) ? hotKey.GroupId : null);
				}
				if (!(groupId != text))
				{
					string id = actionKey.Id;
					InputKeyItemVM actionKey3 = this.ActionKey;
					string text2;
					if (actionKey3 == null)
					{
						text2 = null;
					}
					else
					{
						HotKey hotKey2 = actionKey3.HotKey;
						text2 = ((hotKey2 != null) ? hotKey2.Id : null);
					}
					if (!(id != text2))
					{
						goto IL_8A;
					}
				}
				this.ActionKey = InputKeyItemVM.CreateFromHotKey(actionKey, false);
			}
			IL_8A:
			string groupId2 = previewKey.GroupId;
			InputKeyItemVM previewKey2 = this.PreviewKey;
			string text3;
			if (previewKey2 == null)
			{
				text3 = null;
			}
			else
			{
				HotKey hotKey3 = previewKey2.HotKey;
				text3 = ((hotKey3 != null) ? hotKey3.GroupId : null);
			}
			if (!(groupId2 != text3))
			{
				string id2 = previewKey.Id;
				InputKeyItemVM previewKey3 = this.PreviewKey;
				string text4;
				if (previewKey3 == null)
				{
					text4 = null;
				}
				else
				{
					HotKey hotKey4 = previewKey3.HotKey;
					text4 = ((hotKey4 != null) ? hotKey4.Id : null);
				}
				if (!(id2 != text4))
				{
					goto IL_ED;
				}
			}
			this.PreviewKey = InputKeyItemVM.CreateFromHotKey(previewKey, false);
			IL_ED:
			this.UpdatePreviewAndActionTexts();
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
					this.UpdatePreviewAndActionTexts();
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
					this.UpdatePreviewAndActionTexts();
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

		[DataSourceProperty]
		public bool IsUnequippable
		{
			get
			{
				return this._isUnequippable;
			}
			set
			{
				if (value != this._isUnequippable)
				{
					this._isUnequippable = value;
					base.OnPropertyChangedWithValue(value, "IsUnequippable");
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
		public int ItemType
		{
			get
			{
				return this._itemType;
			}
			set
			{
				if (value != this._itemType)
				{
					this._itemType = value;
					base.OnPropertyChangedWithValue(value, "ItemType");
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

		public readonly CosmeticElement Cosmetic;

		public readonly string CosmeticID;

		private bool _isUnlocked;

		private bool _isUsed;

		private bool _areActionsEnabled;

		private bool _isSelectable;

		private bool _isUnequippable;

		private int _cost;

		private int _rarity;

		private int _itemType;

		private string _name;

		private string _ownedText;

		private string _actionText;

		private string _previewText;

		private ImageIdentifierVM _icon;

		private InputKeyItemVM _actionKey;

		private InputKeyItemVM _previewKey;
	}
}
