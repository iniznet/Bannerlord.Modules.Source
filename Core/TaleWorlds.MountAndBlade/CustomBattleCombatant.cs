using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class CustomBattleCombatant : IBattleCombatant
	{
		public TextObject Name { get; private set; }

		public BattleSideEnum Side { get; set; }

		public BasicCharacterObject General
		{
			get
			{
				return this._general;
			}
		}

		public BasicCultureObject BasicCulture { get; private set; }

		public Tuple<uint, uint> PrimaryColorPair
		{
			get
			{
				return new Tuple<uint, uint>(this.Banner.GetPrimaryColor(), this.Banner.GetFirstIconColor());
			}
		}

		public Tuple<uint, uint> AlternativeColorPair
		{
			get
			{
				return new Tuple<uint, uint>(this.Banner.GetFirstIconColor(), this.Banner.GetPrimaryColor());
			}
		}

		public Banner Banner { get; private set; }

		public int GetTacticsSkillAmount()
		{
			if (this._characters.Count > 0)
			{
				return this._characters.Max((BasicCharacterObject h) => h.GetSkillValue(DefaultSkills.Tactics));
			}
			return 0;
		}

		public IEnumerable<BasicCharacterObject> Characters
		{
			get
			{
				return this._characters.AsReadOnly();
			}
		}

		public int NumberOfAllMembers { get; private set; }

		public int NumberOfHealthyMembers
		{
			get
			{
				return this._characters.Count;
			}
		}

		public CustomBattleCombatant(TextObject name, BasicCultureObject culture, Banner banner)
		{
			this.Name = name;
			this.BasicCulture = culture;
			this.Banner = banner;
			this._characters = new List<BasicCharacterObject>();
			this._general = null;
		}

		public void AddCharacter(BasicCharacterObject characterObject, int number)
		{
			for (int i = 0; i < number; i++)
			{
				this._characters.Add(characterObject);
			}
			this.NumberOfAllMembers += number;
		}

		public void SetGeneral(BasicCharacterObject generalCharacter)
		{
			this._general = generalCharacter;
		}

		public void KillCharacter(BasicCharacterObject character)
		{
			this._characters.Remove(character);
		}

		private List<BasicCharacterObject> _characters;

		private BasicCharacterObject _general;
	}
}
