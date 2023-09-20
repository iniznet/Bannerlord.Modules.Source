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

		internal const int CallbackStringBufferMaxSize = 1024;

		[ThreadStatic]
		private static byte[] _stringBuffer0;

		[ThreadStatic]
		private static byte[] _stringBuffer1;

		[ThreadStatic]
		private static byte[] _stringBuffer2;
	}
}
