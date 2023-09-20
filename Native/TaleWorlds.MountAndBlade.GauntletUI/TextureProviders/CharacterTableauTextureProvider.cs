using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.TextureProviders
{
	public class CharacterTableauTextureProvider : TextureProvider
	{
		public float CustomAnimationProgressRatio
		{
			get
			{
				return this._characterTableau.GetCustomAnimationProgressRatio();
			}
		}

		public string BannerCodeText
		{
			set
			{
				this._characterTableau.SetBannerCode(value);
			}
		}

		public string BodyProperties
		{
			set
			{
				this._characterTableau.SetBodyProperties(value);
			}
		}

		public int StanceIndex
		{
			set
			{
				this._characterTableau.SetStanceIndex(value);
			}
		}

		public bool IsFemale
		{
			set
			{
				this._characterTableau.SetIsFemale(value);
			}
		}

		public int Race
		{
			set
			{
				this._characterTableau.SetRace(value);
			}
		}

		public bool IsBannerShownInBackground
		{
			set
			{
				this._characterTableau.SetIsBannerShownInBackground(value);
			}
		}

		public bool IsEquipmentAnimActive
		{
			set
			{
				this._characterTableau.SetIsEquipmentAnimActive(value);
			}
		}

		public string EquipmentCode
		{
			set
			{
				this._characterTableau.SetEquipmentCode(value);
			}
		}

		public string IdleAction
		{
			set
			{
				this._characterTableau.SetIdleAction(value);
			}
		}

		public string IdleFaceAnim
		{
			set
			{
				this._characterTableau.SetIdleFaceAnim(value);
			}
		}

		public bool CurrentlyRotating
		{
			set
			{
				this._characterTableau.RotateCharacter(value);
			}
		}

		public string MountCreationKey
		{
			set
			{
				this._characterTableau.SetMountCreationKey(value);
			}
		}

		public uint ArmorColor1
		{
			set
			{
				this._characterTableau.SetArmorColor1(value);
			}
		}

		public uint ArmorColor2
		{
			set
			{
				this._characterTableau.SetArmorColor2(value);
			}
		}

		public string CharStringId
		{
			set
			{
				this._characterTableau.SetCharStringID(value);
			}
		}

		public bool TriggerCharacterMountPlacesSwap
		{
			set
			{
				this._characterTableau.TriggerCharacterMountPlacesSwap();
			}
		}

		public float CustomRenderScale
		{
			set
			{
				this._characterTableau.SetCustomRenderScale(value);
			}
		}

		public bool IsPlayingCustomAnimations
		{
			get
			{
				CharacterTableau characterTableau = this._characterTableau;
				return characterTableau != null && characterTableau.IsRunningCustomAnimation;
			}
			set
			{
				if (value)
				{
					this._characterTableau.StartCustomAnimation();
					return;
				}
				this._characterTableau.StopCustomAnimation();
			}
		}

		public bool ShouldLoopCustomAnimation
		{
			get
			{
				return this._characterTableau.ShouldLoopCustomAnimation;
			}
			set
			{
				this._characterTableau.ShouldLoopCustomAnimation = value;
			}
		}

		public int LeftHandWieldedEquipmentIndex
		{
			set
			{
				this._characterTableau.SetLeftHandWieldedEquipmentIndex(value);
			}
		}

		public int RightHandWieldedEquipmentIndex
		{
			set
			{
				this._characterTableau.SetRightHandWieldedEquipmentIndex(value);
			}
		}

		public float CustomAnimationWaitDuration
		{
			set
			{
				this._characterTableau.CustomAnimationWaitDuration = value;
			}
		}

		public string CustomAnimation
		{
			set
			{
				this._characterTableau.SetCustomAnimation(value);
			}
		}

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

		public CharacterTableauTextureProvider()
		{
			this._characterTableau = new CharacterTableau();
		}

		public override void Clear(bool clearNextFrame)
		{
			this._characterTableau.OnFinalize();
			base.Clear(clearNextFrame);
		}

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

		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		public override void SetTargetSize(int width, int height)
		{
			base.SetTargetSize(width, height);
			this._characterTableau.SetTargetSize(width, height);
		}

		public override void Tick(float dt)
		{
			base.Tick(dt);
			this.CheckTexture();
			this._characterTableau.OnTick(dt);
		}

		private CharacterTableau _characterTableau;

		private Texture _texture;

		private Texture _providedTexture;

		private bool _isHidden;
	}
}
