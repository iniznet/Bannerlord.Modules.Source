using System;
using System.Collections.Generic;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x0200000B RID: 11
	internal static class BinaryWriterFactory
	{
		// Token: 0x0600001B RID: 27 RVA: 0x00002284 File Offset: 0x00000484
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

		// Token: 0x0600001C RID: 28 RVA: 0x000022F7 File Offset: 0x000004F7
		public static void ReleaseBinaryWriter(BinaryWriter writer)
		{
			if (BinaryWriterFactory._binaryWriters != null)
			{
				writer.Clear();
				BinaryWriterFactory._binaryWriters.Value.Push(writer);
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002316 File Offset: 0x00000516
		public static void Initialize()
		{
			BinaryWriterFactory._binaryWriters = new ThreadLocal<Stack<BinaryWriter>>();
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002322 File Offset: 0x00000522
		public static void Release()
		{
			BinaryWriterFactory._binaryWriters = null;
		}

		// Token: 0x04000008 RID: 8
		private static ThreadLocal<Stack<BinaryWriter>> _binaryWriters;
	}
}
