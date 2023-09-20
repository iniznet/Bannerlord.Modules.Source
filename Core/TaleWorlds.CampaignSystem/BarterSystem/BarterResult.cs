﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.BarterSystem
{
	public class BarterResult
	{
		public List<Barterable> OfferedBarters
		{
			get
			{
				return this._offeredBarterables;
			}
		}

		public BarterResult(Hero offererHero, Hero otherHero, List<Barterable> offeredBarters, bool isAccepted)
		{
			this.OffererHero = offererHero;
			this.OtherHero = otherHero;
			this.IsAccepted = isAccepted;
			this._offeredBarterables = new List<Barterable>(offeredBarters);
		}

		internal static void AutoGeneratedStaticCollectObjectsBarterResult(object o, List<object> collectedObjects)
		{
			((BarterResult)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this.OffererHero);
			collectedObjects.Add(this.OtherHero);
			collectedObjects.Add(this._offeredBarterables);
		}

		internal static object AutoGeneratedGetMemberValueOffererHero(object o)
		{
			return ((BarterResult)o).OffererHero;
		}

		internal static object AutoGeneratedGetMemberValueOtherHero(object o)
		{
			return ((BarterResult)o).OtherHero;
		}

		internal static object AutoGeneratedGetMemberValueIsAccepted(object o)
		{
			return ((BarterResult)o).IsAccepted;
		}

		internal static object AutoGeneratedGetMemberValue_offeredBarterables(object o)
		{
			return ((BarterResult)o)._offeredBarterables;
		}

		[SaveableField(0)]
		public readonly Hero OffererHero;

		[SaveableField(1)]
		public readonly Hero OtherHero;

		[SaveableField(2)]
		private readonly List<Barterable> _offeredBarterables;

		[SaveableField(3)]
		public readonly bool IsAccepted;
	}
}
