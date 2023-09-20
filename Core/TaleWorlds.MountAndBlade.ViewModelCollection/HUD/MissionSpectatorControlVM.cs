using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	// Token: 0x020000E3 RID: 227
	public class MissionSpectatorControlVM : ViewModel
	{
		// Token: 0x060014AF RID: 5295 RVA: 0x00043B31 File Offset: 0x00041D31
		public MissionSpectatorControlVM(Mission mission)
		{
			this._mission = mission;
			this.RefreshValues();
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x00043B57 File Offset: 0x00041D57
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PrevCharacterText = new TextObject("{=BANC61K5}Previous Character", null).ToString();
			this.NextCharacterText = new TextObject("{=znKxunbQ}Next Character", null).ToString();
			this.UpdateStatusText();
		}

		// Token: 0x060014B1 RID: 5297 RVA: 0x00043B91 File Offset: 0x00041D91
		public void OnSpectatedAgentFocusIn(Agent followedAgent)
		{
			MissionPeer missionPeer = followedAgent.MissionPeer;
			this.SpectatedAgentName = ((missionPeer != null) ? missionPeer.DisplayedName : null) ?? followedAgent.Name;
		}

		// Token: 0x060014B2 RID: 5298 RVA: 0x00043BB5 File Offset: 0x00041DB5
		public void OnSpectatedAgentFocusOut(Agent followedAgent)
		{
			this.SpectatedAgentName = TextObject.Empty.ToString();
		}

		// Token: 0x060014B3 RID: 5299 RVA: 0x00043BC7 File Offset: 0x00041DC7
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM prevCharacterKey = this.PrevCharacterKey;
			if (prevCharacterKey != null)
			{
				prevCharacterKey.OnFinalize();
			}
			InputKeyItemVM nextCharacterKey = this.NextCharacterKey;
			if (nextCharacterKey == null)
			{
				return;
			}
			nextCharacterKey.OnFinalize();
		}

		// Token: 0x060014B4 RID: 5300 RVA: 0x00043BF0 File Offset: 0x00041DF0
		public void SetMainAgentStatus(bool isDead)
		{
			if (this._isMainHeroDead != isDead)
			{
				this._isMainHeroDead = isDead;
				this.UpdateStatusText();
			}
		}

		// Token: 0x060014B5 RID: 5301 RVA: 0x00043C08 File Offset: 0x00041E08
		private void UpdateStatusText()
		{
			if (this._isMainHeroDead)
			{
				this.StatusText = this._deadTextObject.ToString();
				return;
			}
			this.StatusText = string.Empty;
		}

		// Token: 0x170006CE RID: 1742
		// (get) Token: 0x060014B6 RID: 5302 RVA: 0x00043C2F File Offset: 0x00041E2F
		// (set) Token: 0x060014B7 RID: 5303 RVA: 0x00043C37 File Offset: 0x00041E37
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

		// Token: 0x170006CF RID: 1743
		// (get) Token: 0x060014B8 RID: 5304 RVA: 0x00043C55 File Offset: 0x00041E55
		// (set) Token: 0x060014B9 RID: 5305 RVA: 0x00043C5D File Offset: 0x00041E5D
		[DataSourceProperty]
		public string PrevCharacterText
		{
			get
			{
				return this._prevCharacterText;
			}
			set
			{
				if (value != this._prevCharacterText)
				{
					this._prevCharacterText = value;
					base.OnPropertyChangedWithValue<string>(value, "PrevCharacterText");
				}
			}
		}

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x060014BA RID: 5306 RVA: 0x00043C80 File Offset: 0x00041E80
		// (set) Token: 0x060014BB RID: 5307 RVA: 0x00043C88 File Offset: 0x00041E88
		[DataSourceProperty]
		public string NextCharacterText
		{
			get
			{
				return this._nextCharacterText;
			}
			set
			{
				if (value != this._nextCharacterText)
				{
					this._nextCharacterText = value;
					base.OnPropertyChangedWithValue<string>(value, "NextCharacterText");
				}
			}
		}

		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x060014BC RID: 5308 RVA: 0x00043CAB File Offset: 0x00041EAB
		// (set) Token: 0x060014BD RID: 5309 RVA: 0x00043CB3 File Offset: 0x00041EB3
		[DataSourceProperty]
		public string StatusText
		{
			get
			{
				return this._statusText;
			}
			set
			{
				if (value != this._statusText)
				{
					this._statusText = value;
					base.OnPropertyChangedWithValue<string>(value, "StatusText");
				}
			}
		}

		// Token: 0x060014BE RID: 5310 RVA: 0x00043CD6 File Offset: 0x00041ED6
		public void SetPrevCharacterInputKey(GameKey gameKey)
		{
			this.PrevCharacterKey = InputKeyItemVM.CreateFromGameKey(gameKey, false);
		}

		// Token: 0x060014BF RID: 5311 RVA: 0x00043CE5 File Offset: 0x00041EE5
		public void SetNextCharacterInputKey(GameKey gameKey)
		{
			this.NextCharacterKey = InputKeyItemVM.CreateFromGameKey(gameKey, false);
		}

		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x060014C0 RID: 5312 RVA: 0x00043CF4 File Offset: 0x00041EF4
		// (set) Token: 0x060014C1 RID: 5313 RVA: 0x00043CFC File Offset: 0x00041EFC
		[DataSourceProperty]
		public string SpectatedAgentName
		{
			get
			{
				return this._spectatedAgentName;
			}
			set
			{
				if (value != this._spectatedAgentName)
				{
					this._spectatedAgentName = value;
					base.OnPropertyChangedWithValue<string>(value, "SpectatedAgentName");
				}
			}
		}

		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x060014C2 RID: 5314 RVA: 0x00043D1F File Offset: 0x00041F1F
		// (set) Token: 0x060014C3 RID: 5315 RVA: 0x00043D27 File Offset: 0x00041F27
		[DataSourceProperty]
		public InputKeyItemVM PrevCharacterKey
		{
			get
			{
				return this._prevCharacterKey;
			}
			set
			{
				if (value != this._prevCharacterKey)
				{
					this._prevCharacterKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "PrevCharacterKey");
				}
			}
		}

		// Token: 0x170006D4 RID: 1748
		// (get) Token: 0x060014C4 RID: 5316 RVA: 0x00043D45 File Offset: 0x00041F45
		// (set) Token: 0x060014C5 RID: 5317 RVA: 0x00043D4D File Offset: 0x00041F4D
		[DataSourceProperty]
		public InputKeyItemVM NextCharacterKey
		{
			get
			{
				return this._nextCharacterKey;
			}
			set
			{
				if (value != this._nextCharacterKey)
				{
					this._nextCharacterKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "NextCharacterKey");
				}
			}
		}

		// Token: 0x040009E6 RID: 2534
		private readonly Mission _mission;

		// Token: 0x040009E7 RID: 2535
		private bool _isMainHeroDead;

		// Token: 0x040009E8 RID: 2536
		private readonly TextObject _deadTextObject = GameTexts.FindText("str_battle_hero_dead", null);

		// Token: 0x040009E9 RID: 2537
		private bool _isEnabled;

		// Token: 0x040009EA RID: 2538
		private string _prevCharacterText;

		// Token: 0x040009EB RID: 2539
		private string _nextCharacterText;

		// Token: 0x040009EC RID: 2540
		private string _statusText;

		// Token: 0x040009ED RID: 2541
		private string _spectatedAgentName;

		// Token: 0x040009EE RID: 2542
		private InputKeyItemVM _prevCharacterKey;

		// Token: 0x040009EF RID: 2543
		private InputKeyItemVM _nextCharacterKey;
	}
}
