using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x02000099 RID: 153
	public class MPLobbyClanItemVM : ViewModel
	{
		// Token: 0x06000E7C RID: 3708 RVA: 0x000313AC File Offset: 0x0002F5AC
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

		// Token: 0x06000E7D RID: 3709 RVA: 0x000313FC File Offset: 0x0002F5FC
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

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06000E7E RID: 3710 RVA: 0x00031477 File Offset: 0x0002F677
		// (set) Token: 0x06000E7F RID: 3711 RVA: 0x0003147F File Offset: 0x0002F67F
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

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x06000E80 RID: 3712 RVA: 0x000314A2 File Offset: 0x0002F6A2
		// (set) Token: 0x06000E81 RID: 3713 RVA: 0x000314AA File Offset: 0x0002F6AA
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

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x06000E82 RID: 3714 RVA: 0x000314C8 File Offset: 0x0002F6C8
		// (set) Token: 0x06000E83 RID: 3715 RVA: 0x000314D0 File Offset: 0x0002F6D0
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

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x06000E84 RID: 3716 RVA: 0x000314EE File Offset: 0x0002F6EE
		// (set) Token: 0x06000E85 RID: 3717 RVA: 0x000314F6 File Offset: 0x0002F6F6
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

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x06000E86 RID: 3718 RVA: 0x00031514 File Offset: 0x0002F714
		// (set) Token: 0x06000E87 RID: 3719 RVA: 0x0003151C File Offset: 0x0002F71C
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

		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x06000E88 RID: 3720 RVA: 0x0003153A File Offset: 0x0002F73A
		// (set) Token: 0x06000E89 RID: 3721 RVA: 0x00031542 File Offset: 0x0002F742
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

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x06000E8A RID: 3722 RVA: 0x00031560 File Offset: 0x0002F760
		// (set) Token: 0x06000E8B RID: 3723 RVA: 0x00031568 File Offset: 0x0002F768
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

		// Token: 0x040006DC RID: 1756
		private string _name;

		// Token: 0x040006DD RID: 1757
		private string _tag;

		// Token: 0x040006DE RID: 1758
		private string _sigilCode;

		// Token: 0x040006DF RID: 1759
		private string _nameWithTag;

		// Token: 0x040006E0 RID: 1760
		private int _memberCount;

		// Token: 0x040006E1 RID: 1761
		private int _gamesWon;

		// Token: 0x040006E2 RID: 1762
		private int _gamesLost;

		// Token: 0x040006E3 RID: 1763
		private int _ranking;

		// Token: 0x040006E4 RID: 1764
		private bool _isOwnClan;

		// Token: 0x040006E5 RID: 1765
		private ImageIdentifierVM _sigilImage;
	}
}
