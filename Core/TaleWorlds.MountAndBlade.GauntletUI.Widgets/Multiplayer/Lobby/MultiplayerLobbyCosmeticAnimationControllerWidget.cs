using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyCosmeticAnimationControllerWidget : Widget
	{
		private double GetRandomDoubleBetween(double min, double max)
		{
			return base.Context.UIRandom.NextDouble() * (max - min) + max;
		}

		public MultiplayerLobbyCosmeticAnimationControllerWidget(UIContext context)
			: base(context)
		{
		}

		private void RestartAllAnimations()
		{
			this.SetAllAnimationPartColors();
			this.StopAllAnimations();
			this.StartAllAnimations();
		}

		private void SetAllAnimationPartColors()
		{
			this.ApplyActionOnAllAnimations(new Action<MultiplayerLobbyCosmeticAnimationPartWidget>(this.SetColorOfPart));
		}

		private void StartAllAnimations()
		{
			this.ApplyActionOnAllAnimations(new Action<MultiplayerLobbyCosmeticAnimationPartWidget>(this.StartAnimationOfPart));
		}

		private void StopAllAnimations()
		{
			this.ApplyActionOnAllAnimations(new Action<MultiplayerLobbyCosmeticAnimationPartWidget>(this.StopAnimationOfPart));
		}

		private void StartAnimationOfPart(MultiplayerLobbyCosmeticAnimationPartWidget part)
		{
			double randomDoubleBetween = this.GetRandomDoubleBetween((double)this.MinAlphaChangeDuration, (double)this.MaxAlphaChangeDuration);
			double randomDoubleBetween2 = this.GetRandomDoubleBetween((double)this.MinAlphaLowerBound, (double)this.MinAlphaUpperBound);
			double randomDoubleBetween3 = this.GetRandomDoubleBetween((double)this.MaxAlphaLowerBound, (double)this.MaxAlphaUpperBound);
			part.StartAnimation((float)randomDoubleBetween, (float)randomDoubleBetween2, (float)randomDoubleBetween3);
		}

		private void StopAnimationOfPart(MultiplayerLobbyCosmeticAnimationPartWidget part)
		{
			part.StopAnimation();
		}

		private void SetColorOfPart(MultiplayerLobbyCosmeticAnimationPartWidget part)
		{
			switch (this.CosmeticRarity)
			{
			case 0:
			case 1:
				part.Color = this.RarityCommonColor;
				return;
			case 2:
				part.Color = this.RarityRareColor;
				return;
			case 3:
				part.Color = this.RarityUniqueColor;
				return;
			default:
				part.Color = MultiplayerLobbyCosmeticAnimationControllerWidget.DefaultColor;
				return;
			}
		}

		private void ApplyActionOnAllAnimations(Action<MultiplayerLobbyCosmeticAnimationPartWidget> action)
		{
			BasicContainer animationPartContainer = this.AnimationPartContainer;
			if (animationPartContainer == null)
			{
				return;
			}
			animationPartContainer.Children.ForEach(delegate(Widget c)
			{
				action(c as MultiplayerLobbyCosmeticAnimationPartWidget);
			});
		}

		private void OnAnimationPartAdded(Widget parent, Widget child)
		{
			MultiplayerLobbyCosmeticAnimationPartWidget multiplayerLobbyCosmeticAnimationPartWidget = child as MultiplayerLobbyCosmeticAnimationPartWidget;
			this.SetColorOfPart(multiplayerLobbyCosmeticAnimationPartWidget);
			this.StartAnimationOfPart(multiplayerLobbyCosmeticAnimationPartWidget);
		}

		[Editor(false)]
		public int CosmeticRarity
		{
			get
			{
				return this._cosmeticRarity;
			}
			set
			{
				if (value != this._cosmeticRarity)
				{
					this._cosmeticRarity = value;
					base.OnPropertyChanged(value, "CosmeticRarity");
					this.RestartAllAnimations();
				}
			}
		}

		[Editor(false)]
		public float MinAlphaChangeDuration
		{
			get
			{
				return this._minAlphaChangeDuration;
			}
			set
			{
				if (this._minAlphaChangeDuration != value)
				{
					this._minAlphaChangeDuration = value;
					base.OnPropertyChanged(value, "MinAlphaChangeDuration");
					this.RestartAllAnimations();
				}
			}
		}

		[Editor(false)]
		public float MaxAlphaChangeDuration
		{
			get
			{
				return this._maxAlphaChangeDuration;
			}
			set
			{
				if (this._maxAlphaChangeDuration != value)
				{
					this._maxAlphaChangeDuration = value;
					base.OnPropertyChanged(value, "MaxAlphaChangeDuration");
					this.RestartAllAnimations();
				}
			}
		}

		[Editor(false)]
		public float MinAlphaLowerBound
		{
			get
			{
				return this._minAlphaLowerBound;
			}
			set
			{
				if (this._minAlphaLowerBound != value)
				{
					this._minAlphaLowerBound = value;
					base.OnPropertyChanged(value, "MinAlphaLowerBound");
					this.RestartAllAnimations();
				}
			}
		}

		[Editor(false)]
		public float MinAlphaUpperBound
		{
			get
			{
				return this._minAlphaUpperBound;
			}
			set
			{
				if (this._minAlphaUpperBound != value)
				{
					this._minAlphaUpperBound = value;
					base.OnPropertyChanged(value, "MinAlphaUpperBound");
					this.RestartAllAnimations();
				}
			}
		}

		[Editor(false)]
		public float MaxAlphaLowerBound
		{
			get
			{
				return this._maxAlphaLowerBound;
			}
			set
			{
				if (this._maxAlphaLowerBound != value)
				{
					this._maxAlphaLowerBound = value;
					base.OnPropertyChanged(value, "MaxAlphaLowerBound");
					this.RestartAllAnimations();
				}
			}
		}

		[Editor(false)]
		public float MaxAlphaUpperBound
		{
			get
			{
				return this._maxAlphaUpperBound;
			}
			set
			{
				if (this._maxAlphaUpperBound != value)
				{
					this._maxAlphaUpperBound = value;
					base.OnPropertyChanged(value, "MaxAlphaUpperBound");
					this.RestartAllAnimations();
				}
			}
		}

		[Editor(false)]
		public Color RarityCommonColor
		{
			get
			{
				return this._rarityCommonColor;
			}
			set
			{
				if (this._rarityCommonColor != value)
				{
					this._rarityCommonColor = value;
					base.OnPropertyChanged(value, "RarityCommonColor");
					this.RestartAllAnimations();
				}
			}
		}

		[Editor(false)]
		public Color RarityRareColor
		{
			get
			{
				return this._rarityRareColor;
			}
			set
			{
				if (this._rarityRareColor != value)
				{
					this._rarityRareColor = value;
					base.OnPropertyChanged(value, "RarityRareColor");
					this.RestartAllAnimations();
				}
			}
		}

		[Editor(false)]
		public Color RarityUniqueColor
		{
			get
			{
				return this._rarityUniqueColor;
			}
			set
			{
				if (this._rarityUniqueColor != value)
				{
					this._rarityUniqueColor = value;
					base.OnPropertyChanged(value, "RarityUniqueColor");
					this.RestartAllAnimations();
				}
			}
		}

		[Editor(false)]
		public BasicContainer AnimationPartContainer
		{
			get
			{
				return this._animationPartContainer;
			}
			set
			{
				if (value != this._animationPartContainer)
				{
					if (this._animationPartContainer != null)
					{
						this._animationPartContainer.ItemAddEventHandlers.Remove(new Action<Widget, Widget>(this.OnAnimationPartAdded));
					}
					this._animationPartContainer = value;
					if (this._animationPartContainer != null)
					{
						this._animationPartContainer.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnAnimationPartAdded));
					}
					base.OnPropertyChanged<BasicContainer>(value, "AnimationPartContainer");
					this.RestartAllAnimations();
				}
			}
		}

		private static readonly Color DefaultColor = Color.FromUint(0U);

		private int _cosmeticRarity;

		private float _minAlphaChangeDuration = 1.5f;

		private float _maxAlphaChangeDuration = 2.5f;

		private float _minAlphaLowerBound = 0.4f;

		private float _minAlphaUpperBound = 0.6f;

		private float _maxAlphaLowerBound = 0.6f;

		private float _maxAlphaUpperBound = 0.8f;

		private Color _rarityCommonColor;

		private Color _rarityRareColor;

		private Color _rarityUniqueColor;

		private BasicContainer _animationPartContainer;
	}
}
