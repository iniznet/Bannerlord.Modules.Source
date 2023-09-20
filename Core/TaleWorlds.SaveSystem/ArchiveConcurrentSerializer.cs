using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x02000017 RID: 23
	internal class ArchiveConcurrentSerializer : IArchiveContext
	{
		// Token: 0x0600007B RID: 123 RVA: 0x00003A10 File Offset: 0x00001C10
		public ArchiveConcurrentSerializer()
		{
			this._locker = new object();
			this._writers = new Dictionary<int, BinaryWriter>();
			this._folders = new ConcurrentBag<SaveEntryFolder>();
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00003A3C File Offset: 0x00001C3C
		public void SerializeFolderConcurrent(SaveEntryFolder folder)
		{
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			object locker = this._locker;
			BinaryWriter binaryWriter;
			lock (locker)
			{
				if (!this._writers.TryGetValue(managedThreadId, out binaryWriter))
				{
					binaryWriter = new BinaryWriter(262144);
					this._writers.Add(managedThreadId, binaryWriter);
				}
			}
			foreach (SaveEntry saveEntry in folder.AllEntries)
			{
				this.SerializeEntryConcurrent(saveEntry, binaryWriter);
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00003AF0 File Offset: 0x00001CF0
		public SaveEntryFolder CreateFolder(SaveEntryFolder parentFolder, FolderId folderId, int entryCount)
		{
			int num = Interlocked.Increment(ref this._folderCount) - 1;
			SaveEntryFolder saveEntryFolder = new SaveEntryFolder(parentFolder, num, folderId, entryCount);
			parentFolder.AddChildFolderEntry(saveEntryFolder);
			this._folders.Add(saveEntryFolder);
			return saveEntryFolder;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00003B2C File Offset: 0x00001D2C
		private void SerializeEntryConcurrent(SaveEntry entry, BinaryWriter writer)
		{
			BinaryWriter binaryWriter = BinaryWriterFactory.GetBinaryWriter();
			binaryWriter.Write3ByteInt(entry.FolderId);
			binaryWriter.Write3ByteInt(entry.Id.Id);
			binaryWriter.WriteByte((byte)entry.Id.Extension);
			binaryWriter.WriteShort((short)entry.Data.Length);
			binaryWriter.WriteBytes(entry.Data);
			byte[] data = binaryWriter.Data;
			BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter);
			writer.WriteBytes(data);
			Interlocked.Increment(ref this._entryCount);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00003BAC File Offset: 0x00001DAC
		public byte[] FinalizeAndGetBinaryDataConcurrent()
		{
			BinaryWriter binaryWriter = new BinaryWriter();
			binaryWriter.WriteInt(this._folderCount);
			foreach (SaveEntryFolder saveEntryFolder in this._folders)
			{
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
			foreach (BinaryWriter binaryWriter2 in this._writers.Values)
			{
				binaryWriter.AppendData(binaryWriter2);
			}
			return binaryWriter.Data;
		}

		// Token: 0x0400001D RID: 29
		private int _entryCount;

		// Token: 0x0400001E RID: 30
		private int _folderCount;

		// Token: 0x0400001F RID: 31
		private object _locker;

		// Token: 0x04000020 RID: 32
		private Dictionary<int, BinaryWriter> _writers;

		// Token: 0x04000021 RID: 33
		private ConcurrentBag<SaveEntryFolder> _folders;
	}
}
