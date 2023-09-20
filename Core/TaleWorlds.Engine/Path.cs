using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200006F RID: 111
	[EngineClass("rglPath")]
	public sealed class Path : NativeObject
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600088E RID: 2190 RVA: 0x0000886E File Offset: 0x00006A6E
		public int NumberOfPoints
		{
			get
			{
				return EngineApplicationInterface.IPath.GetNumberOfPoints(base.Pointer);
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600088F RID: 2191 RVA: 0x00008880 File Offset: 0x00006A80
		public float TotalDistance
		{
			get
			{
				return EngineApplicationInterface.IPath.GetTotalLength(base.Pointer);
			}
		}

		// Token: 0x06000890 RID: 2192 RVA: 0x00008892 File Offset: 0x00006A92
		internal Path(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x000088A4 File Offset: 0x00006AA4
		public MatrixFrame GetHermiteFrameForDt(float phase, int first_point)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			EngineApplicationInterface.IPath.GetHermiteFrameForDt(base.Pointer, ref identity, phase, first_point);
			return identity;
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x000088CC File Offset: 0x00006ACC
		public MatrixFrame GetFrameForDistance(float distance)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			EngineApplicationInterface.IPath.GetHermiteFrameForDistance(base.Pointer, ref identity, distance);
			return identity;
		}

		// Token: 0x06000893 RID: 2195 RVA: 0x000088F4 File Offset: 0x00006AF4
		public MatrixFrame GetNearestFrameWithValidAlphaForDistance(float distance, bool searchForward = true, float alphaThreshold = 0.5f)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			EngineApplicationInterface.IPath.GetNearestHermiteFrameWithValidAlphaForDistance(base.Pointer, ref identity, distance, searchForward, alphaThreshold);
			return identity;
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x0000891D File Offset: 0x00006B1D
		public void GetFrameAndColorForDistance(float distance, out MatrixFrame frame, out Vec3 color)
		{
			frame = MatrixFrame.Identity;
			EngineApplicationInterface.IPath.GetHermiteFrameAndColorForDistance(base.Pointer, out frame, out color, distance);
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x0000893D File Offset: 0x00006B3D
		public float GetArcLength(int first_point)
		{
			return EngineApplicationInterface.IPath.GetArcLength(base.Pointer, first_point);
		}

		// Token: 0x06000896 RID: 2198 RVA: 0x00008950 File Offset: 0x00006B50
		public void GetPoints(MatrixFrame[] points)
		{
			EngineApplicationInterface.IPath.GetPoints(base.Pointer, points);
		}

		// Token: 0x06000897 RID: 2199 RVA: 0x00008963 File Offset: 0x00006B63
		public float GetTotalLength()
		{
			return EngineApplicationInterface.IPath.GetTotalLength(base.Pointer);
		}

		// Token: 0x06000898 RID: 2200 RVA: 0x00008975 File Offset: 0x00006B75
		public int GetVersion()
		{
			return EngineApplicationInterface.IPath.GetVersion(base.Pointer);
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x00008987 File Offset: 0x00006B87
		public void SetFrameOfPoint(int pointIndex, ref MatrixFrame frame)
		{
			EngineApplicationInterface.IPath.SetFrameOfPoint(base.Pointer, pointIndex, ref frame);
		}

		// Token: 0x0600089A RID: 2202 RVA: 0x0000899B File Offset: 0x00006B9B
		public void SetTangentPositionOfPoint(int pointIndex, int tangentIndex, ref Vec3 position)
		{
			EngineApplicationInterface.IPath.SetTangentPositionOfPoint(base.Pointer, pointIndex, tangentIndex, ref position);
		}

		// Token: 0x0600089B RID: 2203 RVA: 0x000089B0 File Offset: 0x00006BB0
		public int AddPathPoint(int newNodeIndex)
		{
			return EngineApplicationInterface.IPath.AddPathPoint(base.Pointer, newNodeIndex);
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x000089C3 File Offset: 0x00006BC3
		public void DeletePathPoint(int nodeIndex)
		{
			EngineApplicationInterface.IPath.DeletePathPoint(base.Pointer, nodeIndex);
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x000089D6 File Offset: 0x00006BD6
		public bool HasValidAlphaAtPathPoint(int nodeIndex, float alphaThreshold = 0.5f)
		{
			return EngineApplicationInterface.IPath.HasValidAlphaAtPathPoint(base.Pointer, nodeIndex, alphaThreshold);
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x000089EA File Offset: 0x00006BEA
		public string GetName()
		{
			return EngineApplicationInterface.IPath.GetName(base.Pointer);
		}
	}
}
