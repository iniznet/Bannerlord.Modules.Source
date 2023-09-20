using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection
{
	public class MPDeathCardVM : ViewModel
	{
		public MPDeathCardVM(MultiplayerGameType gameType)
		{
			this.KillCountsEnabled = gameType != 5;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.YouText = GameTexts.FindText("str_death_card_you", null).ToString();
			this.Deactivate();
		}

		public void OnMainAgentRemoved(Agent affectorAgent, KillingBlow blow)
		{
			this.ResetProperties();
			if (affectorAgent != null && affectorAgent == Agent.Main)
			{
				this.TitleText = this._killedSelfText.ToString();
				this.IsSelfInflicted = true;
			}
			else if (affectorAgent != null && affectorAgent.IsMount && affectorAgent.RiderAgent == null)
			{
				this._killedByStrayHorse.SetTextVariable("MOUNT_NAME", affectorAgent.Name);
				this.TitleText = this._killedByStrayHorse.ToString();
				this.IsSelfInflicted = true;
			}
			else
			{
				this.IsSelfInflicted = false;
				this.TitleText = this._killedByText.ToString();
			}
			Team team = ((affectorAgent != null) ? affectorAgent.Team : null);
			Agent main = Agent.Main;
			this.KillerText = ((team == ((main != null) ? main.Team : null)) ? this._allyText.ToString() : this._enemyText.ToString());
			if (this.IsSelfInflicted)
			{
				this.PlayerProperties = new MPPlayerVM(PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer));
				this.PlayerProperties.RefreshDivision(false);
			}
			else
			{
				this.KillerName = ((affectorAgent != null) ? affectorAgent.Name : null) ?? "";
				if (blow.WeaponItemKind >= 0)
				{
					this.UsedWeaponName = ItemObject.GetItemFromWeaponKind(blow.WeaponItemKind).Name.ToString();
				}
				else
				{
					this.UsedWeaponName = new TextObject("{=GAZ5QLZi}Unarmed", null).ToString();
				}
				bool isServerOrRecorder = GameNetwork.IsServerOrRecorder;
				if (((affectorAgent != null) ? affectorAgent.MissionPeer : null) != null)
				{
					this.PlayerProperties = new MPPlayerVM(affectorAgent.MissionPeer);
					this.PlayerProperties.RefreshDivision(false);
					this.NumOfTimesPlayerKilled = Agent.Main.MissionPeer.GetNumberOfTimesPeerKilledPeer(affectorAgent.MissionPeer);
					this.NumOfTimesPlayerGotKilled = affectorAgent.MissionPeer.GetNumberOfTimesPeerKilledPeer(Agent.Main.MissionPeer) + (isServerOrRecorder ? 0 : 1);
				}
				else if (((affectorAgent != null) ? affectorAgent.OwningAgentMissionPeer : null) != null)
				{
					this.PlayerProperties = new MPPlayerVM(affectorAgent.OwningAgentMissionPeer);
					this.PlayerProperties.RefreshDivision(false);
				}
				else
				{
					this.PlayerProperties = new MPPlayerVM(affectorAgent);
				}
			}
			this.IsActive = true;
		}

		private void ResetProperties()
		{
			this.IsActive = false;
			this.TitleText = "";
			this.UsedWeaponName = "";
			this.BodyPartHit = -1;
		}

		public void Deactivate()
		{
			this.IsActive = false;
		}

		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChangedWithValue(value, "IsActive");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelfInflicted
		{
			get
			{
				return this._isSelfInflicted;
			}
			set
			{
				if (value != this._isSelfInflicted)
				{
					this._isSelfInflicted = value;
					base.OnPropertyChangedWithValue(value, "IsSelfInflicted");
				}
			}
		}

		[DataSourceProperty]
		public bool KillCountsEnabled
		{
			get
			{
				return this._killCountsEnabled;
			}
			set
			{
				if (value != this._killCountsEnabled)
				{
					this._killCountsEnabled = value;
					base.OnPropertyChangedWithValue(value, "KillCountsEnabled");
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
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string UsedWeaponName
		{
			get
			{
				return this._usedWeaponName;
			}
			set
			{
				if (value != this._usedWeaponName)
				{
					this._usedWeaponName = value;
					base.OnPropertyChangedWithValue<string>(value, "UsedWeaponName");
				}
			}
		}

		[DataSourceProperty]
		public string KillerName
		{
			get
			{
				return this._killerName;
			}
			set
			{
				if (value != this._killerName)
				{
					this._killerName = value;
					base.OnPropertyChangedWithValue<string>(value, "KillerName");
				}
			}
		}

		[DataSourceProperty]
		public string KillerText
		{
			get
			{
				return this._killerText;
			}
			set
			{
				if (value != this._killerText)
				{
					this._killerText = value;
					base.OnPropertyChangedWithValue<string>(value, "KillerText");
				}
			}
		}

		[DataSourceProperty]
		public string YouText
		{
			get
			{
				return this._youText;
			}
			set
			{
				if (value != this._youText)
				{
					this._youText = value;
					base.OnPropertyChangedWithValue<string>(value, "YouText");
				}
			}
		}

		[DataSourceProperty]
		public MPPlayerVM PlayerProperties
		{
			get
			{
				return this._playerProperties;
			}
			set
			{
				if (value != this._playerProperties)
				{
					this._playerProperties = value;
					base.OnPropertyChangedWithValue<MPPlayerVM>(value, "PlayerProperties");
				}
			}
		}

		[DataSourceProperty]
		public int BodyPartHit
		{
			get
			{
				return this._bodyPartHit;
			}
			set
			{
				if (value != this._bodyPartHit)
				{
					this._bodyPartHit = value;
					base.OnPropertyChangedWithValue(value, "BodyPartHit");
				}
			}
		}

		[DataSourceProperty]
		public int NumOfTimesPlayerKilled
		{
			get
			{
				return this._numOfTimesPlayerKilled;
			}
			set
			{
				if (value != this._numOfTimesPlayerKilled)
				{
					this._numOfTimesPlayerKilled = value;
					base.OnPropertyChangedWithValue(value, "NumOfTimesPlayerKilled");
				}
			}
		}

		[DataSourceProperty]
		public int NumOfTimesPlayerGotKilled
		{
			get
			{
				return this._numOfTimesPlayerGotKilled;
			}
			set
			{
				if (value != this._numOfTimesPlayerGotKilled)
				{
					this._numOfTimesPlayerGotKilled = value;
					base.OnPropertyChangedWithValue(value, "NumOfTimesPlayerGotKilled");
				}
			}
		}

		private readonly TextObject _killedByStrayHorse = GameTexts.FindText("str_killed_by_stray_horse", null);

		private readonly TextObject _killedSelfText = GameTexts.FindText("str_killed_self", null);

		private readonly TextObject _killedByText = GameTexts.FindText("str_killed_by", null);

		private readonly TextObject _enemyText = GameTexts.FindText("str_death_card_enemy", null);

		private readonly TextObject _allyText = GameTexts.FindText("str_death_card_ally", null);

		private bool _isActive;

		private bool _isSelfInflicted;

		private bool _killCountsEnabled;

		private int _numOfTimesPlayerKilled;

		private int _numOfTimesPlayerGotKilled;

		private string _titleText;

		private string _usedWeaponName;

		private string _killerName;

		private string _killerText;

		private string _youText;

		private MPPlayerVM _playerProperties;

		private int _bodyPartHit;
	}
}
