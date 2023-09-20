using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.View.Map;
using SandBox.ViewModelCollection.Nameplate;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x02000032 RID: 50
	[OverrideView(typeof(MapSettlementNameplateView))]
	public class GauntletMapSettlementNameplateView : MapView, IGauntletMapEventVisualHandler
	{
		// Token: 0x060001C4 RID: 452 RVA: 0x0000CA50 File Offset: 0x0000AC50
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._dataSource = new SettlementNameplatesVM(base.MapScreen._mapCameraView.Camera, new Action<Vec2>(base.MapScreen.FastMoveCameraToPosition));
			GauntletMapBasicView mapView = base.MapScreen.GetMapView<GauntletMapBasicView>();
			base.Layer = mapView.GauntletNameplateLayer;
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			this._movie = this._layerAsGauntletLayer.LoadMovie("SettlementNameplate", this._dataSource);
			List<Tuple<Settlement, GameEntity>> list = new List<Tuple<Settlement, GameEntity>>();
			foreach (Settlement settlement in Settlement.All)
			{
				GameEntity strategicEntity = ((PartyVisual)settlement.Party.Visuals).StrategicEntity;
				Tuple<Settlement, GameEntity> tuple = new Tuple<Settlement, GameEntity>(settlement, strategicEntity);
				list.Add(tuple);
			}
			this._dataSource.Initialize(list);
			GauntletMapEventVisualCreator gauntletMapEventVisualCreator;
			if ((gauntletMapEventVisualCreator = Campaign.Current.VisualCreator.MapEventVisualCreator as GauntletMapEventVisualCreator) != null)
			{
				gauntletMapEventVisualCreator.Handlers.Add(this);
				foreach (GauntletMapEventVisual gauntletMapEventVisual in gauntletMapEventVisualCreator.GetCurrentEvents())
				{
					SettlementNameplateVM nameplateOfMapEvent = this.GetNameplateOfMapEvent(gauntletMapEventVisual);
					if (nameplateOfMapEvent != null)
					{
						nameplateOfMapEvent.OnMapEventStartedOnSettlement(gauntletMapEventVisual.MapEvent);
					}
				}
			}
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000CBC4 File Offset: 0x0000ADC4
		protected override void OnResume()
		{
			base.OnResume();
			foreach (SettlementNameplateVM settlementNameplateVM in this._dataSource.Nameplates)
			{
				settlementNameplateVM.RefreshDynamicProperties(true);
			}
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000CC1C File Offset: 0x0000AE1C
		protected override void OnMapScreenUpdate(float dt)
		{
			base.OnMapScreenUpdate(dt);
			this._dataSource.Update();
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000CC30 File Offset: 0x0000AE30
		protected override void OnFinalize()
		{
			GauntletMapEventVisualCreator gauntletMapEventVisualCreator;
			if ((gauntletMapEventVisualCreator = Campaign.Current.VisualCreator.MapEventVisualCreator as GauntletMapEventVisualCreator) != null)
			{
				gauntletMapEventVisualCreator.Handlers.Remove(this);
			}
			this._layerAsGauntletLayer.ReleaseMovie(this._movie);
			this._dataSource.OnFinalize();
			this._layerAsGauntletLayer = null;
			base.Layer = null;
			this._movie = null;
			this._dataSource = null;
			base.OnFinalize();
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000CCA0 File Offset: 0x0000AEA0
		private SettlementNameplateVM GetNameplateOfMapEvent(GauntletMapEventVisual mapEvent)
		{
			bool flag;
			if (mapEvent.MapEvent.EventType == 2)
			{
				Settlement mapEventSettlement = mapEvent.MapEvent.MapEventSettlement;
				if (mapEventSettlement == null || !mapEventSettlement.IsUnderRaid)
				{
					GauntletMapEventVisual mapEvent2 = mapEvent;
					flag = mapEvent2 != null && mapEvent2.MapEvent.IsFinished;
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			bool flag3;
			if (mapEvent.MapEvent.EventType == 5)
			{
				Settlement mapEventSettlement2 = mapEvent.MapEvent.MapEventSettlement;
				if (mapEventSettlement2 == null || !mapEventSettlement2.IsUnderSiege)
				{
					GauntletMapEventVisual mapEvent3 = mapEvent;
					flag3 = mapEvent3 != null && mapEvent3.MapEvent.IsFinished;
				}
				else
				{
					flag3 = true;
				}
			}
			else
			{
				flag3 = false;
			}
			bool flag4 = flag3;
			bool flag5;
			if (mapEvent.MapEvent.EventType == 7)
			{
				Settlement mapEventSettlement3 = mapEvent.MapEvent.MapEventSettlement;
				if (mapEventSettlement3 == null || !mapEventSettlement3.IsUnderSiege)
				{
					GauntletMapEventVisual mapEvent4 = mapEvent;
					flag5 = mapEvent4 != null && mapEvent4.MapEvent.IsFinished;
				}
				else
				{
					flag5 = true;
				}
			}
			else
			{
				flag5 = false;
			}
			bool flag6 = flag5;
			if (mapEvent.MapEvent.MapEventSettlement != null && (flag4 || flag2 || flag6))
			{
				return this._dataSource.Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == mapEvent.MapEvent.MapEventSettlement);
			}
			return null;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000CDDE File Offset: 0x0000AFDE
		void IGauntletMapEventVisualHandler.OnNewEventStarted(GauntletMapEventVisual newEvent)
		{
			SettlementNameplateVM nameplateOfMapEvent = this.GetNameplateOfMapEvent(newEvent);
			if (nameplateOfMapEvent == null)
			{
				return;
			}
			nameplateOfMapEvent.OnMapEventStartedOnSettlement(newEvent.MapEvent);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000CDF7 File Offset: 0x0000AFF7
		void IGauntletMapEventVisualHandler.OnInitialized(GauntletMapEventVisual newEvent)
		{
			SettlementNameplateVM nameplateOfMapEvent = this.GetNameplateOfMapEvent(newEvent);
			if (nameplateOfMapEvent == null)
			{
				return;
			}
			nameplateOfMapEvent.OnMapEventStartedOnSettlement(newEvent.MapEvent);
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000CE10 File Offset: 0x0000B010
		void IGauntletMapEventVisualHandler.OnEventEnded(GauntletMapEventVisual newEvent)
		{
			SettlementNameplateVM nameplateOfMapEvent = this.GetNameplateOfMapEvent(newEvent);
			if (nameplateOfMapEvent == null)
			{
				return;
			}
			nameplateOfMapEvent.OnMapEventEndedOnSettlement();
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000CE23 File Offset: 0x0000B023
		void IGauntletMapEventVisualHandler.OnEventVisibilityChanged(GauntletMapEventVisual visibilityChangedEvent)
		{
		}

		// Token: 0x040000EA RID: 234
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x040000EB RID: 235
		private IGauntletMovie _movie;

		// Token: 0x040000EC RID: 236
		private SettlementNameplatesVM _dataSource;
	}
}
