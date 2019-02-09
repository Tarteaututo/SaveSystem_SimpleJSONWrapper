namespace SaveSystem.Example
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using SimpleJSON;
	using SaveSystem;
	using System;

	[System.Serializable]
	public class ModelExample
	{
		public int myIntValue = 1;
		public float myFloatValue = 1.5f;
		public double myDoubleValue = 2.5d;
		public string myStringValue = "this is a string";
		public string[] myStringArray = new string[2] { "first item", "second item" };

		// Exluded
		[System.NonSerialized] public string myStringNotSerialized = "Not serialized";

		// Exluded
		public object[] myObjectArray = null;

		// Exluded
		public List<object> myObjectList = null;

		[SerializeField] private int myIntValueSerialized = 1;
		[SerializeField] private float myFloatValueSerialized = 1.5f;
		[SerializeField] private float myDoubleValueSerialized = 2.5f;
		[SerializeField] private string myStringValueSerialized = "this is a serialized string";
		[SerializeField] public string[] myStringArraySerialized = new string[2] { "first item", "second item" };
	}

	[System.Serializable]
	public class ModelExampleSavable : ISavable
	{
		public class ExampleClass : ISavable
		{
			public string name = "";
			public int index = 0;

			public JSONObject ToSave()
			{
				JSONObject jsonObject = new JSONObject();
				jsonObject.Add(Consts_Save.nameKey, name);
				jsonObject.Add(Consts_Save.indexKey, index);
				return jsonObject;
			}

			public void FromSave(JSONNode jsonSave)
			{
				name = jsonSave[Consts_Save.nameKey].Value;
				index = jsonSave[Consts_Save.indexKey].AsInt;
			}

			public override bool Equals(object obj)
			{
				ExampleClass comparer = obj as ExampleClass;
				return
					comparer != null &&
					string.Compare(name, comparer.name) == 0 &&
					index == comparer.index;
			}

			public override int GetHashCode()
			{
				var hashCode = 515207553;
				hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
				hashCode = hashCode * -1521134295 + index.GetHashCode();
				return hashCode;
			}
		}

		public int myIntValue = 1;
		public float myFloatValue = 1.5f;
		public double myDoubleValue = 2.5d;
		public string myStringValue = "this is a string";
		public string[] myStringArray = new string[2] { "first array item", "second array item" };
		public int[] myIntArray = new int[2] { 8, 6 };
		public GameObject[] myGameObjectArray = new GameObject[] { };
		public List<string> myStringList = new List<string> { "first list item", "second list item" };
		public Dictionary<string, string> myStringDict = new Dictionary<string, string>
		{
			{ "Key0", "Forward"},
			{ "Key1", "right" }
		};
		public Dictionary<KeyCode, string> myInputBinding = new Dictionary<KeyCode, string>
		{
			{ KeyCode.W, "Forward"},
			{ KeyCode.D, "right" }
		};

		public ExampleClass exampleClass = new ExampleClass();

		public JSONObject ToSave()
		{
			JSONObject jsonObject = new JSONObject();
			jsonObject.Add(Consts_Save.myIntValueKey, myIntValue);
			jsonObject.Add(Consts_Save.myFloatValueKey, myFloatValue);
			jsonObject.Add(Consts_Save.myDoubleValueKey, myDoubleValue);
			jsonObject.Add(Consts_Save.myStringValueKey, myStringValue);

			jsonObject.AddArray(Consts_Save.myStringArrayKey, myStringArray);
			jsonObject.AddArray(Consts_Save.myIntArrayKey, myIntArray);

			//JSONArray stringArray = new JSONArray();
			//for (int i = 0, length = myStringArray.Length; i < length; i++)
			//{
			//	stringArray.Add(myStringArray[i]);
			//}
			//jsonObject.Add(Consts_Save.myStringArrayKey, stringArray);

			JSONArray stringArray = null;
			stringArray = new JSONArray();
			for (int i = 0, length = myStringList.Count; i < length; i++)
			{
				stringArray.Add(myStringList[i]);
			}
			jsonObject.Add(Consts_Save.myStringListKey, stringArray);

			stringArray = null;
			stringArray = new JSONArray();
			foreach (KeyValuePair<string, string> item in myStringDict)
			{
				JSONArray nestedArrayBuffer = new JSONArray();
				nestedArrayBuffer.Add(item.Key.ToString());
				nestedArrayBuffer.Add(item.Value);
				stringArray.Add(nestedArrayBuffer);
			}
			jsonObject.Add(Consts_Save.myStringDictKey, stringArray);

			stringArray = null;
			stringArray = new JSONArray();
			foreach (KeyValuePair<KeyCode, string> item in myInputBinding)
			{
				JSONArray nestedArrayBuffer = new JSONArray();
				nestedArrayBuffer.Add(item.Key.ToString());
				nestedArrayBuffer.Add(item.Value);
				stringArray.Add(nestedArrayBuffer);
			}
			jsonObject.Add(Consts_Save.myInputBindingKey, stringArray);

			jsonObject.Add(Consts_Save.exampleClassKey, exampleClass.ToSave());
			return jsonObject;
		}

		public void FromSave(JSONNode jsonSave)
		{
			myIntValue = jsonSave[Consts_Save.myIntValueKey].AsInt;
			myFloatValue = jsonSave[Consts_Save.myFloatValueKey].AsFloat;
			myDoubleValue = jsonSave[Consts_Save.myDoubleValueKey].AsDouble;
			myStringValue = jsonSave[Consts_Save.myStringValueKey].Value;

			myStringArray = SaveSystemHelper.ReadArray<string>(jsonSave[Consts_Save.myStringArrayKey]);
			myIntArray = SaveSystemHelper.ReadArray<int>(jsonSave[Consts_Save.myIntArrayKey]);

			//JSONArray jsonArray = jsonSave[Consts_Save.myStringArrayKey].AsArray;
			//myStringArray = new string[jsonArray.Count];
			//for (int i = 0, length = jsonArray.Count; i < length; i++)
			//{
			//	myStringArray[i] = jsonArray[i].Value;
			//}

			JSONArray jsonArray = jsonSave[Consts_Save.myStringListKey].AsArray;
			myStringList = new List<string>();
			for (int i = 0, length = jsonArray.Count; i < length; i++)
			{
				myStringList.Add(jsonArray[i].Value);
			}

			jsonArray = jsonSave[Consts_Save.myStringDictKey].AsArray;
			myStringDict = null;
			myStringDict = new Dictionary<string, string>();
			for (int i = 0, length = jsonArray.Count; i < length; i++)
			{
				JSONArray nestedArrayBuffer = jsonArray[i].AsArray;
				myStringDict.Add(nestedArrayBuffer[0], nestedArrayBuffer[1]);
			}

			jsonArray = jsonSave[Consts_Save.myInputBindingKey].AsArray;
			myInputBinding = null;
			myInputBinding = new Dictionary<KeyCode, string>();
			for (int i = 0, length = jsonArray.Count; i < length; i++)
			{
				JSONArray nestedArrayBuffer = jsonArray[i].AsArray;
				KeyCode key;
				if (System.Enum.TryParse<KeyCode>(nestedArrayBuffer[0], true, out key) == true)
				{
					myInputBinding.Add(key, nestedArrayBuffer[1]);
				}
			}

			exampleClass.FromSave(jsonSave[Consts_Save.exampleClassKey]);
		}
	}

	public static class Consts_Save
	{
		public const string myIntValueKey = "myIntValue";
		public const string myFloatValueKey = "myFloatValue";
		public const string myDoubleValueKey = "myDoubleValue";
		public const string myStringValueKey = "myStringValue";
		public const string myStringArrayKey = "myStringArray";
		public const string myIntArrayKey = "myIntArray";
		public const string myGameObjectArrayKey = "myGameObjectArray";
		public const string myStringListKey = "myStringList";
		public const string myStringDictKey = "myStringDict";
		public const string myInputBindingKey = "myInputBinding";

		public const string exampleClassKey = "exampleClass";
		public const string nameKey = "name";
		public const string indexKey = "index";
	}
}