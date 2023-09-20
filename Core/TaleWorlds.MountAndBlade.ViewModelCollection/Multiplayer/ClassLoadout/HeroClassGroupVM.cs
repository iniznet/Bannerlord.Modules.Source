using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	public class HeroClassGroupVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.HeroClassGroup.Name.ToString();
			this.SubClasses.ApplyActionOnAllItems(delegate(HeroClassVM x)
			{
				x.RefreshValues();
			});
		}

		public bool IsValid
		{
			get
			{
				return this.SubClasses.Count > 0;
			}
		}

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

		public readonly MultiplayerClassDivisions.MPHeroClassGroup HeroClassGroup;

		private readonly Action<HeroPerkVM, MPPerkVM> _onPerkSelect;

		private string _name;

		private string _iconType;

		private string _iconPath;

		private MBBindingList<HeroClassVM> _subClasses;
	}
}
