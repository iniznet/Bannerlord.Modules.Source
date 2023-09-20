using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000057 RID: 87
	public class ContainerSaveId : SaveId
	{
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000290 RID: 656 RVA: 0x0000A988 File Offset: 0x00008B88
		// (set) Token: 0x06000291 RID: 657 RVA: 0x0000A990 File Offset: 0x00008B90
		public ContainerType ContainerType { get; set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000292 RID: 658 RVA: 0x0000A999 File Offset: 0x00008B99
		// (set) Token: 0x06000293 RID: 659 RVA: 0x0000A9A1 File Offset: 0x00008BA1
		public SaveId KeyId { get; set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000294 RID: 660 RVA: 0x0000A9AA File Offset: 0x00008BAA
		// (set) Token: 0x06000295 RID: 661 RVA: 0x0000A9B2 File Offset: 0x00008BB2
		public SaveId ValueId { get; set; }

		// Token: 0x06000296 RID: 662 RVA: 0x0000A9BB File Offset: 0x00008BBB
		public ContainerSaveId(ContainerType containerType, SaveId elementId)
		{
			this.ContainerType = containerType;
			this.KeyId = elementId;
			this._stringId = this.CalculateStringId();
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000A9DD File Offset: 0x00008BDD
		public ContainerSaveId(ContainerType containerType, SaveId keyId, SaveId valueId)
		{
			this.ContainerType = containerType;
			this.KeyId = keyId;
			this.ValueId = valueId;
			this._stringId = this.CalculateStringId();
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000AA08 File Offset: 0x00008C08
		private string CalculateStringId()
		{
			string text;
			if (this.ContainerType == ContainerType.Dictionary)
			{
				string stringId = this.KeyId.GetStringId();
				string stringId2 = this.ValueId.GetStringId();
				text = string.Concat(new object[]
				{
					"C(",
					(int)this.ContainerType,
					")-(",
					stringId,
					",",
					stringId2,
					")"
				});
			}
			else
			{
				string stringId3 = this.KeyId.GetStringId();
				text = string.Concat(new object[]
				{
					"C(",
					(int)this.ContainerType,
					")-(",
					stringId3,
					")"
				});
			}
			return text;
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0000AAC3 File Offset: 0x00008CC3
		public override string GetStringId()
		{
			return this._stringId;
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0000AACB File Offset: 0x00008CCB
		public override void WriteTo(IWriter writer)
		{
			writer.WriteByte(2);
			writer.WriteByte((byte)this.ContainerType);
			this.KeyId.WriteTo(writer);
			if (this.ContainerType == ContainerType.Dictionary)
			{
				this.ValueId.WriteTo(writer);
			}
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000AB04 File Offset: 0x00008D04
		public static ContainerSaveId ReadFrom(IReader reader)
		{
			ContainerType containerType = (ContainerType)reader.ReadByte();
			int num = ((containerType == ContainerType.Dictionary) ? 2 : 1);
			List<SaveId> list = new List<SaveId>();
			for (int i = 0; i < num; i++)
			{
				SaveId saveId = null;
				byte b = reader.ReadByte();
				if (b == 0)
				{
					saveId = TypeSaveId.ReadFrom(reader);
				}
				else if (b == 1)
				{
					saveId = GenericSaveId.ReadFrom(reader);
				}
				else if (b == 2)
				{
					saveId = ContainerSaveId.ReadFrom(reader);
				}
				list.Add(saveId);
			}
			SaveId saveId2 = list[0];
			SaveId saveId3 = ((list.Count > 1) ? list[1] : null);
			return new ContainerSaveId(containerType, saveId2, saveId3);
		}

		// Token: 0x040000C9 RID: 201
		private readonly string _stringId;
	}
}
