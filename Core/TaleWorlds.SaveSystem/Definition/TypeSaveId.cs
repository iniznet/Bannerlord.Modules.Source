using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	public class TypeSaveId : SaveId
	{
		public int Id { get; private set; }

		public TypeSaveId(int id)
		{
			this.Id = id;
			this._stringId = this.Id.ToString();
		}

		public override string GetStringId()
		{
			return this._stringId;
		}

		public override void WriteTo(IWriter writer)
		{
			writer.WriteByte(0);
			writer.WriteInt(this.Id);
		}

		public static TypeSaveId ReadFrom(IReader reader)
		{
			return new TypeSaveId(reader.ReadInt());
		}

		private readonly string _stringId;
	}
}
