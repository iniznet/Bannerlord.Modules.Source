using System;
using System.Xml;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	// Token: 0x02000095 RID: 149
	public class MBCharacterSkills : MBObjectBase
	{
		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x060007E3 RID: 2019 RVA: 0x0001B2D8 File Offset: 0x000194D8
		// (set) Token: 0x060007E4 RID: 2020 RVA: 0x0001B2E0 File Offset: 0x000194E0
		public CharacterSkills Skills { get; private set; }

		// Token: 0x060007E5 RID: 2021 RVA: 0x0001B2E9 File Offset: 0x000194E9
		public MBCharacterSkills()
		{
			this.Skills = new CharacterSkills();
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x0001B2FC File Offset: 0x000194FC
		public void Init(MBObjectManager objectManager, XmlNode node)
		{
			base.Initialize();
			this.Skills.Deserialize(objectManager, node);
			base.AfterInitialized();
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x0001B317 File Offset: 0x00019517
		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			this.Skills.Deserialize(objectManager, node);
		}
	}
}
