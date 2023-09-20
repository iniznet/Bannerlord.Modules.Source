using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.ClassFilter
{
	public class MPLobbyClassFilterClassGroupItemVM : ViewModel
	{
		public MultiplayerClassDivisions.MPHeroClassGroup ClassGroup { get; set; }

		public MPLobbyClassFilterClassGroupItemVM(MultiplayerClassDivisions.MPHeroClassGroup classGroup)
		{
			this.ClassGroup = classGroup;
			this.Classes = new MBBindingList<MPLobbyClassFilterClassItemVM>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.ClassGroup.Name.ToString();
			this.Classes.ApplyActionOnAllItems(delegate(MPLobbyClassFilterClassItemVM x)
			{
				x.RefreshValues();
			});
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.ClassGroup = null;
		}

		public void AddClass(BasicCultureObject culture, MultiplayerClassDivisions.MPHeroClass heroClass, Action<MPLobbyClassFilterClassItemVM> onSelect)
		{
			MPLobbyClassFilterClassItemVM mplobbyClassFilterClassItemVM = new MPLobbyClassFilterClassItemVM(culture, heroClass, onSelect);
			this.Classes.Add(mplobbyClassFilterClassItemVM);
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
		public MBBindingList<MPLobbyClassFilterClassItemVM> Classes
		{
			get
			{
				return this._classes;
			}
			set
			{
				if (value != this._classes)
				{
					this._classes = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyClassFilterClassItemVM>>(value, "Classes");
				}
			}
		}

		private string _name;

		private MBBindingList<MPLobbyClassFilterClassItemVM> _classes;
	}
}
