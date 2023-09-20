using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	public class MPLobbyClanItemVM : ViewModel
	{
		public MPLobbyClanItemVM(string name, string tag, string sigilCode, int gamesWon, int gamesLost, int ranking, bool isOwnClan)
		{
			this._name = name;
			this._tag = tag;
			this._sigilCode = sigilCode;
			this.GamesWon = gamesWon;
			this.GamesLost = gamesLost;
			this.Ranking = ranking;
			this.IsOwnClan = isOwnClan;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.SigilImage = new ImageIdentifierVM(new Banner(this._sigilCode));
			GameTexts.SetVariable("STR", this._tag);
			string text = new TextObject("{=uTXYEAOg}[{STR}]", null).ToString();
			GameTexts.SetVariable("STR1", this._name);
			GameTexts.SetVariable("STR2", text);
			this.NameWithTag = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
		}

		[DataSourceProperty]
		public string NameWithTag
		{
			get
			{
				return this._nameWithTag;
			}
			set
			{
				if (value != this._nameWithTag)
				{
					this._nameWithTag = value;
					base.OnPropertyChangedWithValue<string>(value, "NameWithTag");
				}
			}
		}

		[DataSourceProperty]
		public int MemberCount
		{
			get
			{
				return this._memberCount;
			}
			set
			{
				if (value != this._memberCount)
				{
					this._memberCount = value;
					base.OnPropertyChangedWithValue(value, "MemberCount");
				}
			}
		}

		[DataSourceProperty]
		public int GamesWon
		{
			get
			{
				return this._gamesWon;
			}
			set
			{
				if (value != this._gamesWon)
				{
					this._gamesWon = value;
					base.OnPropertyChangedWithValue(value, "GamesWon");
				}
			}
		}

		[DataSourceProperty]
		public int GamesLost
		{
			get
			{
				return this._gamesLost;
			}
			set
			{
				if (value != this._gamesLost)
				{
					this._gamesLost = value;
					base.OnPropertyChangedWithValue(value, "GamesLost");
				}
			}
		}

		[DataSourceProperty]
		public int Ranking
		{
			get
			{
				return this._ranking;
			}
			set
			{
				if (value != this._ranking)
				{
					this._ranking = value;
					base.OnPropertyChangedWithValue(value, "Ranking");
				}
			}
		}

		[DataSourceProperty]
		public bool IsOwnClan
		{
			get
			{
				return this._isOwnClan;
			}
			set
			{
				if (value != this._isOwnClan)
				{
					this._isOwnClan = value;
					base.OnPropertyChangedWithValue(value, "IsOwnClan");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM SigilImage
		{
			get
			{
				return this._sigilImage;
			}
			set
			{
				if (value != this._sigilImage)
				{
					this._sigilImage = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "SigilImage");
				}
			}
		}

		private string _name;

		private string _tag;

		private string _sigilCode;

		private string _nameWithTag;

		private int _memberCount;

		private int _gamesWon;

		private int _gamesLost;

		private int _ranking;

		private bool _isOwnClan;

		private ImageIdentifierVM _sigilImage;
	}
}
