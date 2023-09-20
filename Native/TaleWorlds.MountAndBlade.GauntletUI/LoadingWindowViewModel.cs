using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	public class LoadingWindowViewModel : ViewModel
	{
		public bool CurrentlyShowingMultiplayer { get; private set; }

		public LoadingWindowViewModel(Action<bool, int> handleSPPartialLoading)
		{
			this._handleSPPartialLoading = handleSPPartialLoading;
			Action<bool, int> handleSPPartialLoading2 = this._handleSPPartialLoading;
			if (handleSPPartialLoading2 == null)
			{
				return;
			}
			handleSPPartialLoading2(true, this._currentImage + 1);
		}

		internal void Update()
		{
			if (this.Enabled)
			{
				bool flag = this.IsEligableForMultiplayerLoading();
				if (flag && !this.CurrentlyShowingMultiplayer)
				{
					this.SetForMultiplayer();
					return;
				}
				if (!flag && this.CurrentlyShowingMultiplayer)
				{
					this.SetForEmpty();
				}
			}
		}

		private void HandleEnable()
		{
			if (this.IsEligableForMultiplayerLoading())
			{
				this.SetForMultiplayer();
				return;
			}
			this.SetForEmpty();
		}

		private bool IsEligableForMultiplayerLoading()
		{
			return this._isMultiplayer && Mission.Current != null && Game.Current.GameStateManager.ActiveState is MissionState;
		}

		private void SetForMultiplayer()
		{
			MissionState missionState = (MissionState)Game.Current.GameStateManager.ActiveState;
			string missionName = missionState.MissionName;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(missionName);
			string text;
			if (num <= 1531392713U)
			{
				if (num != 731629611U)
				{
					if (num != 1038375761U)
					{
						if (num == 1531392713U)
						{
							if (missionName == "MultiplayerBattle")
							{
								text = "Battle";
								goto IL_12D;
							}
						}
					}
					else if (missionName == "MultiplayerDuel")
					{
						text = "Duel";
						goto IL_12D;
					}
				}
				else if (missionName == "MultiplayerSkirmish")
				{
					text = "Skirmish";
					goto IL_12D;
				}
			}
			else if (num <= 2065855055U)
			{
				if (num != 1705544657U)
				{
					if (num == 2065855055U)
					{
						if (!(missionName == "MultiplayerFreeForAll"))
						{
						}
					}
				}
				else if (missionName == "MultiplayerTeamDeathmatch")
				{
					text = "TeamDeathmatch";
					goto IL_12D;
				}
			}
			else if (num != 2440237701U)
			{
				if (num == 3434966542U)
				{
					if (missionName == "MultiplayerSiege")
					{
						text = "Siege";
						goto IL_12D;
					}
				}
			}
			else if (missionName == "MultiplayerCaptain")
			{
				text = "Captain";
				goto IL_12D;
			}
			text = missionState.MissionName;
			IL_12D:
			if (!string.IsNullOrEmpty(text))
			{
				this.DescriptionText = GameTexts.FindText("str_multiplayer_official_game_type_explainer", text).ToString();
			}
			else
			{
				this.DescriptionText = "";
			}
			this.GameModeText = GameTexts.FindText("str_multiplayer_official_game_type_name", text).ToString();
			TextObject textObject;
			if (GameTexts.TryGetText("str_multiplayer_scene_name", ref textObject, missionState.CurrentMission.SceneName))
			{
				this.TitleText = textObject.ToString();
			}
			else
			{
				this.TitleText = missionState.CurrentMission.SceneName;
			}
			this.LoadingImageName = missionState.CurrentMission.SceneName;
			this.CurrentlyShowingMultiplayer = true;
		}

		private void SetForEmpty()
		{
			this.DescriptionText = "";
			this.TitleText = "";
			this.GameModeText = "";
			this.SetNextGenericImage();
			this.CurrentlyShowingMultiplayer = false;
		}

		private void SetNextGenericImage()
		{
			int num = ((this._currentImage >= 1) ? this._currentImage : this._totalGenericImageCount);
			this._currentImage = ((this._currentImage < this._totalGenericImageCount) ? (this._currentImage + 1) : 1);
			int num2 = ((this._currentImage < this._totalGenericImageCount) ? (this._currentImage + 1) : 1);
			Action<bool, int> handleSPPartialLoading = this._handleSPPartialLoading;
			if (handleSPPartialLoading != null)
			{
				handleSPPartialLoading(false, num);
			}
			Action<bool, int> handleSPPartialLoading2 = this._handleSPPartialLoading;
			if (handleSPPartialLoading2 != null)
			{
				handleSPPartialLoading2(true, num2);
			}
			this.IsDevelopmentMode = NativeConfig.IsDevelopmentMode;
			this.LoadingImageName = "loading_" + this._currentImage.ToString("00");
		}

		public void SetTotalGenericImageCount(int totalGenericImageCount)
		{
			this._totalGenericImageCount = totalGenericImageCount;
		}

		[DataSourceProperty]
		public bool Enabled
		{
			get
			{
				return this._enabled;
			}
			set
			{
				if (this._enabled != value)
				{
					this._enabled = value;
					base.OnPropertyChangedWithValue(value, "Enabled");
					if (value)
					{
						this.HandleEnable();
					}
				}
			}
		}

		[DataSourceProperty]
		public bool IsDevelopmentMode
		{
			get
			{
				return this._isDevelopmentMode;
			}
			set
			{
				if (this._isDevelopmentMode != value)
				{
					this._isDevelopmentMode = value;
					base.OnPropertyChangedWithValue(value, "IsDevelopmentMode");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (this._titleText != value)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string GameModeText
		{
			get
			{
				return this._gameModeText;
			}
			set
			{
				if (this._gameModeText != value)
				{
					this._gameModeText = value;
					base.OnPropertyChangedWithValue<string>(value, "GameModeText");
				}
			}
		}

		[DataSourceProperty]
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (this._descriptionText != value)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMultiplayer
		{
			get
			{
				return this._isMultiplayer;
			}
			set
			{
				if (this._isMultiplayer != value)
				{
					this._isMultiplayer = value;
					base.OnPropertyChangedWithValue(value, "IsMultiplayer");
				}
			}
		}

		[DataSourceProperty]
		public string LoadingImageName
		{
			get
			{
				return this._loadingImageName;
			}
			set
			{
				if (this._loadingImageName != value)
				{
					this._loadingImageName = value;
					base.OnPropertyChangedWithValue<string>(value, "LoadingImageName");
				}
			}
		}

		private int _currentImage;

		private int _totalGenericImageCount;

		private Action<bool, int> _handleSPPartialLoading;

		private bool _enabled;

		private bool _isDevelopmentMode;

		private bool _isMultiplayer;

		private string _loadingImageName;

		private string _titleText;

		private string _descriptionText;

		private string _gameModeText;
	}
}
