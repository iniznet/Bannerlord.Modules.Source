using System;
using System.Text;

namespace TaleWorlds.Library
{
	// Token: 0x02000087 RID: 135
	public class StringWriter : IWriter
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000495 RID: 1173 RVA: 0x0000EEBB File Offset: 0x0000D0BB
		public string Data
		{
			get
			{
				return this._stringBuilder.ToString();
			}
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x0000EEC8 File Offset: 0x0000D0C8
		public StringWriter()
		{
			this._stringBuilder = new StringBuilder();
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x0000EEDB File Offset: 0x0000D0DB
		private void AddToken(string token)
		{
			this._stringBuilder.Append(token);
			this._stringBuilder.Append(" ");
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x0000EEFB File Offset: 0x0000D0FB
		public void WriteSerializableObject(ISerializableObject serializableObject)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x0000EF02 File Offset: 0x0000D102
		public void WriteByte(byte value)
		{
			this.AddToken(Convert.ToString(value));
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x0000EF10 File Offset: 0x0000D110
		public void WriteBytes(byte[] bytes)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x0000EF17 File Offset: 0x0000D117
		public void WriteInt(int value)
		{
			this.AddToken(Convert.ToString(value));
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x0000EF25 File Offset: 0x0000D125
		public void WriteShort(short value)
		{
			this.AddToken(Convert.ToString(value));
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x0000EF33 File Offset: 0x0000D133
		public void WriteString(string value)
		{
			this.WriteInt(value.Length);
			this.AddToken(value);
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x0000EF48 File Offset: 0x0000D148
		public void WriteColor(Color value)
		{
			this.WriteFloat(value.Red);
			this.WriteFloat(value.Green);
			this.WriteFloat(value.Blue);
			this.WriteFloat(value.Alpha);
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x0000EF7A File Offset: 0x0000D17A
		public void WriteBool(bool value)
		{
			this.AddToken(value ? "1" : "0");
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x0000EF91 File Offset: 0x0000D191
		public void WriteFloat(float value)
		{
			this.AddToken((value == 0f) ? "0" : Convert.ToString(value));
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x0000EFAE File Offset: 0x0000D1AE
		public void WriteUInt(uint value)
		{
			this.AddToken(Convert.ToString(value));
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x0000EFBC File Offset: 0x0000D1BC
		public void WriteULong(ulong value)
		{
			this.AddToken(Convert.ToString(value));
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x0000EFCA File Offset: 0x0000D1CA
		public void WriteLong(long value)
		{
			this.AddToken(Convert.ToString(value));
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x0000EFD8 File Offset: 0x0000D1D8
		public void WriteVec2(Vec2 vec2)
		{
			this.WriteFloat(vec2.x);
			this.WriteFloat(vec2.y);
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x0000EFF2 File Offset: 0x0000D1F2
		public void WriteVec3(Vec3 vec3)
		{
			this.WriteFloat(vec3.x);
			this.WriteFloat(vec3.y);
			this.WriteFloat(vec3.z);
			this.WriteFloat(vec3.w);
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x0000F024 File Offset: 0x0000D224
		public void WriteVec3Int(Vec3i vec3)
		{
			this.WriteInt(vec3.X);
			this.WriteInt(vec3.Y);
			this.WriteInt(vec3.Z);
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x0000F04A File Offset: 0x0000D24A
		public void WriteSByte(sbyte value)
		{
			this.AddToken(Convert.ToString(value));
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x0000F058 File Offset: 0x0000D258
		public void WriteUShort(ushort value)
		{
			this.AddToken(Convert.ToString(value));
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x0000F066 File Offset: 0x0000D266
		public void WriteDouble(double value)
		{
			this.AddToken((value == 0.0) ? "0" : Convert.ToString(value));
		}

		// Token: 0x04000163 RID: 355
		private StringBuilder _stringBuilder;
	}
}
