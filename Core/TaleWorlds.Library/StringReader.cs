using System;
using System.Globalization;

namespace TaleWorlds.Library
{
	public class StringReader : IReader
	{
		public string Data { get; private set; }

		private string GetNextToken()
		{
			string text = this._tokens[this._currentIndex];
			this._currentIndex++;
			return text;
		}

		public StringReader(string data)
		{
			this.Data = data;
			this._tokens = data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public ISerializableObject ReadSerializableObject()
		{
			throw new NotImplementedException();
		}

		public int ReadInt()
		{
			return Convert.ToInt32(this.GetNextToken());
		}

		public short ReadShort()
		{
			return Convert.ToInt16(this.GetNextToken());
		}

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

		public Color ReadColor()
		{
			float num = this.ReadFloat();
			float num2 = this.ReadFloat();
			float num3 = this.ReadFloat();
			float num4 = this.ReadFloat();
			return new Color(num, num2, num3, num4);
		}

		public bool ReadBool()
		{
			string nextToken = this.GetNextToken();
			return nextToken == "1" || (!(nextToken == "0") && Convert.ToBoolean(nextToken));
		}

		public float ReadFloat()
		{
			return Convert.ToSingle(this.GetNextToken(), CultureInfo.InvariantCulture);
		}

		public uint ReadUInt()
		{
			return Convert.ToUInt32(this.GetNextToken());
		}

		public ulong ReadULong()
		{
			return Convert.ToUInt64(this.GetNextToken());
		}

		public long ReadLong()
		{
			return Convert.ToInt64(this.GetNextToken());
		}

		public byte ReadByte()
		{
			return Convert.ToByte(this.GetNextToken());
		}

		public byte[] ReadBytes(int length)
		{
			throw new NotImplementedException();
		}

		public Vec2 ReadVec2()
		{
			float num = this.ReadFloat();
			float num2 = this.ReadFloat();
			return new Vec2(num, num2);
		}

		public Vec3 ReadVec3()
		{
			float num = this.ReadFloat();
			float num2 = this.ReadFloat();
			float num3 = this.ReadFloat();
			float num4 = this.ReadFloat();
			return new Vec3(num, num2, num3, num4);
		}

		public Vec3i ReadVec3Int()
		{
			int num = this.ReadInt();
			int num2 = this.ReadInt();
			int num3 = this.ReadInt();
			return new Vec3i(num, num2, num3);
		}

		public sbyte ReadSByte()
		{
			return Convert.ToSByte(this.GetNextToken());
		}

		public ushort ReadUShort()
		{
			return Convert.ToUInt16(this.GetNextToken());
		}

		public double ReadDouble()
		{
			return Convert.ToDouble(this.GetNextToken());
		}

		private string[] _tokens;

		private int _currentIndex;
	}
}
