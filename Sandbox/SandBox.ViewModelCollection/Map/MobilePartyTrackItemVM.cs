using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.Map
{
	// Token: 0x0200002E RID: 46
	public class MobilePartyTrackItemVM : ViewModel
	{
		// Token: 0x1700010B RID: 267
		// (get) Token: 0x0600035E RID: 862 RVA: 0x000104F0 File Offset: 0x0000E6F0
		// (set) Token: 0x0600035F RID: 863 RVA: 0x000104F8 File Offset: 0x0000E6F8
		public MobileParty TrackedParty { get; private set; }

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000360 RID: 864 RVA: 0x00010501 File Offset: 0x0000E701
		// (set) Token: 0x06000361 RID: 865 RVA: 0x00010509 File Offset: 0x0000E709
		public Army TrackedArmy { get; private set; }

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000362 RID: 866 RVA: 0x00010512 File Offset: 0x0000E712
		private MobileParty _concernedMobileParty
		{
			get
			{
				Army trackedArmy = this.TrackedArmy;
				return ((trackedArmy != null) ? trackedArmy.LeaderParty : null) ?? this.TrackedParty;
			}
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00010530 File Offset: 0x0000E730
		public MobilePartyTrackItemVM(MobileParty trackedParty, Camera mapCamera, Action<Vec2> fastMoveCameraToPosition)
		{
			this._mapCamera = mapCamera;
			this._fastMoveCameraToPosition = fastMoveCameraToPosition;
			this.TrackedParty = trackedParty;
			this.IsTracked = Campaign.Current.VisualTrackerManager.CheckTracked(this._concernedMobileParty);
			this.UpdateProperties();
			this.IsArmy = false;
		}

		// Token: 0x06000364 RID: 868 RVA: 0x00010580 File Offset: 0x0000E780
		public MobilePartyTrackItemVM(Army trackedArmy, Camera mapCamera, Action<Vec2> fastMoveCameraToPosition)
		{
			this._mapCamera = mapCamera;
			this._fastMoveCameraToPosition = fastMoveCameraToPosition;
			this.TrackedArmy = trackedArmy;
			this.IsTracked = Campaign.Current.VisualTrackerManager.CheckTracked(this._concernedMobileParty);
			this.UpdateProperties();
			this.IsArmy = true;
		}

		// Token: 0x06000365 RID: 869 RVA: 0x000105D0 File Offset: 0x0000E7D0
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

		// Token: 0x06000366 RID: 870 RVA: 0x000106E4 File Offset: 0x0000E8E4
		private bool GetIsVisibleOnMap()
		{
			MobileParty concernedMobileParty = this._concernedMobileParty;
			return (concernedMobileParty == null || !concernedMobileParty.IsVisible) && (this.TrackedArmy != null || (this.TrackedParty != null && this.TrackedParty.IsActive && this.TrackedParty.AttachedTo == null));
		}

		// Token: 0x06000367 RID: 871 RVA: 0x00010738 File Offset: 0x0000E938
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

		// Token: 0x06000368 RID: 872 RVA: 0x000107E4 File Offset: 0x0000E9E4
		public void ExecuteToggleTrack()
		{
			if (this.IsTracked)
			{
				this.Untrack();
				return;
			}
			this.Track();
		}

		// Token: 0x06000369 RID: 873 RVA: 0x000107FB File Offset: 0x0000E9FB
		private void Track()
		{
			this.IsTracked = true;
			if (!Campaign.Current.VisualTrackerManager.CheckTracked(this._concernedMobileParty))
			{
				Campaign.Current.VisualTrackerManager.RegisterObject(this._concernedMobileParty);
			}
		}

		// Token: 0x0600036A RID: 874 RVA: 0x00010830 File Offset: 0x0000EA30
		private void Untrack()
		{
			this.IsTracked = false;
			if (Campaign.Current.VisualTrackerManager.CheckTracked(this._concernedMobileParty))
			{
				Campaign.Current.VisualTrackerManager.RemoveTrackedObject(this._concernedMobileParty, false);
			}
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00010868 File Offset: 0x0000EA68
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

		// Token: 0x0600036C RID: 876 RVA: 0x000108A0 File Offset: 0x0000EAA0
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

		// Token: 0x0600036D RID: 877 RVA: 0x0001091E File Offset: 0x0000EB1E
		public void ExecuteHideTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x0600036E RID: 878 RVA: 0x00010925 File Offset: 0x0000EB25
		public void RefreshBinding()
		{
			this.PartyPosition = this._partyPositionBind;
			this.Name = this._nameBind;
			this.IsEnabled = this._isVisibleOnMapBind;
			this.IsBehind = this._isBehindBind;
			this.FactionVisual = this._factionVisualBind;
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x0600036F RID: 879 RVA: 0x00010963 File Offset: 0x0000EB63
		// (set) Token: 0x06000370 RID: 880 RVA: 0x0001096B File Offset: 0x0000EB6B
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

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000371 RID: 881 RVA: 0x0001098E File Offset: 0x0000EB8E
		// (set) Token: 0x06000372 RID: 882 RVA: 0x00010996 File Offset: 0x0000EB96
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

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000373 RID: 883 RVA: 0x000109B4 File Offset: 0x0000EBB4
		// (set) Token: 0x06000374 RID: 884 RVA: 0x000109BC File Offset: 0x0000EBBC
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

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000375 RID: 885 RVA: 0x000109DF File Offset: 0x0000EBDF
		// (set) Token: 0x06000376 RID: 886 RVA: 0x000109E7 File Offset: 0x0000EBE7
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

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000377 RID: 887 RVA: 0x00010A05 File Offset: 0x0000EC05
		// (set) Token: 0x06000378 RID: 888 RVA: 0x00010A0D File Offset: 0x0000EC0D
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

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000379 RID: 889 RVA: 0x00010A2B File Offset: 0x0000EC2B
		// (set) Token: 0x0600037A RID: 890 RVA: 0x00010A33 File Offset: 0x0000EC33
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

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x0600037B RID: 891 RVA: 0x00010A51 File Offset: 0x0000EC51
		// (set) Token: 0x0600037C RID: 892 RVA: 0x00010A59 File Offset: 0x0000EC59
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

		// Token: 0x040001C2 RID: 450
		private float _latestX;

		// Token: 0x040001C3 RID: 451
		private float _latestY;

		// Token: 0x040001C4 RID: 452
		private float _latestW;

		// Token: 0x040001C5 RID: 453
		private readonly Camera _mapCamera;

		// Token: 0x040001C6 RID: 454
		private readonly Action<Vec2> _fastMoveCameraToPosition;

		// Token: 0x040001C7 RID: 455
		private Vec2 _partyPositionBind;

		// Token: 0x040001C8 RID: 456
		private ImageIdentifierVM _factionVisualBind;

		// Token: 0x040001C9 RID: 457
		private bool _isVisibleOnMapBind;

		// Token: 0x040001CA RID: 458
		private bool _isBehindBind;

		// Token: 0x040001CB RID: 459
		private string _nameBind;

		// Token: 0x040001CC RID: 460
		private Vec2 _partyPosition;

		// Token: 0x040001CD RID: 461
		private ImageIdentifierVM _factionVisual;

		// Token: 0x040001CE RID: 462
		private string _name;

		// Token: 0x040001CF RID: 463
		private bool _isArmy;

		// Token: 0x040001D0 RID: 464
		private bool _isTracked;

		// Token: 0x040001D1 RID: 465
		private bool _isEnabled;

		// Token: 0x040001D2 RID: 466
		private bool _isBehind;
	}
}
