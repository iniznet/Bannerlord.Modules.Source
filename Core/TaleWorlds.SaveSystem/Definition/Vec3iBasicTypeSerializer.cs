using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	internal class Vec3iBasicTypeSerializer : IBasicTypeSerializer
	{
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			Vec3i vec3i = (Vec3i)value;
			writer.WriteVec3Int(vec3i);
		}

		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadVec3Int();
		}
	}
}
