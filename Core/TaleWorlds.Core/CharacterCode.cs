using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	public class CharacterCode
	{
		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this.Code);
			}
		}

		public string EquipmentCode { get; private set; }

		public string Code { get; private set; }

		public bool IsFemale { get; private set; }

		public bool IsHero { get; private set; }

		public float FaceDirtAmount
		{
			get
			{
				return 0f;
			}
		}

		public Banner Banner
		{
			get
			{
				return null;
			}
		}

		public FormationClass FormationClass { get; set; }

		public uint Color1 { get; set; } = Color.White.ToUnsignedInteger();

		public uint Color2 { get; set; } = Color.White.ToUnsignedInteger();

		public int Race { get; private set; }

		public Equipment CalculateEquipment()
		{
			if (this.EquipmentCode == null)
			{
				return null;
			}
			return Equipment.CreateFromEquipmentCode(this.EquipmentCode);
		}

		private CharacterCode()
		{
		}

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

		public static CharacterCode CreateEmpty()
		{
			return new CharacterCode();
		}

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

		public const string SpecialCodeSeparator = "@---@";

		public const int SpecialCodeSeparatorLength = 5;

		public BodyProperties BodyProperties;
	}
}
