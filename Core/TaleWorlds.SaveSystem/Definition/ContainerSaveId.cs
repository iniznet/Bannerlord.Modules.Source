using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	public class ContainerSaveId : SaveId
	{
		public ContainerType ContainerType { get; set; }

		public SaveId KeyId { get; set; }

		public SaveId ValueId { get; set; }

		public ContainerSaveId(ContainerType containerType, SaveId elementId)
		{
			this.ContainerType = containerType;
			this.KeyId = elementId;
			this._stringId = this.CalculateStringId();
		}

		public ContainerSaveId(ContainerType containerType, SaveId keyId, SaveId valueId)
		{
			this.ContainerType = containerType;
			this.KeyId = keyId;
			this.ValueId = valueId;
			this._stringId = this.CalculateStringId();
		}

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

		public override string GetStringId()
		{
			return this._stringId;
		}

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

		private readonly string _stringId;
	}
}
