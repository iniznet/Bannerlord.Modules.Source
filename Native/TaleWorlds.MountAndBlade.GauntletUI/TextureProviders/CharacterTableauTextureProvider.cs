using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.TextureProviders
{
	// Token: 0x02000016 RID: 22
	public class CharacterTableauTextureProvider : TextureProvider
	{
		// Token: 0x17000013 RID: 19
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x000055D0 File Offset: 0x000037D0
		public string BannerCodeText
		{
			set
			{
				this._characterTableau.SetBannerCode(value);
			}
		}

		// Token: 0x17000014 RID: 20
		// (set) Token: 0x060000A8 RID: 168 RVA: 0x000055DE File Offset: 0x000037DE
		public string BodyProperties
		{
			set
			{
				this._characterTableau.SetBodyProperties(value);
			}
		}

		// Token: 0x17000015 RID: 21
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x000055EC File Offset: 0x000037EC
		public int StanceIndex
		{
			set
			{
				this._characterTableau.SetStanceIndex(value);
			}
		}

		// Token: 0x17000016 RID: 22
		// (set) Token: 0x060000AA RID: 170 RVA: 0x000055FA File Offset: 0x000037FA
		public bool IsFemale
		{
			set
			{
				this._characterTableau.SetIsFemale(value);
			}
		}

		// Token: 0x17000017 RID: 23
		// (set) Token: 0x060000AB RID: 171 RVA: 0x00005608 File Offset: 0x00003808
		public int Race
		{
			set
			{
				this._characterTableau.SetRace(value);
			}
		}

		// Token: 0x17000018 RID: 24
		// (set) Token: 0x060000AC RID: 172 RVA: 0x00005616 File Offset: 0x00003816
		public bool IsBannerShownInBackground
		{
			set
			{
				this._characterTableau.SetIsBannerShownInBackground(value);
			}
		}

		// Token: 0x17000019 RID: 25
		// (set) Token: 0x060000AD RID: 173 RVA: 0x00005624 File Offset: 0x00003824
		public bool IsEquipmentAnimActive
		{
			set
			{
				this._characterTableau.SetIsEquipmentAnimActive(value);
			}
		}

		// Token: 0x1700001A RID: 26
		// (set) Token: 0x060000AE RID: 174 RVA: 0x00005632 File Offset: 0x00003832
		public string EquipmentCode
		{
			set
			{
				this._characterTableau.SetEquipmentCode(value);
			}
		}

		// Token: 0x1700001B RID: 27
		// (set) Token: 0x060000AF RID: 175 RVA: 0x00005640 File Offset: 0x00003840
		public string IdleAction
		{
			set
			{
				this._characterTableau.SetIdleAction(value);
			}
		}

		// Token: 0x1700001C RID: 28
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x0000564E File Offset: 0x0000384E
		public string IdleFaceAnim
		{
			set
			{
				this._characterTableau.SetIdleFaceAnim(value);
			}
		}

		// Token: 0x1700001D RID: 29
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x0000565C File Offset: 0x0000385C
		public bool CurrentlyRotating
		{
			set
			{
				this._characterTableau.RotateCharacter(value);
			}
		}

		// Token: 0x1700001E RID: 30
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x0000566A File Offset: 0x0000386A
		public string MountCreationKey
		{
			set
			{
				this._characterTableau.SetMountCreationKey(value);
			}
		}

		// Token: 0x1700001F RID: 31
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x00005678 File Offset: 0x00003878
		public uint ArmorColor1
		{
			set
			{
				this._characterTableau.SetArmorColor1(value);
			}
		}

		// Token: 0x17000020 RID: 32
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x00005686 File Offset: 0x00003886
		public uint ArmorColor2
		{
			set
			{
				this._characterTableau.SetArmorColor2(value);
			}
		}

		// Token: 0x17000021 RID: 33
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x00005694 File Offset: 0x00003894
		public string CharStringId
		{
			set
			{
				this._characterTableau.SetCharStringID(value);
			}
		}

		// Token: 0x17000022 RID: 34
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x000056A2 File Offset: 0x000038A2
		public bool TriggerCharacterMountPlacesSwap
		{
			set
			{
				this._characterTableau.TriggerCharacterMountPlacesSwap();
			}
		}

		// Token: 0x17000023 RID: 35
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x000056AF File Offset: 0x000038AF
		public float CustomRenderScale
		{
			set
			{
				this._characterTableau.SetCustomRenderScale(value);
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x000056CF File Offset: 0x000038CF
		// (set) Token: 0x060000B8 RID: 184 RVA: 0x000056BD File Offset: 0x000038BD
		public bool IsHidden
		{
			get
			{
				return this._isHidden;
			}
			set
			{
				if (this._isHidden != value)
				{
					this._isHidden = value;
				}
			}
		}

		// Token: 0x060000BA RID: 186 RVA: 0x000056D7 File Offset: 0x000038D7
		public CharacterTableauTextureProvider()
		{
			this._characterTableau = new CharacterTableau();
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000056EA File Offset: 0x000038EA
		public override void Clear()
		{
			this._characterTableau.OnFinalize();
			base.Clear();
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00005700 File Offset: 0x00003900
		private void CheckTexture()
		{
			if (this._texture != this._characterTableau.Texture)
			{
				this._texture = this._characterTableau.Texture;
				if (this._texture != null)
				{
					EngineTexture engineTexture = new EngineTexture(this._texture);
					this._providedTexture = new Texture(engineTexture);
					return;
				}
				this._providedTexture = null;
			}
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00005764 File Offset: 0x00003964
		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00005772 File Offset: 0x00003972
		public override void SetTargetSize(int width, int height)
		{
			base.SetTargetSize(width, height);
			this._characterTableau.SetTargetSize(width, height);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00005789 File Offset: 0x00003989
		public override void Tick(float dt)
		{
			base.Tick(dt);
			this.CheckTexture();
			this._characterTableau.OnTick(dt);
		}

		// Token: 0x0400006B RID: 107
		private CharacterTableau _characterTableau;

		// Token: 0x0400006C RID: 108
		private Texture _texture;

		// Token: 0x0400006D RID: 109
		private Texture _providedTexture;

		// Token: 0x0400006E RID: 110
		private bool _isHidden;
	}
}
