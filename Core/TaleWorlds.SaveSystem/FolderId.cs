using System;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x0200001B RID: 27
	public struct FolderId : IEquatable<FolderId>
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000085 RID: 133 RVA: 0x00003EA3 File Offset: 0x000020A3
		// (set) Token: 0x06000086 RID: 134 RVA: 0x00003EAB File Offset: 0x000020AB
		public int LocalId { get; private set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00003EB4 File Offset: 0x000020B4
		// (set) Token: 0x06000088 RID: 136 RVA: 0x00003EBC File Offset: 0x000020BC
		public SaveFolderExtension Extension { get; private set; }

		// Token: 0x06000089 RID: 137 RVA: 0x00003EC5 File Offset: 0x000020C5
		public FolderId(int localId, SaveFolderExtension extension)
		{
			this.LocalId = localId;
			this.Extension = extension;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003ED8 File Offset: 0x000020D8
		public override bool Equals(object obj)
		{
			if (!(obj is FolderId))
			{
				return false;
			}
			FolderId folderId = (FolderId)obj;
			return folderId.LocalId == this.LocalId && folderId.Extension == this.Extension;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00003F16 File Offset: 0x00002116
		public bool Equals(FolderId other)
		{
			return other.LocalId == this.LocalId && other.Extension == this.Extension;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00003F38 File Offset: 0x00002138
		public override int GetHashCode()
		{
			return (this.LocalId.GetHashCode() * 397) ^ ((int)this.Extension).GetHashCode();
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00003F68 File Offset: 0x00002168
		public static bool operator ==(FolderId a, FolderId b)
		{
			return a.LocalId == b.LocalId && a.Extension == b.Extension;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00003F8C File Offset: 0x0000218C
		public static bool operator !=(FolderId a, FolderId b)
		{
			return !(a == b);
		}
	}
}
