using System;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x0200001C RID: 28
	public struct EntryId : IEquatable<EntryId>
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00003F98 File Offset: 0x00002198
		// (set) Token: 0x06000090 RID: 144 RVA: 0x00003FA0 File Offset: 0x000021A0
		public int Id { get; private set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000091 RID: 145 RVA: 0x00003FA9 File Offset: 0x000021A9
		// (set) Token: 0x06000092 RID: 146 RVA: 0x00003FB1 File Offset: 0x000021B1
		public SaveEntryExtension Extension { get; private set; }

		// Token: 0x06000093 RID: 147 RVA: 0x00003FBA File Offset: 0x000021BA
		public EntryId(int id, SaveEntryExtension extension)
		{
			this.Id = id;
			this.Extension = extension;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00003FCC File Offset: 0x000021CC
		public override bool Equals(object obj)
		{
			if (!(obj is EntryId))
			{
				return false;
			}
			EntryId entryId = (EntryId)obj;
			return entryId.Id == this.Id && entryId.Extension == this.Extension;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000400A File Offset: 0x0000220A
		public bool Equals(EntryId other)
		{
			return other.Id == this.Id && other.Extension == this.Extension;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x0000402C File Offset: 0x0000222C
		public override int GetHashCode()
		{
			return (this.Id.GetHashCode() * 397) ^ ((int)this.Extension).GetHashCode();
		}

		// Token: 0x06000097 RID: 151 RVA: 0x0000405C File Offset: 0x0000225C
		public static bool operator ==(EntryId a, EntryId b)
		{
			return a.Id == b.Id && a.Extension == b.Extension;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00004080 File Offset: 0x00002280
		public static bool operator !=(EntryId a, EntryId b)
		{
			return !(a == b);
		}
	}
}
