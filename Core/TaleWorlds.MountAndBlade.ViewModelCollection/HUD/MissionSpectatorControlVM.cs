using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	public class MissionSpectatorControlVM : ViewModel
	{
		public MissionSpectatorControlVM(Mission mission)
		{
			this._mission = mission;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PrevCharacterText = new TextObject("{=BANC61K5}Previous Character", null).ToString();
			this.NextCharacterText = new TextObject("{=znKxunbQ}Next Character", null).ToString();
			this.UpdateStatusText();
		}

		public void OnSpectatedAgentFocusIn(Agent followedAgent)
		{
			MissionPeer missionPeer = followedAgent.MissionPeer;
			this.SpectatedAgentName = ((missionPeer != null) ? missionPeer.DisplayedName : null) ?? followedAgent.Name;
		}

		public void OnSpectatedAgentFocusOut(Agent followedAgent)
		{
			this.SpectatedAgentName = TextObject.Empty.ToString();
		}

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

		public void SetMainAgentStatus(bool isDead)
		{
			if (this._isMainHeroDead != isDead)
			{
				this._isMainHeroDead = isDead;
				this.UpdateStatusText();
			}
		}

		private void UpdateStatusText()
		{
			if (this._isMainHeroDead)
			{
				this.StatusText = this._deadTextObject.ToString();
				return;
			}
			this.StatusText = string.Empty;
		}

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

		public void SetPrevCharacterInputKey(GameKey gameKey)
		{
			this.PrevCharacterKey = InputKeyItemVM.CreateFromGameKey(gameKey, false);
		}

		public void SetNextCharacterInputKey(GameKey gameKey)
		{
			this.NextCharacterKey = InputKeyItemVM.CreateFromGameKey(gameKey, false);
		}

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

		private readonly Mission _mission;

		private bool _isMainHeroDead;

		private readonly TextObject _deadTextObject = GameTexts.FindText("str_battle_hero_dead", null);

		private bool _isEnabled;

		private string _prevCharacterText;

		private string _nextCharacterText;

		private string _statusText;

		private string _spectatedAgentName;

		private InputKeyItemVM _prevCharacterKey;

		private InputKeyItemVM _nextCharacterKey;
	}
}
