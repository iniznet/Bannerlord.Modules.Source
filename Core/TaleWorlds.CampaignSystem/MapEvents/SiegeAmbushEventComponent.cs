﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem.MapEvents
{
	public class SiegeAmbushEventComponent : MapEventComponent
	{
		internal static void AutoGeneratedStaticCollectObjectsSiegeAmbushEventComponent(object o, List<object> collectedObjects)
		{
			((SiegeAmbushEventComponent)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		public SiegeAmbushEventComponent(MapEvent mapEvent)
			: base(mapEvent)
		{
		}

		public static SiegeAmbushEventComponent CreateSiegeAmbushEvent(PartyBase attackerParty, PartyBase defenderParty)
		{
			MapEvent mapEvent = new MapEvent();
			SiegeAmbushEventComponent siegeAmbushEventComponent = new SiegeAmbushEventComponent(mapEvent);
			mapEvent.Initialize(attackerParty, defenderParty, siegeAmbushEventComponent, MapEvent.BattleTypes.None);
			Campaign.Current.MapEventManager.OnMapEventCreated(mapEvent);
			return siegeAmbushEventComponent;
		}

		protected override void OnFinalize()
		{
			base.MapEvent.ApplyRenownAndInfluenceChanges();
		}
	}
}
