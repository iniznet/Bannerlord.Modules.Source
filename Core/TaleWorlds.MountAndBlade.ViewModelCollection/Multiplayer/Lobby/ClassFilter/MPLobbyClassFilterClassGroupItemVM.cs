using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.ClassFilter
{
	// Token: 0x0200008F RID: 143
	public class MPLobbyClassFilterClassGroupItemVM : ViewModel
	{
		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06000D84 RID: 3460 RVA: 0x0002EC4F File Offset: 0x0002CE4F
		// (set) Token: 0x06000D85 RID: 3461 RVA: 0x0002EC57 File Offset: 0x0002CE57
		public MultiplayerClassDivisions.MPHeroClassGroup ClassGroup { get; set; }

		// Token: 0x06000D86 RID: 3462 RVA: 0x0002EC60 File Offset: 0x0002CE60
		public MPLobbyClassFilterClassGroupItemVM(MultiplayerClassDivisions.MPHeroClassGroup classGroup)
		{
			this.ClassGroup = classGroup;
			this.Classes = new MBBindingList<MPLobbyClassFilterClassItemVM>();
			this.RefreshValues();
		}

		// Token: 0x06000D87 RID: 3463 RVA: 0x0002EC80 File Offset: 0x0002CE80
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.ClassGroup.Name.ToString();
			this.Classes.ApplyActionOnAllItems(delegate(MPLobbyClassFilterClassItemVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06000D88 RID: 3464 RVA: 0x0002ECD3 File Offset: 0x0002CED3
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.ClassGroup = null;
		}

		// Token: 0x06000D89 RID: 3465 RVA: 0x0002ECE4 File Offset: 0x0002CEE4
		public void AddClass(string cultureCode, MultiplayerClassDivisions.MPHeroClass heroClass, Action<MPLobbyClassFilterClassItemVM> onSelect)
		{
			MPLobbyClassFilterClassItemVM mplobbyClassFilterClassItemVM = new MPLobbyClassFilterClassItemVM(cultureCode, heroClass, onSelect);
			this.Classes.Add(mplobbyClassFilterClassItemVM);
		}

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06000D8A RID: 3466 RVA: 0x0002ED06 File Offset: 0x0002CF06
		// (set) Token: 0x06000D8B RID: 3467 RVA: 0x0002ED0E File Offset: 0x0002CF0E
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

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06000D8C RID: 3468 RVA: 0x0002ED31 File Offset: 0x0002CF31
		// (set) Token: 0x06000D8D RID: 3469 RVA: 0x0002ED39 File Offset: 0x0002CF39
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

		// Token: 0x0400067C RID: 1660
		private string _name;

		// Token: 0x0400067D RID: 1661
		private MBBindingList<MPLobbyClassFilterClassItemVM> _classes;
	}
}
