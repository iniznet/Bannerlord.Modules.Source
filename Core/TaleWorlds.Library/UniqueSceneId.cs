using System;
using System.Text.RegularExpressions;

namespace TaleWorlds.Library
{
	// Token: 0x02000096 RID: 150
	public class UniqueSceneId
	{
		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600050C RID: 1292 RVA: 0x0001009D File Offset: 0x0000E29D
		public string UniqueToken { get; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x0600050D RID: 1293 RVA: 0x000100A5 File Offset: 0x0000E2A5
		public string Revision { get; }

		// Token: 0x0600050E RID: 1294 RVA: 0x000100AD File Offset: 0x0000E2AD
		public UniqueSceneId(string uniqueToken, string revision)
		{
			if (uniqueToken == null)
			{
				throw new ArgumentNullException("uniqueToken");
			}
			this.UniqueToken = uniqueToken;
			if (revision == null)
			{
				throw new ArgumentNullException("revision");
			}
			this.Revision = revision;
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x000100E4 File Offset: 0x0000E2E4
		public string Serialize()
		{
			return string.Format(":ut[{0}]{1}:rev[{2}]{3}", new object[]
			{
				this.UniqueToken.Length,
				this.UniqueToken,
				this.Revision.Length,
				this.Revision
			});
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x0001013C File Offset: 0x0000E33C
		public static bool TryParse(string uniqueMapId, out UniqueSceneId identifiers)
		{
			identifiers = null;
			if (uniqueMapId == null)
			{
				return false;
			}
			Match match = UniqueSceneId.IdentifierPattern.Value.Match(uniqueMapId);
			if (match.Success)
			{
				identifiers = new UniqueSceneId(match.Groups[1].Value, match.Groups[2].Value);
				return true;
			}
			return false;
		}

		// Token: 0x04000181 RID: 385
		private static readonly Lazy<Regex> IdentifierPattern = new Lazy<Regex>(() => new Regex("^:ut\\[\\d+\\](.*):rev\\[\\d+\\](.*)$", RegexOptions.Compiled));
	}
}
