using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	internal class ArchiveDeserializer
	{
		public SaveEntryFolder RootFolder { get; private set; }

		public ArchiveDeserializer()
		{
			this.RootFolder = new SaveEntryFolder(-1, -1, new FolderId(-1, SaveFolderExtension.Root), 3);
		}

		public void LoadFrom(byte[] binaryArchive)
		{
			Dictionary<int, SaveEntryFolder> dictionary = new Dictionary<int, SaveEntryFolder>();
			List<SaveEntry> list = new List<SaveEntry>();
			BinaryReader binaryReader = new BinaryReader(binaryArchive);
			int num = binaryReader.ReadInt();
			for (int i = 0; i < num; i++)
			{
				int num2 = binaryReader.Read3ByteInt();
				int num3 = binaryReader.Read3ByteInt();
				int num4 = binaryReader.Read3ByteInt();
				SaveFolderExtension saveFolderExtension = (SaveFolderExtension)binaryReader.ReadByte();
				FolderId folderId = new FolderId(num4, saveFolderExtension);
				SaveEntryFolder saveEntryFolder = new SaveEntryFolder(num2, num3, folderId, 3);
				dictionary.Add(saveEntryFolder.GlobalId, saveEntryFolder);
			}
			int num5 = binaryReader.ReadInt();
			for (int j = 0; j < num5; j++)
			{
				int num6 = binaryReader.Read3ByteInt();
				int num7 = binaryReader.Read3ByteInt();
				SaveEntryExtension saveEntryExtension = (SaveEntryExtension)binaryReader.ReadByte();
				short num8 = binaryReader.ReadShort();
				byte[] array = binaryReader.ReadBytes((int)num8);
				SaveEntry saveEntry = SaveEntry.CreateFrom(num6, new EntryId(num7, saveEntryExtension), array);
				list.Add(saveEntry);
			}
			foreach (SaveEntryFolder saveEntryFolder2 in dictionary.Values)
			{
				if (saveEntryFolder2.ParentGlobalId != -1)
				{
					dictionary[saveEntryFolder2.ParentGlobalId].AddChildFolderEntry(saveEntryFolder2);
				}
				else
				{
					this.RootFolder.AddChildFolderEntry(saveEntryFolder2);
				}
			}
			foreach (SaveEntry saveEntry2 in list)
			{
				if (saveEntry2.FolderId != -1)
				{
					dictionary[saveEntry2.FolderId].AddEntry(saveEntry2);
				}
				else
				{
					this.RootFolder.AddEntry(saveEntry2);
				}
			}
		}
	}
}
