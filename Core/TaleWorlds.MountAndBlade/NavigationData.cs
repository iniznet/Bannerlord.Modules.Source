using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Navigation_data")]
	[Serializable]
	public struct NavigationData
	{
		public NavigationData(Vec3 startPoint, Vec3 endPoint, float agentRadius)
		{
			this.Points = new Vec2[1024];
			this.StartPoint = startPoint;
			this.EndPoint = endPoint;
			this.Points[0] = startPoint.AsVec2;
			this.Points[1] = endPoint.AsVec2;
			this.PointSize = 2;
			this.AgentRadius = agentRadius;
		}

		[Conditional("DEBUG")]
		public void TickDebug()
		{
			for (int i = 0; i < this.PointSize - 1; i++)
			{
			}
		}

		private const int MaxPathSize = 1024;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
		public Vec2[] Points;

		public Vec3 StartPoint;

		public Vec3 EndPoint;

		public readonly int PointSize;

		public readonly float AgentRadius;
	}
}
