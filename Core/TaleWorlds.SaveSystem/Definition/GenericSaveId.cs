using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	internal class GenericSaveId : SaveId
	{
		public SaveId BaseId { get; set; }

		public SaveId[] GenericTypeIDs { get; set; }

		public GenericSaveId(TypeSaveId baseId, SaveId[] saveIds)
		{
			this.BaseId = baseId;
			this.GenericTypeIDs = saveIds;
			this._stringId = this.CalculateStringId();
		}

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

		public override string GetStringId()
		{
			return this._stringId;
		}

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

		private readonly string _stringId;
	}
}
