using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	internal class MatrixFrameBasicTypeSerializer : IBasicTypeSerializer
	{
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			MatrixFrame matrixFrame = (MatrixFrame)value;
			writer.WriteVec3(matrixFrame.origin);
			writer.WriteVec3(matrixFrame.rotation.s);
			writer.WriteVec3(matrixFrame.rotation.f);
			writer.WriteVec3(matrixFrame.rotation.u);
		}

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
