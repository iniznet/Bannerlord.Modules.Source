using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	internal class Vec2iBasicTypeSerializer : IBasicTypeSerializer
	{
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			Vec2i vec2i = (Vec2i)value;
			writer.WriteFloat((float)vec2i.Item1);
			writer.WriteFloat((float)vec2i.Item2);
		}

		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			int num = reader.ReadInt();
			int num2 = reader.ReadInt();
			return new Vec2i(num, num2);
		}
	}
}
