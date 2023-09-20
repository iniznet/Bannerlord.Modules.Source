using System;
using System.IO;

namespace TaleWorlds.Library
{
	public class ResourceDepotLocation
	{
		public string BasePath { get; private set; }

		public string Path { get; private set; }

		public string FullPath { get; private set; }

		public FileSystemWatcher Watcher { get; private set; }

		public ResourceDepotLocation(string basePath, string path, string fullPath)
		{
			this.BasePath = basePath;
			this.Path = path;
			this.FullPath = fullPath;
		}

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

		public void StopWatchingChanges()
		{
			this.Watcher.Dispose();
		}
	}
}
