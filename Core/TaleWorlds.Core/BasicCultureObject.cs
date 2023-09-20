using System;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	// Token: 0x02000018 RID: 24
	public class BasicCultureObject : MBObjectBase
	{
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000144 RID: 324 RVA: 0x00005E36 File Offset: 0x00004036
		// (set) Token: 0x06000145 RID: 325 RVA: 0x00005E3E File Offset: 0x0000403E
		public TextObject Name { get; private set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000146 RID: 326 RVA: 0x00005E47 File Offset: 0x00004047
		// (set) Token: 0x06000147 RID: 327 RVA: 0x00005E4F File Offset: 0x0000404F
		public bool IsMainCulture { get; private set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000148 RID: 328 RVA: 0x00005E58 File Offset: 0x00004058
		// (set) Token: 0x06000149 RID: 329 RVA: 0x00005E60 File Offset: 0x00004060
		public bool IsBandit { get; private set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600014A RID: 330 RVA: 0x00005E69 File Offset: 0x00004069
		// (set) Token: 0x0600014B RID: 331 RVA: 0x00005E71 File Offset: 0x00004071
		public bool CanHaveSettlement { get; private set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600014C RID: 332 RVA: 0x00005E7A File Offset: 0x0000407A
		// (set) Token: 0x0600014D RID: 333 RVA: 0x00005E82 File Offset: 0x00004082
		public uint Color { get; private set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600014E RID: 334 RVA: 0x00005E8B File Offset: 0x0000408B
		// (set) Token: 0x0600014F RID: 335 RVA: 0x00005E93 File Offset: 0x00004093
		public uint Color2 { get; private set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000150 RID: 336 RVA: 0x00005E9C File Offset: 0x0000409C
		// (set) Token: 0x06000151 RID: 337 RVA: 0x00005EA4 File Offset: 0x000040A4
		public uint ClothAlternativeColor { get; private set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00005EAD File Offset: 0x000040AD
		// (set) Token: 0x06000153 RID: 339 RVA: 0x00005EB5 File Offset: 0x000040B5
		public uint ClothAlternativeColor2 { get; private set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000154 RID: 340 RVA: 0x00005EBE File Offset: 0x000040BE
		// (set) Token: 0x06000155 RID: 341 RVA: 0x00005EC6 File Offset: 0x000040C6
		public uint BackgroundColor1 { get; private set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000156 RID: 342 RVA: 0x00005ECF File Offset: 0x000040CF
		// (set) Token: 0x06000157 RID: 343 RVA: 0x00005ED7 File Offset: 0x000040D7
		public uint ForegroundColor1 { get; private set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000158 RID: 344 RVA: 0x00005EE0 File Offset: 0x000040E0
		// (set) Token: 0x06000159 RID: 345 RVA: 0x00005EE8 File Offset: 0x000040E8
		public uint BackgroundColor2 { get; private set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600015A RID: 346 RVA: 0x00005EF1 File Offset: 0x000040F1
		// (set) Token: 0x0600015B RID: 347 RVA: 0x00005EF9 File Offset: 0x000040F9
		public uint ForegroundColor2 { get; private set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600015C RID: 348 RVA: 0x00005F02 File Offset: 0x00004102
		// (set) Token: 0x0600015D RID: 349 RVA: 0x00005F0A File Offset: 0x0000410A
		public string EncounterBackgroundMesh { get; set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600015E RID: 350 RVA: 0x00005F13 File Offset: 0x00004113
		// (set) Token: 0x0600015F RID: 351 RVA: 0x00005F1B File Offset: 0x0000411B
		public string BannerKey { get; set; }

		// Token: 0x06000160 RID: 352 RVA: 0x00005F24 File Offset: 0x00004124
		public override string ToString()
		{
			return this.Name.ToString();
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00005F34 File Offset: 0x00004134
		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			this.Name = new TextObject(node.Attributes["name"].Value, null);
			this.Color = ((node.Attributes["color"] == null) ? uint.MaxValue : Convert.ToUInt32(node.Attributes["color"].Value, 16));
			this.Color2 = ((node.Attributes["color2"] == null) ? uint.MaxValue : Convert.ToUInt32(node.Attributes["color2"].Value, 16));
			this.ClothAlternativeColor = ((node.Attributes["cloth_alternative_color1"] == null) ? uint.MaxValue : Convert.ToUInt32(node.Attributes["cloth_alternative_color1"].Value, 16));
			this.ClothAlternativeColor2 = ((node.Attributes["cloth_alternative_color2"] == null) ? uint.MaxValue : Convert.ToUInt32(node.Attributes["cloth_alternative_color2"].Value, 16));
			this.BackgroundColor1 = ((node.Attributes["banner_background_color1"] == null) ? uint.MaxValue : Convert.ToUInt32(node.Attributes["banner_background_color1"].Value, 16));
			this.ForegroundColor1 = ((node.Attributes["banner_foreground_color1"] == null) ? uint.MaxValue : Convert.ToUInt32(node.Attributes["banner_foreground_color1"].Value, 16));
			this.BackgroundColor2 = ((node.Attributes["banner_background_color2"] == null) ? uint.MaxValue : Convert.ToUInt32(node.Attributes["banner_background_color2"].Value, 16));
			this.ForegroundColor2 = ((node.Attributes["banner_foreground_color2"] == null) ? uint.MaxValue : Convert.ToUInt32(node.Attributes["banner_foreground_color2"].Value, 16));
			this.IsMainCulture = node.Attributes["is_main_culture"] != null && Convert.ToBoolean(node.Attributes["is_main_culture"].Value);
			this.EncounterBackgroundMesh = ((node.Attributes["encounter_background_mesh"] == null) ? null : node.Attributes["encounter_background_mesh"].Value);
			this.BannerKey = ((node.Attributes["faction_banner_key"] == null) ? null : node.Attributes["faction_banner_key"].Value);
			this.IsBandit = false;
			this.IsBandit = node.Attributes["is_bandit"] != null && Convert.ToBoolean(node.Attributes["is_bandit"].Value);
			this.CanHaveSettlement = false;
			this.CanHaveSettlement = node.Attributes["can_have_settlement"] != null && Convert.ToBoolean(node.Attributes["can_have_settlement"].Value);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00006230 File Offset: 0x00004430
		public CultureCode GetCultureCode()
		{
			CultureCode cultureCode;
			if (Enum.TryParse<CultureCode>(base.StringId, true, out cultureCode))
			{
				return cultureCode;
			}
			Debug.FailedAssert("Could not get CultureCode from stringId: " + base.StringId, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\BasicCultureObject.cs", "GetCultureCode", 83);
			return CultureCode.Invalid;
		}
	}
}
