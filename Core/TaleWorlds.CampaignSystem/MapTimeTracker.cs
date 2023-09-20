﻿using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem
{
	internal class MapTimeTracker
	{
		internal static void AutoGeneratedStaticCollectObjectsMapTimeTracker(object o, List<object> collectedObjects)
		{
			((MapTimeTracker)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
		}

		internal static object AutoGeneratedGetMemberValue_numTicks(object o)
		{
			return ((MapTimeTracker)o)._numTicks;
		}

		internal static object AutoGeneratedGetMemberValue_deltaTimeInTicks(object o)
		{
			return ((MapTimeTracker)o)._deltaTimeInTicks;
		}

		internal long NumTicks
		{
			get
			{
				return this._numTicks;
			}
		}

		internal long DeltaTimeInTicks
		{
			get
			{
				return this._deltaTimeInTicks;
			}
		}

		internal MapTimeTracker(CampaignTime initialMapTime)
		{
			this._numTicks = initialMapTime.NumTicks;
		}

		internal MapTimeTracker()
		{
			this._numTicks = 0L;
		}

		internal CampaignTime Now
		{
			get
			{
				return new CampaignTime(this._numTicks);
			}
		}

		internal void Tick(float seconds)
		{
			this._deltaTimeInTicks = (long)(seconds * 10000f);
			this._numTicks += this._deltaTimeInTicks;
		}

		[SaveableField(0)]
		private long _numTicks;

		[SaveableField(1)]
		private long _deltaTimeInTicks;
	}
}
