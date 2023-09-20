using System;
using System.Globalization;

namespace TaleWorlds.Library
{
	// Token: 0x02000086 RID: 134
	public class StringReader : IReader
	{
		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600047F RID: 1151 RVA: 0x0000EC89 File Offset: 0x0000CE89
		// (set) Token: 0x06000480 RID: 1152 RVA: 0x0000EC91 File Offset: 0x0000CE91
		public string Data { get; private set; }

		// Token: 0x06000481 RID: 1153 RVA: 0x0000EC9A File Offset: 0x0000CE9A
		private string GetNextToken()
		{
			string text = this._tokens[this._currentIndex];
			this._currentIndex++;
			return text;
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x0000ECB7 File Offset: 0x0000CEB7
		public StringReader(string data)
		{
			this.Data = data;
			this._tokens = data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x0000ECDE File Offset: 0x0000CEDE
		public ISerializableObject ReadSerializableObject()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x0000ECE5 File Offset: 0x0000CEE5
		public int ReadInt()
		{
			return Convert.ToInt32(this.GetNextToken());
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x0000ECF2 File Offset: 0x0000CEF2
		public short ReadShort()
		{
			return Convert.ToInt16(this.GetNextToken());
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x0000ED00 File Offset: 0x0000CF00
		public string ReadString()
		{
			int num = this.ReadInt();
			int i = 0;
			string text = "";
			while (i < num)
			{
				string nextToken = this.GetNextToken();
				text += nextToken;
				i = text.Length;
				if (i < num)
				{
					text += " ";
				}
			}
			if (text.Length != num)
			{
				throw new Exception("invalid string format, length does not match");
			}
			return text;
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x0000ED60 File Offset: 0x0000CF60
		public Color ReadColor()
		{
			float num = this.ReadFloat();
			float num2 = this.ReadFloat();
			float num3 = this.ReadFloat();
			float num4 = this.ReadFloat();
			return new Color(num, num2, num3, num4);
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x0000ED94 File Offset: 0x0000CF94
		public bool ReadBool()
		{
			string nextToken = this.GetNextToken();
			return nextToken == "1" || (!(nextToken == "0") && Convert.ToBoolean(nextToken));
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x0000EDCC File Offset: 0x0000CFCC
		public float ReadFloat()
		{
			return Convert.ToSingle(this.GetNextToken(), CultureInfo.InvariantCulture);
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x0000EDDE File Offset: 0x0000CFDE
		public uint ReadUInt()
		{
			return Convert.ToUInt32(this.GetNextToken());
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x0000EDEB File Offset: 0x0000CFEB
		public ulong ReadULong()
		{
			return Convert.ToUInt64(this.GetNextToken());
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x0000EDF8 File Offset: 0x0000CFF8
		public long ReadLong()
		{
			return Convert.ToInt64(this.GetNextToken());
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x0000EE05 File Offset: 0x0000D005
		public byte ReadByte()
		{
			return Convert.ToByte(this.GetNextToken());
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x0000EE12 File Offset: 0x0000D012
		public byte[] ReadBytes(int length)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x0000EE1C File Offset: 0x0000D01C
		public Vec2 ReadVec2()
		{
			float num = this.ReadFloat();
			float num2 = this.ReadFloat();
			return new Vec2(num, num2);
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x0000EE3C File Offset: 0x0000D03C
		public Vec3 ReadVec3()
		{
			float num = this.ReadFloat();
			float num2 = this.ReadFloat();
			float num3 = this.ReadFloat();
			float num4 = this.ReadFloat();
			return new Vec3(num, num2, num3, num4);
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x0000EE6C File Offset: 0x0000D06C
		public Vec3i ReadVec3Int()
		{
			int num = this.ReadInt();
			int num2 = this.ReadInt();
			int num3 = this.ReadInt();
			return new Vec3i(num, num2, num3);
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x0000EE94 File Offset: 0x0000D094
		public sbyte ReadSByte()
		{
			return Convert.ToSByte(this.GetNextToken());
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x0000EEA1 File Offset: 0x0000D0A1
		public ushort ReadUShort()
		{
			return Convert.ToUInt16(this.GetNextToken());
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x0000EEAE File Offset: 0x0000D0AE
		public double ReadDouble()
		{
			return Convert.ToDouble(this.GetNextToken());
		}

		// Token: 0x04000161 RID: 353
		private string[] _tokens;

		// Token: 0x04000162 RID: 354
		private int _currentIndex;
	}
}
