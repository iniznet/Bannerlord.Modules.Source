using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000051 RID: 81
	internal class MatrixFrameBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x06000279 RID: 633 RVA: 0x0000A7B8 File Offset: 0x000089B8
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			MatrixFrame matrixFrame = (MatrixFrame)value;
			writer.WriteVec3(matrixFrame.origin);
			writer.WriteVec3(matrixFrame.rotation.s);
			writer.WriteVec3(matrixFrame.rotation.f);
			writer.WriteVec3(matrixFrame.rotation.u);
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000A80C File Offset: 0x00008A0C
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			Vec3 vec = reader.ReadVec3();
			Vec3 vec2 = reader.ReadVec3();
			Vec3 vec3 = reader.ReadVec3();
			Vec3 vec4 = reader.ReadVec3();
			return new MatrixFrame(new Mat3(vec3, vec2, vec4), vec);
		}
	}
}
