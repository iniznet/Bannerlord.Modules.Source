using System;
using System.Collections.Generic;
using System.IO;

namespace TaleWorlds.Library
{
	// Token: 0x0200007D RID: 125
	public class ResourceDepot
	{
		// Token: 0x14000014 RID: 20
		// (add) Token: 0x06000450 RID: 1104 RVA: 0x0000E508 File Offset: 0x0000C708
		// (remove) Token: 0x06000451 RID: 1105 RVA: 0x0000E540 File Offset: 0x0000C740
		public event ResourceChangeEvent OnResourceChange;

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000452 RID: 1106 RVA: 0x0000E575 File Offset: 0x0000C775
		public MBReadOnlyList<ResourceDepotLocation> ResourceLocations
		{
			get
			{
				return this._resourceLocations;
			}
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x0000E57D File Offset: 0x0000C77D
		public ResourceDepot()
		{
			this._resourceLocations = new MBList<ResourceDepotLocation>();
			this._files = new Dictionary<string, ResourceDepotFile>();
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x0000E59C File Offset: 0x0000C79C
		public void AddLocation(string basePath, string location)
		{
			ResourceDepotLocation resourceDepotLocation = new ResourceDepotLocation(basePath, location, Path.GetFullPath(basePath + location));
			this._resourceLocations.Add(resourceDepotLocation);
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x0000E5CC File Offset: 0x0000C7CC
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

		// Token: 0x06000456 RID: 1110 RVA: 0x0000E6DC File Offset: 0x0000C8DC
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

		// Token: 0x06000457 RID: 1111 RVA: 0x0000E7C0 File Offset: 0x0000C9C0
		public string GetFilePath(string file)
		{
			file = file.Replace('\\', '/');
			return this._files[file.ToLower()].FullPath;
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x0000E7E4 File Offset: 0x0000C9E4
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

		// Token: 0x06000459 RID: 1113 RVA: 0x0000E7FC File Offset: 0x0000C9FC
		public void StartWatchingChangesInDepot()
		{
			foreach (ResourceDepotLocation resourceDepotLocation in this._resourceLocations)
			{
				resourceDepotLocation.StartWatchingChanges(new FileSystemEventHandler(this.OnAnyChangeInDepotLocations), new RenamedEventHandler(this.OnAnyRenameInDepotLocations));
			}
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x0000E864 File Offset: 0x0000CA64
		public void StopWatchingChangesInDepot()
		{
			foreach (ResourceDepotLocation resourceDepotLocation in this._resourceLocations)
			{
				resourceDepotLocation.StopWatchingChanges();
			}
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x0000E8B4 File Offset: 0x0000CAB4
		private void OnAnyChangeInDepotLocations(object source, FileSystemEventArgs e)
		{
			this._isThereAnyUnhandledChange = true;
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x0000E8BD File Offset: 0x0000CABD
		private void OnAnyRenameInDepotLocations(object source, RenamedEventArgs e)
		{
			this._isThereAnyUnhandledChange = true;
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x0000E8C6 File Offset: 0x0000CAC6
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

		// Token: 0x04000144 RID: 324
		private readonly MBList<ResourceDepotLocation> _resourceLocations;

		// Token: 0x04000145 RID: 325
		private readonly Dictionary<string, ResourceDepotFile> _files;

		// Token: 0x04000146 RID: 326
		private bool _isThereAnyUnhandledChange;
	}
}
