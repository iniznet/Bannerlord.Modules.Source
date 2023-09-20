using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x0200002E RID: 46
	public class SpriteCategory
	{
		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060001DB RID: 475 RVA: 0x00007915 File Offset: 0x00005B15
		// (set) Token: 0x060001DC RID: 476 RVA: 0x0000791D File Offset: 0x00005B1D
		public string Name { get; private set; }

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060001DD RID: 477 RVA: 0x00007926 File Offset: 0x00005B26
		// (set) Token: 0x060001DE RID: 478 RVA: 0x0000792E File Offset: 0x00005B2E
		public SpriteData SpriteData { get; private set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060001DF RID: 479 RVA: 0x00007937 File Offset: 0x00005B37
		// (set) Token: 0x060001E0 RID: 480 RVA: 0x0000793F File Offset: 0x00005B3F
		public List<SpritePart> SpriteParts { get; private set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x00007948 File Offset: 0x00005B48
		// (set) Token: 0x060001E2 RID: 482 RVA: 0x00007950 File Offset: 0x00005B50
		public List<SpritePart> SortedSpritePartList { get; private set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060001E3 RID: 483 RVA: 0x00007959 File Offset: 0x00005B59
		// (set) Token: 0x060001E4 RID: 484 RVA: 0x00007961 File Offset: 0x00005B61
		public List<Texture> SpriteSheets { get; private set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060001E5 RID: 485 RVA: 0x0000796A File Offset: 0x00005B6A
		// (set) Token: 0x060001E6 RID: 486 RVA: 0x00007972 File Offset: 0x00005B72
		public int SpriteSheetCount { get; set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060001E7 RID: 487 RVA: 0x0000797B File Offset: 0x00005B7B
		// (set) Token: 0x060001E8 RID: 488 RVA: 0x00007983 File Offset: 0x00005B83
		public bool IsLoaded { get; private set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060001E9 RID: 489 RVA: 0x0000798C File Offset: 0x00005B8C
		// (set) Token: 0x060001EA RID: 490 RVA: 0x00007994 File Offset: 0x00005B94
		public Vec2i[] SheetSizes { get; set; }

		// Token: 0x060001EB RID: 491 RVA: 0x000079A0 File Offset: 0x00005BA0
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

		// Token: 0x060001EC RID: 492 RVA: 0x00007A08 File Offset: 0x00005C08
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

		// Token: 0x060001ED RID: 493 RVA: 0x00007A88 File Offset: 0x00005C88
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

		// Token: 0x060001EE RID: 494 RVA: 0x00007ADC File Offset: 0x00005CDC
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

		// Token: 0x060001EF RID: 495 RVA: 0x00007B18 File Offset: 0x00005D18
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

		// Token: 0x060001F0 RID: 496 RVA: 0x00007B58 File Offset: 0x00005D58
		public void PartialLoadAtIndex(ITwoDimensionResourceContext resourceContext, ResourceDepot resourceDepot, int sheetIndex)
		{
			if (sheetIndex >= 1 && sheetIndex <= this.SpriteSheetCount && this.IsLoaded && this.SpriteSheets[sheetIndex - 1] == null)
			{
				Texture texture = resourceContext.LoadTexture(resourceDepot, string.Concat(new object[] { "SpriteSheets\\", this.Name, "\\", this.Name, "_", sheetIndex }));
				this.SpriteSheets[sheetIndex - 1] = texture;
			}
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x00007BE0 File Offset: 0x00005DE0
		public void PartialUnloadAtIndex(int sheetIndex)
		{
			if (sheetIndex >= 1 && sheetIndex <= this.SpriteSheetCount && this.IsLoaded && this.SpriteSheets[sheetIndex - 1] != null)
			{
				this.SpriteSheets[sheetIndex - 1].PlatformTexture.Release();
				this.SpriteSheets[sheetIndex - 1] = null;
			}
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00007C39 File Offset: 0x00005E39
		public void SortList()
		{
			this.SortedSpritePartList.Clear();
			this.SortedSpritePartList.AddRange(this.SpriteParts);
			this.SortedSpritePartList.Sort(this._spritePartComparer);
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x00007C68 File Offset: 0x00005E68
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

		// Token: 0x040000F9 RID: 249
		public const int SpriteSheetSize = 4096;

		// Token: 0x04000101 RID: 257
		public readonly bool AlwaysLoad;

		// Token: 0x04000103 RID: 259
		private SpriteCategory.SpriteSizeComparer _spritePartComparer;

		// Token: 0x02000041 RID: 65
		protected class SpriteSizeComparer : IComparer<SpritePart>
		{
			// Token: 0x060002A6 RID: 678 RVA: 0x0000A3E7 File Offset: 0x000085E7
			public int Compare(SpritePart x, SpritePart y)
			{
				return y.Width * y.Height - x.Width * x.Height;
			}
		}
	}
}
