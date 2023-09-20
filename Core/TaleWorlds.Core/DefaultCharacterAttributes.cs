using System;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	public class DefaultCharacterAttributes
	{
		private static DefaultCharacterAttributes Instance
		{
			get
			{
				return Game.Current.DefaultCharacterAttributes;
			}
		}

		public static CharacterAttribute Vigor
		{
			get
			{
				return DefaultCharacterAttributes.Instance._vigor;
			}
		}

		public static CharacterAttribute Control
		{
			get
			{
				return DefaultCharacterAttributes.Instance._control;
			}
		}

		public static CharacterAttribute Endurance
		{
			get
			{
				return DefaultCharacterAttributes.Instance._endurance;
			}
		}

		public static CharacterAttribute Cunning
		{
			get
			{
				return DefaultCharacterAttributes.Instance._cunning;
			}
		}

		public static CharacterAttribute Social
		{
			get
			{
				return DefaultCharacterAttributes.Instance._social;
			}
		}

		public static CharacterAttribute Intelligence
		{
			get
			{
				return DefaultCharacterAttributes.Instance._intelligence;
			}
		}

		private CharacterAttribute Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<CharacterAttribute>(new CharacterAttribute(stringId));
		}

		internal DefaultCharacterAttributes()
		{
			this.RegisterAll();
		}

		private void RegisterAll()
		{
			this._vigor = this.Create("vigor");
			this._control = this.Create("control");
			this._endurance = this.Create("endurance");
			this._cunning = this.Create("cunning");
			this._social = this.Create("social");
			this._intelligence = this.Create("intelligence");
			this.InitializeAll();
		}

		private void InitializeAll()
		{
			this._vigor.Initialize(new TextObject("{=YWkdD7Ki}Vigor", null), new TextObject("{=jJ9sLOLb}Vigor represents the ability to move with speed and force. It's important for melee combat.", null), new TextObject("{=Ve8xoa3i}VIG", null));
			this._control.Initialize(new TextObject("{=controlskill}Control", null), new TextObject("{=vx0OCvaj}Control represents the ability to use strength without sacrificing precision. It's necessary for using ranged weapons.", null), new TextObject("{=HuXafdmR}CTR", null));
			this._endurance.Initialize(new TextObject("{=kvOavzcs}Endurance", null), new TextObject("{=K8rCOQUZ}Endurance is the ability to perform taxing physical activity for a long time.", null), new TextObject("{=d2ApwXJr}END", null));
			this._cunning.Initialize(new TextObject("{=JZM1mQvb}Cunning", null), new TextObject("{=YO5LUfiO}Cunning is the ability to predict what other people will do, and to outwit their plans.", null), new TextObject("{=tH6Ooj0P}CNG", null));
			this._social.Initialize(new TextObject("{=socialskill}Social", null), new TextObject("{=XMDTt96y}Social is the ability to understand people's motivations and to sway them.", null), new TextObject("{=PHoxdReD}SOC", null));
			this._intelligence.Initialize(new TextObject("{=sOrJoxiC}Intelligence", null), new TextObject("{=TeUtEGV0}Intelligence represents aptitude for reading and theoretical learning.", null), new TextObject("{=Bn7IsMpu}INT", null));
		}

		private CharacterAttribute _control;

		private CharacterAttribute _vigor;

		private CharacterAttribute _endurance;

		private CharacterAttribute _cunning;

		private CharacterAttribute _social;

		private CharacterAttribute _intelligence;
	}
}
