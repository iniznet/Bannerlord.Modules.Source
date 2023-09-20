using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.Map
{
	public class MobilePartyTrackItemVM : ViewModel
	{
		public MobileParty TrackedParty { get; private set; }

		public Army TrackedArmy { get; private set; }

		private MobileParty _concernedMobileParty
		{
			get
			{
				Army trackedArmy = this.TrackedArmy;
				return ((trackedArmy != null) ? trackedArmy.LeaderParty : null) ?? this.TrackedParty;
			}
		}

		public MobilePartyTrackItemVM(MobileParty trackedParty, Camera mapCamera, Action<Vec2> fastMoveCameraToPosition)
		{
			this._mapCamera = mapCamera;
			this._fastMoveCameraToPosition = fastMoveCameraToPosition;
			this.TrackedParty = trackedParty;
			this.IsTracked = Campaign.Current.VisualTrackerManager.CheckTracked(this._concernedMobileParty);
			this.UpdateProperties();
			this.IsArmy = false;
		}

		public MobilePartyTrackItemVM(Army trackedArmy, Camera mapCamera, Action<Vec2> fastMoveCameraToPosition)
		{
			this._mapCamera = mapCamera;
			this._fastMoveCameraToPosition = fastMoveCameraToPosition;
			this.TrackedArmy = trackedArmy;
			this.IsTracked = Campaign.Current.VisualTrackerManager.CheckTracked(this._concernedMobileParty);
			this.UpdateProperties();
			this.IsArmy = true;
		}

		internal void UpdateProperties()
		{
			if (this.TrackedArmy != null)
			{
				Army trackedArmy = this.TrackedArmy;
				this._nameBind = ((trackedArmy != null) ? trackedArmy.Name.ToString() : null);
			}
			else if (this.TrackedParty != null)
			{
				if (this.TrackedParty.IsCaravan && this.TrackedParty.LeaderHero != null)
				{
					Hero leaderHero = this.TrackedParty.LeaderHero;
					this._nameBind = ((leaderHero != null) ? leaderHero.Name.ToString() : null);
				}
				else
				{
					this._nameBind = this.TrackedParty.Name.ToString();
				}
			}
			else
			{
				this._nameBind = "";
			}
			this._isVisibleOnMapBind = this.GetIsVisibleOnMap();
			Hero leaderHero2 = this._concernedMobileParty.LeaderHero;
			if (((leaderHero2 != null) ? leaderHero2.Clan : null) != null)
			{
				this._factionVisualBind = new ImageIdentifierVM(BannerCode.CreateFrom(this._concernedMobileParty.LeaderHero.Clan.Banner), true);
				return;
			}
			IFaction mapFaction = this._concernedMobileParty.MapFaction;
			this._factionVisualBind = new ImageIdentifierVM(BannerCode.CreateFrom((mapFaction != null) ? mapFaction.Banner : null), true);
		}

		private bool GetIsVisibleOnMap()
		{
			MobileParty concernedMobileParty = this._concernedMobileParty;
			return (concernedMobileParty == null || !concernedMobileParty.IsVisible) && (this.TrackedArmy != null || (this.TrackedParty != null && this.TrackedParty.IsActive && this.TrackedParty.AttachedTo == null));
		}

		internal void UpdatePosition()
		{
			if (this._concernedMobileParty != null)
			{
				Vec3 vec = this._concernedMobileParty.GetVisualPosition() + new Vec3(0f, 0f, 1f, -1f);
				this._latestX = 0f;
				this._latestY = 0f;
				this._latestW = 0f;
				MBWindowManager.WorldToScreenInsideUsableArea(this._mapCamera, vec, ref this._latestX, ref this._latestY, ref this._latestW);
				this._partyPositionBind = new Vec2(this._latestX, this._latestY);
				this._isBehindBind = this._latestW < 0f;
			}
		}

		public void ExecuteToggleTrack()
		{
			if (this.IsTracked)
			{
				this.Untrack();
				return;
			}
			this.Track();
		}

		private void Track()
		{
			this.IsTracked = true;
			if (!Campaign.Current.VisualTrackerManager.CheckTracked(this._concernedMobileParty))
			{
				Campaign.Current.VisualTrackerManager.RegisterObject(this._concernedMobileParty);
			}
		}

		private void Untrack()
		{
			this.IsTracked = false;
			if (Campaign.Current.VisualTrackerManager.CheckTracked(this._concernedMobileParty))
			{
				Campaign.Current.VisualTrackerManager.RemoveTrackedObject(this._concernedMobileParty, false);
			}
		}

		public void ExecuteGoToPosition()
		{
			if (this._concernedMobileParty != null)
			{
				Action<Vec2> fastMoveCameraToPosition = this._fastMoveCameraToPosition;
				if (fastMoveCameraToPosition == null)
				{
					return;
				}
				fastMoveCameraToPosition(this._concernedMobileParty.GetLogicalPosition().AsVec2);
			}
		}

		public void ExecuteShowTooltip()
		{
			if (this.TrackedArmy != null)
			{
				InformationManager.ShowTooltip(typeof(Army), new object[] { this.TrackedArmy, true, false });
				return;
			}
			if (this.TrackedParty != null)
			{
				InformationManager.ShowTooltip(typeof(MobileParty), new object[] { this.TrackedParty, true, false });
			}
		}

		public void ExecuteHideTooltip()
		{
			MBInformationManager.HideInformations();
		}

		public void RefreshBinding()
		{
			this.PartyPosition = this._partyPositionBind;
			this.Name = this._nameBind;
			this.IsEnabled = this._isVisibleOnMapBind;
			this.IsBehind = this._isBehindBind;
			this.FactionVisual = this._factionVisualBind;
		}

		public Vec2 PartyPosition
		{
			get
			{
				return this._partyPosition;
			}
			set
			{
				if (value != this._partyPosition)
				{
					this._partyPosition = value;
					base.OnPropertyChangedWithValue(value, "PartyPosition");
				}
			}
		}

		public ImageIdentifierVM FactionVisual
		{
			get
			{
				return this._factionVisual;
			}
			set
			{
				if (value != this._factionVisual)
				{
					this._factionVisual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "FactionVisual");
				}
			}
		}

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

		public bool IsArmy
		{
			get
			{
				return this._isArmy;
			}
			set
			{
				if (value != this._isArmy)
				{
					this._isArmy = value;
					base.OnPropertyChangedWithValue(value, "IsArmy");
				}
			}
		}

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

		public bool IsBehind
		{
			get
			{
				return this._isBehind;
			}
			set
			{
				if (value != this._isBehind)
				{
					this._isBehind = value;
					base.OnPropertyChangedWithValue(value, "IsBehind");
				}
			}
		}

		private float _latestX;

		private float _latestY;

		private float _latestW;

		private readonly Camera _mapCamera;

		private readonly Action<Vec2> _fastMoveCameraToPosition;

		private Vec2 _partyPositionBind;

		private ImageIdentifierVM _factionVisualBind;

		private bool _isVisibleOnMapBind;

		private bool _isBehindBind;

		private string _nameBind;

		private Vec2 _partyPosition;

		private ImageIdentifierVM _factionVisual;

		private string _name;

		private bool _isArmy;

		private bool _isTracked;

		private bool _isEnabled;

		private bool _isBehind;
	}
}
