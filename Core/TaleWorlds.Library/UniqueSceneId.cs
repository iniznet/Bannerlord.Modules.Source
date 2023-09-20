using System;
using System.Text.RegularExpressions;

namespace TaleWorlds.Library
{
	public class UniqueSceneId
	{
		public string UniqueToken { get; }

		public string Revision { get; }

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

		private static readonly Lazy<Regex> IdentifierPattern = new Lazy<Regex>(() => new Regex("^:ut\\[\\d+\\](.*):rev\\[\\d+\\](.*)$", RegexOptions.Compiled));
	}
}
