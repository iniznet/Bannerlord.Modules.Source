using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	internal class Mat3BasicTypeSerializer : IBasicTypeSerializer
	{
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			Mat3 mat = (Mat3)value;
			writer.WriteVec3(mat.s);
			writer.WriteVec3(mat.f);
			writer.WriteVec3(mat.u);
		}

		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			Vec3 vec = reader.ReadVec3();
			Vec3 vec2 = reader.ReadVec3();
			Vec3 vec3 = reader.ReadVec3();
			return new Mat3(vec, vec2, vec3);
		}
	}
}
