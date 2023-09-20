using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.Library
{
	public static class MBMath
	{
		public static float ToRadians(this float f)
		{
			return f * 0.017453292f;
		}

		public static float ToDegrees(this float f)
		{
			return f * 57.295776f;
		}

		public static bool ApproximatelyEqualsTo(this float f, float comparedValue, float epsilon = 1E-05f)
		{
			return Math.Abs(f - comparedValue) <= epsilon;
		}

		public static bool ApproximatelyEquals(float first, float second, float epsilon = 1E-05f)
		{
			return Math.Abs(first - second) <= epsilon;
		}

		public static bool IsValidValue(float f)
		{
			return !float.IsNaN(f) && !float.IsInfinity(f);
		}

		public static int ClampIndex(int value, int minValue, int maxValue)
		{
			return MBMath.ClampInt(value, minValue, maxValue - 1);
		}

		public static int ClampInt(int value, int minValue, int maxValue)
		{
			return Math.Max(Math.Min(value, maxValue), minValue);
		}

		public static float ClampFloat(float value, float minValue, float maxValue)
		{
			return Math.Max(Math.Min(value, maxValue), minValue);
		}

		public static void ClampUnit(ref float value)
		{
			value = MBMath.ClampFloat(value, 0f, 1f);
		}

		public static int GetNumberOfBitsToRepresentNumber(uint value)
		{
			int num = 0;
			for (uint num2 = value; num2 > 0U; num2 >>= 1)
			{
				num++;
			}
			return num;
		}

		public static IEnumerable<ValueTuple<T, int>> DistributeShares<T>(int totalAward, IEnumerable<T> stakeHolders, Func<T, int> shareFunction)
		{
			List<ValueTuple<T, int>> sharesList = new List<ValueTuple<T, int>>(20);
			int num = 0;
			foreach (T t in stakeHolders)
			{
				int num2 = shareFunction(t);
				sharesList.Add(new ValueTuple<T, int>(t, num2));
				num += num2;
			}
			if (num > 0)
			{
				int remainingShares = num;
				int remaingAward = totalAward;
				int i = 0;
				while (i < sharesList.Count && remaingAward > 0)
				{
					int item = sharesList[i].Item2;
					int num3 = MathF.Round((float)remaingAward * (float)item / (float)remainingShares);
					if (num3 > remaingAward)
					{
						num3 = remaingAward;
					}
					remaingAward -= num3;
					remainingShares -= item;
					yield return new ValueTuple<T, int>(sharesList[i].Item1, num3);
					int num4 = i + 1;
					i = num4;
				}
			}
			yield break;
		}

		public static int GetNumberOfBitsToRepresentNumber(ulong value)
		{
			int num = 0;
			for (ulong num2 = value; num2 > 0UL; num2 >>= 1)
			{
				num++;
			}
			return num;
		}

		public static float Lerp(float valueFrom, float valueTo, float amount, float minimumDifference = 1E-05f)
		{
			if (Math.Abs(valueFrom - valueTo) <= minimumDifference)
			{
				return valueTo;
			}
			return valueFrom + (valueTo - valueFrom) * amount;
		}

		public static float LinearExtrapolation(float valueFrom, float valueTo, float amount)
		{
			return valueFrom + (valueTo - valueFrom) * amount;
		}

		public static Vec3 Lerp(Vec3 vecFrom, Vec3 vecTo, float amount, float minimumDifference)
		{
			return new Vec3(MBMath.Lerp(vecFrom.x, vecTo.x, amount, minimumDifference), MBMath.Lerp(vecFrom.y, vecTo.y, amount, minimumDifference), MBMath.Lerp(vecFrom.z, vecTo.z, amount, minimumDifference), -1f);
		}

		public static Vec2 Lerp(Vec2 vecFrom, Vec2 vecTo, float amount, float minimumDifference)
		{
			return new Vec2(MBMath.Lerp(vecFrom.x, vecTo.x, amount, minimumDifference), MBMath.Lerp(vecFrom.y, vecTo.y, amount, minimumDifference));
		}

		public static float Map(float input, float inputMinimum, float inputMaximum, float outputMinimum, float outputMaximum)
		{
			input = MBMath.ClampFloat(input, inputMinimum, inputMaximum);
			return (input - inputMinimum) * (outputMaximum - outputMinimum) / (inputMaximum - inputMinimum) + outputMinimum;
		}

		public static Mat3 Lerp(ref Mat3 matFrom, ref Mat3 matTo, float amount, float minimumDifference)
		{
			return new Mat3(MBMath.Lerp(matFrom.s, matTo.s, amount, minimumDifference), MBMath.Lerp(matFrom.f, matTo.f, amount, minimumDifference), MBMath.Lerp(matFrom.u, matTo.u, amount, minimumDifference));
		}

		public static float LerpRadians(float valueFrom, float valueTo, float amount, float minChange, float maxChange)
		{
			float smallestDifferenceBetweenTwoAngles = MBMath.GetSmallestDifferenceBetweenTwoAngles(valueFrom, valueTo);
			if (Math.Abs(smallestDifferenceBetweenTwoAngles) <= minChange)
			{
				return valueTo;
			}
			float num = (float)Math.Sign(smallestDifferenceBetweenTwoAngles) * MBMath.ClampFloat(Math.Abs(smallestDifferenceBetweenTwoAngles * amount), minChange, maxChange);
			return MBMath.WrapAngle(valueFrom + num);
		}

		public static float SplitLerp(float value1, float value2, float value3, float cutOff, float amount, float minimumDifference)
		{
			if (amount <= cutOff)
			{
				float num = amount / cutOff;
				return MBMath.Lerp(value1, value2, num, minimumDifference);
			}
			float num2 = 1f - cutOff;
			float num3 = (amount - cutOff) / num2;
			return MBMath.Lerp(value2, value3, num3, minimumDifference);
		}

		public static float InverseLerp(float valueFrom, float valueTo, float value)
		{
			return (value - valueFrom) / (valueTo - valueFrom);
		}

		public static float SmoothStep(float edge0, float edge1, float value)
		{
			float num = MBMath.ClampFloat((value - edge0) / (edge1 - edge0), 0f, 1f);
			return num * num * (3f - 2f * num);
		}

		public static float BilinearLerp(float topLeft, float topRight, float botLeft, float botRight, float x, float y)
		{
			float num = MBMath.Lerp(topLeft, topRight, x, 1E-05f);
			float num2 = MBMath.Lerp(botLeft, botRight, x, 1E-05f);
			return MBMath.Lerp(num, num2, y, 1E-05f);
		}

		public static float GetSmallestDifferenceBetweenTwoAngles(float fromAngle, float toAngle)
		{
			float num = toAngle - fromAngle;
			if (num > 3.1415927f)
			{
				num = -6.2831855f + num;
			}
			if (num < -3.1415927f)
			{
				num = 6.2831855f + num;
			}
			return num;
		}

		public static float ClampAngle(float angle, float restrictionCenter, float restrictionRange)
		{
			restrictionRange /= 2f;
			float smallestDifferenceBetweenTwoAngles = MBMath.GetSmallestDifferenceBetweenTwoAngles(restrictionCenter, angle);
			if (smallestDifferenceBetweenTwoAngles > restrictionRange)
			{
				angle = restrictionCenter + restrictionRange;
			}
			else if (smallestDifferenceBetweenTwoAngles < -restrictionRange)
			{
				angle = restrictionCenter - restrictionRange;
			}
			if (angle > 3.1415927f)
			{
				angle -= 6.2831855f;
			}
			else if (angle < -3.1415927f)
			{
				angle += 6.2831855f;
			}
			return angle;
		}

		public static float WrapAngle(float angle)
		{
			angle = (float)Math.IEEERemainder((double)angle, 6.283185307179586);
			if (angle <= -3.1415927f)
			{
				angle += 6.2831855f;
			}
			else if (angle > 3.1415927f)
			{
				angle -= 6.2831855f;
			}
			return angle;
		}

		public static bool IsBetween(float numberToCheck, float bottom, float top)
		{
			return numberToCheck > bottom && numberToCheck < top;
		}

		public static bool IsBetween(int value, int minValue, int maxValue)
		{
			return value >= minValue && value < maxValue;
		}

		public static bool IsBetweenInclusive(float numberToCheck, float bottom, float top)
		{
			return numberToCheck >= bottom && numberToCheck <= top;
		}

		public static uint ColorFromRGBA(float red, float green, float blue, float alpha)
		{
			return ((uint)(alpha * 255f) << 24) + ((uint)(red * 255f) << 16) + ((uint)(green * 255f) << 8) + (uint)(blue * 255f);
		}

		public static Color HSBtoRGB(float hue, float saturation, float brightness, float outputAlpha)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = brightness * saturation;
			float num5 = num4 * (1f - MathF.Abs(hue * 0.016666668f % 2f - 1f));
			float num6 = brightness - num4;
			switch ((int)(hue * 0.016666668f % 6f))
			{
			case 0:
				num = num4;
				num2 = num5;
				num3 = 0f;
				break;
			case 1:
				num = num5;
				num2 = num4;
				num3 = 0f;
				break;
			case 2:
				num = 0f;
				num2 = num4;
				num3 = num5;
				break;
			case 3:
				num = 0f;
				num2 = num5;
				num3 = num4;
				break;
			case 4:
				num = num5;
				num2 = 0f;
				num3 = num4;
				break;
			case 5:
				num = num4;
				num2 = 0f;
				num3 = num5;
				break;
			}
			return new Color(num + num6, num2 + num6, num3 + num6, outputAlpha);
		}

		public static Vec3 RGBtoHSB(Color rgb)
		{
			Vec3 vec = new Vec3(0f, 0f, 0f, -1f);
			float num = MathF.Min(MathF.Min(rgb.Red, rgb.Green), rgb.Blue);
			float num2 = MathF.Max(MathF.Max(rgb.Red, rgb.Green), rgb.Blue);
			float num3 = num2 - num;
			vec.z = num2;
			if (MathF.Abs(num3) < 0.0001f)
			{
				vec.x = 0f;
			}
			else if (MathF.Abs(num2 - rgb.Red) < 0.0001f)
			{
				vec.x = 60f * ((rgb.Green - rgb.Blue) / num3 % 6f);
			}
			else if (MathF.Abs(num2 - rgb.Green) < 0.0001f)
			{
				vec.x = 60f * ((rgb.Blue - rgb.Red) / num3 + 2f);
			}
			else
			{
				vec.x = 60f * ((rgb.Red - rgb.Green) / num3 + 4f);
			}
			vec.x %= 360f;
			if (vec.x < 0f)
			{
				vec.x += 360f;
			}
			if (MathF.Abs(num2) < 0.0001f)
			{
				vec.y = 0f;
			}
			else
			{
				vec.y = num3 / num2;
			}
			return vec;
		}

		public static Vec3 GammaCorrectRGB(float gamma, Vec3 rgb)
		{
			float num = 1f / gamma;
			rgb.x = MathF.Pow(rgb.x, num);
			rgb.y = MathF.Pow(rgb.y, num);
			rgb.z = MathF.Pow(rgb.z, num);
			return rgb;
		}

		public static Vec3 GetClosestPointInLineSegmentToPoint(Vec3 point, Vec3 lineSegmentBegin, Vec3 lineSegmentEnd)
		{
			Vec3 vec = lineSegmentEnd - lineSegmentBegin;
			if (!vec.IsNonZero)
			{
				return lineSegmentBegin;
			}
			float num = Vec3.DotProduct(point - lineSegmentBegin, vec) / Vec3.DotProduct(vec, vec);
			if (num < 0f)
			{
				return lineSegmentBegin;
			}
			if (num > 1f)
			{
				return lineSegmentEnd;
			}
			return lineSegmentBegin + vec * num;
		}

		public static bool GetRayPlaneIntersectionPoint(in Vec3 planeNormal, in Vec3 planeCenter, in Vec3 rayOrigin, in Vec3 rayDirection, out float t)
		{
			float num = Vec3.DotProduct(planeNormal, rayDirection);
			if (num > 1E-06f)
			{
				Vec3 vec = planeCenter - rayOrigin;
				t = Vec3.DotProduct(vec, planeNormal) / num;
				return t >= 0f;
			}
			t = -1f;
			return false;
		}

		public static Vec2 GetClosestPointInLineSegmentToPoint(Vec2 point, Vec2 lineSegmentBegin, Vec2 lineSegmentEnd)
		{
			Vec2 vec = lineSegmentEnd - lineSegmentBegin;
			if (!vec.IsNonZero())
			{
				return lineSegmentBegin;
			}
			float num = Vec2.DotProduct(point - lineSegmentBegin, vec) / Vec2.DotProduct(vec, vec);
			if (num < 0f)
			{
				return lineSegmentBegin;
			}
			if (num > 1f)
			{
				return lineSegmentEnd;
			}
			return lineSegmentBegin + vec * num;
		}

		public static bool CheckLineToLineSegmentIntersection(Vec2 lineOrigin, Vec2 lineDirection, Vec2 segmentA, Vec2 segmentB, out float t, out Vec2 intersect)
		{
			t = float.MaxValue;
			intersect = Vec2.Zero;
			Vec2 vec = lineOrigin - segmentA;
			Vec2 vec2 = segmentB - segmentA;
			Vec2 vec3 = new Vec2(-lineDirection.y, lineDirection.x);
			float num = vec2.DotProduct(vec3);
			if (MathF.Abs(num) < 1E-05f)
			{
				return false;
			}
			float num2 = vec2.x * vec.y - vec2.y * vec.x;
			t = num2 / num;
			intersect = lineOrigin + lineDirection * t;
			float num3 = vec.DotProduct(vec3) / num;
			return num3 >= 0f && num3 <= 1f;
		}

		public static float GetClosestPointOnLineSegment(Vec2 point, Vec2 segmentA, Vec2 segmentB, out Vec2 closest)
		{
			Vec2 vec = point - segmentA;
			Vec2 vec2 = segmentB - segmentA;
			float num = vec.DotProduct(vec2) / Math.Max(vec2.LengthSquared, 1E-05f);
			if (num < 0f)
			{
				closest = segmentA;
			}
			else if (num > 1f)
			{
				closest = segmentB;
			}
			else
			{
				closest = segmentA + vec2 * num;
			}
			return point.Distance(closest);
		}

		public static bool IntersectRayWithBoundaryList(Vec2 rayOrigin, Vec2 rayDir, List<Vec2> boundaries, out Vec2 intersectionPoint)
		{
			List<ValueTuple<float, Vec2>> list = new List<ValueTuple<float, Vec2>>();
			for (int j = 0; j < boundaries.Count; j++)
			{
				Vec2 vec = boundaries[j];
				Vec2 vec2 = boundaries[(j + 1) % boundaries.Count];
				float num;
				Vec2 vec3;
				if (MBMath.CheckLineToLineSegmentIntersection(rayOrigin, rayDir, vec, vec2, out num, out vec3) && num > 0f)
				{
					list.Add(new ValueTuple<float, Vec2>(num, vec3));
				}
			}
			list = list.OrderBy((ValueTuple<float, Vec2> i) => i.Item1).ToList<ValueTuple<float, Vec2>>();
			if (list.Count != 0)
			{
				intersectionPoint = list[0].Item2;
				return true;
			}
			intersectionPoint = rayOrigin;
			return false;
		}

		public static string ToOrdinal(int number)
		{
			if (number < 0)
			{
				return number.ToString();
			}
			long num = (long)(number % 100);
			if (num >= 11L && num <= 13L)
			{
				return number + "th";
			}
			switch (number % 10)
			{
			case 1:
				return number + "st";
			case 2:
				return number + "nd";
			case 3:
				return number + "rd";
			default:
				return number + "th";
			}
		}

		public static int IndexOfMax<T>(MBReadOnlyList<T> array, Func<T, int> func)
		{
			int num = int.MinValue;
			int num2 = -1;
			for (int i = 0; i < array.Count; i++)
			{
				int num3 = func(array[i]);
				if (num3 > num)
				{
					num = num3;
					num2 = i;
				}
			}
			return num2;
		}

		public static T MaxElement<T>(IEnumerable<T> collection, Func<T, float> func)
		{
			float num = float.MinValue;
			T t = default(T);
			foreach (T t2 in collection)
			{
				float num2 = func(t2);
				if (num2 > num)
				{
					num = num2;
					t = t2;
				}
			}
			return t;
		}

		public static ValueTuple<T, T> MaxElements2<T>(IEnumerable<T> collection, Func<T, float> func)
		{
			float num = float.MinValue;
			float num2 = float.MinValue;
			T t = default(T);
			T t2 = default(T);
			foreach (T t3 in collection)
			{
				float num3 = func(t3);
				if (num3 > num2)
				{
					if (num3 > num)
					{
						num2 = num;
						t2 = t;
						num = num3;
						t = t3;
					}
					else
					{
						num2 = num3;
						t2 = t3;
					}
				}
			}
			return new ValueTuple<T, T>(t, t2);
		}

		public static ValueTuple<T, T, T> MaxElements3<T>(IEnumerable<T> collection, Func<T, float> func)
		{
			float num = float.MinValue;
			float num2 = float.MinValue;
			float num3 = float.MinValue;
			T t = default(T);
			T t2 = default(T);
			T t3 = default(T);
			foreach (T t4 in collection)
			{
				float num4 = func(t4);
				if (num4 > num3)
				{
					if (num4 > num2)
					{
						num3 = num2;
						t3 = t2;
						if (num4 > num)
						{
							num2 = num;
							t2 = t;
							num = num4;
							t = t4;
						}
						else
						{
							num2 = num4;
							t2 = t4;
						}
					}
					else
					{
						num3 = num4;
						t3 = t4;
					}
				}
			}
			return new ValueTuple<T, T, T>(t, t2, t3);
		}

		public static ValueTuple<T, T, T, T> MaxElements4<T>(IEnumerable<T> collection, Func<T, float> func)
		{
			float num = float.MinValue;
			float num2 = float.MinValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			T t = default(T);
			T t2 = default(T);
			T t3 = default(T);
			T t4 = default(T);
			foreach (T t5 in collection)
			{
				float num5 = func(t5);
				if (num5 > num4)
				{
					if (num5 > num3)
					{
						num4 = num3;
						t4 = t3;
						if (num5 > num2)
						{
							num3 = num2;
							t3 = t2;
							if (num5 > num)
							{
								num2 = num;
								t2 = t;
								num = num5;
								t = t5;
							}
							else
							{
								num2 = num5;
								t2 = t5;
							}
						}
						else
						{
							num3 = num5;
							t3 = t5;
						}
					}
					else
					{
						num4 = num5;
						t4 = t5;
					}
				}
			}
			return new ValueTuple<T, T, T, T>(t, t2, t3, t4);
		}

		public static ValueTuple<T, T, T, T, T> MaxElements5<T>(IEnumerable<T> collection, Func<T, float> func)
		{
			float num = float.MinValue;
			float num2 = float.MinValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			float num5 = float.MinValue;
			T t = default(T);
			T t2 = default(T);
			T t3 = default(T);
			T t4 = default(T);
			T t5 = default(T);
			foreach (T t6 in collection)
			{
				float num6 = func(t6);
				if (num6 > num5)
				{
					if (num6 > num4)
					{
						num5 = num4;
						t5 = t4;
						if (num6 > num3)
						{
							num4 = num3;
							t4 = t3;
							if (num6 > num2)
							{
								num3 = num2;
								t3 = t2;
								if (num6 > num)
								{
									num2 = num;
									t2 = t;
									num = num6;
									t = t6;
								}
								else
								{
									num2 = num6;
									t2 = t6;
								}
							}
							else
							{
								num3 = num6;
								t3 = t6;
							}
						}
						else
						{
							num4 = num6;
							t4 = t6;
						}
					}
					else
					{
						num5 = num6;
						t5 = t6;
					}
				}
			}
			return new ValueTuple<T, T, T, T, T>(t, t2, t3, t4, t5);
		}

		public static IList<T> TopologySort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
		{
			List<T> list = new List<T>();
			Dictionary<T, bool> dictionary = new Dictionary<T, bool>();
			foreach (T t in source)
			{
				MBMath.Visit<T>(t, getDependencies, list, dictionary);
			}
			return list;
		}

		private static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
		{
			bool flag;
			if (visited.TryGetValue(item, out flag))
			{
				return;
			}
			visited[item] = true;
			IEnumerable<T> enumerable = getDependencies(item);
			if (enumerable != null)
			{
				foreach (T t in enumerable)
				{
					MBMath.Visit<T>(t, getDependencies, sorted, visited);
				}
			}
			visited[item] = false;
			sorted.Add(item);
		}

		public const float TwoPI = 6.2831855f;

		public const float PI = 3.1415927f;

		public const float HalfPI = 1.5707964f;

		public const float E = 2.7182817f;

		public const float DegreesToRadians = 0.017453292f;

		public const float RadiansToDegrees = 57.295776f;

		public const float Epsilon = 1E-05f;
	}
}
