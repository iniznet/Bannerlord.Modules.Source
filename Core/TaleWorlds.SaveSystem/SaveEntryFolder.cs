using System;
using System.Collections.Generic;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x0200001D RID: 29
	public class SaveEntryFolder
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000099 RID: 153 RVA: 0x0000408C File Offset: 0x0000228C
		// (set) Token: 0x0600009A RID: 154 RVA: 0x00004094 File Offset: 0x00002294
		public int GlobalId { get; private set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600009B RID: 155 RVA: 0x0000409D File Offset: 0x0000229D
		// (set) Token: 0x0600009C RID: 156 RVA: 0x000040A5 File Offset: 0x000022A5
		public int ParentGlobalId { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600009D RID: 157 RVA: 0x000040AE File Offset: 0x000022AE
		// (set) Token: 0x0600009E RID: 158 RVA: 0x000040B6 File Offset: 0x000022B6
		public FolderId FolderId { get; private set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600009F RID: 159 RVA: 0x000040BF File Offset: 0x000022BF
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

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x000040CF File Offset: 0x000022CF
		public Dictionary<EntryId, SaveEntry>.ValueCollection ChildEntries
		{
			get
			{
				return this._entries.Values;
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000040DC File Offset: 0x000022DC
		public static SaveEntryFolder CreateRootFolder()
		{
			return new SaveEntryFolder(-1, -1, new FolderId(-1, SaveFolderExtension.Root), 3);
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x000040ED File Offset: 0x000022ED
		public Dictionary<FolderId, SaveEntryFolder>.ValueCollection ChildFolders
		{
			get
			{
				return this._saveEntryFolders.Values;
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000040FA File Offset: 0x000022FA
		public SaveEntryFolder(SaveEntryFolder parent, int globalId, FolderId folderId, int entryCount)
			: this(parent.GlobalId, globalId, folderId, entryCount)
		{
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x0000410C File Offset: 0x0000230C
		public SaveEntryFolder(int parentGlobalId, int globalId, FolderId folderId, int entryCount)
		{
			this.ParentGlobalId = parentGlobalId;
			this.GlobalId = globalId;
			this.FolderId = folderId;
			this._entries = new Dictionary<EntryId, SaveEntry>(entryCount);
			this._saveEntryFolders = new Dictionary<FolderId, SaveEntryFolder>(3);
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00004142 File Offset: 0x00002342
		public void AddEntry(SaveEntry saveEntry)
		{
			this._entries.Add(saveEntry.Id, saveEntry);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00004156 File Offset: 0x00002356
		public SaveEntry GetEntry(EntryId entryId)
		{
			return this._entries[entryId];
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00004164 File Offset: 0x00002364
		public void AddChildFolderEntry(SaveEntryFolder saveEntryFolder)
		{
			this._saveEntryFolders.Add(saveEntryFolder.FolderId, saveEntryFolder);
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00004178 File Offset: 0x00002378
		internal SaveEntryFolder GetChildFolder(FolderId folderId)
		{
			return this._saveEntryFolders[folderId];
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00004188 File Offset: 0x00002388
		public SaveEntry CreateEntry(EntryId entryId)
		{
			SaveEntry saveEntry = SaveEntry.CreateNew(this, entryId);
			this.AddEntry(saveEntry);
			return saveEntry;
		}

		// Token: 0x0400003F RID: 63
		private Dictionary<FolderId, SaveEntryFolder> _saveEntryFolders;

		// Token: 0x04000040 RID: 64
		private Dictionary<EntryId, SaveEntry> _entries;
	}
}
