using System;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x0200001C RID: 28
	[Serializable]
	internal struct MeshData
	{
		// Token: 0x040000AB RID: 171
		public MeshTopology Topology;

		// Token: 0x040000AC RID: 172
		public float[] Vertices;

		// Token: 0x040000AD RID: 173
		public float[] TextureCoordinates;

		// Token: 0x040000AE RID: 174
		public uint[] Indices;

		// Token: 0x040000AF RID: 175
		public int VertexCount;
	}
}
