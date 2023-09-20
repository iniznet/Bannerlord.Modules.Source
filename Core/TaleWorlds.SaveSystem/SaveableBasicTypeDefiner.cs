using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem
{
	public class SaveableBasicTypeDefiner : SaveableTypeDefiner
	{
		public SaveableBasicTypeDefiner()
			: base(30000)
		{
		}

		protected internal override void DefineBasicTypes()
		{
			base.AddBasicTypeDefinition(typeof(int), 1, new IntBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(uint), 2, new UintBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(short), 3, new ShortBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(ushort), 4, new UshortBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(byte), 5, new ByteBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(sbyte), 6, new SbyteBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(float), 7, new FloatBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(double), 8, new DoubleBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(long), 9, new LongBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(ulong), 10, new UlongBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(Vec2), 11, new Vec2BasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(Vec2i), 12, new Vec2iBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(Vec3), 13, new Vec3BasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(Vec3i), 14, new Vec3iBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(Mat2), 15, new Mat2BasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(Mat3), 16, new Mat3BasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(MatrixFrame), 17, new MatrixFrameBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(Quaternion), 18, new QuaternionBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(Color), 19, new ColorBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(bool), 20, new BoolBasicTypeSerializer());
			base.AddBasicTypeDefinition(typeof(string), 21, new StringSerializer());
		}

		protected internal override void DefineClassTypes()
		{
			base.AddClassDefinition(typeof(object), 0, null);
			base.AddClassDefinitionWithCustomFields(typeof(Tuple<, >), 100, new Tuple<string, short>[]
			{
				new Tuple<string, short>("m_Item1", 1),
				new Tuple<string, short>("m_Item2", 2)
			}, null);
			base.AddClassDefinitionWithCustomFields(typeof(PriorityQueue<, >), 103, new Tuple<string, short>[]
			{
				new Tuple<string, short>("_baseHeap", 1)
			}, null);
			base.AddClassDefinitionWithCustomFields(typeof(MBReadOnlyDictionary<, >), 105, new Tuple<string, short>[]
			{
				new Tuple<string, short>("_dictionary", 1)
			}, null);
			base.AddClassDefinition(typeof(GenericComparer<>), 106, null);
		}

		protected internal override void DefineStructTypes()
		{
			base.AddStructDefinitionWithCustomFields(typeof(Nullable<>), 101, new Tuple<string, short>[]
			{
				new Tuple<string, short>("value", 1)
			}, null);
			base.AddStructDefinitionWithCustomFields(typeof(KeyValuePair<, >), 102, new Tuple<string, short>[]
			{
				new Tuple<string, short>("key", 1),
				new Tuple<string, short>("value", 2)
			}, null);
			base.AddStructDefinitionWithCustomFields(typeof(ValueTuple<, >), 107, new Tuple<string, short>[]
			{
				new Tuple<string, short>("Item1", 1),
				new Tuple<string, short>("Item2", 2)
			}, null);
		}

		protected internal override void DefineGenericStructDefinitions()
		{
			base.ConstructGenericStructDefinition(typeof(KeyValuePair<string, string>));
			base.ConstructGenericStructDefinition(typeof(KeyValuePair<string, int>));
			base.ConstructGenericStructDefinition(typeof(KeyValuePair<int, string>));
			base.ConstructGenericStructDefinition(typeof(KeyValuePair<int, int>));
			base.ConstructGenericStructDefinition(typeof(ValueTuple<int, int>));
			base.ConstructGenericStructDefinition(typeof(ValueTuple<string, int>));
		}

		protected internal override void DefineGenericClassDefinitions()
		{
			base.ConstructGenericClassDefinition(typeof(Tuple<string, int>));
			base.ConstructGenericClassDefinition(typeof(Tuple<bool, float>));
			base.ConstructGenericClassDefinition(typeof(GenericComparer<int>));
			base.ConstructGenericClassDefinition(typeof(GenericComparer<float>));
		}

		protected internal override void DefineContainerDefinitions()
		{
			base.ConstructContainerDefinition(typeof(List<int>));
			base.ConstructContainerDefinition(typeof(List<uint>));
			base.ConstructContainerDefinition(typeof(List<short>));
			base.ConstructContainerDefinition(typeof(List<ushort>));
			base.ConstructContainerDefinition(typeof(List<byte>));
			base.ConstructContainerDefinition(typeof(List<sbyte>));
			base.ConstructContainerDefinition(typeof(List<float>));
			base.ConstructContainerDefinition(typeof(List<double>));
			base.ConstructContainerDefinition(typeof(List<long>));
			base.ConstructContainerDefinition(typeof(List<ulong>));
			base.ConstructContainerDefinition(typeof(List<Vec2>));
			base.ConstructContainerDefinition(typeof(List<Vec2i>));
			base.ConstructContainerDefinition(typeof(List<Vec3>));
			base.ConstructContainerDefinition(typeof(List<Vec3i>));
			base.ConstructContainerDefinition(typeof(List<Mat2>));
			base.ConstructContainerDefinition(typeof(List<Mat3>));
			base.ConstructContainerDefinition(typeof(List<MatrixFrame>));
			base.ConstructContainerDefinition(typeof(List<Quaternion>));
			base.ConstructContainerDefinition(typeof(List<Color>));
			base.ConstructContainerDefinition(typeof(List<bool>));
			base.ConstructContainerDefinition(typeof(List<string>));
			base.ConstructContainerDefinition(typeof(List<KeyValuePair<string, string>>));
			base.ConstructContainerDefinition(typeof(List<KeyValuePair<string, int>>));
			base.ConstructContainerDefinition(typeof(List<KeyValuePair<int, string>>));
			base.ConstructContainerDefinition(typeof(List<KeyValuePair<int, int>>));
			base.ConstructContainerDefinition(typeof(List<Tuple<bool, float>>));
			base.ConstructContainerDefinition(typeof(Queue<int>));
			base.ConstructContainerDefinition(typeof(Queue<uint>));
			base.ConstructContainerDefinition(typeof(Queue<short>));
			base.ConstructContainerDefinition(typeof(Queue<ushort>));
			base.ConstructContainerDefinition(typeof(Queue<byte>));
			base.ConstructContainerDefinition(typeof(Queue<sbyte>));
			base.ConstructContainerDefinition(typeof(Queue<float>));
			base.ConstructContainerDefinition(typeof(Queue<double>));
			base.ConstructContainerDefinition(typeof(Queue<long>));
			base.ConstructContainerDefinition(typeof(Queue<ulong>));
			base.ConstructContainerDefinition(typeof(Queue<Vec2>));
			base.ConstructContainerDefinition(typeof(Queue<Vec2i>));
			base.ConstructContainerDefinition(typeof(Queue<Vec3>));
			base.ConstructContainerDefinition(typeof(Queue<Vec3i>));
			base.ConstructContainerDefinition(typeof(Queue<Mat2>));
			base.ConstructContainerDefinition(typeof(Queue<Mat3>));
			base.ConstructContainerDefinition(typeof(Queue<MatrixFrame>));
			base.ConstructContainerDefinition(typeof(Queue<Quaternion>));
			base.ConstructContainerDefinition(typeof(Queue<Color>));
			base.ConstructContainerDefinition(typeof(Queue<bool>));
			base.ConstructContainerDefinition(typeof(Queue<string>));
			base.ConstructContainerDefinition(typeof(int[]));
			base.ConstructContainerDefinition(typeof(uint[]));
			base.ConstructContainerDefinition(typeof(short[]));
			base.ConstructContainerDefinition(typeof(ushort[]));
			base.ConstructContainerDefinition(typeof(byte[]));
			base.ConstructContainerDefinition(typeof(sbyte[]));
			base.ConstructContainerDefinition(typeof(float[]));
			base.ConstructContainerDefinition(typeof(double[]));
			base.ConstructContainerDefinition(typeof(long[]));
			base.ConstructContainerDefinition(typeof(ulong[]));
			base.ConstructContainerDefinition(typeof(Vec2[]));
			base.ConstructContainerDefinition(typeof(Vec2i[]));
			base.ConstructContainerDefinition(typeof(Vec3[]));
			base.ConstructContainerDefinition(typeof(Vec3i[]));
			base.ConstructContainerDefinition(typeof(Mat2[]));
			base.ConstructContainerDefinition(typeof(Mat3[]));
			base.ConstructContainerDefinition(typeof(MatrixFrame[]));
			base.ConstructContainerDefinition(typeof(Quaternion[]));
			base.ConstructContainerDefinition(typeof(Color[]));
			base.ConstructContainerDefinition(typeof(bool[]));
			base.ConstructContainerDefinition(typeof(string[]));
			base.ConstructContainerDefinition(typeof(Dictionary<int, string>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, int>));
			base.ConstructContainerDefinition(typeof(Dictionary<int, int>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, string>));
			base.ConstructContainerDefinition(typeof(Dictionary<long, int>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, object>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, float>));
		}
	}
}
