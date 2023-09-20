using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x0200001E RID: 30
	public class SaveEntry
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000AA RID: 170 RVA: 0x000041A5 File Offset: 0x000023A5
		public byte[] Data
		{
			get
			{
				return this._data;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000AB RID: 171 RVA: 0x000041AD File Offset: 0x000023AD
		// (set) Token: 0x060000AC RID: 172 RVA: 0x000041B5 File Offset: 0x000023B5
		public EntryId Id { get; private set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000AD RID: 173 RVA: 0x000041BE File Offset: 0x000023BE
		// (set) Token: 0x060000AE RID: 174 RVA: 0x000041C6 File Offset: 0x000023C6
		public int FolderId { get; private set; }

		// Token: 0x060000B0 RID: 176 RVA: 0x000041D7 File Offset: 0x000023D7
		public static SaveEntry CreateFrom(int entryFolderId, EntryId entryId, byte[] data)
		{
			return new SaveEntry
			{
				FolderId = entryFolderId,
				Id = entryId,
				_data = data
			};
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000041F3 File Offset: 0x000023F3
		public static SaveEntry CreateNew(SaveEntryFolder parentFolder, EntryId entryId)
		{
			return new SaveEntry
			{
				Id = entryId,
				FolderId = parentFolder.GlobalId
			};
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x0000420D File Offset: 0x0000240D
		public BinaryReader GetBinaryReader()
		{
			return new BinaryReader(this._data);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x0000421A File Offset: 0x0000241A
		public void FillFrom(BinaryWriter writer)
		{
			this._data = writer.Data;
		}

		// Token: 0x04000041 RID: 65
		private byte[] _data;
	}
}
