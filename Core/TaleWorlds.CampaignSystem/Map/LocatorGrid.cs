using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Map
{
	// Token: 0x020000CE RID: 206
	internal class LocatorGrid<T> where T : ILocatable<T>
	{
		// Token: 0x060012A0 RID: 4768 RVA: 0x0005446D File Offset: 0x0005266D
		internal LocatorGrid(float gridNodeSize = 5f, int gridWidth = 32, int gridHeight = 32)
		{
			this._width = gridWidth;
			this._height = gridHeight;
			this._gridNodeSize = gridNodeSize;
			this._nodes = new T[this._width * this._height];
		}

		// Token: 0x060012A1 RID: 4769 RVA: 0x000544A2 File Offset: 0x000526A2
		private int MapCoordinates(int x, int y)
		{
			x %= this._width;
			if (x < 0)
			{
				x += this._width;
			}
			y %= this._height;
			if (y < 0)
			{
				y += this._height;
			}
			return y * this._width + x;
		}

		// Token: 0x060012A2 RID: 4770 RVA: 0x000544E0 File Offset: 0x000526E0
		internal bool CheckWhetherPositionsAreInSameNode(Vec2 pos1, ILocatable<T> locatable)
		{
			int num = this.Pos2NodeIndex(pos1);
			int locatorNodeIndex = locatable.LocatorNodeIndex;
			return num == locatorNodeIndex;
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x00054500 File Offset: 0x00052700
		internal bool UpdateLocator(T locatable)
		{
			ILocatable<T> locatable2 = locatable;
			Vec2 getPosition2D = locatable2.GetPosition2D;
			int num = this.Pos2NodeIndex(getPosition2D);
			if (num != locatable2.LocatorNodeIndex)
			{
				if (locatable2.LocatorNodeIndex >= 0)
				{
					this.RemoveFromList(locatable2);
				}
				this.AddToList(num, locatable);
				locatable2.LocatorNodeIndex = num;
				return true;
			}
			return false;
		}

		// Token: 0x060012A4 RID: 4772 RVA: 0x00054550 File Offset: 0x00052750
		private void RemoveFromList(ILocatable<T> locatable)
		{
			if (this._nodes[locatable.LocatorNodeIndex] == locatable)
			{
				this._nodes[locatable.LocatorNodeIndex] = locatable.NextLocatable;
				locatable.NextLocatable = default(T);
				return;
			}
			ILocatable<T> locatable2;
			if ((locatable2 = this._nodes[locatable.LocatorNodeIndex]) != null)
			{
				while (locatable2.NextLocatable != null)
				{
					if (locatable2.NextLocatable == locatable)
					{
						locatable2.NextLocatable = locatable.NextLocatable;
						locatable.NextLocatable = default(T);
						return;
					}
					locatable2 = locatable2.NextLocatable;
				}
				Debug.FailedAssert("cannot remove party from MapLocator: " + locatable.ToString(), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Map\\LocatorGrid.cs", "RemoveFromList", 130);
			}
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x00054620 File Offset: 0x00052820
		private void AddToList(int nodeIndex, T locator)
		{
			T t = this._nodes[nodeIndex];
			this._nodes[nodeIndex] = locator;
			locator.NextLocatable = t;
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x00054654 File Offset: 0x00052854
		private T FindLocatableOnNextNode(ref LocatableSearchData<T> data)
		{
			T t = default(T);
			do
			{
				data.CurrentY++;
				if (data.CurrentY > data.MaxYInclusive)
				{
					data.CurrentY = data.MinY;
					data.CurrentX++;
				}
				if (data.CurrentX <= data.MaxXInclusive)
				{
					int num = this.MapCoordinates(data.CurrentX, data.CurrentY);
					t = this._nodes[num];
				}
			}
			while (t == null && data.CurrentX <= data.MaxXInclusive);
			return t;
		}

		// Token: 0x060012A7 RID: 4775 RVA: 0x000546E0 File Offset: 0x000528E0
		internal T FindNextLocatable(ref LocatableSearchData<T> data)
		{
			if (data.CurrentLocatable != null)
			{
				data.CurrentLocatable = data.CurrentLocatable.NextLocatable;
				while (data.CurrentLocatable != null)
				{
					if (data.CurrentLocatable.GetPosition2D.DistanceSquared(data.Position) < data.RadiusSquared)
					{
						break;
					}
					data.CurrentLocatable = data.CurrentLocatable.NextLocatable;
				}
			}
			while (data.CurrentLocatable == null && data.CurrentX <= data.MaxXInclusive)
			{
				data.CurrentLocatable = this.FindLocatableOnNextNode(ref data);
				while (data.CurrentLocatable != null && data.CurrentLocatable.GetPosition2D.DistanceSquared(data.Position) >= data.RadiusSquared)
				{
					data.CurrentLocatable = data.CurrentLocatable.NextLocatable;
				}
			}
			return (T)((object)data.CurrentLocatable);
		}

		// Token: 0x060012A8 RID: 4776 RVA: 0x000547C8 File Offset: 0x000529C8
		internal LocatableSearchData<T> StartFindingLocatablesAroundPosition(Vec2 position, float radius)
		{
			int num;
			int num2;
			int num3;
			int num4;
			this.GetBoundaries(position, radius, out num, out num2, out num3, out num4);
			return new LocatableSearchData<T>(position, radius, num, num2, num3, num4);
		}

		// Token: 0x060012A9 RID: 4777 RVA: 0x000547F0 File Offset: 0x000529F0
		internal void RemoveLocatable(T locatable)
		{
			ILocatable<T> locatable2 = locatable;
			if (locatable2.LocatorNodeIndex >= 0)
			{
				this.RemoveFromList(locatable2);
			}
		}

		// Token: 0x060012AA RID: 4778 RVA: 0x00054814 File Offset: 0x00052A14
		private void GetBoundaries(Vec2 position, float radius, out int minX, out int minY, out int maxX, out int maxY)
		{
			Vec2 vec = new Vec2(MathF.Min(radius, (float)(this._width - 1) * this._gridNodeSize * 0.5f), MathF.Min(radius, (float)(this._height - 1) * this._gridNodeSize * 0.5f));
			this.GetGridIndices(position - vec, out minX, out minY);
			this.GetGridIndices(position + vec, out maxX, out maxY);
		}

		// Token: 0x060012AB RID: 4779 RVA: 0x00054881 File Offset: 0x00052A81
		private void GetGridIndices(Vec2 position, out int x, out int y)
		{
			x = MathF.Floor(position.x / this._gridNodeSize);
			y = MathF.Floor(position.y / this._gridNodeSize);
		}

		// Token: 0x060012AC RID: 4780 RVA: 0x000548AC File Offset: 0x00052AAC
		private int Pos2NodeIndex(Vec2 position)
		{
			int num;
			int num2;
			this.GetGridIndices(position, out num, out num2);
			return this.MapCoordinates(num, num2);
		}

		// Token: 0x04000684 RID: 1668
		private readonly T[] _nodes;

		// Token: 0x04000685 RID: 1669
		private readonly float _gridNodeSize;

		// Token: 0x04000686 RID: 1670
		private readonly int _width;

		// Token: 0x04000687 RID: 1671
		private readonly int _height;
	}
}
