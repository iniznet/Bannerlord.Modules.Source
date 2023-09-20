using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x02000013 RID: 19
	public class LoadingWindowViewModel : ViewModel
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00004F82 File Offset: 0x00003182
		// (set) Token: 0x06000085 RID: 133 RVA: 0x00004F8A File Offset: 0x0000318A
		public bool CurrentlyShowingMultiplayer { get; private set; }

		// Token: 0x06000086 RID: 134 RVA: 0x00004F93 File Offset: 0x00003193
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

		// Token: 0x06000087 RID: 135 RVA: 0x00004FBC File Offset: 0x000031BC
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

		// Token: 0x06000088 RID: 136 RVA: 0x00004FFB File Offset: 0x000031FB
		private void HandleEnable()
		{
			if (this.IsEligableForMultiplayerLoading())
			{
				this.SetForMultiplayer();
				return;
			}
			this.SetForEmpty();
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005012 File Offset: 0x00003212
		private bool IsEligableForMultiplayerLoading()
		{
			return this._isMultiplayer && Mission.Current != null && Game.Current.GameStateManager.ActiveState is MissionState;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x0000503C File Offset: 0x0000323C
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

		// Token: 0x0600008B RID: 139 RVA: 0x00005207 File Offset: 0x00003407
		private void SetForEmpty()
		{
			this.DescriptionText = "";
			this.TitleText = "";
			this.GameModeText = "";
			this.SetNextGenericImage();
			this.CurrentlyShowingMultiplayer = false;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00005238 File Offset: 0x00003438
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

		// Token: 0x0600008D RID: 141 RVA: 0x000052E7 File Offset: 0x000034E7
		public void SetTotalGenericImageCount(int totalGenericImageCount)
		{
			this._totalGenericImageCount = totalGenericImageCount;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600008E RID: 142 RVA: 0x000052F0 File Offset: 0x000034F0
		// (set) Token: 0x0600008F RID: 143 RVA: 0x000052F8 File Offset: 0x000034F8
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

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000090 RID: 144 RVA: 0x0000531F File Offset: 0x0000351F
		// (set) Token: 0x06000091 RID: 145 RVA: 0x00005327 File Offset: 0x00003527
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

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00005345 File Offset: 0x00003545
		// (set) Token: 0x06000093 RID: 147 RVA: 0x0000534D File Offset: 0x0000354D
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

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00005370 File Offset: 0x00003570
		// (set) Token: 0x06000095 RID: 149 RVA: 0x00005378 File Offset: 0x00003578
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

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000096 RID: 150 RVA: 0x0000539B File Offset: 0x0000359B
		// (set) Token: 0x06000097 RID: 151 RVA: 0x000053A3 File Offset: 0x000035A3
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

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000098 RID: 152 RVA: 0x000053C6 File Offset: 0x000035C6
		// (set) Token: 0x06000099 RID: 153 RVA: 0x000053CE File Offset: 0x000035CE
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

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600009A RID: 154 RVA: 0x000053EC File Offset: 0x000035EC
		// (set) Token: 0x0600009B RID: 155 RVA: 0x000053F4 File Offset: 0x000035F4
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

		// Token: 0x0400005A RID: 90
		private int _currentImage;

		// Token: 0x0400005B RID: 91
		private int _totalGenericImageCount;

		// Token: 0x0400005C RID: 92
		private Action<bool, int> _handleSPPartialLoading;

		// Token: 0x0400005E RID: 94
		private bool _enabled;

		// Token: 0x0400005F RID: 95
		private bool _isDevelopmentMode;

		// Token: 0x04000060 RID: 96
		private bool _isMultiplayer;

		// Token: 0x04000061 RID: 97
		private string _loadingImageName;

		// Token: 0x04000062 RID: 98
		private string _titleText;

		// Token: 0x04000063 RID: 99
		private string _descriptionText;

		// Token: 0x04000064 RID: 100
		private string _gameModeText;
	}
}
