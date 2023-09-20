using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	public class MBFastRandomSelector<T>
	{
		public ushort RemainingCount { get; private set; }

		public MBFastRandomSelector(ushort capacity = 32)
		{
			this.ReallocateIndexArray(capacity);
			this._list = null;
		}

		public MBFastRandomSelector(MBReadOnlyList<T> list, ushort capacity = 32)
		{
			this.ReallocateIndexArray(capacity);
			this.Initialize(list);
		}

		public void Initialize(MBReadOnlyList<T> list)
		{
			if (list != null && list.Count <= 65535)
			{
				this._list = list;
				this.TryExpand();
			}
			else
			{
				Debug.FailedAssert("Cannot initialize random selector as passed list is null or it exceeds " + ushort.MaxValue + " elements).", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\MBFastRandomSelector.cs", "Initialize", 63);
				this._list = null;
			}
			this.Reset();
		}

		public void Reset()
		{
			if (this._list != null)
			{
				if (this._currentVersion < 65535)
				{
					this._currentVersion += 1;
				}
				else
				{
					for (int i = 0; i < this._indexArray.Length; i++)
					{
						this._indexArray[i] = default(MBFastRandomSelector<T>.IndexEntry);
					}
					this._currentVersion = 1;
				}
				this.RemainingCount = (ushort)this._list.Count;
				return;
			}
			this._currentVersion = 1;
			this.RemainingCount = 0;
		}

		public void Pack()
		{
			if (this._list != null)
			{
				ushort num = (ushort)MathF.Max(32, this._list.Count);
				if (this._indexArray.Length != (int)num)
				{
					this.ReallocateIndexArray(num);
					return;
				}
			}
			else if (this._indexArray.Length != 32)
			{
				this.ReallocateIndexArray(32);
			}
		}

		public bool SelectRandom(out T selection, Predicate<T> conditions = null)
		{
			selection = default(T);
			if (this._list == null)
			{
				return false;
			}
			bool flag = false;
			while (this.RemainingCount > 0 && !flag)
			{
				ushort num = (ushort)MBRandom.RandomInt((int)this.RemainingCount);
				ushort num2 = this.RemainingCount - 1;
				MBFastRandomSelector<T>.IndexEntry indexEntry = this._indexArray[(int)num];
				T t = ((indexEntry.Version == this._currentVersion) ? this._list[(int)indexEntry.Index] : this._list[(int)num]);
				if (conditions == null || conditions(t))
				{
					flag = true;
					selection = t;
				}
				MBFastRandomSelector<T>.IndexEntry indexEntry2 = this._indexArray[(int)num2];
				this._indexArray[(int)num] = ((indexEntry2.Version == this._currentVersion) ? new MBFastRandomSelector<T>.IndexEntry(indexEntry2.Index, this._currentVersion) : new MBFastRandomSelector<T>.IndexEntry(num2, this._currentVersion));
				ushort remainingCount = this.RemainingCount;
				this.RemainingCount = remainingCount - 1;
			}
			return flag;
		}

		private void TryExpand()
		{
			if (this._indexArray.Length >= this._list.Count)
			{
				return;
			}
			ushort num = (ushort)(this._list.Count * 2);
			this.ReallocateIndexArray(num);
		}

		private void ReallocateIndexArray(ushort capacity)
		{
			capacity = (ushort)MBMath.ClampInt((int)capacity, 32, 65535);
			this._indexArray = new MBFastRandomSelector<T>.IndexEntry[(int)capacity];
			this._currentVersion = 1;
		}

		public const ushort MinimumCapacity = 32;

		public const ushort MaximumCapacity = 65535;

		private const ushort InitialVersion = 1;

		private const ushort MaximumVersion = 65535;

		private MBReadOnlyList<T> _list;

		private MBFastRandomSelector<T>.IndexEntry[] _indexArray;

		private ushort _currentVersion;

		public struct IndexEntry
		{
			public IndexEntry(ushort index, ushort version)
			{
				this.Index = index;
				this.Version = version;
			}

			public ushort Index;

			public ushort Version;
		}
	}
}
