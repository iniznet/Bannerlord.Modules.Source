using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001F2 RID: 498
	public class CustomBattleCombatant : IBattleCombatant
	{
		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x06001BAC RID: 7084 RVA: 0x0006232C File Offset: 0x0006052C
		// (set) Token: 0x06001BAD RID: 7085 RVA: 0x00062334 File Offset: 0x00060534
		public TextObject Name { get; private set; }

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x06001BAE RID: 7086 RVA: 0x0006233D File Offset: 0x0006053D
		// (set) Token: 0x06001BAF RID: 7087 RVA: 0x00062345 File Offset: 0x00060545
		public BattleSideEnum Side { get; set; }

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x06001BB0 RID: 7088 RVA: 0x0006234E File Offset: 0x0006054E
		public BasicCharacterObject General
		{
			get
			{
				return this._general;
			}
		}

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x06001BB1 RID: 7089 RVA: 0x00062356 File Offset: 0x00060556
		// (set) Token: 0x06001BB2 RID: 7090 RVA: 0x0006235E File Offset: 0x0006055E
		public BasicCultureObject BasicCulture { get; private set; }

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x06001BB3 RID: 7091 RVA: 0x00062367 File Offset: 0x00060567
		public Tuple<uint, uint> PrimaryColorPair
		{
			get
			{
				return new Tuple<uint, uint>(this.Banner.GetPrimaryColor(), this.Banner.GetFirstIconColor());
			}
		}

		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x06001BB4 RID: 7092 RVA: 0x00062384 File Offset: 0x00060584
		public Tuple<uint, uint> AlternativeColorPair
		{
			get
			{
				return new Tuple<uint, uint>(this.Banner.GetFirstIconColor(), this.Banner.GetPrimaryColor());
			}
		}

		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x06001BB5 RID: 7093 RVA: 0x000623A1 File Offset: 0x000605A1
		// (set) Token: 0x06001BB6 RID: 7094 RVA: 0x000623A9 File Offset: 0x000605A9
		public Banner Banner { get; private set; }

		// Token: 0x06001BB7 RID: 7095 RVA: 0x000623B2 File Offset: 0x000605B2
		public int GetTacticsSkillAmount()
		{
			if (this._characters.Count > 0)
			{
				return this._characters.Max((BasicCharacterObject h) => h.GetSkillValue(DefaultSkills.Tactics));
			}
			return 0;
		}

		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x06001BB8 RID: 7096 RVA: 0x000623EE File Offset: 0x000605EE
		public IEnumerable<BasicCharacterObject> Characters
		{
			get
			{
				return this._characters.AsReadOnly();
			}
		}

		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x06001BB9 RID: 7097 RVA: 0x000623FB File Offset: 0x000605FB
		// (set) Token: 0x06001BBA RID: 7098 RVA: 0x00062403 File Offset: 0x00060603
		public int NumberOfAllMembers { get; private set; }

		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x06001BBB RID: 7099 RVA: 0x0006240C File Offset: 0x0006060C
		public int NumberOfHealthyMembers
		{
			get
			{
				return this._characters.Count;
			}
		}

		// Token: 0x06001BBC RID: 7100 RVA: 0x00062419 File Offset: 0x00060619
		public CustomBattleCombatant(TextObject name, BasicCultureObject culture, Banner banner)
		{
			this.Name = name;
			this.BasicCulture = culture;
			this.Banner = banner;
			this._characters = new List<BasicCharacterObject>();
			this._general = null;
		}

		// Token: 0x06001BBD RID: 7101 RVA: 0x00062448 File Offset: 0x00060648
		public void AddCharacter(BasicCharacterObject characterObject, int number)
		{
			for (int i = 0; i < number; i++)
			{
				this._characters.Add(characterObject);
			}
			this.NumberOfAllMembers += number;
		}

		// Token: 0x06001BBE RID: 7102 RVA: 0x0006247B File Offset: 0x0006067B
		public void SetGeneral(BasicCharacterObject generalCharacter)
		{
			this._general = generalCharacter;
		}

		// Token: 0x06001BBF RID: 7103 RVA: 0x00062484 File Offset: 0x00060684
		public void KillCharacter(BasicCharacterObject character)
		{
			this._characters.Remove(character);
		}

		// Token: 0x040008F0 RID: 2288
		private List<BasicCharacterObject> _characters;

		// Token: 0x040008F1 RID: 2289
		private BasicCharacterObject _general;
	}
}
