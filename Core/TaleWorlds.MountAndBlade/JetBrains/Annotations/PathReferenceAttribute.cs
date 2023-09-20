using System;

namespace JetBrains.Annotations
{
	[AttributeUsage(AttributeTargets.Parameter)]
	public class PathReferenceAttribute : Attribute
	{
		public PathReferenceAttribute()
		{
		}

		[UsedImplicitly]
		public PathReferenceAttribute([PathReference] string basePath)
		{
			this.BasePath = basePath;
		}

		[UsedImplicitly]
		public string BasePath { get; private set; }
	}
}
