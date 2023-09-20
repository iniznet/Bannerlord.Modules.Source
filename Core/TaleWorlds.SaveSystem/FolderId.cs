using System;

namespace TaleWorlds.SaveSystem
{
	public struct FolderId : IEquatable<FolderId>
	{
		public int LocalId { get; private set; }

		public SaveFolderExtension Extension { get; private set; }

		public FolderId(int localId, SaveFolderExtension extension)
		{
			this.LocalId = localId;
			this.Extension = extension;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is FolderId))
			{
				return false;
			}
			FolderId folderId = (FolderId)obj;
			return folderId.LocalId == this.LocalId && folderId.Extension == this.Extension;
		}

		public bool Equals(FolderId other)
		{
			return other.LocalId == this.LocalId && other.Extension == this.Extension;
		}

		public override int GetHashCode()
		{
			return (this.LocalId.GetHashCode() * 397) ^ ((int)this.Extension).GetHashCode();
		}

		public static bool operator ==(FolderId a, FolderId b)
		{
			return a.LocalId == b.LocalId && a.Extension == b.Extension;
		}

		public static bool operator !=(FolderId a, FolderId b)
		{
			return !(a == b);
		}
	}
}
