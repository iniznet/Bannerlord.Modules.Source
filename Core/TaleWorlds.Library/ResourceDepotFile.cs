using System;

namespace TaleWorlds.Library
{
	public class ResourceDepotFile
	{
		public ResourceDepotLocation ResourceDepotLocation { get; private set; }

		public string BasePath
		{
			get
			{
				return this.ResourceDepotLocation.BasePath;
			}
		}

		public string Location
		{
			get
			{
				return this.ResourceDepotLocation.Path;
			}
		}

		public string FileName { get; private set; }

		public string FullPath { get; private set; }

		public string FullPathLowerCase { get; private set; }

		public ResourceDepotFile(ResourceDepotLocation resourceDepotLocation, string fileName, string fullPath)
		{
			this.ResourceDepotLocation = resourceDepotLocation;
			this.FileName = fileName;
			this.FullPath = fullPath;
			this.FullPathLowerCase = fullPath.ToLower();
		}
	}
}
