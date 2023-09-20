using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Clans
{
	public class KingdomClanItemVM : KingdomItemVM
	{
		public KingdomClanItemVM(Clan clan, Action<KingdomClanItemVM> onSelect)
		{
			this.Clan = clan;
			this._onSelect = onSelect;
			this.Banner = new ImageIdentifierVM(clan.Banner);
			this.Banner_9 = new ImageIdentifierVM(BannerCode.CreateFrom(clan.Banner), true);
			this.RefreshValues();
			this.Refresh();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.Clan.Name.ToString();
			GameTexts.SetVariable("TIER", this.Clan.Tier);
			this.TierText = GameTexts.FindText("str_clan_tier", null).ToString();
		}

		public void Refresh()
		{
			this.Members = new MBBindingList<HeroVM>();
			this.ClanType = 0;
			if (this.Clan.IsUnderMercenaryService)
			{
				this.ClanType = 2;
			}
			else if (this.Clan.Kingdom.RulingClan == this.Clan)
			{
				this.ClanType = 1;
			}
			foreach (Hero hero in this.Clan.Heroes.Where((Hero h) => !h.IsDisabled && !h.IsNotSpawned && h.IsAlive && !h.IsChild))
			{
				this.Members.Add(new HeroVM(hero, false));
			}
			this.NumOfMembers = this.Members.Count;
			this.Fiefs = new MBBindingList<KingdomClanFiefItemVM>();
			foreach (Settlement settlement in this.Clan.Settlements.Where((Settlement s) => s.IsTown || s.IsCastle))
			{
				this.Fiefs.Add(new KingdomClanFiefItemVM(settlement));
			}
			this.NumOfFiefs = this.Fiefs.Count;
			this.Influence = (int)this.Clan.Influence;
		}

		protected override void OnSelect()
		{
			base.OnSelect();
			this._onSelect(this);
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
		public int ClanType
		{
			get
			{
				return this._clanType;
			}
			set
			{
				if (value != this._clanType)
				{
					this._clanType = value;
					base.OnPropertyChangedWithValue(value, "ClanType");
				}
			}
		}

		[DataSourceProperty]
		public int NumOfMembers
		{
			get
			{
				return this._numOfMembers;
			}
			set
			{
				if (value != this._numOfMembers)
				{
					this._numOfMembers = value;
					base.OnPropertyChangedWithValue(value, "NumOfMembers");
				}
			}
		}

		[DataSourceProperty]
		public int NumOfFiefs
		{
			get
			{
				return this._numOfFiefs;
			}
			set
			{
				if (value != this._numOfFiefs)
				{
					this._numOfFiefs = value;
					base.OnPropertyChangedWithValue(value, "NumOfFiefs");
				}
			}
		}

		[DataSourceProperty]
		public string TierText
		{
			get
			{
				return this._tierText;
			}
			set
			{
				if (value != this._tierText)
				{
					this._tierText = value;
					base.OnPropertyChangedWithValue<string>(value, "TierText");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM Banner
		{
			get
			{
				return this._banner;
			}
			set
			{
				if (value != this._banner)
				{
					this._banner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Banner");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM Banner_9
		{
			get
			{
				return this._banner_9;
			}
			set
			{
				if (value != this._banner_9)
				{
					this._banner_9 = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Banner_9");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<HeroVM> Members
		{
			get
			{
				return this._members;
			}
			set
			{
				if (value != this._members)
				{
					this._members = value;
					base.OnPropertyChangedWithValue<MBBindingList<HeroVM>>(value, "Members");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<KingdomClanFiefItemVM> Fiefs
		{
			get
			{
				return this._fiefs;
			}
			set
			{
				if (value != this._fiefs)
				{
					this._fiefs = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomClanFiefItemVM>>(value, "Fiefs");
				}
			}
		}

		[DataSourceProperty]
		public int Influence
		{
			get
			{
				return this._influence;
			}
			set
			{
				if (value != this._influence)
				{
					this._influence = value;
					base.OnPropertyChangedWithValue(value, "Influence");
				}
			}
		}

		private readonly Action<KingdomClanItemVM> _onSelect;

		public readonly Clan Clan;

		private string _name;

		private ImageIdentifierVM _banner;

		private ImageIdentifierVM _banner_9;

		private MBBindingList<HeroVM> _members;

		private MBBindingList<KingdomClanFiefItemVM> _fiefs;

		private int _influence;

		private int _numOfMembers;

		private int _numOfFiefs;

		private string _tierText;

		private int _clanType = -1;

		private enum ClanTypes
		{
			Normal,
			Leader,
			Mercenary
		}
	}
}
