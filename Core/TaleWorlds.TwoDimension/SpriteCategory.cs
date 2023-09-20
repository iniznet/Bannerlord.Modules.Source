using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	public class SpriteCategory
	{
		public string Name { get; private set; }

		public SpriteData SpriteData { get; private set; }

		public List<SpritePart> SpriteParts { get; private set; }

		public List<SpritePart> SortedSpritePartList { get; private set; }

		public List<Texture> SpriteSheets { get; private set; }

		public int SpriteSheetCount { get; set; }

		public bool IsLoaded { get; private set; }

		public Vec2i[] SheetSizes { get; set; }

		public SpriteCategory(string name, SpriteData spriteData, int spriteSheetCount, bool alwaysLoad = false)
		{
			this.Name = name;
			this.SpriteData = spriteData;
			this.SpriteSheetCount = spriteSheetCount;
			this.AlwaysLoad = alwaysLoad;
			this.SpriteSheets = new List<Texture>();
			this.SpriteParts = new List<SpritePart>();
			this.SortedSpritePartList = new List<SpritePart>();
			this.SheetSizes = new Vec2i[spriteSheetCount];
			this._spritePartComparer = new SpriteCategory.SpriteSizeComparer();
		}

		public void Load(ITwoDimensionResourceContext resourceContext, ResourceDepot resourceDepot)
		{
			if (!this.IsLoaded)
			{
				this.IsLoaded = true;
				for (int i = 1; i <= this.SpriteSheetCount; i++)
				{
					Texture texture = resourceContext.LoadTexture(resourceDepot, string.Concat(new object[] { "SpriteSheets\\", this.Name, "\\", this.Name, "_", i }));
					this.SpriteSheets.Add(texture);
				}
			}
		}

		public void Unload()
		{
			if (this.IsLoaded)
			{
				this.SpriteSheets.ForEach(delegate(Texture s)
				{
					s.PlatformTexture.Release();
				});
				this.SpriteSheets.Clear();
				this.IsLoaded = false;
			}
		}

		public void InitializePartialLoad()
		{
			if (!this.IsLoaded)
			{
				this.IsLoaded = true;
				for (int i = 1; i <= this.SpriteSheetCount; i++)
				{
					this.SpriteSheets.Add(null);
				}
			}
		}

		public void ReleasePartialLoad()
		{
			if (this.IsLoaded)
			{
				for (int i = 1; i < this.SpriteSheetCount; i++)
				{
					this.PartialUnloadAtIndex(i);
				}
				this.SpriteSheets.Clear();
				this.IsLoaded = false;
			}
		}

		public void PartialLoadAtIndex(ITwoDimensionResourceContext resourceContext, ResourceDepot resourceDepot, int sheetIndex)
		{
			if (sheetIndex >= 1 && sheetIndex <= this.SpriteSheetCount && this.IsLoaded && this.SpriteSheets[sheetIndex - 1] == null)
			{
				Texture texture = resourceContext.LoadTexture(resourceDepot, string.Concat(new object[] { "SpriteSheets\\", this.Name, "\\", this.Name, "_", sheetIndex }));
				this.SpriteSheets[sheetIndex - 1] = texture;
			}
		}

		public void PartialUnloadAtIndex(int sheetIndex)
		{
			if (sheetIndex >= 1 && sheetIndex <= this.SpriteSheetCount && this.IsLoaded && this.SpriteSheets[sheetIndex - 1] != null)
			{
				this.SpriteSheets[sheetIndex - 1].PlatformTexture.Release();
				this.SpriteSheets[sheetIndex - 1] = null;
			}
		}

		public void SortList()
		{
			this.SortedSpritePartList.Clear();
			this.SortedSpritePartList.AddRange(this.SpriteParts);
			this.SortedSpritePartList.Sort(this._spritePartComparer);
		}

		public bool IsCategoryFullyLoaded()
		{
			for (int i = 0; i < this.SpriteSheets.Count; i++)
			{
				Texture texture = this.SpriteSheets[i];
				if (texture != null && !texture.IsLoaded())
				{
					return false;
				}
			}
			return true;
		}

		public const int SpriteSheetSize = 4096;

		public readonly bool AlwaysLoad;

		private SpriteCategory.SpriteSizeComparer _spritePartComparer;

		protected class SpriteSizeComparer : IComparer<SpritePart>
		{
			public int Compare(SpritePart x, SpritePart y)
			{
				return y.Width * y.Height - x.Width * x.Height;
			}
		}
	}
}
