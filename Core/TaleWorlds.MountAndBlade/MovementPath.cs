using System;
using System.Diagnostics;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class MovementPath
	{
		private int LineCount
		{
			get
			{
				return this._navigationData.PointSize - 1;
			}
		}

		public Vec2 InitialDirection { get; }

		public Vec2 FinalDirection { get; }

		public Vec3 Destination
		{
			get
			{
				return this._navigationData.EndPoint;
			}
		}

		public MovementPath(NavigationData navigationData, Vec2 initialDirection, Vec2 finalDirection)
		{
			this._navigationData = navigationData;
			this.InitialDirection = initialDirection;
			this.FinalDirection = finalDirection;
		}

		public MovementPath(Vec3 currentPosition, Vec3 orderPosition, float agentRadius, Vec2 previousDirection, Vec2 finalDirection)
			: this(new NavigationData(currentPosition, orderPosition, agentRadius), previousDirection, finalDirection)
		{
		}

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

		[Conditional("DEBUG")]
		public void TickDebug(Vec2 position)
		{
			Vec2 vec;
			int num;
			this.GetClosestPointTo(position, out vec, out num);
			float pathProggress = this.GetPathProggress(vec, num);
			Vec2.Slerp(this.InitialDirection, this.FinalDirection, pathProggress).Normalize();
		}

		private float[] _lineLengthAccumulations;

		private NavigationData _navigationData;
	}
}
