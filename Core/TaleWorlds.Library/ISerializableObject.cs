using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200003F RID: 63
	public interface ISerializableObject
	{
		// Token: 0x060001EE RID: 494
		void DeserializeFrom(IReader reader);

		// Token: 0x060001EF RID: 495
		void SerializeTo(IWriter writer);
	}
}
