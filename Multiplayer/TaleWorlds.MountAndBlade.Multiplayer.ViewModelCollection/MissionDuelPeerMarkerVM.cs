using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.ClassLoadout;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection
{
	public class MissionDuelPeerMarkerVM : ViewModel
	{
		public MissionPeer TargetPeer { get; private set; }

		public float Distance { get; private set; }

		public bool IsInDuel { get; private set; }

		public MissionDuelPeerMarkerVM(MissionPeer peer)
		{
			this.TargetPeer = peer;
			this.Bounty = (peer.Representative as DuelMissionRepresentative).Bounty;
			this.IsEnabled = true;
			TargetIconType iconType = MultiplayerClassDivisions.GetMPHeroClassForPeer(this.TargetPeer, false).IconType;
			this.CompassElement = new MPTeammateCompassTargetVM(iconType, Color.White.ToUnsignedInteger(), Color.White.ToUnsignedInteger(), BannerCode.CreateFrom(new Banner()), true);
			this.SelectedPerks = new MBBindingList<MPPerkVM>();
			this.RefreshPerkSelection();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.TargetPeer.DisplayedName;
			this._acceptDuelRequestText = new TextObject("{=tidE1V1k}Accept duel", null);
			this._sendDuelRequestText = new TextObject("{=YLPJWgqF}Challenge", null);
			this._waitingForDuelResponseText = new TextObject("{=MPgnsZoo}Waiting for response", null);
		}

		public void OnTick(float dt)
		{
			if (Agent.Main != null && this.TargetPeer.ControlledAgent != null)
			{
				this.Distance = this._latestW;
			}
			if (this.HasSentDuelRequest)
			{
				this._currentDuelRequestTimeRemaining -= dt;
				GameTexts.SetVariable("SECONDS", (int)this._currentDuelRequestTimeRemaining);
				GameTexts.SetVariable("ACTION", this._waitingForDuelResponseText);
				this.ActionDescriptionText = new TextObject("{=HXWpxvgT}{ACTION} ({SECONDS})", null).ToString();
				if (this._currentDuelRequestTimeRemaining <= 0f)
				{
					this.HasSentDuelRequest = false;
				}
			}
		}

		public void UpdateScreenPosition(Camera missionCamera)
		{
			if (this.TargetPeer.ControlledAgent == null)
			{
				return;
			}
			Vec3 vec = this.TargetPeer.ControlledAgent.GetWorldPosition().GetGroundVec3();
			vec += new Vec3(0f, 0f, this.TargetPeer.ControlledAgent.GetEyeGlobalHeight(), -1f);
			this._latestX = 0f;
			this._latestY = 0f;
			this._latestW = 0f;
			MBWindowManager.WorldToScreen(missionCamera, vec, ref this._latestX, ref this._latestY, ref this._latestW);
			this.ScreenPosition = new Vec2(this._latestX, this._latestY);
			this.IsAgentInScreenBoundaries = this._latestX <= Screen.RealScreenResolutionWidth && this._latestY <= Screen.RealScreenResolutionHeight && this._latestX + 200f >= 0f && this._latestY + 100f >= 0f;
			this._wPosAfterPositionCalculation = ((this._latestW < 0f) ? (-1f) : 1.1f);
			this.WSign = (int)this._wPosAfterPositionCalculation;
		}

		private void OnInteractionChanged()
		{
			this.ActionDescriptionText = "";
			if (this.HasDuelRequestForPlayer)
			{
				string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13));
				GameTexts.SetVariable("KEY", keyHyperlinkText);
				GameTexts.SetVariable("ACTION", this._acceptDuelRequestText);
				this.ActionDescriptionText = GameTexts.FindText("str_key_action", null).ToString();
				return;
			}
			if (this.HasSentDuelRequest)
			{
				this._currentDuelRequestTimeRemaining = 10f;
			}
		}

		private void SetFocused(bool isFocused)
		{
			if (!this.HasDuelRequestForPlayer && !this.HasSentDuelRequest)
			{
				if (isFocused)
				{
					string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13));
					GameTexts.SetVariable("KEY", keyHyperlinkText);
					GameTexts.SetVariable("ACTION", this._sendDuelRequestText);
					this.ActionDescriptionText = GameTexts.FindText("str_key_action", null).ToString();
					return;
				}
				this.ActionDescriptionText = string.Empty;
			}
		}

		public void UpdateBounty()
		{
			this.Bounty = (this.TargetPeer.Representative as DuelMissionRepresentative).Bounty;
		}

		private void UpdateTracked()
		{
			if (!this.IsEnabled)
			{
				this.IsTracked = false;
			}
			else if (this.HasDuelRequestForPlayer || this.HasSentDuelRequest || this.IsFocused)
			{
				this.IsTracked = true;
			}
			else
			{
				this.IsTracked = false;
			}
			this.ShouldShowInformation = this.IsTracked || this.IsFocused;
		}

		public void OnDuelStarted()
		{
			this.IsEnabled = false;
			this.IsInDuel = true;
		}

		public void OnDuelEnded()
		{
			this.IsEnabled = true;
			this.IsInDuel = false;
		}

		public void UpdateCurentDuelStatus(bool isInDuel)
		{
			this.IsInDuel = isInDuel;
			this.IsEnabled = !this.IsInDuel;
		}

		public void RefreshPerkSelection()
		{
			this.SelectedPerks.Clear();
			this.TargetPeer.RefreshSelectedPerks();
			foreach (MPPerkObject mpperkObject in this.TargetPeer.SelectedPerks)
			{
				this.SelectedPerks.Add(new MPPerkVM(null, mpperkObject, true, 0));
			}
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
					this.UpdateTracked();
				}
			}
		}

		[DataSourceProperty]
		public bool IsTracked
		{
			get
			{
				return this._isTracked;
			}
			set
			{
				if (value != this._isTracked)
				{
					this._isTracked = value;
					base.OnPropertyChangedWithValue(value, "IsTracked");
				}
			}
		}

		[DataSourceProperty]
		public bool ShouldShowInformation
		{
			get
			{
				return this._shouldShowInformation;
			}
			set
			{
				if (value != this._shouldShowInformation)
				{
					this._shouldShowInformation = value;
					base.OnPropertyChangedWithValue(value, "ShouldShowInformation");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAgentInScreenBoundaries
		{
			get
			{
				return this._isAgentInScreenBoundaries;
			}
			set
			{
				if (value != this._isAgentInScreenBoundaries)
				{
					this._isAgentInScreenBoundaries = value;
					base.OnPropertyChangedWithValue(value, "IsAgentInScreenBoundaries");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFocused
		{
			get
			{
				return this._isFocused;
			}
			set
			{
				if (value != this._isFocused)
				{
					this._isFocused = value;
					base.OnPropertyChangedWithValue(value, "IsFocused");
					this.SetFocused(value);
					this.UpdateTracked();
				}
			}
		}

		[DataSourceProperty]
		public bool HasDuelRequestForPlayer
		{
			get
			{
				return this._hasDuelRequestForPlayer;
			}
			set
			{
				if (value != this._hasDuelRequestForPlayer)
				{
					this._hasDuelRequestForPlayer = value;
					base.OnPropertyChangedWithValue(value, "HasDuelRequestForPlayer");
					this.OnInteractionChanged();
					this.UpdateTracked();
				}
			}
		}

		[DataSourceProperty]
		public bool HasSentDuelRequest
		{
			get
			{
				return this._hasSentDuelRequest;
			}
			set
			{
				if (value != this._hasSentDuelRequest)
				{
					this._hasSentDuelRequest = value;
					base.OnPropertyChangedWithValue(value, "HasSentDuelRequest");
					this.OnInteractionChanged();
					this.UpdateTracked();
				}
			}
		}

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

		[DataSourceProperty]
		public string ActionDescriptionText
		{
			get
			{
				return this._actionDescriptionText;
			}
			set
			{
				if (value != this._actionDescriptionText)
				{
					this._actionDescriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "ActionDescriptionText");
				}
			}
		}

		[DataSourceProperty]
		public int Bounty
		{
			get
			{
				return this._bounty;
			}
			set
			{
				if (value != this._bounty)
				{
					this._bounty = value;
					base.OnPropertyChangedWithValue(value, "Bounty");
				}
			}
		}

		[DataSourceProperty]
		public int PreferredArenaType
		{
			get
			{
				return this._preferredArenaType;
			}
			set
			{
				if (value != this._preferredArenaType)
				{
					this._preferredArenaType = value;
					base.OnPropertyChangedWithValue(value, "PreferredArenaType");
				}
			}
		}

		[DataSourceProperty]
		public int WSign
		{
			get
			{
				return this._wSign;
			}
			set
			{
				if (value != this._wSign)
				{
					this._wSign = value;
					base.OnPropertyChangedWithValue(value, "WSign");
				}
			}
		}

		[DataSourceProperty]
		public Vec2 ScreenPosition
		{
			get
			{
				return this._screenPosition;
			}
			set
			{
				if (value.x != this._screenPosition.x || value.y != this._screenPosition.y)
				{
					this._screenPosition = value;
					base.OnPropertyChangedWithValue(value, "ScreenPosition");
				}
			}
		}

		[DataSourceProperty]
		public MPTeammateCompassTargetVM CompassElement
		{
			get
			{
				return this._compassElement;
			}
			set
			{
				if (value != this._compassElement)
				{
					this._compassElement = value;
					base.OnPropertyChangedWithValue<MPTeammateCompassTargetVM>(value, "CompassElement");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPPerkVM> SelectedPerks
		{
			get
			{
				return this._selectedPerks;
			}
			set
			{
				if (value != this._selectedPerks)
				{
					this._selectedPerks = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPPerkVM>>(value, "SelectedPerks");
				}
			}
		}

		private float _currentDuelRequestTimeRemaining;

		private float _latestX;

		private float _latestY;

		private float _latestW;

		private float _wPosAfterPositionCalculation;

		private TextObject _acceptDuelRequestText;

		private TextObject _sendDuelRequestText;

		private TextObject _waitingForDuelResponseText;

		private bool _isEnabled;

		private bool _isTracked;

		private bool _shouldShowInformation;

		private bool _isAgentInScreenBoundaries;

		private bool _isFocused;

		private bool _hasDuelRequestForPlayer;

		private bool _hasSentDuelRequest;

		private string _name;

		private string _actionDescriptionText;

		private int _bounty;

		private int _preferredArenaType;

		private int _wSign;

		private Vec2 _screenPosition;

		private MPTeammateCompassTargetVM _compassElement;

		private MBBindingList<MPPerkVM> _selectedPerks;
	}
}
