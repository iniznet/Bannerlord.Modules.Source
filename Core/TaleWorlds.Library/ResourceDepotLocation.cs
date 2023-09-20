using System;
using System.IO;

namespace TaleWorlds.Library
{
	// Token: 0x0200007F RID: 127
	public class ResourceDepotLocation
	{
		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000469 RID: 1129 RVA: 0x0000E975 File Offset: 0x0000CB75
		// (set) Token: 0x0600046A RID: 1130 RVA: 0x0000E97D File Offset: 0x0000CB7D
		public string BasePath { get; private set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600046B RID: 1131 RVA: 0x0000E986 File Offset: 0x0000CB86
		// (set) Token: 0x0600046C RID: 1132 RVA: 0x0000E98E File Offset: 0x0000CB8E
		public string Path { get; private set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600046D RID: 1133 RVA: 0x0000E997 File Offset: 0x0000CB97
		// (set) Token: 0x0600046E RID: 1134 RVA: 0x0000E99F File Offset: 0x0000CB9F
		public string FullPath { get; private set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600046F RID: 1135 RVA: 0x0000E9A8 File Offset: 0x0000CBA8
		// (set) Token: 0x06000470 RID: 1136 RVA: 0x0000E9B0 File Offset: 0x0000CBB0
		public FileSystemWatcher Watcher { get; private set; }

		// Token: 0x06000471 RID: 1137 RVA: 0x0000E9B9 File Offset: 0x0000CBB9
		public ResourceDepotLocation(string basePath, string path, string fullPath)
		{
			this.BasePath = basePath;
			this.Path = path;
			this.FullPath = fullPath;
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x0000E9D8 File Offset: 0x0000CBD8
		public void StartWatchingChanges(FileSystemEventHandler onChangeEvent, RenamedEventHandler onRenameEvent)
		{
			this.Watcher = new FileSystemWatcher
			{
				Path = this.FullPath,
				NotifyFilter = (NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.CreationTime),
				Filter = "*.*",
				IncludeSubdirectories = true,
				EnableRaisingEvents = true
			};
			this.Watcher.Changed += onChangeEvent;
			this.Watcher.Created += onChangeEvent;
			this.Watcher.Deleted += onChangeEvent;
			this.Watcher.Renamed += onRenameEvent;
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x0000EA4D File Offset: 0x0000CC4D
		public void StopWatchingChanges()
		{
			this.Watcher.Dispose();
		}
	}
}
