using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.DedicatedCustomServer.ClientHelper
{
	// Token: 0x02000002 RID: 2
	public class DCSHelperMapItemVM : ViewModel
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public DCSHelperMapItemVM(string mapName, Action<DCSHelperMapItemVM> onSelection, bool currentlyPlaying, UniqueSceneId identifiers)
		{
			this._mapName = mapName;
			this._onSelection = onSelection;
			this._currentlyPlaying = currentlyPlaying;
			this._currentlyPlayingText = new TextObject("{=fy9RJLYf}(Currently playing)", null).ToString();
			this._identifiers = identifiers;
			this.LocalMapHint = new BasicTooltipViewModel();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002099 File Offset: 0x00000299
		public void ExecuteToggleSelection()
		{
			Action<DCSHelperMapItemVM> onSelection = this._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020AC File Offset: 0x000002AC
		public void RefreshLocalMapData()
		{
			string text;
			if (Utilities.TryGetFullFilePathOfScene(this.MapName, ref text))
			{
				this.IsSelected = false;
				this.MapPathClean = Path.GetDirectoryName(Path.GetFullPath(text));
				this.ExistsLocally = true;
				UniqueSceneId uniqueSceneId;
				bool flag = Utilities.TryGetUniqueIdentifiersForSceneFile(text, ref uniqueSceneId);
				bool flag2;
				if (this._identifiers != null || flag)
				{
					UniqueSceneId identifiers = this._identifiers;
					flag2 = identifiers == null || !identifiers.Equals(uniqueSceneId);
				}
				else
				{
					flag2 = false;
				}
				this.IsCautionSpriteVisible = flag2;
				return;
			}
			this.MapPathClean = null;
			this.ExistsLocally = false;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000212C File Offset: 0x0000032C
		private List<TooltipProperty> GetTooltipProperties()
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			if (this.IsCautionSpriteVisible)
			{
				list.Add(new TooltipProperty("", new TextObject("{=maLeU9XO}The map played on the server may not be identical to the local version.", null).ToString(), 0, false, 0));
				list.Add(new TooltipProperty("", "", 0, false, 1024));
			}
			if (this.ExistsLocally)
			{
				list.Add(new TooltipProperty("", new TextObject("{=E8bDYaJq}This map already exists at {MAP_PATH}", null).SetTextVariable("MAP_PATH", this.MapPathClean).ToString(), 0, false, 0));
			}
			return list;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000005 RID: 5 RVA: 0x000021C2 File Offset: 0x000003C2
		[DataSourceProperty]
		public string ExclamationMark
		{
			get
			{
				return "!";
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000006 RID: 6 RVA: 0x000021C9 File Offset: 0x000003C9
		// (set) Token: 0x06000007 RID: 7 RVA: 0x000021D1 File Offset: 0x000003D1
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

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000021EF File Offset: 0x000003EF
		// (set) Token: 0x06000009 RID: 9 RVA: 0x000021F8 File Offset: 0x000003F8
		[DataSourceProperty]
		public bool ExistsLocally
		{
			get
			{
				return this._existsLocally;
			}
			set
			{
				if (value != this._existsLocally)
				{
					this._existsLocally = value;
					BasicTooltipViewModel localMapHint = this.LocalMapHint;
					if (localMapHint != null)
					{
						localMapHint.SetToolipCallback(this._existsLocally ? new Func<List<TooltipProperty>>(this.GetTooltipProperties) : null);
					}
					base.OnPropertyChangedWithValue(value, "ExistsLocally");
				}
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002249 File Offset: 0x00000449
		// (set) Token: 0x0600000B RID: 11 RVA: 0x00002251 File Offset: 0x00000451
		[DataSourceProperty]
		public bool IsCautionSpriteVisible
		{
			get
			{
				return this._isCautionSpriteVisible;
			}
			set
			{
				if (value != this._isCautionSpriteVisible)
				{
					this._isCautionSpriteVisible = value;
					base.OnPropertyChangedWithValue(value, "IsCautionSpriteVisible");
				}
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000C RID: 12 RVA: 0x0000226F File Offset: 0x0000046F
		// (set) Token: 0x0600000D RID: 13 RVA: 0x00002277 File Offset: 0x00000477
		[DataSourceProperty]
		public bool CurrentlyPlaying
		{
			get
			{
				return this._currentlyPlaying;
			}
			set
			{
				if (value != this._currentlyPlaying)
				{
					this._currentlyPlaying = value;
					base.OnPropertyChangedWithValue(value, "CurrentlyPlaying");
				}
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002295 File Offset: 0x00000495
		// (set) Token: 0x0600000F RID: 15 RVA: 0x0000229D File Offset: 0x0000049D
		[DataSourceProperty]
		public string CurrentlyPlayingText
		{
			get
			{
				return this._currentlyPlayingText;
			}
			set
			{
				if (value != this._currentlyPlayingText)
				{
					this._currentlyPlayingText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentlyPlayingText");
				}
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000022C0 File Offset: 0x000004C0
		// (set) Token: 0x06000011 RID: 17 RVA: 0x000022C8 File Offset: 0x000004C8
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

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000022EB File Offset: 0x000004EB
		// (set) Token: 0x06000013 RID: 19 RVA: 0x000022F3 File Offset: 0x000004F3
		[DataSourceProperty]
		public string MapPathClean
		{
			get
			{
				return this._mapPathClean;
			}
			set
			{
				if (value != this._mapPathClean)
				{
					this._mapPathClean = value;
					base.OnPropertyChangedWithValue<string>(value, "MapPathClean");
				}
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000014 RID: 20 RVA: 0x00002316 File Offset: 0x00000516
		// (set) Token: 0x06000015 RID: 21 RVA: 0x0000231E File Offset: 0x0000051E
		[DataSourceProperty]
		public BasicTooltipViewModel LocalMapHint
		{
			get
			{
				return this._localMapHint;
			}
			set
			{
				if (value != this._localMapHint)
				{
					this._localMapHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "LocalMapHint");
				}
			}
		}

		// Token: 0x04000001 RID: 1
		private readonly Action<DCSHelperMapItemVM> _onSelection;

		// Token: 0x04000002 RID: 2
		private readonly UniqueSceneId _identifiers;

		// Token: 0x04000003 RID: 3
		private bool _isSelected;

		// Token: 0x04000004 RID: 4
		private bool _existsLocally;

		// Token: 0x04000005 RID: 5
		private bool _isCautionSpriteVisible;

		// Token: 0x04000006 RID: 6
		private bool _currentlyPlaying;

		// Token: 0x04000007 RID: 7
		private string _currentlyPlayingText;

		// Token: 0x04000008 RID: 8
		private string _mapName;

		// Token: 0x04000009 RID: 9
		private string _mapPathClean;

		// Token: 0x0400000A RID: 10
		private BasicTooltipViewModel _localMapHint;
	}
}
