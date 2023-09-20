using System;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x0200006A RID: 106
	public class TypeDefinitionBase
	{
		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600033F RID: 831 RVA: 0x0000E5BC File Offset: 0x0000C7BC
		// (set) Token: 0x06000340 RID: 832 RVA: 0x0000E5C4 File Offset: 0x0000C7C4
		public SaveId SaveId { get; private set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000341 RID: 833 RVA: 0x0000E5CD File Offset: 0x0000C7CD
		// (set) Token: 0x06000342 RID: 834 RVA: 0x0000E5D5 File Offset: 0x0000C7D5
		public Type Type { get; private set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000343 RID: 835 RVA: 0x0000E5DE File Offset: 0x0000C7DE
		// (set) Token: 0x06000344 RID: 836 RVA: 0x0000E5E6 File Offset: 0x0000C7E6
		public byte TypeLevel { get; private set; }

		// Token: 0x06000345 RID: 837 RVA: 0x0000E5EF File Offset: 0x0000C7EF
		protected TypeDefinitionBase(Type type, SaveId saveId)
		{
			this.Type = type;
			this.SaveId = saveId;
			this.TypeLevel = TypeDefinitionBase.GetClassLevel(type);
		}

		// Token: 0x06000346 RID: 838 RVA: 0x0000E614 File Offset: 0x0000C814
		public static byte GetClassLevel(Type type)
		{
			byte b = 1;
			if (type.IsClass)
			{
				Type type2 = type;
				while (type2 != typeof(object))
				{
					b += 1;
					type2 = type2.BaseType;
				}
			}
			return b;
		}
	}
}
