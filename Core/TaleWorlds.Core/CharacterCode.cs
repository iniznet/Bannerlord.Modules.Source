using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	// Token: 0x02000021 RID: 33
	public class CharacterCode
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000193 RID: 403 RVA: 0x00006A81 File Offset: 0x00004C81
		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this.Code);
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000194 RID: 404 RVA: 0x00006A8E File Offset: 0x00004C8E
		// (set) Token: 0x06000195 RID: 405 RVA: 0x00006A96 File Offset: 0x00004C96
		public string EquipmentCode { get; private set; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000196 RID: 406 RVA: 0x00006A9F File Offset: 0x00004C9F
		// (set) Token: 0x06000197 RID: 407 RVA: 0x00006AA7 File Offset: 0x00004CA7
		public string Code { get; private set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000198 RID: 408 RVA: 0x00006AB0 File Offset: 0x00004CB0
		// (set) Token: 0x06000199 RID: 409 RVA: 0x00006AB8 File Offset: 0x00004CB8
		public bool IsFemale { get; private set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600019A RID: 410 RVA: 0x00006AC1 File Offset: 0x00004CC1
		// (set) Token: 0x0600019B RID: 411 RVA: 0x00006AC9 File Offset: 0x00004CC9
		public bool IsHero { get; private set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600019C RID: 412 RVA: 0x00006AD2 File Offset: 0x00004CD2
		public float FaceDirtAmount
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600019D RID: 413 RVA: 0x00006AD9 File Offset: 0x00004CD9
		public Banner Banner
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600019E RID: 414 RVA: 0x00006ADC File Offset: 0x00004CDC
		// (set) Token: 0x0600019F RID: 415 RVA: 0x00006AE4 File Offset: 0x00004CE4
		public FormationClass FormationClass { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x00006AED File Offset: 0x00004CED
		// (set) Token: 0x060001A1 RID: 417 RVA: 0x00006AF5 File Offset: 0x00004CF5
		public uint Color1 { get; set; } = Color.White.ToUnsignedInteger();

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x00006AFE File Offset: 0x00004CFE
		// (set) Token: 0x060001A3 RID: 419 RVA: 0x00006B06 File Offset: 0x00004D06
		public uint Color2 { get; set; } = Color.White.ToUnsignedInteger();

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x00006B0F File Offset: 0x00004D0F
		// (set) Token: 0x060001A5 RID: 421 RVA: 0x00006B17 File Offset: 0x00004D17
		public int Race { get; private set; }

		// Token: 0x060001A6 RID: 422 RVA: 0x00006B20 File Offset: 0x00004D20
		public Equipment CalculateEquipment()
		{
			if (this.EquipmentCode == null)
			{
				return null;
			}
			return Equipment.CreateFromEquipmentCode(this.EquipmentCode);
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00006B38 File Offset: 0x00004D38
		private CharacterCode()
		{
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00006B74 File Offset: 0x00004D74
		public static CharacterCode CreateFrom(BasicCharacterObject character)
		{
			CharacterCode characterCode = new CharacterCode();
			Equipment equipment = character.Equipment;
			string text = ((equipment != null) ? equipment.CalculateEquipmentCode() : null);
			characterCode.EquipmentCode = text;
			characterCode.BodyProperties = character.GetBodyProperties(character.Equipment, -1);
			characterCode.IsFemale = character.IsFemale;
			characterCode.IsHero = character.IsHero;
			characterCode.FormationClass = character.DefaultFormationClass;
			characterCode.Race = character.Race;
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "CreateFrom");
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(text);
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(characterCode.BodyProperties.ToString());
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(characterCode.IsFemale ? "1" : "0");
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(characterCode.IsHero ? "1" : "0");
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(((int)characterCode.FormationClass).ToString());
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(characterCode.Color1.ToString());
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(characterCode.Color2.ToString());
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(characterCode.Race.ToString());
			mbstringBuilder.Append<string>("@---@");
			characterCode.Code = mbstringBuilder.ToStringAndRelease();
			return characterCode;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00006D34 File Offset: 0x00004F34
		public static CharacterCode CreateFrom(string equipmentCode, BodyProperties bodyProperties, bool isFemale, bool isHero, uint color1, uint color2, FormationClass formationClass, int race)
		{
			CharacterCode characterCode = new CharacterCode();
			characterCode.EquipmentCode = equipmentCode;
			characterCode.BodyProperties = bodyProperties;
			characterCode.IsFemale = isFemale;
			characterCode.IsHero = isHero;
			characterCode.Color1 = color1;
			characterCode.Color2 = color2;
			characterCode.FormationClass = formationClass;
			characterCode.Race = race;
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "CreateFrom");
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(equipmentCode);
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(characterCode.BodyProperties.ToString());
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(characterCode.IsFemale ? "1" : "0");
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(characterCode.IsHero ? "1" : "0");
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(((int)characterCode.FormationClass).ToString());
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(characterCode.Color1.ToString());
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(characterCode.Color2.ToString());
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(characterCode.Race.ToString());
			mbstringBuilder.Append<string>("@---@");
			characterCode.Code = mbstringBuilder.ToStringAndRelease();
			return characterCode;
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00006ED0 File Offset: 0x000050D0
		public string CreateNewCodeString()
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "CreateNewCodeString");
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(this.EquipmentCode);
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(this.BodyProperties.ToString());
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(this.IsFemale ? "1" : "0");
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(this.IsHero ? "1" : "0");
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(((int)this.FormationClass).ToString());
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(this.Color1.ToString());
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(this.Color2.ToString());
			mbstringBuilder.Append<string>("@---@");
			mbstringBuilder.Append<string>(this.Race.ToString());
			mbstringBuilder.Append<string>("@---@");
			return mbstringBuilder.ToStringAndRelease();
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00007026 File Offset: 0x00005226
		public static CharacterCode CreateEmpty()
		{
			return new CharacterCode();
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00007030 File Offset: 0x00005230
		public static CharacterCode CreateFrom(string code)
		{
			CharacterCode characterCode = new CharacterCode();
			int num = 0;
			int num2;
			for (num2 = code.IndexOf("@---@", StringComparison.InvariantCulture); num2 == num; num2 = code.IndexOf("@---@", num, StringComparison.InvariantCulture))
			{
				num = num2 + 5;
			}
			string text = code.Substring(num, num2 - num);
			do
			{
				num = num2 + 5;
				num2 = code.IndexOf("@---@", num, StringComparison.InvariantCulture);
			}
			while (num2 == num);
			string text2 = code.Substring(num, num2 - num);
			do
			{
				num = num2 + 5;
				num2 = code.IndexOf("@---@", num, StringComparison.InvariantCulture);
			}
			while (num2 == num);
			string text3 = code.Substring(num, num2 - num);
			do
			{
				num = num2 + 5;
				num2 = code.IndexOf("@---@", num, StringComparison.InvariantCulture);
			}
			while (num2 == num);
			string text4 = code.Substring(num, num2 - num);
			do
			{
				num = num2 + 5;
				num2 = code.IndexOf("@---@", num, StringComparison.InvariantCulture);
			}
			while (num2 == num);
			string text5 = code.Substring(num, num2 - num);
			do
			{
				num = num2 + 5;
				num2 = code.IndexOf("@---@", num, StringComparison.InvariantCulture);
			}
			while (num2 == num);
			string text6 = code.Substring(num, num2 - num);
			do
			{
				num = num2 + 5;
				num2 = code.IndexOf("@---@", num, StringComparison.InvariantCulture);
			}
			while (num2 == num);
			string text7 = code.Substring(num, num2 - num);
			num = num2 + 5;
			num2 = code.IndexOf("@---@", num, StringComparison.InvariantCulture);
			string text8 = ((num2 >= 0) ? code.Substring(num, num2 - num) : code.Substring(num));
			characterCode.EquipmentCode = text;
			BodyProperties bodyProperties;
			if (BodyProperties.FromString(text2, out bodyProperties))
			{
				characterCode.BodyProperties = bodyProperties;
			}
			else
			{
				Debug.FailedAssert("Cannot read the character code body property", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\CharacterCode.cs", "CreateFrom", 235);
			}
			characterCode.IsFemale = Convert.ToInt32(text3) == 1;
			characterCode.IsHero = Convert.ToInt32(text4) == 1;
			characterCode.FormationClass = (FormationClass)Convert.ToInt32(text5);
			characterCode.Color1 = Convert.ToUInt32(text6);
			characterCode.Color2 = Convert.ToUInt32(text7);
			characterCode.Race = Convert.ToInt32(text8);
			characterCode.Code = code;
			return characterCode;
		}

		// Token: 0x0400015A RID: 346
		public const string SpecialCodeSeparator = "@---@";

		// Token: 0x0400015B RID: 347
		public const int SpecialCodeSeparatorLength = 5;

		// Token: 0x0400015E RID: 350
		public BodyProperties BodyProperties;
	}
}
