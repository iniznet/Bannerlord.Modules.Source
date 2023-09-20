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
	// Token: 0x020000A5 RID: 165
	public class MPArmoryCosmeticItemVM : ViewModel
	{
		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x06000FC0 RID: 4032 RVA: 0x0003422F File Offset: 0x0003242F
		// (set) Token: 0x06000FC1 RID: 4033 RVA: 0x00034237 File Offset: 0x00032437
		public string UnequipText { get; private set; }

		// Token: 0x06000FC2 RID: 4034 RVA: 0x00034240 File Offset: 0x00032440
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

		// Token: 0x06000FC3 RID: 4035 RVA: 0x000342C8 File Offset: 0x000324C8
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.EquipmentElement.Item.Name.ToString();
			this.OwnedText = new TextObject("{=B5bcj3pC}Owned", null).ToString();
			this.PreviewText = new TextObject("{=un7poy9x}Preview", null).ToString();
			this.UnequipText = new TextObject("{=QndVFTbx}Unequip", null).ToString();
			this.IsUnlockedUpdated();
		}

		// Token: 0x06000FC4 RID: 4036 RVA: 0x00034341 File Offset: 0x00032541
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

		// Token: 0x06000FC5 RID: 4037 RVA: 0x00034364 File Offset: 0x00032564
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

		// Token: 0x06000FC6 RID: 4038 RVA: 0x000343A3 File Offset: 0x000325A3
		private void ExecutePreview()
		{
			this._onPreviewed(this);
		}

		// Token: 0x06000FC7 RID: 4039 RVA: 0x000343B1 File Offset: 0x000325B1
		private void ExecuteEnableActions()
		{
			this.AreActionsEnabled = true;
		}

		// Token: 0x06000FC8 RID: 4040 RVA: 0x000343BA File Offset: 0x000325BA
		private void ExecuteDisableActions()
		{
			this.AreActionsEnabled = false;
		}

		// Token: 0x06000FC9 RID: 4041 RVA: 0x000343C3 File Offset: 0x000325C3
		public void IsUnlockedUpdated()
		{
			this.ActionText = (this.IsUnlocked ? new TextObject("{=DKqLY1aJ}Equip", null).ToString() : new TextObject("{=i2mNBaxE}Obtain", null).ToString());
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x000343F5 File Offset: 0x000325F5
		public void RefreshKeyBindings(HotKey actionKey, HotKey previewKey)
		{
			this.ActionKey = InputKeyItemVM.CreateFromHotKey(actionKey, false);
			this.PreviewKey = InputKeyItemVM.CreateFromHotKey(previewKey, false);
		}

		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06000FCB RID: 4043 RVA: 0x00034411 File Offset: 0x00032611
		// (set) Token: 0x06000FCC RID: 4044 RVA: 0x00034419 File Offset: 0x00032619
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

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x06000FCD RID: 4045 RVA: 0x0003443D File Offset: 0x0003263D
		// (set) Token: 0x06000FCE RID: 4046 RVA: 0x00034445 File Offset: 0x00032645
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

		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x06000FCF RID: 4047 RVA: 0x00034463 File Offset: 0x00032663
		// (set) Token: 0x06000FD0 RID: 4048 RVA: 0x0003446B File Offset: 0x0003266B
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

		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x06000FD1 RID: 4049 RVA: 0x00034489 File Offset: 0x00032689
		// (set) Token: 0x06000FD2 RID: 4050 RVA: 0x00034491 File Offset: 0x00032691
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

		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x06000FD3 RID: 4051 RVA: 0x000344AF File Offset: 0x000326AF
		// (set) Token: 0x06000FD4 RID: 4052 RVA: 0x000344B7 File Offset: 0x000326B7
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

		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x06000FD5 RID: 4053 RVA: 0x000344D5 File Offset: 0x000326D5
		// (set) Token: 0x06000FD6 RID: 4054 RVA: 0x000344DD File Offset: 0x000326DD
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

		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x06000FD7 RID: 4055 RVA: 0x00034500 File Offset: 0x00032700
		// (set) Token: 0x06000FD8 RID: 4056 RVA: 0x00034508 File Offset: 0x00032708
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

		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x06000FD9 RID: 4057 RVA: 0x0003452B File Offset: 0x0003272B
		// (set) Token: 0x06000FDA RID: 4058 RVA: 0x00034533 File Offset: 0x00032733
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

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x06000FDB RID: 4059 RVA: 0x00034556 File Offset: 0x00032756
		// (set) Token: 0x06000FDC RID: 4060 RVA: 0x0003455E File Offset: 0x0003275E
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

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x06000FDD RID: 4061 RVA: 0x00034581 File Offset: 0x00032781
		// (set) Token: 0x06000FDE RID: 4062 RVA: 0x00034589 File Offset: 0x00032789
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

		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x06000FDF RID: 4063 RVA: 0x000345A7 File Offset: 0x000327A7
		// (set) Token: 0x06000FE0 RID: 4064 RVA: 0x000345AF File Offset: 0x000327AF
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

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x06000FE1 RID: 4065 RVA: 0x000345CD File Offset: 0x000327CD
		// (set) Token: 0x06000FE2 RID: 4066 RVA: 0x000345D5 File Offset: 0x000327D5
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

		// Token: 0x0400076C RID: 1900
		public readonly CosmeticsManager.CosmeticElement Cosmetic;

		// Token: 0x0400076D RID: 1901
		public readonly EquipmentElement EquipmentElement;

		// Token: 0x0400076E RID: 1902
		public readonly string CosmeticID;

		// Token: 0x0400076F RID: 1903
		private readonly Action<MPArmoryCosmeticItemVM> _onEquipped;

		// Token: 0x04000770 RID: 1904
		private readonly Action<MPArmoryCosmeticItemVM> _onPurchaseRequested;

		// Token: 0x04000771 RID: 1905
		private readonly Action<MPArmoryCosmeticItemVM> _onPreviewed;

		// Token: 0x04000773 RID: 1907
		private bool _isUnlocked;

		// Token: 0x04000774 RID: 1908
		private bool _isUsed;

		// Token: 0x04000775 RID: 1909
		private bool _areActionsEnabled;

		// Token: 0x04000776 RID: 1910
		private int _cost;

		// Token: 0x04000777 RID: 1911
		private int _rarity;

		// Token: 0x04000778 RID: 1912
		private string _name;

		// Token: 0x04000779 RID: 1913
		private string _ownedText;

		// Token: 0x0400077A RID: 1914
		private string _actionText;

		// Token: 0x0400077B RID: 1915
		private string _previewText;

		// Token: 0x0400077C RID: 1916
		private ImageIdentifierVM _icon;

		// Token: 0x0400077D RID: 1917
		private InputKeyItemVM _actionKey;

		// Token: 0x0400077E RID: 1918
		private InputKeyItemVM _previewKey;
	}
}
