using System;

namespace TaleWorlds.Diamond.ChatSystem.Library
{
	// Token: 0x02000002 RID: 2
	public class Base64Helper
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public static string Base64UrlEncode(byte[] data)
		{
			return Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_')
				.Trim(new char[] { '=' });
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002072 File Offset: 0x00000272
		public static byte[] Base64UrlDecode(string input)
		{
			return Convert.FromBase64String(input.Replace('-', '+').Replace('_', '/').PadRight(4 * ((input.Length + 3) / 4), '='));
		}
	}
}
