using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.OfficialGame
{
	public class MPMatchmakingItemVM : ViewModel
	{
		public event Action<MPMatchmakingItemVM, bool> OnSelectionChanged;

		public event Action<MPMatchmakingItemVM> OnSetFocusItem;

		public event Action OnRemoveFocus;

		public MPMatchmakingItemVM(MultiplayerGameType type)
		{
			this.Type = type.ToString();
			this.IsAvailable = true;
			this.IsSelected = this.IsAvailable;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = GameTexts.FindText("str_multiplayer_official_game_type_name", this.Type).ToString();
		}

		private void ExecuteSetFocusItem()
		{
			Action<MPMatchmakingItemVM> onSetFocusItem = this.OnSetFocusItem;
			if (onSetFocusItem == null)
			{
				return;
			}
			onSetFocusItem(this);
		}

		private void ExecuteRemoveFocus()
		{
			Action onRemoveFocus = this.OnRemoveFocus;
			if (onRemoveFocus == null)
			{
				return;
			}
			onRemoveFocus();
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

		private string _name;

		private string _type;

		private bool _isSelected;

		private bool _isAvailable;
	}
}
