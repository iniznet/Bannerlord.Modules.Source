using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	// Token: 0x020000F8 RID: 248
	public abstract class KeyOptionVM : ViewModel
	{
		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x060015E8 RID: 5608 RVA: 0x0004693F File Offset: 0x00044B3F
		// (set) Token: 0x060015E9 RID: 5609 RVA: 0x00046947 File Offset: 0x00044B47
		public Key CurrentKey { get; protected set; }

		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x060015EA RID: 5610 RVA: 0x00046950 File Offset: 0x00044B50
		// (set) Token: 0x060015EB RID: 5611 RVA: 0x00046958 File Offset: 0x00044B58
		public Key Key { get; protected set; }

		// Token: 0x060015EC RID: 5612 RVA: 0x00046961 File Offset: 0x00044B61
		public KeyOptionVM(string groupId, string id, Action<KeyOptionVM> onKeybindRequest)
		{
			this._groupId = groupId;
			this._id = id;
			this._onKeybindRequest = onKeybindRequest;
		}

		// Token: 0x060015ED RID: 5613
		public abstract void Set(InputKey newKey);

		// Token: 0x060015EE RID: 5614
		public abstract void Update();

		// Token: 0x060015EF RID: 5615
		public abstract void OnDone();

		// Token: 0x060015F0 RID: 5616
		internal abstract bool IsChanged();

		// Token: 0x060015F1 RID: 5617
		internal abstract void OnGamepadActiveStateChanged(GamepadActiveStateChangedEvent obj);

		// Token: 0x17000734 RID: 1844
		// (get) Token: 0x060015F2 RID: 5618 RVA: 0x0004697E File Offset: 0x00044B7E
		// (set) Token: 0x060015F3 RID: 5619 RVA: 0x00046986 File Offset: 0x00044B86
		[DataSourceProperty]
		public string OptionValueText
		{
			get
			{
				return this._optionValueText;
			}
			set
			{
				if (value != this._optionValueText)
				{
					this._optionValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "OptionValueText");
				}
			}
		}

		// Token: 0x17000735 RID: 1845
		// (get) Token: 0x060015F4 RID: 5620 RVA: 0x000469A9 File Offset: 0x00044BA9
		// (set) Token: 0x060015F5 RID: 5621 RVA: 0x000469B1 File Offset: 0x00044BB1
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

		// Token: 0x17000736 RID: 1846
		// (get) Token: 0x060015F6 RID: 5622 RVA: 0x000469D4 File Offset: 0x00044BD4
		// (set) Token: 0x060015F7 RID: 5623 RVA: 0x000469DC File Offset: 0x00044BDC
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

		// Token: 0x04000A70 RID: 2672
		protected readonly string _groupId;

		// Token: 0x04000A71 RID: 2673
		protected readonly string _id;

		// Token: 0x04000A72 RID: 2674
		protected readonly Action<KeyOptionVM> _onKeybindRequest;

		// Token: 0x04000A73 RID: 2675
		private string _optionValueText;

		// Token: 0x04000A74 RID: 2676
		private string _name;

		// Token: 0x04000A75 RID: 2677
		private string _description;
	}
}
