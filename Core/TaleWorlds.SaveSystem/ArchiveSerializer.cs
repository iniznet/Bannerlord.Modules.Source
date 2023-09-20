using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	internal class ArchiveSerializer : IArchiveContext
	{
		public ArchiveSerializer()
		{
			this._writer = BinaryWriterFactory.GetBinaryWriter();
			this._folders = new List<SaveEntryFolder>();
		}

		public void SerializeEntry(SaveEntry entry)
		{
			this._writer.Write3ByteInt(entry.FolderId);
			this._writer.Write3ByteInt(entry.Id.Id);
			this._writer.WriteByte((byte)entry.Id.Extension);
			this._writer.WriteShort((short)entry.Data.Length);
			this._writer.WriteBytes(entry.Data);
			this._entryCount++;
		}

		public void SerializeFolder(SaveEntryFolder folder)
		{
			foreach (SaveEntry saveEntry in folder.AllEntries)
			{
				this.SerializeEntry(saveEntry);
			}
		}

		public SaveEntryFolder CreateFolder(SaveEntryFolder parentFolder, FolderId folderId, int entryCount)
		{
			int folderCount = this._folderCount;
			this._folderCount++;
			SaveEntryFolder saveEntryFolder = new SaveEntryFolder(parentFolder, folderCount, folderId, entryCount);
			parentFolder.AddChildFolderEntry(saveEntryFolder);
			this._folders.Add(saveEntryFolder);
			return saveEntryFolder;
		}

		public byte[] FinalizeAndGetBinaryData()
		{
			BinaryWriter binaryWriter = BinaryWriterFactory.GetBinaryWriter();
			binaryWriter.WriteInt(this._folderCount);
			for (int i = 0; i < this._folderCount; i++)
			{
				SaveEntryFolder saveEntryFolder = this._folders[i];
				int parentGlobalId = saveEntryFolder.ParentGlobalId;
				int globalId = saveEntryFolder.GlobalId;
				int localId = saveEntryFolder.FolderId.LocalId;
				SaveFolderExtension extension = saveEntryFolder.FolderId.Extension;
				binaryWriter.Write3ByteInt(parentGlobalId);
				binaryWriter.Write3ByteInt(globalId);
				binaryWriter.Write3ByteInt(localId);
				binaryWriter.WriteByte((byte)extension);
			}
			binaryWriter.WriteInt(this._entryCount);
			binaryWriter.AppendData(this._writer);
			byte[] data = binaryWriter.Data;
			BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter);
			BinaryWriterFactory.ReleaseBinaryWriter(this._writer);
			this._writer = null;
			return data;
		}

		private BinaryWriter _writer;

		private int _entryCount;

		private int _folderCount;

		private List<SaveEntryFolder> _folders;
	}
}
