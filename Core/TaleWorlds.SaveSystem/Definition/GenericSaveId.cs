using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x0200005F RID: 95
	internal class GenericSaveId : SaveId
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x0000BCAB File Offset: 0x00009EAB
		// (set) Token: 0x060002D5 RID: 725 RVA: 0x0000BCB3 File Offset: 0x00009EB3
		public SaveId BaseId { get; set; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x0000BCBC File Offset: 0x00009EBC
		// (set) Token: 0x060002D7 RID: 727 RVA: 0x0000BCC4 File Offset: 0x00009EC4
		public SaveId[] GenericTypeIDs { get; set; }

		// Token: 0x060002D8 RID: 728 RVA: 0x0000BCCD File Offset: 0x00009ECD
		public GenericSaveId(TypeSaveId baseId, SaveId[] saveIds)
		{
			this.BaseId = baseId;
			this.GenericTypeIDs = saveIds;
			this._stringId = this.CalculateStringId();
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0000BCF0 File Offset: 0x00009EF0
		private string CalculateStringId()
		{
			string text = "";
			for (int i = 0; i < this.GenericTypeIDs.Length; i++)
			{
				if (i != 0)
				{
					text += ",";
				}
				SaveId saveId = this.GenericTypeIDs[i];
				text += saveId.GetStringId();
			}
			return string.Concat(new string[]
			{
				"G(",
				this.BaseId.GetStringId(),
				")-(",
				text,
				")"
			});
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0000BD70 File Offset: 0x00009F70
		public override string GetStringId()
		{
			return this._stringId;
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0000BD78 File Offset: 0x00009F78
		public override void WriteTo(IWriter writer)
		{
			writer.WriteByte(1);
			this.BaseId.WriteTo(writer);
			writer.WriteByte((byte)this.GenericTypeIDs.Length);
			for (int i = 0; i < this.GenericTypeIDs.Length; i++)
			{
				this.GenericTypeIDs[i].WriteTo(writer);
			}
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0000BDC8 File Offset: 0x00009FC8
		public static GenericSaveId ReadFrom(IReader reader)
		{
			reader.ReadByte();
			TypeSaveId typeSaveId = TypeSaveId.ReadFrom(reader);
			byte b = reader.ReadByte();
			List<SaveId> list = new List<SaveId>();
			for (int i = 0; i < (int)b; i++)
			{
				SaveId saveId = null;
				byte b2 = reader.ReadByte();
				if (b2 == 0)
				{
					saveId = TypeSaveId.ReadFrom(reader);
				}
				else if (b2 == 1)
				{
					saveId = GenericSaveId.ReadFrom(reader);
				}
				else if (b2 == 2)
				{
					saveId = ContainerSaveId.ReadFrom(reader);
				}
				list.Add(saveId);
			}
			return new GenericSaveId(typeSaveId, list.ToArray());
		}

		// Token: 0x040000E5 RID: 229
		private readonly string _stringId;
	}
}
