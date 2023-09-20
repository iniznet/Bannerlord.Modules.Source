using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.DedicatedCustomServer.ClientHelper
{
	public class DCSHelperMapItemVM : ViewModel
	{
		public DCSHelperMapItemVM(string mapName, Action<DCSHelperMapItemVM> onSelection, bool currentlyPlaying, UniqueSceneId identifiers)
		{
			this._mapName = mapName;
			this._onSelection = onSelection;
			this._currentlyPlaying = currentlyPlaying;
			this._currentlyPlayingText = new TextObject("{=fy9RJLYf}(Currently playing)", null).ToString();
			this._identifiers = identifiers;
			this.LocalMapHint = new BasicTooltipViewModel();
		}

		public void ExecuteToggleSelection()
		{
			Action<DCSHelperMapItemVM> onSelection = this._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
		}

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

		[DataSourceProperty]
		public string ExclamationMark
		{
			get
			{
				return "!";
			}
		}

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

		private readonly Action<DCSHelperMapItemVM> _onSelection;

		private readonly UniqueSceneId _identifiers;

		private bool _isSelected;

		private bool _existsLocally;

		private bool _isCautionSpriteVisible;

		private bool _currentlyPlaying;

		private string _currentlyPlayingText;

		private string _mapName;

		private string _mapPathClean;

		private BasicTooltipViewModel _localMapHint;
	}
}
