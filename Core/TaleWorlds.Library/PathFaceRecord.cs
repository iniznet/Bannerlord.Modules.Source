using System;

namespace TaleWorlds.Library
{
	public struct PathFaceRecord
	{
		public int FaceIndex { get; private set; }

		public PathFaceRecord(int index, int groupIndex, int islandIndex)
		{
			this.FaceIndex = index;
			this.FaceGroupIndex = groupIndex;
			this.FaceIslandIndex = islandIndex;
		}

		public bool IsValid()
		{
			return this.FaceIndex != -1;
		}

		public int FaceGroupIndex;

		public int FaceIslandIndex;

		public static readonly PathFaceRecord NullFaceRecord = new PathFaceRecord(-1, -1, -1);

		public struct StackArray6PathFaceRecord
		{
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

			private PathFaceRecord _element0;

			private PathFaceRecord _element1;

			private PathFaceRecord _element2;

			private PathFaceRecord _element3;

			private PathFaceRecord _element4;

			private PathFaceRecord _element5;

			public const int Length = 6;
		}
	}
}
