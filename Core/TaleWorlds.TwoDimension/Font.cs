using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	public class Font
	{
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
					goto IL_3DD;
				}
			}
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
			IL_3DD:
			SpriteGeneric spriteGeneric = spriteData.GetSprite(fileNameWithoutExtension) as SpriteGeneric;
			SpritePart spritePart = ((spriteGeneric != null) ? spriteGeneric.SpritePart : null);
			this.FontSprite = spritePart;
			this.Size = (int)((float)this._realSize / this.CustomScale);
		}

		protected Font(string name)
		{
			this.Name = name;
		}

		public int Size { get; private set; }

		public bool Smooth { get; private set; }

		public float SmoothingConstant { get; private set; }

		public float CustomScale { get; private set; } = 1f;

		public int LineHeight { get; private set; }

		public int Base { get; private set; }

		public int CharacterCount { get; private set; }

		public SpritePart FontSprite { get; private set; }

		public Dictionary<int, BitmapFontCharacter> Characters { get; private set; }

		public float GetWordWidth(string word, float extraPadding)
		{
			float num = 0f;
			for (int i = 0; i < word.Length; i++)
			{
				num += this.GetCharacterWidth(word[i], extraPadding);
			}
			return num;
		}

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

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.Name))
			{
				return base.ToString();
			}
			return this.Name;
		}

		public readonly string Name;

		private int _realSize;
	}
}
