using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x0200006B RID: 107
	public class TypeSaveId : SaveId
	{
		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000347 RID: 839 RVA: 0x0000E64E File Offset: 0x0000C84E
		// (set) Token: 0x06000348 RID: 840 RVA: 0x0000E656 File Offset: 0x0000C856
		public int Id { get; private set; }

		// Token: 0x06000349 RID: 841 RVA: 0x0000E660 File Offset: 0x0000C860
		public TypeSaveId(int id)
		{
			this.Id = id;
			this._stringId = this.Id.ToString();
		}

		// Token: 0x0600034A RID: 842 RVA: 0x0000E68E File Offset: 0x0000C88E
		public override string GetStringId()
		{
			return this._stringId;
		}

		// Token: 0x0600034B RID: 843 RVA: 0x0000E696 File Offset: 0x0000C896
		public override void WriteTo(IWriter writer)
		{
			writer.WriteByte(0);
			writer.WriteInt(this.Id);
		}

		// Token: 0x0600034C RID: 844 RVA: 0x0000E6AB File Offset: 0x0000C8AB
		public static TypeSaveId ReadFrom(IReader reader)
		{
			return new TypeSaveId(reader.ReadInt());
		}

		// Token: 0x0400010E RID: 270
		private readonly string _stringId;
	}
}
