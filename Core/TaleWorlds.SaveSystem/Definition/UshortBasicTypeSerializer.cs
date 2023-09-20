﻿using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	internal class UshortBasicTypeSerializer : IBasicTypeSerializer
	{
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			writer.WriteUShort((ushort)value);
		}

		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadUShort();
		}
	}
}
