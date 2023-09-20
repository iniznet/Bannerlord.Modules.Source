using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace TaleWorlds.Library
{
	// Token: 0x0200001C RID: 28
	public struct MBStringBuilder
	{
		// Token: 0x06000094 RID: 148 RVA: 0x00003B87 File Offset: 0x00001D87
		public void Initialize(int capacity = 16, [CallerMemberName] string callerMemberName = "")
		{
			this._cachedStringBuilder = MBStringBuilder.CachedStringBuilder.Acquire(capacity);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00003B95 File Offset: 0x00001D95
		public string ToStringAndRelease()
		{
			string text = this._cachedStringBuilder.ToString();
			this.Release();
			return text;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00003BA8 File Offset: 0x00001DA8
		public void Release()
		{
			MBStringBuilder.CachedStringBuilder.Release(this._cachedStringBuilder);
			this._cachedStringBuilder = null;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00003BBC File Offset: 0x00001DBC
		public MBStringBuilder Append(char value)
		{
			this._cachedStringBuilder.Append(value);
			return this;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00003BD1 File Offset: 0x00001DD1
		public MBStringBuilder Append(int value)
		{
			this._cachedStringBuilder.Append(value);
			return this;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00003BE6 File Offset: 0x00001DE6
		public MBStringBuilder Append(uint value)
		{
			this._cachedStringBuilder.Append(value);
			return this;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00003BFB File Offset: 0x00001DFB
		public MBStringBuilder Append(float value)
		{
			this._cachedStringBuilder.Append(value);
			return this;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00003C10 File Offset: 0x00001E10
		public MBStringBuilder Append(double value)
		{
			this._cachedStringBuilder.Append(value);
			return this;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00003C25 File Offset: 0x00001E25
		public MBStringBuilder Append<T>(T value)
		{
			this._cachedStringBuilder.Append(value);
			return this;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00003C3F File Offset: 0x00001E3F
		public MBStringBuilder AppendLine()
		{
			this._cachedStringBuilder.AppendLine();
			return this;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00003C53 File Offset: 0x00001E53
		public MBStringBuilder AppendLine<T>(T value)
		{
			this.Append<T>(value);
			this.AppendLine();
			return this;
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00003C6A File Offset: 0x00001E6A
		public int Length
		{
			get
			{
				return this._cachedStringBuilder.Length;
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00003C77 File Offset: 0x00001E77
		public override string ToString()
		{
			Debug.FailedAssert("Don't use this. Use ToStringAndRelease instead!", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\CachedStringBuilder.cs", "ToString", 190);
			return null;
		}

		// Token: 0x04000059 RID: 89
		private StringBuilder _cachedStringBuilder;

		// Token: 0x020000BE RID: 190
		private static class CachedStringBuilder
		{
			// Token: 0x060006B4 RID: 1716 RVA: 0x00014803 File Offset: 0x00012A03
			public static StringBuilder Acquire(int capacity = 16)
			{
				if (capacity <= 4096 && MBStringBuilder.CachedStringBuilder._cachedStringBuilder != null)
				{
					StringBuilder cachedStringBuilder = MBStringBuilder.CachedStringBuilder._cachedStringBuilder;
					MBStringBuilder.CachedStringBuilder._cachedStringBuilder = null;
					cachedStringBuilder.EnsureCapacity(capacity);
					return cachedStringBuilder;
				}
				return new StringBuilder(capacity);
			}

			// Token: 0x060006B5 RID: 1717 RVA: 0x0001482E File Offset: 0x00012A2E
			public static void Release(StringBuilder sb)
			{
				if (sb.Capacity <= 4096)
				{
					MBStringBuilder.CachedStringBuilder._cachedStringBuilder = sb;
					MBStringBuilder.CachedStringBuilder._cachedStringBuilder.Clear();
				}
			}

			// Token: 0x060006B6 RID: 1718 RVA: 0x0001484E File Offset: 0x00012A4E
			public static string GetStringAndReleaseBuilder(StringBuilder sb)
			{
				string text = sb.ToString();
				MBStringBuilder.CachedStringBuilder.Release(sb);
				return text;
			}

			// Token: 0x0400021D RID: 541
			private const int MaxBuilderSize = 4096;

			// Token: 0x0400021E RID: 542
			[ThreadStatic]
			private static StringBuilder _cachedStringBuilder;
		}
	}
}
