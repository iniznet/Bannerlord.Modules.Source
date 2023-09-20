using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000013 RID: 19
	public class BrushRenderer
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600012A RID: 298 RVA: 0x00007457 File Offset: 0x00005657
		private float _brushTimer
		{
			get
			{
				if (!this.UseLocalTimer)
				{
					return this._globalTime;
				}
				return this._brushLocalTimer;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x0600012B RID: 299 RVA: 0x0000746E File Offset: 0x0000566E
		// (set) Token: 0x0600012C RID: 300 RVA: 0x00007476 File Offset: 0x00005676
		public bool ForcePixelPerfectPlacement { get; set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600012D RID: 301 RVA: 0x0000747F File Offset: 0x0000567F
		public Style CurrentStyle
		{
			get
			{
				return this._styleOfCurrentState;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x0600012E RID: 302 RVA: 0x00007487 File Offset: 0x00005687
		// (set) Token: 0x0600012F RID: 303 RVA: 0x00007490 File Offset: 0x00005690
		public Brush Brush
		{
			get
			{
				return this._brush;
			}
			set
			{
				if (this._brush != value)
				{
					this._brush = value;
					this._brushLocalTimer = 0f;
					int num = ((this._brush != null) ? this._brush.Layers.Count : 0);
					if (this._startBrushLayerState == null)
					{
						this._startBrushLayerState = new Dictionary<string, BrushLayerState>(num);
						this._currentBrushLayerState = new Dictionary<string, BrushLayerState>(num);
					}
					else
					{
						this._startBrushLayerState.Clear();
						this._currentBrushLayerState.Clear();
					}
					if (this._brush != null)
					{
						Style defaultStyle = this._brush.DefaultStyle;
						BrushState brushState = default(BrushState);
						brushState.FillFrom(defaultStyle);
						this._startBrushState = brushState;
						this._currentBrushState = brushState;
						foreach (BrushLayer brushLayer in this._brush.Layers)
						{
							BrushLayerState brushLayerState = default(BrushLayerState);
							brushLayerState.FillFrom(brushLayer);
							this._startBrushLayerState[brushLayer.Name] = brushLayerState;
							this._currentBrushLayerState[brushLayer.Name] = brushLayerState;
						}
						if (!string.IsNullOrEmpty(this.CurrentState))
						{
							this._styleOfCurrentState = this.Brush.GetStyleOrDefault(this.CurrentState);
						}
					}
				}
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000130 RID: 304 RVA: 0x000075E8 File Offset: 0x000057E8
		// (set) Token: 0x06000131 RID: 305 RVA: 0x000075F0 File Offset: 0x000057F0
		public string CurrentState
		{
			get
			{
				return this._currentState;
			}
			set
			{
				if (this._currentState != value)
				{
					string currentState = this._currentState;
					this._brushLocalTimer = 0f;
					this._currentState = value;
					this._startBrushState = this._currentBrushState;
					foreach (KeyValuePair<string, BrushLayerState> keyValuePair in this._currentBrushLayerState)
					{
						this._startBrushLayerState[keyValuePair.Key] = keyValuePair.Value;
					}
					if (this.Brush != null)
					{
						Style styleOrDefault = this.Brush.GetStyleOrDefault(this.CurrentState);
						this._styleOfCurrentState = styleOrDefault;
						this._brushRendererAnimationState = BrushRenderer.BrushRendererAnimationState.None;
						if (styleOrDefault.AnimationMode == StyleAnimationMode.BasicTransition)
						{
							if (!string.IsNullOrEmpty(currentState))
							{
								this._brushRendererAnimationState = BrushRenderer.BrushRendererAnimationState.PlayingBasicTranisition;
								return;
							}
						}
						else if (styleOrDefault.AnimationMode == StyleAnimationMode.Animation && (!string.IsNullOrEmpty(currentState) || !string.IsNullOrEmpty(styleOrDefault.AnimationToPlayOnBegin)))
						{
							this._brushRendererAnimationState = BrushRenderer.BrushRendererAnimationState.PlayingAnimation;
						}
					}
				}
			}
		}

		// Token: 0x06000132 RID: 306 RVA: 0x000076F4 File Offset: 0x000058F4
		public BrushRenderer()
		{
			this._startBrushState = default(BrushState);
			this._currentBrushState = default(BrushState);
			this._startBrushLayerState = new Dictionary<string, BrushLayerState>();
			this._currentBrushLayerState = new Dictionary<string, BrushLayerState>();
			this._brushLocalTimer = 0f;
			this._brushRendererAnimationState = BrushRenderer.BrushRendererAnimationState.None;
			this._randomXOffset = -1f;
			this._randomYOffset = -1f;
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00007760 File Offset: 0x00005960
		private float GetRandomXOffset()
		{
			if (this._randomXOffset < 0f)
			{
				Random random = new Random(this._offsetSeed);
				this._randomXOffset = (float)random.Next(0, 2048);
				this._randomYOffset = (float)random.Next(0, 2048);
			}
			return this._randomXOffset;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x000077B4 File Offset: 0x000059B4
		private float GetRandomYOffset()
		{
			if (this._randomYOffset < 0f)
			{
				Random random = new Random(this._offsetSeed);
				this._randomXOffset = (float)random.Next(0, 2048);
				this._randomYOffset = (float)random.Next(0, 2048);
			}
			return this._randomYOffset;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00007808 File Offset: 0x00005A08
		public void Update(float globalAnimTime, float dt)
		{
			this._globalTime = globalAnimTime;
			this._brushLocalTimer += dt;
			if (this.Brush != null)
			{
				Style styleOfCurrentState = this._styleOfCurrentState;
				if ((this._brushRendererAnimationState == BrushRenderer.BrushRendererAnimationState.None || this._brushRendererAnimationState == BrushRenderer.BrushRendererAnimationState.Ended) && (!string.IsNullOrEmpty(styleOfCurrentState.AnimationToPlayOnBegin) || this._styleOfCurrentState.Version != this._latestStyleVersion))
				{
					this._latestStyleVersion = styleOfCurrentState.Version;
					BrushState brushState = default(BrushState);
					brushState.FillFrom(styleOfCurrentState);
					this._startBrushState = brushState;
					this._currentBrushState = brushState;
					using (List<StyleLayer>.Enumerator enumerator = styleOfCurrentState.Layers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							StyleLayer styleLayer = enumerator.Current;
							BrushLayerState brushLayerState = default(BrushLayerState);
							brushLayerState.FillFrom(styleLayer);
							this._currentBrushLayerState[styleLayer.Name] = brushLayerState;
							this._startBrushLayerState[styleLayer.Name] = brushLayerState;
						}
						return;
					}
				}
				if (this._brushRendererAnimationState == BrushRenderer.BrushRendererAnimationState.PlayingBasicTranisition)
				{
					float num = (this.UseLocalTimer ? this._brushLocalTimer : globalAnimTime);
					if (num >= this.Brush.TransitionDuration)
					{
						this.EndAnimation();
						return;
					}
					float num2 = num / this.Brush.TransitionDuration;
					if (num2 > 1f)
					{
						num2 = 1f;
					}
					BrushState startBrushState = this._startBrushState;
					BrushState brushState2 = default(BrushState);
					brushState2.LerpFrom(startBrushState, styleOfCurrentState, num2);
					this._currentBrushState = brushState2;
					using (List<StyleLayer>.Enumerator enumerator = styleOfCurrentState.Layers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							StyleLayer styleLayer2 = enumerator.Current;
							BrushLayerState brushLayerState2 = this._startBrushLayerState[styleLayer2.Name];
							BrushLayerState brushLayerState3 = default(BrushLayerState);
							brushLayerState3.LerpFrom(brushLayerState2, styleLayer2, num2);
							this._currentBrushLayerState[styleLayer2.Name] = brushLayerState3;
						}
						return;
					}
				}
				if (this._brushRendererAnimationState == BrushRenderer.BrushRendererAnimationState.PlayingAnimation)
				{
					string animationToPlayOnBegin = styleOfCurrentState.AnimationToPlayOnBegin;
					BrushAnimation animation = this.Brush.GetAnimation(animationToPlayOnBegin);
					if (animation == null || (!animation.Loop && this._brushTimer >= animation.Duration))
					{
						this.EndAnimation();
						return;
					}
					float num3 = this._brushTimer % animation.Duration;
					bool flag = this._brushTimer < animation.Duration;
					BrushState startBrushState2 = this._startBrushState;
					BrushLayerAnimation styleAnimation = animation.StyleAnimation;
					BrushState brushState3 = this.AnimateBrushState(animation, styleAnimation, num3, flag, startBrushState2, styleOfCurrentState);
					this._currentBrushState = brushState3;
					foreach (StyleLayer styleLayer3 in styleOfCurrentState.Layers)
					{
						BrushLayerState brushLayerState4 = this._startBrushLayerState[styleLayer3.Name];
						BrushLayerAnimation layerAnimation = animation.GetLayerAnimation(styleLayer3.Name);
						BrushLayerState brushLayerState5 = this.AnimateBrushLayerState(animation, layerAnimation, num3, flag, brushLayerState4, styleLayer3);
						this._currentBrushLayerState[styleLayer3.Name] = brushLayerState5;
					}
				}
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00007B28 File Offset: 0x00005D28
		private BrushLayerState AnimateBrushLayerState(BrushAnimation animation, BrushLayerAnimation layerAnimation, float brushStateTimer, bool isFirstCycle, BrushLayerState startState, IBrushLayerData source)
		{
			BrushLayerState brushLayerState = default(BrushLayerState);
			if (isFirstCycle)
			{
				float num = brushStateTimer * (1f / animation.Duration);
				brushLayerState.LerpFrom(startState, source, num);
			}
			else
			{
				brushLayerState.FillFrom(source);
			}
			if (layerAnimation != null)
			{
				foreach (BrushAnimationProperty brushAnimationProperty in layerAnimation.Collections)
				{
					BrushAnimationProperty.BrushAnimationPropertyType propertyType = brushAnimationProperty.PropertyType;
					BrushAnimationKeyFrame brushAnimationKeyFrame = null;
					BrushAnimationKeyFrame brushAnimationKeyFrame2;
					if (animation.Loop)
					{
						BrushAnimationKeyFrame frameAt = brushAnimationProperty.GetFrameAt(0);
						if (isFirstCycle && this._brushTimer < frameAt.Time)
						{
							brushAnimationKeyFrame2 = frameAt;
						}
						else
						{
							brushAnimationKeyFrame2 = brushAnimationProperty.GetFrameAfter(brushStateTimer);
							if (brushAnimationKeyFrame2 == null)
							{
								brushAnimationKeyFrame2 = frameAt;
								brushAnimationKeyFrame = brushAnimationProperty.GetFrameAt(brushAnimationProperty.Count - 1);
							}
							else if (brushAnimationKeyFrame2 == frameAt)
							{
								brushAnimationKeyFrame = brushAnimationProperty.GetFrameAt(brushAnimationProperty.Count - 1);
							}
							else
							{
								brushAnimationKeyFrame = brushAnimationProperty.GetFrameAt(brushAnimationKeyFrame2.Index - 1);
							}
						}
					}
					else
					{
						brushAnimationKeyFrame2 = brushAnimationProperty.GetFrameAfter(brushStateTimer);
						if (brushAnimationKeyFrame2 != null)
						{
							brushAnimationKeyFrame = brushAnimationProperty.GetFrameAt(brushAnimationKeyFrame2.Index - 1);
						}
						else
						{
							brushAnimationKeyFrame = brushAnimationProperty.GetFrameAt(brushAnimationProperty.Count - 1);
						}
					}
					BrushAnimationKeyFrame brushAnimationKeyFrame3 = null;
					BrushLayerState brushLayerState2 = default(BrushLayerState);
					IBrushLayerData brushLayerData = null;
					BrushAnimationKeyFrame brushAnimationKeyFrame4 = null;
					float num4;
					if (brushAnimationKeyFrame2 != null)
					{
						if (brushAnimationKeyFrame != null)
						{
							float num2;
							float num3;
							if (animation.Loop)
							{
								if (brushAnimationKeyFrame2.Index == 0)
								{
									num2 = brushAnimationKeyFrame2.Time + (animation.Duration - brushAnimationKeyFrame.Time);
									if (brushStateTimer >= brushAnimationKeyFrame.Time)
									{
										num3 = brushStateTimer - brushAnimationKeyFrame.Time;
									}
									else
									{
										num3 = animation.Duration - brushAnimationKeyFrame.Time + brushStateTimer;
									}
								}
								else
								{
									num2 = brushAnimationKeyFrame2.Time - brushAnimationKeyFrame.Time;
									num3 = brushStateTimer - brushAnimationKeyFrame.Time;
								}
							}
							else
							{
								num2 = brushAnimationKeyFrame2.Time - brushAnimationKeyFrame.Time;
								num3 = brushStateTimer - brushAnimationKeyFrame.Time;
							}
							num4 = num3 * (1f / num2);
							brushAnimationKeyFrame3 = brushAnimationKeyFrame;
							brushAnimationKeyFrame4 = brushAnimationKeyFrame2;
						}
						else
						{
							num4 = brushStateTimer * (1f / brushAnimationKeyFrame2.Time);
							brushLayerState2 = startState;
							brushAnimationKeyFrame4 = brushAnimationKeyFrame2;
						}
					}
					else
					{
						num4 = (brushStateTimer - brushAnimationKeyFrame.Time) * (1f / (animation.Duration - brushAnimationKeyFrame.Time));
						brushAnimationKeyFrame3 = brushAnimationKeyFrame;
						brushLayerData = source;
					}
					switch (propertyType)
					{
					case BrushAnimationProperty.BrushAnimationPropertyType.ColorFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.AlphaFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.HueFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.SaturationFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.ValueFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.OverlayXOffset:
					case BrushAnimationProperty.BrushAnimationPropertyType.OverlayYOffset:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineAmount:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowRadius:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextBlur:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextShadowOffset:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextShadowAngle:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextColorFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextAlphaFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextHueFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextSaturationFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextValueFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.XOffset:
					case BrushAnimationProperty.BrushAnimationPropertyType.YOffset:
					case BrushAnimationProperty.BrushAnimationPropertyType.OverridenWidth:
					case BrushAnimationProperty.BrushAnimationPropertyType.OverridenHeight:
					case BrushAnimationProperty.BrushAnimationPropertyType.ExtendLeft:
					case BrushAnimationProperty.BrushAnimationPropertyType.ExtendRight:
					case BrushAnimationProperty.BrushAnimationPropertyType.ExtendTop:
					case BrushAnimationProperty.BrushAnimationPropertyType.ExtendBottom:
					{
						float num5 = ((brushAnimationKeyFrame3 != null) ? brushAnimationKeyFrame3.GetValueAsFloat() : brushLayerState2.GetValueAsFloat(propertyType));
						float num6 = ((brushLayerData != null) ? brushLayerData.GetValueAsFloat(propertyType) : brushAnimationKeyFrame4.GetValueAsFloat());
						brushLayerState.SetValueAsFloat(propertyType, MathF.Lerp(num5, num6, num4, 1E-05f));
						break;
					}
					case BrushAnimationProperty.BrushAnimationPropertyType.Color:
					case BrushAnimationProperty.BrushAnimationPropertyType.FontColor:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowColor:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineColor:
					{
						Color color = ((brushAnimationKeyFrame3 != null) ? brushAnimationKeyFrame3.GetValueAsColor() : brushLayerState2.GetValueAsColor(propertyType));
						Color color2 = ((brushLayerData != null) ? brushLayerData.GetValueAsColor(propertyType) : brushAnimationKeyFrame4.GetValueAsColor());
						BrushAnimationProperty.BrushAnimationPropertyType brushAnimationPropertyType = propertyType;
						Color color3 = Color.Lerp(color, color2, num4);
						brushLayerState.SetValueAsColor(brushAnimationPropertyType, color3);
						break;
					}
					case BrushAnimationProperty.BrushAnimationPropertyType.Sprite:
					case BrushAnimationProperty.BrushAnimationPropertyType.OverlaySprite:
					{
						Sprite sprite = ((brushAnimationKeyFrame3 != null) ? brushAnimationKeyFrame3.GetValueAsSprite() : null) ?? brushLayerState2.GetValueAsSprite(propertyType);
						Sprite sprite2 = ((brushLayerData != null) ? brushLayerData.GetValueAsSprite(propertyType) : null) ?? brushAnimationKeyFrame4.GetValueAsSprite();
						brushLayerState.SetValueAsSprite(propertyType, ((double)num4 <= 0.9) ? sprite : sprite2);
						break;
					}
					}
				}
			}
			return brushLayerState;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00007F2C File Offset: 0x0000612C
		public bool IsUpdateNeeded()
		{
			return this._brushRendererAnimationState == BrushRenderer.BrushRendererAnimationState.PlayingBasicTranisition || this._brushRendererAnimationState == BrushRenderer.BrushRendererAnimationState.PlayingAnimation || (this._styleOfCurrentState != null && this._styleOfCurrentState.Version != this._latestStyleVersion);
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00007F64 File Offset: 0x00006164
		private BrushState AnimateBrushState(BrushAnimation animation, BrushLayerAnimation layerAnimation, float brushStateTimer, bool isFirstCycle, BrushState startState, Style source)
		{
			BrushState brushState = default(BrushState);
			if (isFirstCycle)
			{
				float num = brushStateTimer * (1f / animation.Duration);
				brushState.LerpFrom(startState, source, num);
			}
			else
			{
				brushState.FillFrom(source);
			}
			if (layerAnimation != null)
			{
				foreach (BrushAnimationProperty brushAnimationProperty in layerAnimation.Collections)
				{
					BrushAnimationProperty.BrushAnimationPropertyType propertyType = brushAnimationProperty.PropertyType;
					BrushAnimationKeyFrame brushAnimationKeyFrame = null;
					BrushAnimationKeyFrame brushAnimationKeyFrame2;
					if (animation.Loop)
					{
						BrushAnimationKeyFrame frameAt = brushAnimationProperty.GetFrameAt(0);
						if (isFirstCycle && this._brushTimer < frameAt.Time)
						{
							brushAnimationKeyFrame2 = frameAt;
						}
						else
						{
							brushAnimationKeyFrame2 = brushAnimationProperty.GetFrameAfter(brushStateTimer);
							if (brushAnimationKeyFrame2 == null)
							{
								brushAnimationKeyFrame2 = frameAt;
								brushAnimationKeyFrame = brushAnimationProperty.GetFrameAt(brushAnimationProperty.Count - 1);
							}
							else if (brushAnimationKeyFrame2 == frameAt)
							{
								brushAnimationKeyFrame = brushAnimationProperty.GetFrameAt(brushAnimationProperty.Count - 1);
							}
							else
							{
								brushAnimationKeyFrame = brushAnimationProperty.GetFrameAt(brushAnimationKeyFrame2.Index - 1);
							}
						}
					}
					else
					{
						brushAnimationKeyFrame2 = brushAnimationProperty.GetFrameAfter(brushStateTimer);
						brushAnimationKeyFrame = ((brushAnimationKeyFrame2 != null) ? brushAnimationProperty.GetFrameAt(brushAnimationKeyFrame2.Index - 1) : brushAnimationProperty.GetFrameAt(brushAnimationProperty.Count - 1));
					}
					BrushAnimationKeyFrame brushAnimationKeyFrame3 = null;
					BrushState brushState2 = default(BrushState);
					Style style = null;
					BrushAnimationKeyFrame brushAnimationKeyFrame4 = null;
					float num4;
					if (brushAnimationKeyFrame2 != null)
					{
						if (brushAnimationKeyFrame != null)
						{
							float num2;
							float num3;
							if (animation.Loop)
							{
								if (brushAnimationKeyFrame2.Index == 0)
								{
									num2 = brushAnimationKeyFrame2.Time + (animation.Duration - brushAnimationKeyFrame.Time);
									if (brushStateTimer >= brushAnimationKeyFrame.Time)
									{
										num3 = brushStateTimer - brushAnimationKeyFrame.Time;
									}
									else
									{
										num3 = animation.Duration - brushAnimationKeyFrame.Time + brushStateTimer;
									}
								}
								else
								{
									num2 = brushAnimationKeyFrame2.Time - brushAnimationKeyFrame.Time;
									num3 = brushStateTimer - brushAnimationKeyFrame.Time;
								}
							}
							else
							{
								num2 = brushAnimationKeyFrame2.Time - brushAnimationKeyFrame.Time;
								num3 = brushStateTimer - brushAnimationKeyFrame.Time;
							}
							num4 = num3 * (1f / num2);
							brushAnimationKeyFrame3 = brushAnimationKeyFrame;
							brushAnimationKeyFrame4 = brushAnimationKeyFrame2;
						}
						else
						{
							num4 = brushStateTimer * (1f / brushAnimationKeyFrame2.Time);
							brushState2 = startState;
							brushAnimationKeyFrame4 = brushAnimationKeyFrame2;
						}
					}
					else
					{
						num4 = (brushStateTimer - brushAnimationKeyFrame.Time) * (1f / (animation.Duration - brushAnimationKeyFrame.Time));
						brushAnimationKeyFrame3 = brushAnimationKeyFrame;
						style = source;
					}
					num4 = MathF.Clamp(num4, 0f, 1f);
					switch (propertyType)
					{
					case BrushAnimationProperty.BrushAnimationPropertyType.ColorFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.AlphaFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.HueFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.SaturationFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.ValueFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.OverlayXOffset:
					case BrushAnimationProperty.BrushAnimationPropertyType.OverlayYOffset:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineAmount:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowRadius:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextBlur:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextShadowOffset:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextShadowAngle:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextColorFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextAlphaFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextHueFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextSaturationFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextValueFactor:
					case BrushAnimationProperty.BrushAnimationPropertyType.XOffset:
					case BrushAnimationProperty.BrushAnimationPropertyType.YOffset:
					case BrushAnimationProperty.BrushAnimationPropertyType.OverridenWidth:
					case BrushAnimationProperty.BrushAnimationPropertyType.OverridenHeight:
					case BrushAnimationProperty.BrushAnimationPropertyType.ExtendLeft:
					case BrushAnimationProperty.BrushAnimationPropertyType.ExtendRight:
					case BrushAnimationProperty.BrushAnimationPropertyType.ExtendTop:
					case BrushAnimationProperty.BrushAnimationPropertyType.ExtendBottom:
					{
						float num5 = ((brushAnimationKeyFrame3 != null) ? brushAnimationKeyFrame3.GetValueAsFloat() : brushState2.GetValueAsFloat(propertyType));
						float num6 = ((style != null) ? style.GetValueAsFloat(propertyType) : brushAnimationKeyFrame4.GetValueAsFloat());
						brushState.SetValueAsFloat(propertyType, MathF.Lerp(num5, num6, num4, 1E-05f));
						break;
					}
					case BrushAnimationProperty.BrushAnimationPropertyType.Color:
					case BrushAnimationProperty.BrushAnimationPropertyType.FontColor:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowColor:
					case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineColor:
					{
						Color color = ((brushAnimationKeyFrame3 != null) ? brushAnimationKeyFrame3.GetValueAsColor() : brushState2.GetValueAsColor(propertyType));
						Color color2 = ((style != null) ? style.GetValueAsColor(propertyType) : brushAnimationKeyFrame4.GetValueAsColor());
						BrushAnimationProperty.BrushAnimationPropertyType brushAnimationPropertyType = propertyType;
						Color color3 = Color.Lerp(color, color2, num4);
						brushState.SetValueAsColor(brushAnimationPropertyType, color3);
						break;
					}
					case BrushAnimationProperty.BrushAnimationPropertyType.Sprite:
					case BrushAnimationProperty.BrushAnimationPropertyType.OverlaySprite:
					{
						Sprite sprite = ((brushAnimationKeyFrame3 != null) ? brushAnimationKeyFrame3.GetValueAsSprite() : null) ?? brushState2.GetValueAsSprite(propertyType);
						Sprite sprite2 = ((style != null) ? style.GetValueAsSprite(propertyType) : null) ?? brushAnimationKeyFrame4.GetValueAsSprite();
						brushState.SetValueAsSprite(propertyType, ((double)num4 <= 0.9) ? sprite : sprite2);
						break;
					}
					}
				}
			}
			return brushState;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00008370 File Offset: 0x00006570
		private void EndAnimation()
		{
			if (this.Brush != null)
			{
				Style styleOfCurrentState = this._styleOfCurrentState;
				BrushState brushState = default(BrushState);
				brushState.FillFrom(styleOfCurrentState);
				this._startBrushState = brushState;
				this._currentBrushState = brushState;
				if (this.Brush.TransitionDuration == 0f)
				{
					this._brushRendererAnimationState = BrushRenderer.BrushRendererAnimationState.None;
				}
				foreach (StyleLayer styleLayer in styleOfCurrentState.Layers)
				{
					BrushLayerState brushLayerState = default(BrushLayerState);
					brushLayerState.FillFrom(styleLayer);
					this._startBrushLayerState[styleLayer.Name] = brushLayerState;
					this._currentBrushLayerState[styleLayer.Name] = brushLayerState;
				}
				this._brushRendererAnimationState = BrushRenderer.BrushRendererAnimationState.Ended;
			}
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00008444 File Offset: 0x00006644
		public void Render(TwoDimensionDrawContext drawContext, Vector2 targetPosition, Vector2 targetSize, float scale, float contextAlpha, Vector2 overlayOffset = default(Vector2))
		{
			if (this.Brush != null)
			{
				if (this.ForcePixelPerfectPlacement)
				{
					targetPosition.X = (float)MathF.Round(targetPosition.X);
					targetPosition.Y = (float)MathF.Round(targetPosition.Y);
				}
				Style styleOfCurrentState = this._styleOfCurrentState;
				for (int i = 0; i < styleOfCurrentState.LayerCount; i++)
				{
					StyleLayer layer = styleOfCurrentState.GetLayer(i);
					if (!layer.IsHidden)
					{
						BrushLayerState brushLayerState;
						if (this._currentBrushLayerState.Count == 1)
						{
							Dictionary<string, BrushLayerState>.ValueCollection.Enumerator enumerator = this._currentBrushLayerState.Values.GetEnumerator();
							enumerator.MoveNext();
							brushLayerState = enumerator.Current;
						}
						else
						{
							brushLayerState = this._currentBrushLayerState[layer.Name];
						}
						Sprite sprite = brushLayerState.Sprite;
						if (sprite != null)
						{
							Texture texture = sprite.Texture;
							if (texture != null)
							{
								float num = targetPosition.X + brushLayerState.XOffset * scale;
								float num2 = targetPosition.Y + brushLayerState.YOffset * scale;
								SimpleMaterial simpleMaterial = drawContext.CreateSimpleMaterial();
								simpleMaterial.OverlayEnabled = false;
								simpleMaterial.CircularMaskingEnabled = false;
								if (layer.OverlayMethod == BrushOverlayMethod.CoverWithTexture && layer.OverlaySprite != null)
								{
									Sprite overlaySprite = layer.OverlaySprite;
									Texture texture2 = overlaySprite.Texture;
									if (texture2 != null)
									{
										simpleMaterial.OverlayEnabled = true;
										simpleMaterial.StartCoordinate = new Vector2(num, num2);
										simpleMaterial.Size = targetSize;
										simpleMaterial.OverlayTexture = texture2;
										simpleMaterial.UseOverlayAlphaAsMask = layer.UseOverlayAlphaAsMask;
										float num3;
										float num4;
										if (layer.UseOverlayAlphaAsMask)
										{
											num3 = brushLayerState.XOffset;
											num4 = brushLayerState.YOffset;
										}
										else if (overlayOffset == default(Vector2))
										{
											num3 = brushLayerState.OverlayXOffset;
											num4 = brushLayerState.OverlayYOffset;
										}
										else
										{
											num3 = overlayOffset.X;
											num4 = overlayOffset.Y;
										}
										if (layer.UseRandomBaseOverlayXOffset)
										{
											num3 += this.GetRandomXOffset();
										}
										if (layer.UseRandomBaseOverlayYOffset)
										{
											num4 += this.GetRandomYOffset();
										}
										simpleMaterial.OverlayXOffset = num3 * scale;
										simpleMaterial.OverlayYOffset = num4 * scale;
										simpleMaterial.Scale = scale;
										simpleMaterial.OverlayTextureWidth = (layer.UseOverlayAlphaAsMask ? targetSize.X : ((float)overlaySprite.Width));
										simpleMaterial.OverlayTextureHeight = (layer.UseOverlayAlphaAsMask ? targetSize.Y : ((float)overlaySprite.Height));
									}
								}
								simpleMaterial.Texture = texture;
								simpleMaterial.Color = brushLayerState.Color * this.Brush.GlobalColor;
								simpleMaterial.ColorFactor = brushLayerState.ColorFactor * this.Brush.GlobalColorFactor;
								simpleMaterial.AlphaFactor = brushLayerState.AlphaFactor * this.Brush.GlobalAlphaFactor * contextAlpha;
								simpleMaterial.HueFactor = brushLayerState.HueFactor;
								simpleMaterial.SaturationFactor = brushLayerState.SaturationFactor;
								simpleMaterial.ValueFactor = brushLayerState.ValueFactor;
								float num5 = 0f;
								float num6 = 0f;
								if (layer.WidthPolicy == BrushLayerSizePolicy.StretchToTarget)
								{
									float num7 = layer.ExtendLeft;
									if (layer.HorizontalFlip)
									{
										num7 = layer.ExtendRight;
									}
									num5 = targetSize.X;
									num5 += (layer.ExtendRight + layer.ExtendLeft) * scale;
									num -= num7 * scale;
								}
								else if (layer.WidthPolicy == BrushLayerSizePolicy.Original)
								{
									num5 = (float)sprite.Width * scale;
								}
								else if (layer.WidthPolicy == BrushLayerSizePolicy.Overriden)
								{
									num5 = layer.OverridenWidth * scale;
								}
								if (layer.HeightPolicy == BrushLayerSizePolicy.StretchToTarget)
								{
									float num8 = layer.ExtendTop;
									if (layer.HorizontalFlip)
									{
										num8 = layer.ExtendBottom;
									}
									num6 = targetSize.Y;
									num6 += (layer.ExtendTop + layer.ExtendBottom) * scale;
									num2 -= num8 * scale;
								}
								else if (layer.HeightPolicy == BrushLayerSizePolicy.Original)
								{
									num6 = (float)sprite.Height * scale;
								}
								else if (layer.HeightPolicy == BrushLayerSizePolicy.Overriden)
								{
									num6 = layer.OverridenHeight * scale;
								}
								bool horizontalFlip = layer.HorizontalFlip;
								bool verticalFlip = layer.VerticalFlip;
								drawContext.DrawSprite(sprite, simpleMaterial, num, num2, scale, num5, num6, horizontalFlip, verticalFlip);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00008838 File Offset: 0x00006A38
		public TextMaterial CreateTextMaterial(TwoDimensionDrawContext drawContext)
		{
			TextMaterial textMaterial = this._currentBrushState.CreateTextMaterial(drawContext);
			if (this.Brush != null)
			{
				textMaterial.ColorFactor *= this.Brush.GlobalColorFactor;
				textMaterial.AlphaFactor *= this.Brush.GlobalAlphaFactor;
				textMaterial.Color *= this.Brush.GlobalColor;
			}
			return textMaterial;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x000088A8 File Offset: 0x00006AA8
		public void RestartAnimation()
		{
			if (this.Brush != null)
			{
				this._brushLocalTimer = 0f;
				Style styleOfCurrentState = this._styleOfCurrentState;
				this._brushRendererAnimationState = BrushRenderer.BrushRendererAnimationState.None;
				if (styleOfCurrentState != null)
				{
					if (styleOfCurrentState.AnimationMode == StyleAnimationMode.BasicTransition)
					{
						this._brushRendererAnimationState = BrushRenderer.BrushRendererAnimationState.PlayingBasicTranisition;
						return;
					}
					if (styleOfCurrentState.AnimationMode == StyleAnimationMode.Animation)
					{
						this._brushRendererAnimationState = BrushRenderer.BrushRendererAnimationState.PlayingAnimation;
					}
				}
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x000088FA File Offset: 0x00006AFA
		public void SetSeed(int seed)
		{
			this._offsetSeed = seed;
		}

		// Token: 0x04000076 RID: 118
		private BrushState _startBrushState;

		// Token: 0x04000077 RID: 119
		private BrushState _currentBrushState;

		// Token: 0x04000078 RID: 120
		private Dictionary<string, BrushLayerState> _startBrushLayerState;

		// Token: 0x04000079 RID: 121
		private Dictionary<string, BrushLayerState> _currentBrushLayerState;

		// Token: 0x0400007A RID: 122
		public bool UseLocalTimer;

		// Token: 0x0400007B RID: 123
		private float _brushLocalTimer;

		// Token: 0x0400007C RID: 124
		private float _globalTime;

		// Token: 0x0400007D RID: 125
		private int _offsetSeed;

		// Token: 0x0400007E RID: 126
		private float _randomXOffset;

		// Token: 0x0400007F RID: 127
		private float _randomYOffset;

		// Token: 0x04000080 RID: 128
		private BrushRenderer.BrushRendererAnimationState _brushRendererAnimationState;

		// Token: 0x04000082 RID: 130
		private Brush _brush;

		// Token: 0x04000083 RID: 131
		private long _latestStyleVersion;

		// Token: 0x04000084 RID: 132
		private string _currentState;

		// Token: 0x04000085 RID: 133
		private Style _styleOfCurrentState;

		// Token: 0x02000074 RID: 116
		public enum BrushRendererAnimationState
		{
			// Token: 0x040003F8 RID: 1016
			None,
			// Token: 0x040003F9 RID: 1017
			PlayingAnimation,
			// Token: 0x040003FA RID: 1018
			PlayingBasicTranisition,
			// Token: 0x040003FB RID: 1019
			Ended
		}
	}
}
