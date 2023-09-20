using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x02000018 RID: 24
	internal class ArchiveSerializer : IArchiveContext
	{
		// Token: 0x06000080 RID: 128 RVA: 0x00003CAC File Offset: 0x00001EAC
		public ArchiveSerializer()
		{
			this._writer = BinaryWriterFactory.GetBinaryWriter();
			this._folders = new List<SaveEntryFolder>();
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00003CCC File Offset: 0x00001ECC
		public void SerializeEntry(SaveEntry entry)
		{
			this._writer.Write3ByteInt(entry.FolderId);
			this._writer.Write3ByteInt(entry.Id.Id);
			this._writer.WriteByte((byte)entry.Id.Extension);
			this._writer.WriteShort((short)entry.Data.Length);
			this._writer.WriteBytes(entry.Data);
			this._entryCount++;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00003D50 File Offset: 0x00001F50
		public void SerializeFolder(SaveEntryFolder folder)
		{
			foreach (SaveEntry saveEntry in folder.AllEntries)
			{
				this.SerializeEntry(saveEntry);
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00003DA0 File Offset: 0x00001FA0
		public SaveEntryFolder CreateFolder(SaveEntryFolder parentFolder, FolderId folderId, int entryCount)
		{
			int folderCount = this._folderCount;
			this._folderCount++;
			SaveEntryFolder saveEntryFolder = new SaveEntryFolder(parentFolder, folderCount, folderId, entryCount);
			parentFolder.AddChildFolderEntry(saveEntryFolder);
			this._folders.Add(saveEntryFolder);
			return saveEntryFolder;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003DE0 File Offset: 0x00001FE0
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

		// Token: 0x04000022 RID: 34
		private BinaryWriter _writer;

		// Token: 0x04000023 RID: 35
		private int _entryCount;

		// Token: 0x04000024 RID: 36
		private int _folderCount;

		// Token: 0x04000025 RID: 37
		private List<SaveEntryFolder> _folders;
	}
}
