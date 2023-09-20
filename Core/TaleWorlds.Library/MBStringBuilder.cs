using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace TaleWorlds.Library
{
	public struct MBStringBuilder
	{
		public void Initialize(int capacity = 16, [CallerMemberName] string callerMemberName = "")
		{
			this._cachedStringBuilder = MBStringBuilder.CachedStringBuilder.Acquire(capacity);
		}

		public string ToStringAndRelease()
		{
			string text = this._cachedStringBuilder.ToString();
			this.Release();
			return text;
		}

		public void Release()
		{
			MBStringBuilder.CachedStringBuilder.Release(this._cachedStringBuilder);
			this._cachedStringBuilder = null;
		}

		public MBStringBuilder Append(char value)
		{
			this._cachedStringBuilder.Append(value);
			return this;
		}

		public MBStringBuilder Append(int value)
		{
			this._cachedStringBuilder.Append(value);
			return this;
		}

		public MBStringBuilder Append(uint value)
		{
			this._cachedStringBuilder.Append(value);
			return this;
		}

		public MBStringBuilder Append(float value)
		{
			this._cachedStringBuilder.Append(value);
			return this;
		}

		public MBStringBuilder Append(double value)
		{
			this._cachedStringBuilder.Append(value);
			return this;
		}

		public MBStringBuilder Append<T>(T value)
		{
			this._cachedStringBuilder.Append(value);
			return this;
		}

		public MBStringBuilder AppendLine()
		{
			this._cachedStringBuilder.AppendLine();
			return this;
		}

		public MBStringBuilder AppendLine<T>(T value)
		{
			this.Append<T>(value);
			this.AppendLine();
			return this;
		}

		public int Length
		{
			get
			{
				return this._cachedStringBuilder.Length;
			}
		}

		public override string ToString()
		{
			Debug.FailedAssert("Don't use this. Use ToStringAndRelease instead!", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\CachedStringBuilder.cs", "ToString", 190);
			return null;
		}

		private StringBuilder _cachedStringBuilder;

		private static class CachedStringBuilder
		{
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

			public static void Release(StringBuilder sb)
			{
				if (sb.Capacity <= 4096)
				{
					MBStringBuilder.CachedStringBuilder._cachedStringBuilder = sb;
					MBStringBuilder.CachedStringBuilder._cachedStringBuilder.Clear();
				}
			}

			public static string GetStringAndReleaseBuilder(StringBuilder sb)
			{
				string text = sb.ToString();
				MBStringBuilder.CachedStringBuilder.Release(sb);
				return text;
			}

			private const int MaxBuilderSize = 4096;

			[ThreadStatic]
			private static StringBuilder _cachedStringBuilder;
		}
	}
}
