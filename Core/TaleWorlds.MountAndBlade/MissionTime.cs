using System;

namespace TaleWorlds.MountAndBlade
{
	public struct MissionTime : IComparable<MissionTime>
	{
		public long NumberOfTicks
		{
			get
			{
				return this._numberOfTicks;
			}
		}

		public MissionTime(long numberOfTicks)
		{
			this._numberOfTicks = numberOfTicks;
		}

		private static long CurrentNumberOfTicks
		{
			get
			{
				return Mission.Current.MissionTimeTracker.NumberOfTicks;
			}
		}

		public static MissionTime DeltaTime
		{
			get
			{
				return new MissionTime(Mission.Current.MissionTimeTracker.DeltaTimeInTicks);
			}
		}

		private static long DeltaTimeInTicks
		{
			get
			{
				return Mission.Current.MissionTimeTracker.DeltaTimeInTicks;
			}
		}

		public static MissionTime Now
		{
			get
			{
				return new MissionTime(Mission.Current.MissionTimeTracker.NumberOfTicks);
			}
		}

		public bool IsFuture
		{
			get
			{
				return MissionTime.CurrentNumberOfTicks < this._numberOfTicks;
			}
		}

		public bool IsPast
		{
			get
			{
				return MissionTime.CurrentNumberOfTicks > this._numberOfTicks;
			}
		}

		public bool IsNow
		{
			get
			{
				return MissionTime.CurrentNumberOfTicks == this._numberOfTicks;
			}
		}

		public float ElapsedHours
		{
			get
			{
				return (float)(MissionTime.CurrentNumberOfTicks - this._numberOfTicks) / 3.6E+10f;
			}
		}

		public float ElapsedSeconds
		{
			get
			{
				return (float)(MissionTime.CurrentNumberOfTicks - this._numberOfTicks) * 1E-07f;
			}
		}

		public float ElapsedMilliseconds
		{
			get
			{
				return (float)(MissionTime.CurrentNumberOfTicks - this._numberOfTicks) / 10000f;
			}
		}

		public double ToHours
		{
			get
			{
				return (double)this._numberOfTicks / 36000000000.0;
			}
		}

		public double ToMinutes
		{
			get
			{
				return (double)this._numberOfTicks / 600000000.0;
			}
		}

		public double ToSeconds
		{
			get
			{
				return (double)this._numberOfTicks * 1.0000000116860974E-07;
			}
		}

		public double ToMilliseconds
		{
			get
			{
				return (double)this._numberOfTicks / 10000.0;
			}
		}

		public static MissionTime MillisecondsFromNow(float valueInMilliseconds)
		{
			return new MissionTime((long)(valueInMilliseconds * 10000f + (float)MissionTime.CurrentNumberOfTicks));
		}

		public static MissionTime SecondsFromNow(float valueInSeconds)
		{
			return new MissionTime((long)(valueInSeconds * 10000000f + (float)MissionTime.CurrentNumberOfTicks));
		}

		public bool Equals(MissionTime other)
		{
			return this._numberOfTicks == other._numberOfTicks;
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is MissionTime && this.Equals((MissionTime)obj);
		}

		public override int GetHashCode()
		{
			return this._numberOfTicks.GetHashCode();
		}

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

		public static bool operator <(MissionTime x, MissionTime y)
		{
			return x._numberOfTicks < y._numberOfTicks;
		}

		public static bool operator >(MissionTime x, MissionTime y)
		{
			return x._numberOfTicks > y._numberOfTicks;
		}

		public static bool operator ==(MissionTime x, MissionTime y)
		{
			return x._numberOfTicks == y._numberOfTicks;
		}

		public static bool operator !=(MissionTime x, MissionTime y)
		{
			return !(x == y);
		}

		public static bool operator <=(MissionTime x, MissionTime y)
		{
			return x._numberOfTicks <= y._numberOfTicks;
		}

		public static bool operator >=(MissionTime x, MissionTime y)
		{
			return x._numberOfTicks >= y._numberOfTicks;
		}

		public static MissionTime Milliseconds(float valueInMilliseconds)
		{
			return new MissionTime((long)(valueInMilliseconds * 10000f));
		}

		public static MissionTime Seconds(float valueInSeconds)
		{
			return new MissionTime((long)(valueInSeconds * 10000000f));
		}

		public static MissionTime Minutes(float valueInMinutes)
		{
			return new MissionTime((long)(valueInMinutes * 600000000f));
		}

		public static MissionTime Hours(float valueInHours)
		{
			return new MissionTime((long)(valueInHours * 3.6E+10f));
		}

		public static MissionTime Zero
		{
			get
			{
				return new MissionTime(0L);
			}
		}

		public static MissionTime operator +(MissionTime g1, MissionTime g2)
		{
			return new MissionTime(g1._numberOfTicks + g2._numberOfTicks);
		}

		public static MissionTime operator -(MissionTime g1, MissionTime g2)
		{
			return new MissionTime(g1._numberOfTicks - g2._numberOfTicks);
		}

		public const long TimeTicksPerMilliSecond = 10000L;

		public const long TimeTicksPerSecond = 10000000L;

		public const long TimeTicksPerMinute = 600000000L;

		public const long TimeTicksPerHour = 36000000000L;

		public const float InvTimeTicksPerSecond = 1E-07f;

		private readonly long _numberOfTicks;
	}
}
