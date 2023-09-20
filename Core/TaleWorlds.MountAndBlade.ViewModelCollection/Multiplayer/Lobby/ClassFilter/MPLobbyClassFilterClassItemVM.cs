using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.ClassFilter
{
	// Token: 0x02000090 RID: 144
	public class MPLobbyClassFilterClassItemVM : ViewModel
	{
		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06000D8E RID: 3470 RVA: 0x0002ED57 File Offset: 0x0002CF57
		// (set) Token: 0x06000D8F RID: 3471 RVA: 0x0002ED5F File Offset: 0x0002CF5F
		public MultiplayerClassDivisions.MPHeroClass HeroClass { get; private set; }

		// Token: 0x06000D90 RID: 3472 RVA: 0x0002ED68 File Offset: 0x0002CF68
		public MPLobbyClassFilterClassItemVM(string cultureCode, MultiplayerClassDivisions.MPHeroClass heroClass, Action<MPLobbyClassFilterClassItemVM> onSelect)
		{
			this.HeroClass = heroClass;
			this._onSelect = onSelect;
			this.CultureCode = cultureCode;
			this.IconType = this.HeroClass.IconType.ToString();
			this.RefreshValues();
		}

		// Token: 0x06000D91 RID: 3473 RVA: 0x0002EDB5 File Offset: 0x0002CFB5
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.HeroClass.HeroName.ToString();
		}

		// Token: 0x06000D92 RID: 3474 RVA: 0x0002EDD3 File Offset: 0x0002CFD3
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.HeroClass = null;
		}

		// Token: 0x06000D93 RID: 3475 RVA: 0x0002EDE2 File Offset: 0x0002CFE2
		private void ExecuteSelect()
		{
			if (this._onSelect != null)
			{
				this._onSelect(this);
			}
		}

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06000D94 RID: 3476 RVA: 0x0002EDF8 File Offset: 0x0002CFF8
		// (set) Token: 0x06000D95 RID: 3477 RVA: 0x0002EE00 File Offset: 0x0002D000
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x06000D96 RID: 3478 RVA: 0x0002EE1E File Offset: 0x0002D01E
		// (set) Token: 0x06000D97 RID: 3479 RVA: 0x0002EE26 File Offset: 0x0002D026
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

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06000D98 RID: 3480 RVA: 0x0002EE44 File Offset: 0x0002D044
		// (set) Token: 0x06000D99 RID: 3481 RVA: 0x0002EE4C File Offset: 0x0002D04C
		[DataSourceProperty]
		public string CultureCode
		{
			get
			{
				return this._cultureCode;
			}
			set
			{
				if (value != this._cultureCode)
				{
					this._cultureCode = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureCode");
				}
			}
		}

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06000D9A RID: 3482 RVA: 0x0002EE6F File Offset: 0x0002D06F
		// (set) Token: 0x06000D9B RID: 3483 RVA: 0x0002EE77 File Offset: 0x0002D077
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

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06000D9C RID: 3484 RVA: 0x0002EE9A File Offset: 0x0002D09A
		// (set) Token: 0x06000D9D RID: 3485 RVA: 0x0002EEA2 File Offset: 0x0002D0A2
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
				}
			}
		}

		// Token: 0x0400067E RID: 1662
		private Action<MPLobbyClassFilterClassItemVM> _onSelect;

		// Token: 0x04000680 RID: 1664
		private bool _isEnabled;

		// Token: 0x04000681 RID: 1665
		private bool _isSelected;

		// Token: 0x04000682 RID: 1666
		private string _cultureCode;

		// Token: 0x04000683 RID: 1667
		private string _name;

		// Token: 0x04000684 RID: 1668
		private string _iconType;
	}
}
