using System;
using System.Diagnostics;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200013B RID: 315
	public class MovementPath
	{
		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06000FDB RID: 4059 RVA: 0x0002FCBD File Offset: 0x0002DEBD
		private int LineCount
		{
			get
			{
				return this._navigationData.PointSize - 1;
			}
		}

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06000FDC RID: 4060 RVA: 0x0002FCCC File Offset: 0x0002DECC
		public Vec2 InitialDirection { get; }

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06000FDD RID: 4061 RVA: 0x0002FCD4 File Offset: 0x0002DED4
		public Vec2 FinalDirection { get; }

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06000FDE RID: 4062 RVA: 0x0002FCDC File Offset: 0x0002DEDC
		public Vec3 Destination
		{
			get
			{
				return this._navigationData.EndPoint;
			}
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x0002FCE9 File Offset: 0x0002DEE9
		public MovementPath(NavigationData navigationData, Vec2 initialDirection, Vec2 finalDirection)
		{
			this._navigationData = navigationData;
			this.InitialDirection = initialDirection;
			this.FinalDirection = finalDirection;
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x0002FD06 File Offset: 0x0002DF06
		public MovementPath(Vec3 currentPosition, Vec3 orderPosition, float agentRadius, Vec2 previousDirection, Vec2 finalDirection)
			: this(new NavigationData(currentPosition, orderPosition, agentRadius), previousDirection, finalDirection)
		{
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x0002FD1C File Offset: 0x0002DF1C
		private void UpdateLineLengths()
		{
			if (this._lineLengthAccumulations == null)
			{
				this._lineLengthAccumulations = new float[this.LineCount];
				for (int i = 0; i < this.LineCount; i++)
				{
					this._lineLengthAccumulations[i] = (this._navigationData.Points[i + 1] - this._navigationData.Points[i]).Length;
					if (i > 0)
					{
						this._lineLengthAccumulations[i] += this._lineLengthAccumulations[i - 1];
					}
				}
			}
		}

		// Token: 0x06000FE2 RID: 4066 RVA: 0x0002FDAC File Offset: 0x0002DFAC
		private float GetPathProggress(Vec2 point, int lineIndex)
		{
			this.UpdateLineLengths();
			float num = this._lineLengthAccumulations[this.LineCount - 1];
			if (num == 0f)
			{
				return 1f;
			}
			return (((lineIndex > 0) ? this._lineLengthAccumulations[lineIndex - 1] : 0f) + (point - this._navigationData.Points[lineIndex]).Length) / num;
		}

		// Token: 0x06000FE3 RID: 4067 RVA: 0x0002FE14 File Offset: 0x0002E014
		private void GetClosestPointTo(Vec2 point, out Vec2 closest, out int lineIndex)
		{
			closest = Vec2.Invalid;
			lineIndex = -1;
			float num = float.MaxValue;
			for (int i = 0; i < this.LineCount; i++)
			{
				Vec2 closestPointInLineSegmentToPoint = MBMath.GetClosestPointInLineSegmentToPoint(point, this._navigationData.Points[i], this._navigationData.Points[i + 1]);
				float num2 = closestPointInLineSegmentToPoint.DistanceSquared(point);
				if (num2 < num)
				{
					num = num2;
					closest = closestPointInLineSegmentToPoint;
					lineIndex = i;
				}
			}
		}

		// Token: 0x06000FE4 RID: 4068 RVA: 0x0002FE8C File Offset: 0x0002E08C
		[Conditional("DEBUG")]
		public void TickDebug(Vec2 position)
		{
			Vec2 vec;
			int num;
			this.GetClosestPointTo(position, out vec, out num);
			float pathProggress = this.GetPathProggress(vec, num);
			Vec2.Slerp(this.InitialDirection, this.FinalDirection, pathProggress).Normalize();
		}

		// Token: 0x040003B5 RID: 949
		private float[] _lineLengthAccumulations;

		// Token: 0x040003B6 RID: 950
		private NavigationData _navigationData;
	}
}
