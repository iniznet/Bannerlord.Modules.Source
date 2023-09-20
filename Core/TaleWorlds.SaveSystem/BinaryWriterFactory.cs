using System;
using System.Collections.Generic;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	internal static class BinaryWriterFactory
	{
		public static BinaryWriter GetBinaryWriter()
		{
			if (BinaryWriterFactory._binaryWriters.Value == null)
			{
				BinaryWriterFactory._binaryWriters.Value = new Stack<BinaryWriter>();
				for (int i = 0; i < 5; i++)
				{
					BinaryWriter binaryWriter = new BinaryWriter(4096);
					BinaryWriterFactory._binaryWriters.Value.Push(binaryWriter);
				}
			}
			Stack<BinaryWriter> value = BinaryWriterFactory._binaryWriters.Value;
			if (value.Count != 0)
			{
				return value.Pop();
			}
			return new BinaryWriter(4096);
		}

		public static void ReleaseBinaryWriter(BinaryWriter writer)
		{
			if (BinaryWriterFactory._binaryWriters != null)
			{
				writer.Clear();
				BinaryWriterFactory._binaryWriters.Value.Push(writer);
			}
		}

		public static void Initialize()
		{
			BinaryWriterFactory._binaryWriters = new ThreadLocal<Stack<BinaryWriter>>();
		}

		public static void Release()
		{
			BinaryWriterFactory._binaryWriters = null;
		}

		private static ThreadLocal<Stack<BinaryWriter>> _binaryWriters;
	}
}
