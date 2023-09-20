using System;
using System.Collections.Generic;

namespace TaleWorlds.SaveSystem
{
	public class SaveEntryFolder
	{
		public int GlobalId { get; private set; }

		public int ParentGlobalId { get; private set; }

		public FolderId FolderId { get; private set; }

		public IEnumerable<SaveEntry> AllEntries
		{
			get
			{
				foreach (SaveEntry saveEntry in this._entries.Values)
				{
					yield return saveEntry;
				}
				Dictionary<EntryId, SaveEntry>.ValueCollection.Enumerator enumerator = default(Dictionary<EntryId, SaveEntry>.ValueCollection.Enumerator);
				foreach (SaveEntryFolder saveEntryFolder in this._saveEntryFolders.Values)
				{
					foreach (SaveEntry saveEntry2 in saveEntryFolder.AllEntries)
					{
						yield return saveEntry2;
					}
					IEnumerator<SaveEntry> enumerator3 = null;
				}
				Dictionary<FolderId, SaveEntryFolder>.ValueCollection.Enumerator enumerator2 = default(Dictionary<FolderId, SaveEntryFolder>.ValueCollection.Enumerator);
				yield break;
				yield break;
			}
		}

		public Dictionary<EntryId, SaveEntry>.ValueCollection ChildEntries
		{
			get
			{
				return this._entries.Values;
			}
		}

		public static SaveEntryFolder CreateRootFolder()
		{
			return new SaveEntryFolder(-1, -1, new FolderId(-1, SaveFolderExtension.Root), 3);
		}

		public Dictionary<FolderId, SaveEntryFolder>.ValueCollection ChildFolders
		{
			get
			{
				return this._saveEntryFolders.Values;
			}
		}

		public SaveEntryFolder(SaveEntryFolder parent, int globalId, FolderId folderId, int entryCount)
			: this(parent.GlobalId, globalId, folderId, entryCount)
		{
		}

		public SaveEntryFolder(int parentGlobalId, int globalId, FolderId folderId, int entryCount)
		{
			this.ParentGlobalId = parentGlobalId;
			this.GlobalId = globalId;
			this.FolderId = folderId;
			this._entries = new Dictionary<EntryId, SaveEntry>(entryCount);
			this._saveEntryFolders = new Dictionary<FolderId, SaveEntryFolder>(3);
		}

		public void AddEntry(SaveEntry saveEntry)
		{
			this._entries.Add(saveEntry.Id, saveEntry);
		}

		public SaveEntry GetEntry(EntryId entryId)
		{
			return this._entries[entryId];
		}

		public void AddChildFolderEntry(SaveEntryFolder saveEntryFolder)
		{
			this._saveEntryFolders.Add(saveEntryFolder.FolderId, saveEntryFolder);
		}

		internal SaveEntryFolder GetChildFolder(FolderId folderId)
		{
			return this._saveEntryFolders[folderId];
		}

		public SaveEntry CreateEntry(EntryId entryId)
		{
			SaveEntry saveEntry = SaveEntry.CreateNew(this, entryId);
			this.AddEntry(saveEntry);
			return saveEntry;
		}

		private Dictionary<FolderId, SaveEntryFolder> _saveEntryFolders;

		private Dictionary<EntryId, SaveEntry> _entries;
	}
}
