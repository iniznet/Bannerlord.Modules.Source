using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200006F RID: 111
	public struct PathFaceRecord
	{
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060003C7 RID: 967 RVA: 0x0000C03C File Offset: 0x0000A23C
		// (set) Token: 0x060003C8 RID: 968 RVA: 0x0000C044 File Offset: 0x0000A244
		public int FaceIndex { get; private set; }

		// Token: 0x060003C9 RID: 969 RVA: 0x0000C04D File Offset: 0x0000A24D
		public PathFaceRecord(int index, int groupIndex, int islandIndex)
		{
			this.FaceIndex = index;
			this.FaceGroupIndex = groupIndex;
			this.FaceIslandIndex = islandIndex;
		}

		// Token: 0x060003CA RID: 970 RVA: 0x0000C064 File Offset: 0x0000A264
		public bool IsValid()
		{
			return this.FaceIndex != -1;
		}

		// Token: 0x04000120 RID: 288
		public int FaceGroupIndex;

		// Token: 0x04000121 RID: 289
		public int FaceIslandIndex;

		// Token: 0x04000122 RID: 290
		public static readonly PathFaceRecord NullFaceRecord = new PathFaceRecord(-1, -1, -1);

		// Token: 0x020000CB RID: 203
		public struct StackArray6PathFaceRecord
		{
			// Token: 0x170000F0 RID: 240
			public PathFaceRecord this[int index]
			{
				get
				{
					switch (index)
					{
					case 0:
						return this._element0;
					case 1:
						return this._element1;
					case 2:
						return this._element2;
					case 3:
						return this._element3;
					case 4:
						return this._element4;
					case 5:
						return this._element5;
					default:
						Debug.FailedAssert("Index out of range.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\PathFaceRecord.cs", "Item", 34);
						return PathFaceRecord.NullFaceRecord;
					}
				}
				set
				{
					switch (index)
					{
					case 0:
						this._element0 = value;
						return;
					case 1:
						this._element1 = value;
						return;
					case 2:
						this._element2 = value;
						return;
					case 3:
						this._element3 = value;
						return;
					case 4:
						this._element4 = value;
						return;
					case 5:
						this._element5 = value;
						return;
					default:
						Debug.FailedAssert("Index out of range.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\PathFaceRecord.cs", "Item", 50);
						return;
					}
				}
			}

			// Token: 0x04000280 RID: 640
			private PathFaceRecord _element0;

			// Token: 0x04000281 RID: 641
			private PathFaceRecord _element1;

			// Token: 0x04000282 RID: 642
			private PathFaceRecord _element2;

			// Token: 0x04000283 RID: 643
			private PathFaceRecord _element3;

			// Token: 0x04000284 RID: 644
			private PathFaceRecord _element4;

			// Token: 0x04000285 RID: 645
			private PathFaceRecord _element5;

			// Token: 0x04000286 RID: 646
			public const int Length = 6;
		}
	}
}
