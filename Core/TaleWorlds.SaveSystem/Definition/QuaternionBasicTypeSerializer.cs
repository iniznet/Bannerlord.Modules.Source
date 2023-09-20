using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	internal class QuaternionBasicTypeSerializer : IBasicTypeSerializer
	{
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			Quaternion quaternion = (Quaternion)value;
			writer.WriteFloat(quaternion.X);
			writer.WriteFloat(quaternion.Y);
			writer.WriteFloat(quaternion.Z);
			writer.WriteFloat(quaternion.W);
		}

		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			float num = reader.ReadFloat();
			float num2 = reader.ReadFloat();
			float num3 = reader.ReadFloat();
			float num4 = reader.ReadFloat();
			return new Quaternion(num, num2, num3, num4);
		}
	}
}
