using System;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x02000016 RID: 22
	internal interface IArchiveContext
	{
		// Token: 0x0600007A RID: 122
		SaveEntryFolder CreateFolder(SaveEntryFolder parentFolder, FolderId folderId, int entryCount);
	}
}
