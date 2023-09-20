using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.ProfileSelection
{
	// Token: 0x02000015 RID: 21
	public class ProfileSelectionVM : ViewModel
	{
		// Token: 0x0600019D RID: 413 RVA: 0x00006B3C File Offset: 0x00004D3C
		public ProfileSelectionVM(bool isDirectPlayPossible)
		{
			this.SelectProfileText = new TextObject("{=wubDWOlh}Select Profile", null).ToString();
			this.SelectProfileKey = InputKeyItemVM.CreateFromHotKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SelectProfile"), false);
			this.PlayKey = InputKeyItemVM.CreateFromHotKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Play"), false);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x00006BA8 File Offset: 0x00004DA8
		public void OnActivate(bool isDirectPlayPossible)
		{
			this.IsPlayEnabled = isDirectPlayPossible;
			if (!string.IsNullOrEmpty(PlatformServices.Instance.UserDisplayName))
			{
				this.PlayText = new TextObject("{=FTXx0aRp}Play as", null).ToString() + PlatformServices.Instance.UserDisplayName;
				return;
			}
			this.PlayText = new TextObject("{=playgame}Play", null).ToString();
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00006C09 File Offset: 0x00004E09
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.SelectProfileKey.OnFinalize();
			this.PlayKey.OnFinalize();
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x00006C27 File Offset: 0x00004E27
		// (set) Token: 0x060001A1 RID: 417 RVA: 0x00006C2F File Offset: 0x00004E2F
		[DataSourceProperty]
		public string SelectProfileText
		{
			get
			{
				return this._selectProfileText;
			}
			set
			{
				if (value != this._selectProfileText)
				{
					this._selectProfileText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectProfileText");
				}
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x00006C52 File Offset: 0x00004E52
		// (set) Token: 0x060001A3 RID: 419 RVA: 0x00006C5A File Offset: 0x00004E5A
		[DataSourceProperty]
		public bool IsPlayEnabled
		{
			get
			{
				return this._isPlayEnabled;
			}
			set
			{
				if (value != this._isPlayEnabled)
				{
					this._isPlayEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsPlayEnabled");
				}
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x00006C78 File Offset: 0x00004E78
		// (set) Token: 0x060001A5 RID: 421 RVA: 0x00006C80 File Offset: 0x00004E80
		[DataSourceProperty]
		public string PlayText
		{
			get
			{
				return this._playText;
			}
			set
			{
				if (value != this._playText)
				{
					this._playText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayText");
				}
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x00006CA3 File Offset: 0x00004EA3
		// (set) Token: 0x060001A7 RID: 423 RVA: 0x00006CAB File Offset: 0x00004EAB
		[DataSourceProperty]
		public InputKeyItemVM SelectProfileKey
		{
			get
			{
				return this._selectProfileKey;
			}
			set
			{
				if (value != this._selectProfileKey)
				{
					this._selectProfileKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "SelectProfileKey");
				}
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x00006CC9 File Offset: 0x00004EC9
		// (set) Token: 0x060001A9 RID: 425 RVA: 0x00006CD1 File Offset: 0x00004ED1
		[DataSourceProperty]
		public InputKeyItemVM PlayKey
		{
			get
			{
				return this._playKey;
			}
			set
			{
				if (value != this._playKey)
				{
					this._playKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "PlayKey");
				}
			}
		}

		// Token: 0x040000BD RID: 189
		private bool _isPlayEnabled;

		// Token: 0x040000BE RID: 190
		private string _selectProfileText;

		// Token: 0x040000BF RID: 191
		private string _playText;

		// Token: 0x040000C0 RID: 192
		private InputKeyItemVM _playKey;

		// Token: 0x040000C1 RID: 193
		private InputKeyItemVM _selectProfileKey;
	}
}
