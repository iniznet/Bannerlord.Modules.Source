using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	internal class ColorBasicTypeSerializer : IBasicTypeSerializer
	{
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			Color color = (Color)value;
			writer.WriteColor(color);
		}

		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadColor();
		}
	}
}
