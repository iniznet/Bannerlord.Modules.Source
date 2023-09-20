using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.OfficialGame
{
	// Token: 0x02000072 RID: 114
	public class MPMatchmakingItemVM : ViewModel
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000A8D RID: 2701 RVA: 0x00025DD4 File Offset: 0x00023FD4
		// (remove) Token: 0x06000A8E RID: 2702 RVA: 0x00025E0C File Offset: 0x0002400C
		public event Action<MPMatchmakingItemVM, bool> OnSelectionChanged;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000A8F RID: 2703 RVA: 0x00025E44 File Offset: 0x00024044
		// (remove) Token: 0x06000A90 RID: 2704 RVA: 0x00025E7C File Offset: 0x0002407C
		public event Action<MPMatchmakingItemVM> OnSetFocusItem;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000A91 RID: 2705 RVA: 0x00025EB4 File Offset: 0x000240B4
		// (remove) Token: 0x06000A92 RID: 2706 RVA: 0x00025EEC File Offset: 0x000240EC
		public event Action OnRemoveFocus;

		// Token: 0x06000A93 RID: 2707 RVA: 0x00025F21 File Offset: 0x00024121
		public MPMatchmakingItemVM(MissionLobbyComponent.MultiplayerGameType type)
		{
			this.Type = type.ToString();
			this.IsAvailable = true;
			this.IsSelected = this.IsAvailable;
			this.RefreshValues();
		}

		// Token: 0x06000A94 RID: 2708 RVA: 0x00025F55 File Offset: 0x00024155
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = GameTexts.FindText("str_multiplayer_official_game_type_name", this.Type).ToString();
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x00025F78 File Offset: 0x00024178
		private void ExecuteSetFocusItem()
		{
			Action<MPMatchmakingItemVM> onSetFocusItem = this.OnSetFocusItem;
			if (onSetFocusItem == null)
			{
				return;
			}
			onSetFocusItem(this);
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x00025F8B File Offset: 0x0002418B
		private void ExecuteRemoveFocus()
		{
			Action onRemoveFocus = this.OnRemoveFocus;
			if (onRemoveFocus == null)
			{
				return;
			}
			onRemoveFocus();
		}

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06000A97 RID: 2711 RVA: 0x00025F9D File Offset: 0x0002419D
		// (set) Token: 0x06000A98 RID: 2712 RVA: 0x00025FA5 File Offset: 0x000241A5
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

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x06000A99 RID: 2713 RVA: 0x00025FC8 File Offset: 0x000241C8
		// (set) Token: 0x06000A9A RID: 2714 RVA: 0x00025FD0 File Offset: 0x000241D0
		[DataSourceProperty]
		public string Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (value != this._type)
				{
					this._type = value;
					base.OnPropertyChangedWithValue<string>(value, "Type");
				}
			}
		}

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x06000A9B RID: 2715 RVA: 0x00025FF3 File Offset: 0x000241F3
		// (set) Token: 0x06000A9C RID: 2716 RVA: 0x00025FFB File Offset: 0x000241FB
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
					Action<MPMatchmakingItemVM, bool> onSelectionChanged = this.OnSelectionChanged;
					if (onSelectionChanged == null)
					{
						return;
					}
					onSelectionChanged(this, this._isSelected);
				}
			}
		}

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x06000A9D RID: 2717 RVA: 0x00026030 File Offset: 0x00024230
		// (set) Token: 0x06000A9E RID: 2718 RVA: 0x00026038 File Offset: 0x00024238
		[DataSourceProperty]
		public bool IsAvailable
		{
			get
			{
				return this._isAvailable;
			}
			set
			{
				if (value != this._isAvailable)
				{
					this._isAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsAvailable");
				}
			}
		}

		// Token: 0x04000520 RID: 1312
		private string _name;

		// Token: 0x04000521 RID: 1313
		private string _type;

		// Token: 0x04000522 RID: 1314
		private bool _isSelected;

		// Token: 0x04000523 RID: 1315
		private bool _isAvailable;
	}
}
