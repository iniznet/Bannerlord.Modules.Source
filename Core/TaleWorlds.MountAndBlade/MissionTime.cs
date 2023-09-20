using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000287 RID: 647
	public struct MissionTime : IComparable<MissionTime>
	{
		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x0600224B RID: 8779 RVA: 0x0007D662 File Offset: 0x0007B862
		public long NumberOfTicks
		{
			get
			{
				return this._numberOfTicks;
			}
		}

		// Token: 0x0600224C RID: 8780 RVA: 0x0007D66A File Offset: 0x0007B86A
		public MissionTime(long numberOfTicks)
		{
			this._numberOfTicks = numberOfTicks;
		}

		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x0600224D RID: 8781 RVA: 0x0007D673 File Offset: 0x0007B873
		private static long CurrentNumberOfTicks
		{
			get
			{
				return Mission.Current.MissionTimeTracker.NumberOfTicks;
			}
		}

		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x0600224E RID: 8782 RVA: 0x0007D684 File Offset: 0x0007B884
		public static MissionTime DeltaTime
		{
			get
			{
				return new MissionTime(Mission.Current.MissionTimeTracker.DeltaTimeInTicks);
			}
		}

		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x0600224F RID: 8783 RVA: 0x0007D69A File Offset: 0x0007B89A
		private static long DeltaTimeInTicks
		{
			get
			{
				return Mission.Current.MissionTimeTracker.DeltaTimeInTicks;
			}
		}

		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x06002250 RID: 8784 RVA: 0x0007D6AB File Offset: 0x0007B8AB
		public static MissionTime Now
		{
			get
			{
				return new MissionTime(Mission.Current.MissionTimeTracker.NumberOfTicks);
			}
		}

		// Token: 0x17000680 RID: 1664
		// (get) Token: 0x06002251 RID: 8785 RVA: 0x0007D6C1 File Offset: 0x0007B8C1
		public bool IsFuture
		{
			get
			{
				return MissionTime.CurrentNumberOfTicks < this._numberOfTicks;
			}
		}

		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x06002252 RID: 8786 RVA: 0x0007D6D0 File Offset: 0x0007B8D0
		public bool IsPast
		{
			get
			{
				return MissionTime.CurrentNumberOfTicks > this._numberOfTicks;
			}
		}

		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x06002253 RID: 8787 RVA: 0x0007D6DF File Offset: 0x0007B8DF
		public bool IsNow
		{
			get
			{
				return MissionTime.CurrentNumberOfTicks == this._numberOfTicks;
			}
		}

		// Token: 0x17000683 RID: 1667
		// (get) Token: 0x06002254 RID: 8788 RVA: 0x0007D6EE File Offset: 0x0007B8EE
		public float ElapsedHours
		{
			get
			{
				return (float)(MissionTime.CurrentNumberOfTicks - this._numberOfTicks) / 3.6E+10f;
			}
		}

		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x06002255 RID: 8789 RVA: 0x0007D703 File Offset: 0x0007B903
		public float ElapsedSeconds
		{
			get
			{
				return (float)(MissionTime.CurrentNumberOfTicks - this._numberOfTicks) * 1E-07f;
			}
		}

		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x06002256 RID: 8790 RVA: 0x0007D718 File Offset: 0x0007B918
		public float ElapsedMilliseconds
		{
			get
			{
				return (float)(MissionTime.CurrentNumberOfTicks - this._numberOfTicks) / 10000f;
			}
		}

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x06002257 RID: 8791 RVA: 0x0007D72D File Offset: 0x0007B92D
		public double ToHours
		{
			get
			{
				return (double)this._numberOfTicks / 36000000000.0;
			}
		}

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x06002258 RID: 8792 RVA: 0x0007D740 File Offset: 0x0007B940
		public double ToMinutes
		{
			get
			{
				return (double)this._numberOfTicks / 600000000.0;
			}
		}

		// Token: 0x17000688 RID: 1672
		// (get) Token: 0x06002259 RID: 8793 RVA: 0x0007D753 File Offset: 0x0007B953
		public double ToSeconds
		{
			get
			{
				return (double)this._numberOfTicks * 1.0000000116860974E-07;
			}
		}

		// Token: 0x17000689 RID: 1673
		// (get) Token: 0x0600225A RID: 8794 RVA: 0x0007D766 File Offset: 0x0007B966
		public double ToMilliseconds
		{
			get
			{
				return (double)this._numberOfTicks / 10000.0;
			}
		}

		// Token: 0x0600225B RID: 8795 RVA: 0x0007D779 File Offset: 0x0007B979
		public static MissionTime MillisecondsFromNow(float valueInMilliseconds)
		{
			return new MissionTime((long)(valueInMilliseconds * 10000f + (float)MissionTime.CurrentNumberOfTicks));
		}

		// Token: 0x0600225C RID: 8796 RVA: 0x0007D78F File Offset: 0x0007B98F
		public static MissionTime SecondsFromNow(float valueInSeconds)
		{
			return new MissionTime((long)(valueInSeconds * 10000000f + (float)MissionTime.CurrentNumberOfTicks));
		}

		// Token: 0x0600225D RID: 8797 RVA: 0x0007D7A5 File Offset: 0x0007B9A5
		public bool Equals(MissionTime other)
		{
			return this._numberOfTicks == other._numberOfTicks;
		}

		// Token: 0x0600225E RID: 8798 RVA: 0x0007D7B5 File Offset: 0x0007B9B5
		public override bool Equals(object obj)
		{
			return obj != null && obj is MissionTime && this.Equals((MissionTime)obj);
		}

		// Token: 0x0600225F RID: 8799 RVA: 0x0007D7D0 File Offset: 0x0007B9D0
		public override int GetHashCode()
		{
			return this._numberOfTicks.GetHashCode();
		}

		// Token: 0x06002260 RID: 8800 RVA: 0x0007D7EB File Offset: 0x0007B9EB
		public int CompareTo(MissionTime other)
		{
			if (this._numberOfTicks == other._numberOfTicks)
			{
				return 0;
			}
			if (this._numberOfTicks > other._numberOfTicks)
			{
				return 1;
			}
			return -1;
		}

		// Token: 0x06002261 RID: 8801 RVA: 0x0007D80E File Offset: 0x0007BA0E
		public static bool operator <(MissionTime x, MissionTime y)
		{
			return x._numberOfTicks < y._numberOfTicks;
		}

		// Token: 0x06002262 RID: 8802 RVA: 0x0007D81E File Offset: 0x0007BA1E
		public static bool operator >(MissionTime x, MissionTime y)
		{
			return x._numberOfTicks > y._numberOfTicks;
		}

		// Token: 0x06002263 RID: 8803 RVA: 0x0007D82E File Offset: 0x0007BA2E
		public static bool operator ==(MissionTime x, MissionTime y)
		{
			return x._numberOfTicks == y._numberOfTicks;
		}

		// Token: 0x06002264 RID: 8804 RVA: 0x0007D83E File Offset: 0x0007BA3E
		public static bool operator !=(MissionTime x, MissionTime y)
		{
			return !(x == y);
		}

		// Token: 0x06002265 RID: 8805 RVA: 0x0007D84A File Offset: 0x0007BA4A
		public static bool operator <=(MissionTime x, MissionTime y)
		{
			return x._numberOfTicks <= y._numberOfTicks;
		}

		// Token: 0x06002266 RID: 8806 RVA: 0x0007D85D File Offset: 0x0007BA5D
		public static bool operator >=(MissionTime x, MissionTime y)
		{
			return x._numberOfTicks >= y._numberOfTicks;
		}

		// Token: 0x06002267 RID: 8807 RVA: 0x0007D870 File Offset: 0x0007BA70
		public static MissionTime Milliseconds(float valueInMilliseconds)
		{
			return new MissionTime((long)(valueInMilliseconds * 10000f));
		}

		// Token: 0x06002268 RID: 8808 RVA: 0x0007D87F File Offset: 0x0007BA7F
		public static MissionTime Seconds(float valueInSeconds)
		{
			return new MissionTime((long)(valueInSeconds * 10000000f));
		}

		// Token: 0x06002269 RID: 8809 RVA: 0x0007D88E File Offset: 0x0007BA8E
		public static MissionTime Minutes(float valueInMinutes)
		{
			return new MissionTime((long)(valueInMinutes * 600000000f));
		}

		// Token: 0x0600226A RID: 8810 RVA: 0x0007D89D File Offset: 0x0007BA9D
		public static MissionTime Hours(float valueInHours)
		{
			return new MissionTime((long)(valueInHours * 3.6E+10f));
		}

		// Token: 0x1700068A RID: 1674
		// (get) Token: 0x0600226B RID: 8811 RVA: 0x0007D8AC File Offset: 0x0007BAAC
		public static MissionTime Zero
		{
			get
			{
				return new MissionTime(0L);
			}
		}

		// Token: 0x0600226C RID: 8812 RVA: 0x0007D8B5 File Offset: 0x0007BAB5
		public static MissionTime operator +(MissionTime g1, MissionTime g2)
		{
			return new MissionTime(g1._numberOfTicks + g2._numberOfTicks);
		}

		// Token: 0x0600226D RID: 8813 RVA: 0x0007D8C9 File Offset: 0x0007BAC9
		public static MissionTime operator -(MissionTime g1, MissionTime g2)
		{
			return new MissionTime(g1._numberOfTicks - g2._numberOfTicks);
		}

		// Token: 0x04000CD4 RID: 3284
		public const long TimeTicksPerMilliSecond = 10000L;

		// Token: 0x04000CD5 RID: 3285
		public const long TimeTicksPerSecond = 10000000L;

		// Token: 0x04000CD6 RID: 3286
		public const long TimeTicksPerMinute = 600000000L;

		// Token: 0x04000CD7 RID: 3287
		public const long TimeTicksPerHour = 36000000000L;

		// Token: 0x04000CD8 RID: 3288
		public const float InvTimeTicksPerSecond = 1E-07f;

		// Token: 0x04000CD9 RID: 3289
		private readonly long _numberOfTicks;
	}
}
