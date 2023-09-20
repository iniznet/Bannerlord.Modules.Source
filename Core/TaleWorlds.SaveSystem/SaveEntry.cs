using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	public class SaveEntry
	{
		public byte[] Data
		{
			get
			{
				return this._data;
			}
		}

		public EntryId Id { get; private set; }

		public int FolderId { get; private set; }

		public static SaveEntry CreateFrom(int entryFolderId, EntryId entryId, byte[] data)
		{
			return new SaveEntry
			{
				FolderId = entryFolderId,
				Id = entryId,
				_data = data
			};
		}

		public static SaveEntry CreateNew(SaveEntryFolder parentFolder, EntryId entryId)
		{
			return new SaveEntry
			{
				Id = entryId,
				FolderId = parentFolder.GlobalId
			};
		}

		public BinaryReader GetBinaryReader()
		{
			return new BinaryReader(this._data);
		}

		public void FillFrom(BinaryWriter writer)
		{
			this._data = writer.Data;
		}

		private byte[] _data;
	}
}
