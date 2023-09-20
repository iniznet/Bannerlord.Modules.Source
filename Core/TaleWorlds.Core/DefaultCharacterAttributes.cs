using System;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	// Token: 0x0200004B RID: 75
	public class DefaultCharacterAttributes
	{
		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06000580 RID: 1408 RVA: 0x000142D0 File Offset: 0x000124D0
		private static DefaultCharacterAttributes Instance
		{
			get
			{
				return Game.Current.DefaultCharacterAttributes;
			}
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06000581 RID: 1409 RVA: 0x000142DC File Offset: 0x000124DC
		public static CharacterAttribute Vigor
		{
			get
			{
				return DefaultCharacterAttributes.Instance._vigor;
			}
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x06000582 RID: 1410 RVA: 0x000142E8 File Offset: 0x000124E8
		public static CharacterAttribute Control
		{
			get
			{
				return DefaultCharacterAttributes.Instance._control;
			}
		}

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06000583 RID: 1411 RVA: 0x000142F4 File Offset: 0x000124F4
		public static CharacterAttribute Endurance
		{
			get
			{
				return DefaultCharacterAttributes.Instance._endurance;
			}
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000584 RID: 1412 RVA: 0x00014300 File Offset: 0x00012500
		public static CharacterAttribute Cunning
		{
			get
			{
				return DefaultCharacterAttributes.Instance._cunning;
			}
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000585 RID: 1413 RVA: 0x0001430C File Offset: 0x0001250C
		public static CharacterAttribute Social
		{
			get
			{
				return DefaultCharacterAttributes.Instance._social;
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000586 RID: 1414 RVA: 0x00014318 File Offset: 0x00012518
		public static CharacterAttribute Intelligence
		{
			get
			{
				return DefaultCharacterAttributes.Instance._intelligence;
			}
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x00014324 File Offset: 0x00012524
		private CharacterAttribute Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<CharacterAttribute>(new CharacterAttribute(stringId));
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x0001433B File Offset: 0x0001253B
		internal DefaultCharacterAttributes()
		{
			this.RegisterAll();
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x0001434C File Offset: 0x0001254C
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

		// Token: 0x0600058A RID: 1418 RVA: 0x000143C8 File Offset: 0x000125C8
		private void InitializeAll()
		{
			this._vigor.Initialize(new TextObject("{=YWkdD7Ki}Vigor", null), new TextObject("{=jJ9sLOLb}Vigor represents the ability to move with speed and force. It's important for melee combat.", null), new TextObject("{=Ve8xoa3i}VIG", null));
			this._control.Initialize(new TextObject("{=controlskill}Control", null), new TextObject("{=vx0OCvaj}Control represents the ability to use strength without sacrificing precision. It's necessary for using ranged weapons.", null), new TextObject("{=HuXafdmR}CTR", null));
			this._endurance.Initialize(new TextObject("{=kvOavzcs}Endurance", null), new TextObject("{=K8rCOQUZ}Endurance is the ability to perform taxing physical activity for a long time.", null), new TextObject("{=d2ApwXJr}END", null));
			this._cunning.Initialize(new TextObject("{=JZM1mQvb}Cunning", null), new TextObject("{=YO5LUfiO}Cunning is the ability to predict what other people will do, and to outwit their plans.", null), new TextObject("{=tH6Ooj0P}CNG", null));
			this._social.Initialize(new TextObject("{=socialskill}Social", null), new TextObject("{=XMDTt96y}Social is the ability to understand people's motivations and to sway them.", null), new TextObject("{=PHoxdReD}SOC", null));
			this._intelligence.Initialize(new TextObject("{=sOrJoxiC}Intelligence", null), new TextObject("{=TeUtEGV0}Intelligence represents aptitude for reading and theoretical learning.", null), new TextObject("{=Bn7IsMpu}INT", null));
		}

		// Token: 0x040002AD RID: 685
		private CharacterAttribute _control;

		// Token: 0x040002AE RID: 686
		private CharacterAttribute _vigor;

		// Token: 0x040002AF RID: 687
		private CharacterAttribute _endurance;

		// Token: 0x040002B0 RID: 688
		private CharacterAttribute _cunning;

		// Token: 0x040002B1 RID: 689
		private CharacterAttribute _social;

		// Token: 0x040002B2 RID: 690
		private CharacterAttribute _intelligence;
	}
}
