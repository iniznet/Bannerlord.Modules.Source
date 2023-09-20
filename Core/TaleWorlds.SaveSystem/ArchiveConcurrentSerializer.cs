using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	internal class ArchiveConcurrentSerializer : IArchiveContext
	{
		public ArchiveConcurrentSerializer()
		{
			this._locker = new object();
			this._writers = new Dictionary<int, BinaryWriter>();
			this._folders = new ConcurrentBag<SaveEntryFolder>();
		}

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

		public SaveEntryFolder CreateFolder(SaveEntryFolder parentFolder, FolderId folderId, int entryCount)
		{
			int num = Interlocked.Increment(ref this._folderCount) - 1;
			SaveEntryFolder saveEntryFolder = new SaveEntryFolder(parentFolder, num, folderId, entryCount);
			parentFolder.AddChildFolderEntry(saveEntryFolder);
			this._folders.Add(saveEntryFolder);
			return saveEntryFolder;
		}

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

		private int _entryCount;

		private int _folderCount;

		private object _locker;

		private Dictionary<int, BinaryWriter> _writers;

		private ConcurrentBag<SaveEntryFolder> _folders;
	}
}
