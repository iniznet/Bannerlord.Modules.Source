using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	public struct ExplainedNumber
	{
		public float ResultNumber
		{
			get
			{
				return MathF.Clamp(this.BaseNumber + this.BaseNumber * this._sumOfFactors, this.LimitMinValue, this.LimitMaxValue);
			}
		}

		public float BaseNumber { get; private set; }

		public bool IncludeDescriptions
		{
			get
			{
				return this._explainer != null;
			}
		}

		public float LimitMinValue
		{
			get
			{
				if (this._limitMinValue == null)
				{
					return float.MinValue;
				}
				return this._limitMinValue.Value;
			}
		}

		public float LimitMaxValue
		{
			get
			{
				if (this._limitMaxValue == null)
				{
					return float.MaxValue;
				}
				return this._limitMaxValue.Value;
			}
		}

		public ExplainedNumber(float baseNumber = 0f, bool includeDescriptions = false, TextObject baseText = null)
		{
			this.BaseNumber = baseNumber;
			this._explainer = (includeDescriptions ? new ExplainedNumber.StatExplainer() : null);
			this._sumOfFactors = 0f;
			this._limitMinValue = new float?(float.MinValue);
			this._limitMaxValue = new float?(float.MaxValue);
			if (this._explainer != null && !this.BaseNumber.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				this._explainer.AddLine((baseText ?? ExplainedNumber.BaseText).ToString(), this.BaseNumber, ExplainedNumber.StatExplainer.OperationType.Base);
			}
		}

		public string GetExplanations()
		{
			if (this._explainer == null)
			{
				return "";
			}
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "GetExplanations");
			foreach (ValueTuple<string, float> valueTuple in this._explainer.GetLines(this.BaseNumber, this.ResultNumber))
			{
				string text = string.Format("{0} : {1}{2:0.##}\n", valueTuple.Item1, (valueTuple.Item2 > 0.001f) ? "+" : "", valueTuple.Item2);
				mbstringBuilder.Append<string>(text);
			}
			return mbstringBuilder.ToStringAndRelease();
		}

		[return: TupleElementNames(new string[] { "name", "number" })]
		public List<ValueTuple<string, float>> GetLines()
		{
			if (this._explainer == null)
			{
				return new List<ValueTuple<string, float>>();
			}
			return this._explainer.GetLines(this.BaseNumber, this.ResultNumber);
		}

		public void AddFromExplainedNumber(ExplainedNumber explainedNumber, TextObject baseText)
		{
			if (explainedNumber._explainer != null && this._explainer != null)
			{
				if (explainedNumber._explainer.BaseLine != null && explainedNumber._explainer.BaseLine != null && !explainedNumber.BaseNumber.ApproximatelyEqualsTo(0f, 1E-05f))
				{
					float num = explainedNumber._explainer.BaseLine.Value.Number + explainedNumber._explainer.BaseLine.Value.Number * explainedNumber._sumOfFactors;
					this._explainer.AddLine(((baseText != null) ? baseText.ToString() : null) ?? ExplainedNumber.BaseText.ToString(), num, ExplainedNumber.StatExplainer.OperationType.Add);
				}
				foreach (ExplainedNumber.StatExplainer.ExplanationLine explanationLine in explainedNumber._explainer.Lines)
				{
					if (explanationLine.OperationType == ExplainedNumber.StatExplainer.OperationType.Add)
					{
						float num2 = explanationLine.Number + explanationLine.Number * explainedNumber._sumOfFactors;
						this._explainer.AddLine(explanationLine.Name, num2, explanationLine.OperationType);
					}
				}
			}
			this.BaseNumber += explainedNumber.ResultNumber;
		}

		public void Add(float value, TextObject description = null, TextObject variable = null)
		{
			if (value.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				return;
			}
			this.BaseNumber += value;
			if (description != null && this._explainer != null && !value.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				if (variable != null)
				{
					description.SetTextVariable("A0", variable);
				}
				this._explainer.AddLine(description.ToString(), value, ExplainedNumber.StatExplainer.OperationType.Add);
			}
		}

		public void AddFactor(float value, TextObject description = null)
		{
			if (value.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				return;
			}
			this._sumOfFactors += value;
			if (description != null && this._explainer != null && !value.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				this._explainer.AddLine(description.ToString(), MathF.Round(value, 3) * 100f, ExplainedNumber.StatExplainer.OperationType.Multiply);
			}
		}

		public void LimitMin(float minValue)
		{
			this._limitMinValue = new float?(minValue);
			if (this._explainer != null)
			{
				this._explainer.AddLine(ExplainedNumber.LimitMinText.ToString(), minValue, ExplainedNumber.StatExplainer.OperationType.LimitMin);
			}
		}

		public void LimitMax(float maxValue)
		{
			this._limitMaxValue = new float?(maxValue);
			if (this._explainer != null)
			{
				this._explainer.AddLine(ExplainedNumber.LimitMaxText.ToString(), maxValue, ExplainedNumber.StatExplainer.OperationType.LimitMax);
			}
		}

		public void Clamp(float minValue, float maxValue)
		{
			this.LimitMin(minValue);
			this.LimitMax(maxValue);
		}

		private static readonly TextObject LimitMinText = new TextObject("{=GNalaRaN}Minimum", null);

		private static readonly TextObject LimitMaxText = new TextObject("{=cfjTtxWv}Maximum", null);

		private static readonly TextObject BaseText = new TextObject("{=basevalue}Base", null);

		private float? _limitMinValue;

		private float? _limitMaxValue;

		private ExplainedNumber.StatExplainer _explainer;

		private float _sumOfFactors;

		private class StatExplainer
		{
			public List<ExplainedNumber.StatExplainer.ExplanationLine> Lines { get; private set; } = new List<ExplainedNumber.StatExplainer.ExplanationLine>();

			public ExplainedNumber.StatExplainer.ExplanationLine? BaseLine { get; private set; }

			public ExplainedNumber.StatExplainer.ExplanationLine? LimitMinLine { get; private set; }

			public ExplainedNumber.StatExplainer.ExplanationLine? LimitMaxLine { get; private set; }

			[return: TupleElementNames(new string[] { "name", "number" })]
			public List<ValueTuple<string, float>> GetLines(float baseNumber, float resultNumber)
			{
				List<ValueTuple<string, float>> list = new List<ValueTuple<string, float>>();
				if (this.BaseLine != null)
				{
					list.Add(new ValueTuple<string, float>(this.BaseLine.Value.Name, this.BaseLine.Value.Number));
				}
				foreach (ExplainedNumber.StatExplainer.ExplanationLine explanationLine in this.Lines)
				{
					float num = explanationLine.Number;
					if (explanationLine.OperationType == ExplainedNumber.StatExplainer.OperationType.Multiply)
					{
						num = baseNumber * num * 0.01f;
					}
					list.Add(new ValueTuple<string, float>(explanationLine.Name, num));
				}
				if (this.LimitMinLine != null && this.LimitMinLine.Value.Number > resultNumber)
				{
					list.Add(new ValueTuple<string, float>(this.LimitMinLine.Value.Name, this.LimitMinLine.Value.Number));
				}
				if (this.LimitMaxLine != null && this.LimitMaxLine.Value.Number < resultNumber)
				{
					list.Add(new ValueTuple<string, float>(this.LimitMaxLine.Value.Name, this.LimitMaxLine.Value.Number));
				}
				return list;
			}

			public void AddLine(string name, float number, ExplainedNumber.StatExplainer.OperationType opType)
			{
				ExplainedNumber.StatExplainer.ExplanationLine explanationLine = new ExplainedNumber.StatExplainer.ExplanationLine(name, number, opType);
				if (opType == ExplainedNumber.StatExplainer.OperationType.Add || opType == ExplainedNumber.StatExplainer.OperationType.Multiply)
				{
					int num = -1;
					for (int i = 0; i < this.Lines.Count; i++)
					{
						if (this.Lines[i].Name.Equals(name) && this.Lines[i].OperationType == opType)
						{
							num = i;
							break;
						}
					}
					if (num < 0)
					{
						this.Lines.Add(explanationLine);
						return;
					}
					explanationLine = new ExplainedNumber.StatExplainer.ExplanationLine(name, number + this.Lines[num].Number, opType);
					this.Lines[num] = explanationLine;
					return;
				}
				else
				{
					if (opType == ExplainedNumber.StatExplainer.OperationType.Base)
					{
						this.BaseLine = new ExplainedNumber.StatExplainer.ExplanationLine?(explanationLine);
						return;
					}
					if (opType == ExplainedNumber.StatExplainer.OperationType.LimitMin)
					{
						this.LimitMinLine = new ExplainedNumber.StatExplainer.ExplanationLine?(explanationLine);
						return;
					}
					if (opType == ExplainedNumber.StatExplainer.OperationType.LimitMax)
					{
						this.LimitMaxLine = new ExplainedNumber.StatExplainer.ExplanationLine?(explanationLine);
					}
					return;
				}
			}

			public enum OperationType
			{
				Base,
				Add,
				Multiply,
				LimitMin,
				LimitMax
			}

			public readonly struct ExplanationLine
			{
				public ExplanationLine(string name, float number, ExplainedNumber.StatExplainer.OperationType operationType)
				{
					this.Name = name;
					this.Number = number;
					this.OperationType = operationType;
				}

				public readonly float Number;

				public readonly string Name;

				public readonly ExplainedNumber.StatExplainer.OperationType OperationType;
			}
		}
	}
}
