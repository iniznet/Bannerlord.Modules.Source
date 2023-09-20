using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	// Token: 0x020000C8 RID: 200
	public class HeroClassGroupVM : ViewModel
	{
		// Token: 0x060012B0 RID: 4784 RVA: 0x0003D758 File Offset: 0x0003B958
		public HeroClassGroupVM(Action<HeroClassVM> onSelect, Action<HeroPerkVM, MPPerkVM> onPerkSelect, MultiplayerClassDivisions.MPHeroClassGroup heroClassGroup, bool useSecondary)
		{
			this.HeroClassGroup = heroClassGroup;
			this._onPerkSelect = onPerkSelect;
			this.IconType = heroClassGroup.StringId;
			this.SubClasses = new MBBindingList<HeroClassVM>();
			Team team = GameNetwork.MyPeer.GetComponent<MissionPeer>().Team;
			IEnumerable<MultiplayerClassDivisions.MPHeroClass> mpheroClasses = MultiplayerClassDivisions.GetMPHeroClasses(GameNetwork.MyPeer.GetComponent<MissionPeer>().Culture);
			Func<MultiplayerClassDivisions.MPHeroClass, bool> <>9__0;
			Func<MultiplayerClassDivisions.MPHeroClass, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (MultiplayerClassDivisions.MPHeroClass h) => h.ClassGroup.Equals(heroClassGroup));
			}
			foreach (MultiplayerClassDivisions.MPHeroClass mpheroClass in mpheroClasses.Where(func))
			{
				this.SubClasses.Add(new HeroClassVM(onSelect, this._onPerkSelect, mpheroClass, useSecondary));
			}
			this.RefreshValues();
		}

		// Token: 0x060012B1 RID: 4785 RVA: 0x0003D844 File Offset: 0x0003BA44
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.HeroClassGroup.Name.ToString();
			this.SubClasses.ApplyActionOnAllItems(delegate(HeroClassVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x060012B2 RID: 4786 RVA: 0x0003D897 File Offset: 0x0003BA97
		public bool IsValid
		{
			get
			{
				return this.SubClasses.Count > 0;
			}
		}

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x060012B3 RID: 4787 RVA: 0x0003D8A7 File Offset: 0x0003BAA7
		// (set) Token: 0x060012B4 RID: 4788 RVA: 0x0003D8AF File Offset: 0x0003BAAF
		[DataSourceProperty]
		public MBBindingList<HeroClassVM> SubClasses
		{
			get
			{
				return this._subClasses;
			}
			set
			{
				if (value != this._subClasses)
				{
					this._subClasses = value;
					base.OnPropertyChangedWithValue<MBBindingList<HeroClassVM>>(value, "SubClasses");
				}
			}
		}

		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x060012B5 RID: 4789 RVA: 0x0003D8CD File Offset: 0x0003BACD
		// (set) Token: 0x060012B6 RID: 4790 RVA: 0x0003D8D5 File Offset: 0x0003BAD5
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

		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x060012B7 RID: 4791 RVA: 0x0003D8F8 File Offset: 0x0003BAF8
		// (set) Token: 0x060012B8 RID: 4792 RVA: 0x0003D900 File Offset: 0x0003BB00
		[DataSourceProperty]
		public string IconType
		{
			get
			{
				return this._iconType;
			}
			set
			{
				if (value != this._iconType)
				{
					this._iconType = value;
					base.OnPropertyChangedWithValue<string>(value, "IconType");
					this.IconPath = "TroopBanners\\ClassType_" + value;
				}
			}
		}

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x060012B9 RID: 4793 RVA: 0x0003D934 File Offset: 0x0003BB34
		// (set) Token: 0x060012BA RID: 4794 RVA: 0x0003D93C File Offset: 0x0003BB3C
		[DataSourceProperty]
		public string IconPath
		{
			get
			{
				return this._iconPath;
			}
			set
			{
				if (value != this._iconPath)
				{
					this._iconPath = value;
					base.OnPropertyChangedWithValue<string>(value, "IconPath");
				}
			}
		}

		// Token: 0x040008F7 RID: 2295
		public readonly MultiplayerClassDivisions.MPHeroClassGroup HeroClassGroup;

		// Token: 0x040008F8 RID: 2296
		private readonly Action<HeroPerkVM, MPPerkVM> _onPerkSelect;

		// Token: 0x040008F9 RID: 2297
		private string _name;

		// Token: 0x040008FA RID: 2298
		private string _iconType;

		// Token: 0x040008FB RID: 2299
		private string _iconPath;

		// Token: 0x040008FC RID: 2300
		private MBBindingList<HeroClassVM> _subClasses;
	}
}
