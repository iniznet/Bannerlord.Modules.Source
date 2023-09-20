using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.ObjectSystem
{
	// Token: 0x02000012 RID: 18
	internal class MBGUIDBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x0600008A RID: 138 RVA: 0x00004494 File Offset: 0x00002694
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			writer.WriteUInt(((MBGUID)value).InternalValue);
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000044B5 File Offset: 0x000026B5
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return new MBGUID(reader.ReadUInt());
		}
	}
}
