using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000006 RID: 6
	public class Font
	{
		// Token: 0x0600001F RID: 31 RVA: 0x000027CC File Offset: 0x000009CC
		public Font(string name, string path, SpriteData spriteData)
		{
			Debug.Print("Loading " + name + " font, at: " + path, 0, Debug.DebugColor.White, 17592186044416UL);
			this.Name = name;
			this.Characters = new Dictionary<int, BitmapFontCharacter>();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(path);
			XmlElement xmlElement = xmlDocument["font"];
			XmlElement xmlElement2 = xmlElement["info"];
			this._realSize = Math.Abs(Convert.ToInt32(xmlElement2.Attributes["size"].Value));
			this.Smooth = true;
			if (xmlElement2.Attributes["smooth"] != null)
			{
				this.Smooth = Convert.ToBoolean(Convert.ToInt32(xmlElement2.Attributes["smooth"].Value));
			}
			this.SmoothingConstant = 0.47f;
			if (xmlElement2.Attributes["smoothingConstant"] != null)
			{
				this.SmoothingConstant = Convert.ToSingle(xmlElement2.Attributes["smoothingConstant"].Value, CultureInfo.InvariantCulture);
			}
			if (xmlElement2.Attributes["customScale"] != null)
			{
				this.CustomScale = Convert.ToSingle(xmlElement2.Attributes["customScale"].Value, CultureInfo.InvariantCulture);
			}
			XmlElement xmlElement3 = xmlElement["common"];
			this.LineHeight = Convert.ToInt32(xmlElement3.Attributes["lineHeight"].Value);
			this.Base = Convert.ToInt32(xmlElement3.Attributes["base"].Value);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(xmlElement["pages"].ChildNodes[0].Attributes["file"].Value);
			XmlElement xmlElement4 = xmlElement["chars"];
			this.CharacterCount = Convert.ToInt32(xmlElement4.Attributes["count"].Value);
			string text = Path.ChangeExtension(path, ".bfnt");
			if (File.Exists(text))
			{
				using (BinaryReader binaryReader = new BinaryReader(File.Open(text, FileMode.Open, FileAccess.Read)))
				{
					for (int i = 0; i < this.CharacterCount; i++)
					{
						GCHandle gchandle = GCHandle.Alloc(binaryReader.ReadBytes(Marshal.SizeOf(typeof(BitmapFontCharacter))), GCHandleType.Pinned);
						BitmapFontCharacter bitmapFontCharacter = (BitmapFontCharacter)Marshal.PtrToStructure(gchandle.AddrOfPinnedObject(), typeof(BitmapFontCharacter));
						this.Characters.Add(bitmapFontCharacter.ID, bitmapFontCharacter);
						gchandle.Free();
					}
					goto IL_3F3;
				}
			}
			Debug.FailedAssert("Binary character data should exist for all official fonts! This is only to support modded fonts!", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.TwoDimension\\BitmapFont\\Font.cs", ".ctor", 84);
			for (int j = 0; j < this.CharacterCount; j++)
			{
				XmlNode xmlNode = xmlElement4.ChildNodes[j];
				BitmapFontCharacter bitmapFontCharacter2;
				bitmapFontCharacter2.ID = Convert.ToInt32(xmlNode.Attributes["id"].Value);
				bitmapFontCharacter2.X = Convert.ToInt32(xmlNode.Attributes["x"].Value);
				bitmapFontCharacter2.Y = Convert.ToInt32(xmlNode.Attributes["y"].Value);
				bitmapFontCharacter2.Width = Convert.ToInt32(xmlNode.Attributes["width"].Value);
				bitmapFontCharacter2.Height = Convert.ToInt32(xmlNode.Attributes["height"].Value);
				bitmapFontCharacter2.XOffset = Convert.ToInt32(xmlNode.Attributes["xoffset"].Value);
				bitmapFontCharacter2.YOffset = Convert.ToInt32(xmlNode.Attributes["yoffset"].Value);
				bitmapFontCharacter2.XAdvance = Convert.ToInt32(xmlNode.Attributes["xadvance"].Value);
				this.Characters.Add(bitmapFontCharacter2.ID, bitmapFontCharacter2);
			}
			IL_3F3:
			SpriteGeneric spriteGeneric = spriteData.GetSprite(fileNameWithoutExtension) as SpriteGeneric;
			SpritePart spritePart = ((spriteGeneric != null) ? spriteGeneric.SpritePart : null);
			this.FontSprite = spritePart;
			this.Size = (int)((float)this._realSize / this.CustomScale);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002C18 File Offset: 0x00000E18
		protected Font(string name)
		{
			this.Name = name;
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002C32 File Offset: 0x00000E32
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002C3A File Offset: 0x00000E3A
		public int Size { get; private set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002C43 File Offset: 0x00000E43
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002C4B File Offset: 0x00000E4B
		public bool Smooth { get; private set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00002C54 File Offset: 0x00000E54
		// (set) Token: 0x06000026 RID: 38 RVA: 0x00002C5C File Offset: 0x00000E5C
		public float SmoothingConstant { get; private set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00002C65 File Offset: 0x00000E65
		// (set) Token: 0x06000028 RID: 40 RVA: 0x00002C6D File Offset: 0x00000E6D
		public float CustomScale { get; private set; } = 1f;

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002C76 File Offset: 0x00000E76
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00002C7E File Offset: 0x00000E7E
		public int LineHeight { get; private set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00002C87 File Offset: 0x00000E87
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002C8F File Offset: 0x00000E8F
		public int Base { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002C98 File Offset: 0x00000E98
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002CA0 File Offset: 0x00000EA0
		public int CharacterCount { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002CA9 File Offset: 0x00000EA9
		// (set) Token: 0x06000030 RID: 48 RVA: 0x00002CB1 File Offset: 0x00000EB1
		public SpritePart FontSprite { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002CBA File Offset: 0x00000EBA
		// (set) Token: 0x06000032 RID: 50 RVA: 0x00002CC2 File Offset: 0x00000EC2
		public Dictionary<int, BitmapFontCharacter> Characters { get; private set; }

		// Token: 0x06000033 RID: 51 RVA: 0x00002CCC File Offset: 0x00000ECC
		public float GetWordWidth(string word, float extraPadding)
		{
			float num = 0f;
			for (int i = 0; i < word.Length; i++)
			{
				num += this.GetCharacterWidth(word[i], extraPadding);
			}
			return num;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002D04 File Offset: 0x00000F04
		public float GetCharacterWidth(char character, float extraPadding)
		{
			float num = 0f;
			int num2 = (int)character;
			if (!this.Characters.ContainsKey(num2))
			{
				num2 = 0;
			}
			BitmapFontCharacter bitmapFontCharacter = this.Characters[num2];
			return num + ((float)bitmapFontCharacter.XAdvance + extraPadding);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002D3F File Offset: 0x00000F3F
		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.Name))
			{
				return base.ToString();
			}
			return this.Name;
		}

		// Token: 0x04000015 RID: 21
		public readonly string Name;

		// Token: 0x04000016 RID: 22
		private int _realSize;
	}
}
