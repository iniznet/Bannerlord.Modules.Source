using System;
using System.Xml;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public class MBCharacterSkills : MBObjectBase
	{
		public CharacterSkills Skills { get; private set; }

		public MBCharacterSkills()
		{
			this.Skills = new CharacterSkills();
		}

		public void Init(MBObjectManager objectManager, XmlNode node)
		{
			base.Initialize();
			this.Skills.Deserialize(objectManager, node);
			base.AfterInitialized();
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			this.Skills.Deserialize(objectManager, node);
		}
	}
}
