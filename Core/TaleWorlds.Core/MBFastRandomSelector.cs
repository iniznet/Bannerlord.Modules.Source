using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	// Token: 0x020000A5 RID: 165
	public class MBFastRandomSelector<T>
	{
		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06000808 RID: 2056 RVA: 0x0001B847 File Offset: 0x00019A47
		// (set) Token: 0x06000809 RID: 2057 RVA: 0x0001B84F File Offset: 0x00019A4F
		public ushort RemainingCount { get; private set; }

		// Token: 0x0600080A RID: 2058 RVA: 0x0001B858 File Offset: 0x00019A58
		public MBFastRandomSelector(ushort capacity = 32)
		{
			this.ReallocateIndexArray(capacity);
			this._list = null;
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x0001B86E File Offset: 0x00019A6E
		public MBFastRandomSelector(MBReadOnlyList<T> list, ushort capacity = 32)
		{
			this.ReallocateIndexArray(capacity);
			this.Initialize(list);
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x0001B884 File Offset: 0x00019A84
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

		// Token: 0x0600080D RID: 2061 RVA: 0x0001B8E8 File Offset: 0x00019AE8
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

		// Token: 0x0600080E RID: 2062 RVA: 0x0001B968 File Offset: 0x00019B68
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

		// Token: 0x0600080F RID: 2063 RVA: 0x0001B9B8 File Offset: 0x00019BB8
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

		// Token: 0x06000810 RID: 2064 RVA: 0x0001BAB4 File Offset: 0x00019CB4
		private void TryExpand()
		{
			if (this._indexArray.Length >= this._list.Count)
			{
				return;
			}
			ushort num = (ushort)(this._list.Count * 2);
			this.ReallocateIndexArray(num);
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x0001BAED File Offset: 0x00019CED
		private void ReallocateIndexArray(ushort capacity)
		{
			capacity = (ushort)MBMath.ClampInt((int)capacity, 32, 65535);
			this._indexArray = new MBFastRandomSelector<T>.IndexEntry[(int)capacity];
			this._currentVersion = 1;
		}

		// Token: 0x0400049A RID: 1178
		public const ushort MinimumCapacity = 32;

		// Token: 0x0400049B RID: 1179
		public const ushort MaximumCapacity = 65535;

		// Token: 0x0400049C RID: 1180
		private const ushort InitialVersion = 1;

		// Token: 0x0400049D RID: 1181
		private const ushort MaximumVersion = 65535;

		// Token: 0x0400049F RID: 1183
		private MBReadOnlyList<T> _list;

		// Token: 0x040004A0 RID: 1184
		private MBFastRandomSelector<T>.IndexEntry[] _indexArray;

		// Token: 0x040004A1 RID: 1185
		private ushort _currentVersion;

		// Token: 0x02000103 RID: 259
		public struct IndexEntry
		{
			// Token: 0x06000A48 RID: 2632 RVA: 0x00021581 File Offset: 0x0001F781
			public IndexEntry(ushort index, ushort version)
			{
				this.Index = index;
				this.Version = version;
			}

			// Token: 0x040006D7 RID: 1751
			public ushort Index;

			// Token: 0x040006D8 RID: 1752
			public ushort Version;
		}
	}
}
