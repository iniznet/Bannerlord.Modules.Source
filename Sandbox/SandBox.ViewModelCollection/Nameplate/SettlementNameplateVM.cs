using System;
using System.Collections.Generic;
using SandBox.ViewModelCollection.Nameplate.NameplateNotifications.SettlementNotificationTypes;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.Nameplate
{
	// Token: 0x0200001B RID: 27
	public class SettlementNameplateVM : NameplateVM
	{
		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000272 RID: 626 RVA: 0x0000C5BF File Offset: 0x0000A7BF
		public Settlement Settlement { get; }

		// Token: 0x06000273 RID: 627 RVA: 0x0000C5C8 File Offset: 0x0000A7C8
		public SettlementNameplateVM(Settlement settlement, GameEntity entity, Camera mapCamera, Action<Vec2> fastMoveCameraToPosition)
		{
			this.Settlement = settlement;
			this._mapCamera = mapCamera;
			this._entity = entity;
			this._fastMoveCameraToPosition = fastMoveCameraToPosition;
			this.SettlementNotifications = new SettlementNameplateNotificationsVM(settlement);
			this.SettlementParties = new SettlementNameplatePartyMarkersVM(settlement);
			this.SettlementEvents = new SettlementNameplateEventsVM(settlement);
			this.Name = this.Settlement.Name.ToString();
			this.IsTracked = Campaign.Current.VisualTrackerManager.CheckTracked(settlement);
			if (this.Settlement.IsCastle)
			{
				this.SettlementType = 1;
				this._isCastle = true;
			}
			else if (this.Settlement.IsVillage)
			{
				this.SettlementType = 0;
				this._isVillage = true;
			}
			else if (this.Settlement.IsTown)
			{
				this.SettlementType = 2;
				this._isTown = true;
			}
			else
			{
				this.SettlementType = 0;
				this._isTown = true;
			}
			if (this._entity != null)
			{
				this._worldPos = this._entity.GlobalPosition;
			}
			else
			{
				this._worldPos = this.Settlement.GetLogicalPosition();
			}
			this.RefreshDynamicProperties(false);
			base.SizeType = 1;
			this._rebelliousClans = new List<Clan>();
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000C6FF File Offset: 0x0000A8FF
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.Settlement.Name.ToString();
			this.RefreshDynamicProperties(true);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000C724 File Offset: 0x0000A924
		public override void RefreshDynamicProperties(bool forceUpdate)
		{
			base.RefreshDynamicProperties(forceUpdate);
			if ((this._bindIsVisibleOnMap && this._currentFaction != this.Settlement.MapFaction) || forceUpdate)
			{
				string text = "#";
				IFaction mapFaction = this.Settlement.MapFaction;
				this._bindFactionColor = text + Color.UIntToColorString((mapFaction != null) ? mapFaction.Color : uint.MaxValue);
				Banner banner = null;
				if (this.Settlement.OwnerClan != null)
				{
					banner = this.Settlement.OwnerClan.Banner;
					IFaction mapFaction2 = this.Settlement.MapFaction;
					if (mapFaction2 != null && mapFaction2.IsKingdomFaction && ((Kingdom)this.Settlement.MapFaction).RulingClan == this.Settlement.OwnerClan)
					{
						banner = this.Settlement.OwnerClan.Kingdom.Banner;
					}
				}
				int num = ((banner != null) ? banner.GetVersionNo() : 0);
				if (!BannerExtensions.IsContentsSameWith(this._latestBanner, banner) || this._latestBannerVersionNo != num)
				{
					this._bindBanner = ((banner != null) ? new ImageIdentifierVM(BannerCode.CreateFrom(banner), true) : new ImageIdentifierVM(0));
					this._latestBannerVersionNo = banner.GetVersionNo();
					this._latestBanner = banner;
				}
				this._currentFaction = this.Settlement.MapFaction;
			}
			this._bindIsTracked = Campaign.Current.VisualTrackerManager.CheckTracked(this.Settlement);
			this._bindIsInRange = this.Settlement.IsInspected;
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000C890 File Offset: 0x0000AA90
		public override void RefreshRelationStatus()
		{
			this._bindRelation = 0;
			if (this.Settlement.OwnerClan != null)
			{
				if (FactionManager.IsAtWarAgainstFaction(this.Settlement.MapFaction, Hero.MainHero.MapFaction))
				{
					this._bindRelation = 2;
					return;
				}
				if (FactionManager.IsAlliedWithFaction(this.Settlement.MapFaction, Hero.MainHero.MapFaction))
				{
					this._bindRelation = 1;
				}
			}
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000C8F8 File Offset: 0x0000AAF8
		public override void RefreshPosition()
		{
			base.RefreshPosition();
			this._bindWPos = this._wPosAfterPositionCalculation;
			this._bindWSign = (int)this._bindWPos;
			this._bindIsInside = this._latestIsInsideWindow;
			if (this._bindIsVisibleOnMap)
			{
				this._bindPosition = new Vec2(this._latestX, this._latestY);
				return;
			}
			this._bindPosition = new Vec2(-1000f, -1000f);
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000C965 File Offset: 0x0000AB65
		public override void RefreshTutorialStatus(string newTutorialHighlightElementID)
		{
			base.RefreshTutorialStatus(newTutorialHighlightElementID);
			this._bindIsTargetedByTutorial = this.Settlement.Party.Id == newTutorialHighlightElementID;
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000C98C File Offset: 0x0000AB8C
		public void OnSiegeEventStartedOnSettlement(SiegeEvent siegeEvent)
		{
			this.MapEventVisualType = 2;
			if (this.Settlement.MapFaction == Hero.MainHero.MapFaction && (BannerlordConfig.AutoTrackAttackedSettlements == 0 || (BannerlordConfig.AutoTrackAttackedSettlements == 1 && this.Settlement.MapFaction.Leader == Hero.MainHero)))
			{
				this.Track();
			}
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000C9E4 File Offset: 0x0000ABE4
		public void OnSiegeEventEndedOnSettlement(SiegeEvent siegeEvent)
		{
			Settlement settlement = this.Settlement;
			bool flag;
			if (settlement == null)
			{
				flag = null != null;
			}
			else
			{
				PartyBase party = settlement.Party;
				flag = ((party != null) ? party.MapEvent : null) != null;
			}
			if (flag && !this.Settlement.Party.MapEvent.IsFinished)
			{
				this.OnMapEventStartedOnSettlement(this.Settlement.Party.MapEvent);
			}
			else
			{
				this.OnMapEventEndedOnSettlement();
			}
			if (!this._isTrackedManually && BannerlordConfig.AutoTrackAttackedSettlements < 2 && this.Settlement.MapFaction == Hero.MainHero.MapFaction)
			{
				this.Untrack();
			}
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000CA74 File Offset: 0x0000AC74
		public void OnMapEventStartedOnSettlement(MapEvent mapEvent)
		{
			this.MapEventVisualType = (int)SandBoxUIHelper.GetMapEventVisualTypeFromMapEvent(mapEvent);
			if (this.Settlement.MapFaction == Hero.MainHero.MapFaction && (this.Settlement.IsUnderRaid || this.Settlement.IsUnderSiege || this.Settlement.InRebelliousState) && (BannerlordConfig.AutoTrackAttackedSettlements == 0 || (BannerlordConfig.AutoTrackAttackedSettlements == 1 && this.Settlement.MapFaction.Leader == Hero.MainHero)))
			{
				this.Track();
			}
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000CAF8 File Offset: 0x0000ACF8
		public void OnMapEventEndedOnSettlement()
		{
			this.MapEventVisualType = 0;
			if (!this._isTrackedManually && BannerlordConfig.AutoTrackAttackedSettlements < 2 && !this.Settlement.IsUnderSiege && !this.Settlement.IsUnderRaid && !this.Settlement.InRebelliousState)
			{
				this.Untrack();
			}
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000CB4C File Offset: 0x0000AD4C
		public void OnRebelliousClanFormed(Clan clan)
		{
			this.MapEventVisualType = 4;
			this._rebelliousClans.Add(clan);
			if (this.Settlement.MapFaction == Hero.MainHero.MapFaction && (BannerlordConfig.AutoTrackAttackedSettlements == 0 || (BannerlordConfig.AutoTrackAttackedSettlements == 1 && this.Settlement.MapFaction.Leader == Hero.MainHero)))
			{
				this.Track();
			}
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000CBB0 File Offset: 0x0000ADB0
		public void OnRebelliousClanDisbanded(Clan clan)
		{
			this._rebelliousClans.Remove(clan);
			if (Extensions.IsEmpty<Clan>(this._rebelliousClans))
			{
				if (this.Settlement.IsUnderSiege)
				{
					this.MapEventVisualType = 2;
					return;
				}
				this.MapEventVisualType = 0;
				if (!this._isTrackedManually && BannerlordConfig.AutoTrackAttackedSettlements < 2)
				{
					this.Untrack();
				}
			}
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000CC0C File Offset: 0x0000AE0C
		public void CalculatePosition(in Vec3 cameraPosition)
		{
			this._worldPosWithHeight = this._worldPos;
			if (this._isVillage)
			{
				this._heightOffset = 0.5f + MathF.Clamp(cameraPosition.z / 30f, 0f, 1f) * 2.5f;
			}
			else if (this._isCastle)
			{
				this._heightOffset = 0.5f + MathF.Clamp(cameraPosition.z / 30f, 0f, 1f) * 3f;
			}
			else if (this._isTown)
			{
				this._heightOffset = 0.5f + MathF.Clamp(cameraPosition.z / 30f, 0f, 1f) * 6f;
			}
			else
			{
				this._heightOffset = 1f;
			}
			this._worldPosWithHeight += new Vec3(0f, 0f, this._heightOffset, -1f);
			this._latestX = 0f;
			this._latestY = 0f;
			this._latestW = 0f;
			MBWindowManager.WorldToScreenInsideUsableArea(this._mapCamera, this._worldPosWithHeight, ref this._latestX, ref this._latestY, ref this._latestW);
			this._wPosAfterPositionCalculation = ((this._latestW < 0f) ? (-1f) : 1.1f);
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000CD68 File Offset: 0x0000AF68
		public void DetermineIsVisibleOnMap(in Vec3 cameraPosition)
		{
			this._bindIsVisibleOnMap = this.IsVisible(cameraPosition);
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000CD77 File Offset: 0x0000AF77
		public void DetermineIsInsideWindow()
		{
			this._latestIsInsideWindow = this.IsInsideWindow();
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000CD88 File Offset: 0x0000AF88
		public void RefreshBindValues()
		{
			base.FactionColor = this._bindFactionColor;
			this.Banner = this._bindBanner;
			this.Relation = this._bindRelation;
			this.WPos = this._bindWPos;
			this.WSign = this._bindWSign;
			this.IsInside = this._bindIsInside;
			base.Position = this._bindPosition;
			base.IsVisibleOnMap = this._bindIsVisibleOnMap;
			this.IsInRange = this._bindIsInRange;
			base.IsTargetedByTutorial = this._bindIsTargetedByTutorial;
			this.IsTracked = this._bindIsTracked;
			base.DistanceToCamera = this._bindDistanceToCamera;
			if (this.SettlementNotifications.IsEventsRegistered)
			{
				this.SettlementNotifications.Tick();
			}
			if (this.SettlementEvents.IsEventsRegistered)
			{
				this.SettlementEvents.Tick();
			}
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000CE58 File Offset: 0x0000B058
		private bool IsVisible(in Vec3 cameraPosition)
		{
			this._bindDistanceToCamera = this._worldPos.Distance(cameraPosition);
			if (this.IsTracked)
			{
				return true;
			}
			if (this.WPos < 0f || !this._latestIsInsideWindow)
			{
				return false;
			}
			if (cameraPosition.z > 400f)
			{
				return this.Settlement.IsTown;
			}
			if (cameraPosition.z > 200f)
			{
				return this.Settlement.IsFortification;
			}
			return this._bindDistanceToCamera < cameraPosition.z + 60f;
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0000CEE4 File Offset: 0x0000B0E4
		private bool IsInsideWindow()
		{
			return this._latestX <= Screen.RealScreenResolutionWidth && this._latestY <= Screen.RealScreenResolutionHeight && this._latestX + 200f >= 0f && this._latestY + 100f >= 0f;
		}

		// Token: 0x06000285 RID: 645 RVA: 0x0000CF36 File Offset: 0x0000B136
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.SettlementNotifications.UnloadEvents();
			this.SettlementParties.UnloadEvents();
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000CF54 File Offset: 0x0000B154
		public void ExecuteTrack()
		{
			if (this.IsTracked)
			{
				this.Untrack();
				this._isTrackedManually = false;
				return;
			}
			this.Track();
			this._isTrackedManually = true;
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0000CF79 File Offset: 0x0000B179
		private void Track()
		{
			this.IsTracked = true;
			if (!Campaign.Current.VisualTrackerManager.CheckTracked(this.Settlement))
			{
				Campaign.Current.VisualTrackerManager.RegisterObject(this.Settlement);
			}
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0000CFAE File Offset: 0x0000B1AE
		private void Untrack()
		{
			this.IsTracked = false;
			if (Campaign.Current.VisualTrackerManager.CheckTracked(this.Settlement))
			{
				Campaign.Current.VisualTrackerManager.RemoveTrackedObject(this.Settlement, false);
			}
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0000CFE4 File Offset: 0x0000B1E4
		public void ExecuteSetCameraPosition()
		{
			this._fastMoveCameraToPosition(this.Settlement.Position2D);
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0000CFFC File Offset: 0x0000B1FC
		public void ExecuteOpenEncyclopedia()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this.Settlement.EncyclopediaLink);
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x0600028B RID: 651 RVA: 0x0000D018 File Offset: 0x0000B218
		// (set) Token: 0x0600028C RID: 652 RVA: 0x0000D020 File Offset: 0x0000B220
		public SettlementNameplateNotificationsVM SettlementNotifications
		{
			get
			{
				return this._settlementNotifications;
			}
			set
			{
				if (value != this._settlementNotifications)
				{
					this._settlementNotifications = value;
					base.OnPropertyChangedWithValue<SettlementNameplateNotificationsVM>(value, "SettlementNotifications");
				}
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x0600028D RID: 653 RVA: 0x0000D03E File Offset: 0x0000B23E
		// (set) Token: 0x0600028E RID: 654 RVA: 0x0000D046 File Offset: 0x0000B246
		public SettlementNameplatePartyMarkersVM SettlementParties
		{
			get
			{
				return this._settlementParties;
			}
			set
			{
				if (value != this._settlementParties)
				{
					this._settlementParties = value;
					base.OnPropertyChangedWithValue<SettlementNameplatePartyMarkersVM>(value, "SettlementParties");
				}
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x0600028F RID: 655 RVA: 0x0000D064 File Offset: 0x0000B264
		// (set) Token: 0x06000290 RID: 656 RVA: 0x0000D06C File Offset: 0x0000B26C
		public SettlementNameplateEventsVM SettlementEvents
		{
			get
			{
				return this._settlementEvents;
			}
			set
			{
				if (value != this._settlementEvents)
				{
					this._settlementEvents = value;
					base.OnPropertyChangedWithValue<SettlementNameplateEventsVM>(value, "SettlementEvents");
				}
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000291 RID: 657 RVA: 0x0000D08A File Offset: 0x0000B28A
		// (set) Token: 0x06000292 RID: 658 RVA: 0x0000D092 File Offset: 0x0000B292
		public int Relation
		{
			get
			{
				return this._relation;
			}
			set
			{
				if (value != this._relation)
				{
					this._relation = value;
					base.OnPropertyChangedWithValue(value, "Relation");
				}
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000293 RID: 659 RVA: 0x0000D0B0 File Offset: 0x0000B2B0
		// (set) Token: 0x06000294 RID: 660 RVA: 0x0000D0B8 File Offset: 0x0000B2B8
		public int MapEventVisualType
		{
			get
			{
				return this._mapEventVisualType;
			}
			set
			{
				if (value != this._mapEventVisualType)
				{
					this._mapEventVisualType = value;
					base.OnPropertyChangedWithValue(value, "MapEventVisualType");
				}
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000295 RID: 661 RVA: 0x0000D0D6 File Offset: 0x0000B2D6
		// (set) Token: 0x06000296 RID: 662 RVA: 0x0000D0DE File Offset: 0x0000B2DE
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

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000297 RID: 663 RVA: 0x0000D0FC File Offset: 0x0000B2FC
		// (set) Token: 0x06000298 RID: 664 RVA: 0x0000D104 File Offset: 0x0000B304
		public float WPos
		{
			get
			{
				return this._wPos;
			}
			set
			{
				if (value != this._wPos)
				{
					this._wPos = value;
					base.OnPropertyChangedWithValue(value, "WPos");
				}
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000299 RID: 665 RVA: 0x0000D122 File Offset: 0x0000B322
		// (set) Token: 0x0600029A RID: 666 RVA: 0x0000D12A File Offset: 0x0000B32A
		public ImageIdentifierVM Banner
		{
			get
			{
				return this._banner;
			}
			set
			{
				if (value != this._banner)
				{
					this._banner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Banner");
				}
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x0600029B RID: 667 RVA: 0x0000D148 File Offset: 0x0000B348
		// (set) Token: 0x0600029C RID: 668 RVA: 0x0000D150 File Offset: 0x0000B350
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

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x0600029D RID: 669 RVA: 0x0000D173 File Offset: 0x0000B373
		// (set) Token: 0x0600029E RID: 670 RVA: 0x0000D185 File Offset: 0x0000B385
		public bool IsTracked
		{
			get
			{
				return this._isTracked || this._bindIsTargetedByTutorial;
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

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x0600029F RID: 671 RVA: 0x0000D1A3 File Offset: 0x0000B3A3
		// (set) Token: 0x060002A0 RID: 672 RVA: 0x0000D1AB File Offset: 0x0000B3AB
		public bool IsInside
		{
			get
			{
				return this._isInside;
			}
			set
			{
				if (value != this._isInside)
				{
					this._isInside = value;
					base.OnPropertyChangedWithValue(value, "IsInside");
				}
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060002A1 RID: 673 RVA: 0x0000D1C9 File Offset: 0x0000B3C9
		// (set) Token: 0x060002A2 RID: 674 RVA: 0x0000D1D4 File Offset: 0x0000B3D4
		public bool IsInRange
		{
			get
			{
				return this._isInRange;
			}
			set
			{
				if (value != this._isInRange)
				{
					this._isInRange = value;
					base.OnPropertyChangedWithValue(value, "IsInRange");
					if (this.IsInRange)
					{
						this.SettlementNotifications.RegisterEvents();
						this.SettlementParties.RegisterEvents();
						SettlementNameplateEventsVM settlementEvents = this.SettlementEvents;
						if (settlementEvents == null)
						{
							return;
						}
						settlementEvents.RegisterEvents();
						return;
					}
					else
					{
						this.SettlementNotifications.UnloadEvents();
						this.SettlementParties.UnloadEvents();
						SettlementNameplateEventsVM settlementEvents2 = this.SettlementEvents;
						if (settlementEvents2 == null)
						{
							return;
						}
						settlementEvents2.UnloadEvents();
					}
				}
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060002A3 RID: 675 RVA: 0x0000D252 File Offset: 0x0000B452
		// (set) Token: 0x060002A4 RID: 676 RVA: 0x0000D25A File Offset: 0x0000B45A
		public int SettlementType
		{
			get
			{
				return this._settlementType;
			}
			set
			{
				if (value != this._settlementType)
				{
					this._settlementType = value;
					base.OnPropertyChangedWithValue(value, "SettlementType");
				}
			}
		}

		// Token: 0x04000126 RID: 294
		private readonly Camera _mapCamera;

		// Token: 0x04000128 RID: 296
		private float _latestX;

		// Token: 0x04000129 RID: 297
		private float _latestY;

		// Token: 0x0400012A RID: 298
		private float _latestW;

		// Token: 0x0400012B RID: 299
		private float _heightOffset;

		// Token: 0x0400012C RID: 300
		private bool _latestIsInsideWindow;

		// Token: 0x0400012D RID: 301
		private Banner _latestBanner;

		// Token: 0x0400012E RID: 302
		private int _latestBannerVersionNo;

		// Token: 0x0400012F RID: 303
		private bool _isTrackedManually;

		// Token: 0x04000130 RID: 304
		private readonly GameEntity _entity;

		// Token: 0x04000131 RID: 305
		private Vec3 _worldPos;

		// Token: 0x04000132 RID: 306
		private Vec3 _worldPosWithHeight;

		// Token: 0x04000133 RID: 307
		private IFaction _currentFaction;

		// Token: 0x04000134 RID: 308
		private readonly Action<Vec2> _fastMoveCameraToPosition;

		// Token: 0x04000135 RID: 309
		private readonly bool _isVillage;

		// Token: 0x04000136 RID: 310
		private readonly bool _isCastle;

		// Token: 0x04000137 RID: 311
		private readonly bool _isTown;

		// Token: 0x04000138 RID: 312
		private float _wPosAfterPositionCalculation;

		// Token: 0x04000139 RID: 313
		private string _bindFactionColor;

		// Token: 0x0400013A RID: 314
		private bool _bindIsTracked;

		// Token: 0x0400013B RID: 315
		private ImageIdentifierVM _bindBanner;

		// Token: 0x0400013C RID: 316
		private int _bindRelation;

		// Token: 0x0400013D RID: 317
		private float _bindWPos;

		// Token: 0x0400013E RID: 318
		private float _bindDistanceToCamera;

		// Token: 0x0400013F RID: 319
		private int _bindWSign;

		// Token: 0x04000140 RID: 320
		private bool _bindIsInside;

		// Token: 0x04000141 RID: 321
		private Vec2 _bindPosition;

		// Token: 0x04000142 RID: 322
		private bool _bindIsVisibleOnMap;

		// Token: 0x04000143 RID: 323
		private bool _bindIsInRange;

		// Token: 0x04000144 RID: 324
		private List<Clan> _rebelliousClans;

		// Token: 0x04000145 RID: 325
		private string _name;

		// Token: 0x04000146 RID: 326
		private int _settlementType = -1;

		// Token: 0x04000147 RID: 327
		private ImageIdentifierVM _banner;

		// Token: 0x04000148 RID: 328
		private int _relation;

		// Token: 0x04000149 RID: 329
		private int _wSign;

		// Token: 0x0400014A RID: 330
		private float _wPos;

		// Token: 0x0400014B RID: 331
		private bool _isTracked;

		// Token: 0x0400014C RID: 332
		private bool _isInside;

		// Token: 0x0400014D RID: 333
		private bool _isInRange;

		// Token: 0x0400014E RID: 334
		private int _mapEventVisualType;

		// Token: 0x0400014F RID: 335
		private SettlementNameplateNotificationsVM _settlementNotifications;

		// Token: 0x04000150 RID: 336
		private SettlementNameplatePartyMarkersVM _settlementParties;

		// Token: 0x04000151 RID: 337
		private SettlementNameplateEventsVM _settlementEvents;

		// Token: 0x02000074 RID: 116
		public enum Type
		{
			// Token: 0x040002E9 RID: 745
			Village,
			// Token: 0x040002EA RID: 746
			Castle,
			// Token: 0x040002EB RID: 747
			Town
		}

		// Token: 0x02000075 RID: 117
		public enum RelationType
		{
			// Token: 0x040002ED RID: 749
			Neutral,
			// Token: 0x040002EE RID: 750
			Ally,
			// Token: 0x040002EF RID: 751
			Enemy
		}

		// Token: 0x02000076 RID: 118
		public enum IssueTypes
		{
			// Token: 0x040002F1 RID: 753
			None,
			// Token: 0x040002F2 RID: 754
			Possible,
			// Token: 0x040002F3 RID: 755
			Active
		}

		// Token: 0x02000077 RID: 119
		public enum MainQuestTypes
		{
			// Token: 0x040002F5 RID: 757
			None,
			// Token: 0x040002F6 RID: 758
			Possible,
			// Token: 0x040002F7 RID: 759
			Active
		}
	}
}
