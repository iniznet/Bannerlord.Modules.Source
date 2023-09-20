using System;
using System.Collections.Generic;
using System.IO;

namespace TaleWorlds.Library
{
	public class ResourceDepot
	{
		public event ResourceChangeEvent OnResourceChange;

		public MBReadOnlyList<ResourceDepotLocation> ResourceLocations
		{
			get
			{
				return this._resourceLocations;
			}
		}

		public ResourceDepot()
		{
			this._resourceLocations = new MBList<ResourceDepotLocation>();
			this._files = new Dictionary<string, ResourceDepotFile>();
		}

		public void AddLocation(string basePath, string location)
		{
			ResourceDepotLocation resourceDepotLocation = new ResourceDepotLocation(basePath, location, Path.GetFullPath(basePath + location));
			this._resourceLocations.Add(resourceDepotLocation);
		}

		public void CollectResources()
		{
			this._files.Clear();
			foreach (ResourceDepotLocation resourceDepotLocation in this._resourceLocations)
			{
				string fullPath = Path.GetFullPath(resourceDepotLocation.BasePath + resourceDepotLocation.Path);
				string[] files = Directory.GetFiles(resourceDepotLocation.BasePath + resourceDepotLocation.Path, "*", SearchOption.AllDirectories);
				for (int i = 0; i < files.Length; i++)
				{
					string text = Path.GetFullPath(files[i]);
					text = text.Replace('\\', '/');
					string text2 = text.Substring(fullPath.Length);
					string text3 = text2.ToLower();
					ResourceDepotFile resourceDepotFile = new ResourceDepotFile(resourceDepotLocation, text2, text);
					if (this._files.ContainsKey(text3))
					{
						this._files[text3] = resourceDepotFile;
					}
					else
					{
						this._files.Add(text3, resourceDepotFile);
					}
				}
			}
		}

		public string[] GetFiles(string subDirectory, string extension, bool excludeSubContents = false)
		{
			string text = extension.ToLower();
			List<string> list = new List<string>();
			foreach (ResourceDepotFile resourceDepotFile in this._files.Values)
			{
				string text2 = Path.GetFullPath(resourceDepotFile.BasePath + resourceDepotFile.Location + subDirectory).Replace('\\', '/').ToLower();
				string fullPath = resourceDepotFile.FullPath;
				string fullPathLowerCase = resourceDepotFile.FullPathLowerCase;
				bool flag = (!excludeSubContents && fullPathLowerCase.StartsWith(text2)) || (excludeSubContents && string.Equals(Directory.GetParent(text2).FullName, text2, StringComparison.CurrentCultureIgnoreCase));
				bool flag2 = fullPathLowerCase.EndsWith(text, StringComparison.OrdinalIgnoreCase);
				if (flag && flag2)
				{
					list.Add(fullPath);
				}
			}
			return list.ToArray();
		}

		public string GetFilePath(string file)
		{
			file = file.Replace('\\', '/');
			return this._files[file.ToLower()].FullPath;
		}

		public IEnumerable<string> GetFilesEndingWith(string fileEndName)
		{
			fileEndName = fileEndName.Replace('\\', '/');
			foreach (KeyValuePair<string, ResourceDepotFile> keyValuePair in this._files)
			{
				if (keyValuePair.Key.EndsWith(fileEndName.ToLower()))
				{
					yield return keyValuePair.Value.FullPath;
				}
			}
			Dictionary<string, ResourceDepotFile>.Enumerator enumerator = default(Dictionary<string, ResourceDepotFile>.Enumerator);
			yield break;
			yield break;
		}

		public void StartWatchingChangesInDepot()
		{
			foreach (ResourceDepotLocation resourceDepotLocation in this._resourceLocations)
			{
				resourceDepotLocation.StartWatchingChanges(new FileSystemEventHandler(this.OnAnyChangeInDepotLocations), new RenamedEventHandler(this.OnAnyRenameInDepotLocations));
			}
		}

		public void StopWatchingChangesInDepot()
		{
			foreach (ResourceDepotLocation resourceDepotLocation in this._resourceLocations)
			{
				resourceDepotLocation.StopWatchingChanges();
			}
		}

		private void OnAnyChangeInDepotLocations(object source, FileSystemEventArgs e)
		{
			this._isThereAnyUnhandledChange = true;
		}

		private void OnAnyRenameInDepotLocations(object source, RenamedEventArgs e)
		{
			this._isThereAnyUnhandledChange = true;
		}

		public void CheckForChanges()
		{
			if (this._isThereAnyUnhandledChange)
			{
				this.CollectResources();
				ResourceChangeEvent onResourceChange = this.OnResourceChange;
				if (onResourceChange != null)
				{
					onResourceChange();
				}
				this._isThereAnyUnhandledChange = false;
			}
		}

		private readonly MBList<ResourceDepotLocation> _resourceLocations;

		private readonly Dictionary<string, ResourceDepotFile> _files;

		private bool _isThereAnyUnhandledChange;
	}
}
