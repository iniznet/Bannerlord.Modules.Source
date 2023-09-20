using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002E0 RID: 736
	[EngineStruct("Navigation_data")]
	[Serializable]
	public struct NavigationData
	{
		// Token: 0x0600284D RID: 10317 RVA: 0x0009BE8C File Offset: 0x0009A08C
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

		// Token: 0x0600284E RID: 10318 RVA: 0x0009BEEC File Offset: 0x0009A0EC
		[Conditional("DEBUG")]
		public void TickDebug()
		{
			for (int i = 0; i < this.PointSize - 1; i++)
			{
			}
		}

		// Token: 0x04000EC6 RID: 3782
		private const int MaxPathSize = 1024;

		// Token: 0x04000EC7 RID: 3783
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
		public Vec2[] Points;

		// Token: 0x04000EC8 RID: 3784
		public Vec3 StartPoint;

		// Token: 0x04000EC9 RID: 3785
		public Vec3 EndPoint;

		// Token: 0x04000ECA RID: 3786
		public readonly int PointSize;

		// Token: 0x04000ECB RID: 3787
		public readonly float AgentRadius;
	}
}
