using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Intermission
{
	public class MPIntermissionMapItemVM : ViewModel
	{
		public MPIntermissionMapItemVM(string mapID, Action<MPIntermissionMapItemVM> onPlayerVoted)
		{
			this.MapID = mapID;
			this._onPlayerVoted = onPlayerVoted;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			TextObject textObject;
			if (GameTexts.TryGetText("str_multiplayer_scene_name", ref textObject, this.MapID))
			{
				this.MapName = textObject.ToString();
				return;
			}
			this.MapName = this.MapID;
		}

		public void ExecuteVote()
		{
			this._onPlayerVoted(this);
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
				}
			}
		}

		[DataSourceProperty]
		public string MapID
		{
			get
			{
				return this._mapID;
			}
			set
			{
				if (value != this._mapID)
				{
					this._mapID = value;
					base.OnPropertyChangedWithValue<string>(value, "MapID");
				}
			}
		}

		[DataSourceProperty]
		public string MapName
		{
			get
			{
				return this._mapName;
			}
			set
			{
				if (value != this._mapName)
				{
					this._mapName = value;
					base.OnPropertyChangedWithValue<string>(value, "MapName");
				}
			}
		}

		[DataSourceProperty]
		public int Votes
		{
			get
			{
				return this._votes;
			}
			set
			{
				if (value != this._votes)
				{
					this._votes = value;
					base.OnPropertyChangedWithValue(value, "Votes");
				}
			}
		}

		private readonly Action<MPIntermissionMapItemVM> _onPlayerVoted;

		private bool _isSelected;

		private string _mapID;

		private string _mapName;

		private int _votes;
	}
}
