using System;

namespace TaleWorlds.SaveSystem
{
	public struct EntryId : IEquatable<EntryId>
	{
		public int Id { get; private set; }

		public SaveEntryExtension Extension { get; private set; }

		public EntryId(int id, SaveEntryExtension extension)
		{
			this.Id = id;
			this.Extension = extension;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is EntryId))
			{
				return false;
			}
			EntryId entryId = (EntryId)obj;
			return entryId.Id == this.Id && entryId.Extension == this.Extension;
		}

		public bool Equals(EntryId other)
		{
			return other.Id == this.Id && other.Extension == this.Extension;
		}

		public override int GetHashCode()
		{
			return (this.Id.GetHashCode() * 397) ^ ((int)this.Extension).GetHashCode();
		}

		public static bool operator ==(EntryId a, EntryId b)
		{
			return a.Id == b.Id && a.Extension == b.Extension;
		}

		public static bool operator !=(EntryId a, EntryId b)
		{
			return !(a == b);
		}
	}
}
