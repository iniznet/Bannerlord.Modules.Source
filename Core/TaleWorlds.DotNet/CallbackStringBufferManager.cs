using System;

namespace TaleWorlds.DotNet
{
	public static class CallbackStringBufferManager
	{
		public static byte[] StringBuffer0
		{
			get
			{
				byte[] array;
				if ((array = CallbackStringBufferManager._stringBuffer0) == null)
				{
					array = (CallbackStringBufferManager._stringBuffer0 = new byte[1024]);
				}
				return array;
			}
		}

		public static byte[] StringBuffer1
		{
			get
			{
				byte[] array;
				if ((array = CallbackStringBufferManager._stringBuffer1) == null)
				{
					array = (CallbackStringBufferManager._stringBuffer1 = new byte[1024]);
				}
				return array;
			}
		}

		public static byte[] StringBuffer2
		{
			get
			{
				byte[] array;
				if ((array = CallbackStringBufferManager._stringBuffer2) == null)
				{
					array = (CallbackStringBufferManager._stringBuffer2 = new byte[1024]);
				}
				return array;
			}
		}

		public static byte[] StringBuffer3
		{
			get
			{
				byte[] array;
				if ((array = CallbackStringBufferManager._stringBuffer3) == null)
				{
					array = (CallbackStringBufferManager._stringBuffer3 = new byte[1024]);
				}
				return array;
			}
		}

		public static byte[] StringBuffer4
		{
			get
			{
				byte[] array;
				if ((array = CallbackStringBufferManager._stringBuffer4) == null)
				{
					array = (CallbackStringBufferManager._stringBuffer4 = new byte[1024]);
				}
				return array;
			}
		}

		public static byte[] StringBuffer5
		{
			get
			{
				byte[] array;
				if ((array = CallbackStringBufferManager._stringBuffer5) == null)
				{
					array = (CallbackStringBufferManager._stringBuffer5 = new byte[1024]);
				}
				return array;
			}
		}

		internal const int CallbackStringBufferMaxSize = 1024;

		[ThreadStatic]
		private static byte[] _stringBuffer0;

		[ThreadStatic]
		private static byte[] _stringBuffer1;

		[ThreadStatic]
		private static byte[] _stringBuffer2;

		[ThreadStatic]
		private static byte[] _stringBuffer3;

		[ThreadStatic]
		private static byte[] _stringBuffer4;

		[ThreadStatic]
		private static byte[] _stringBuffer5;
	}
}
