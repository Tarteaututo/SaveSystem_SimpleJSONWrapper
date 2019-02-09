namespace SaveSystem
{
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine;
	using Example;

	namespace Tests
	{
		public class SaveSystem_SimpleJSON_Test
		{
			[Test]
			public void SaveSystem_SimpleJSON_NewDefaultModel()
			{
				ModelExampleSavable model = new ModelExampleSavable();
				string filename = string.Format("{0} - SaveSystem_SimpleJSON_NewDefaultModel", SaveSystemHelper.SAVE_FILENAME);

				bool result = SaveSystem_SimpleJSON.Save(model, filename);
				string savegamePath = SaveSystemHelper.FormatFilePath(filename);

				Assert.IsTrue(result);
				Assert.IsTrue(File.Exists(savegamePath));

				// Load test
				model = null;
				model = new ModelExampleSavable();
				ModelExampleSavable comparer = new ModelExampleSavable();

				result = SaveSystem_SimpleJSON.Load(model, filename);
				Assert.IsTrue(result);

				Assert.IsTrue(model.myIntValue == comparer.myIntValue);
				Assert.IsTrue(model.myFloatValue == comparer.myFloatValue);
				Assert.IsTrue(model.myDoubleValue == comparer.myDoubleValue);
				Assert.IsTrue(model.myStringValue == comparer.myStringValue);

				Assert.IsTrue(model.myStringArray.Length == 2);
				Assert.IsTrue(model.myStringArray[0] == comparer.myStringArray[0]);
				Assert.IsTrue(model.myStringArray[1] == comparer.myStringArray[1]);

				Assert.IsTrue(model.myIntArray.Length == 2);
				Assert.IsTrue(model.myIntArray[0] == comparer.myIntArray[0]);
				Assert.IsTrue(model.myIntArray[1] == comparer.myIntArray[1]);

				Assert.IsTrue(model.myStringList.Count == 2);
				Assert.IsTrue(model.myStringList[0] == comparer.myStringList[0]);
				Assert.IsTrue(model.myStringList[1] == comparer.myStringList[1]);

				Assert.IsTrue(model.myStringDict.Count == 2);
				Assert.IsNotNull(model.myStringDict["Key0"]);
				Assert.IsTrue(model.myStringDict["Key0"] == comparer.myStringDict["Key0"]);
				Assert.IsNotNull(model.myStringDict["Key1"]);
				Assert.IsTrue(model.myStringDict["Key1"] == comparer.myStringDict["Key1"]);

				Assert.IsTrue(model.myInputBinding.Count == 2);
				Assert.IsNotNull(model.myInputBinding[KeyCode.W]);
				Assert.IsTrue(model.myInputBinding[KeyCode.W] == comparer.myInputBinding[KeyCode.W]);
				Assert.IsNotNull(model.myInputBinding[KeyCode.D]);
				Assert.IsTrue(model.myInputBinding[KeyCode.D] == comparer.myInputBinding[KeyCode.D]);

				Assert.IsNotNull(model.exampleClass);
				Assert.IsTrue(model.exampleClass.Equals(comparer.exampleClass));
			}

			[Test]
			public void SaveSystem_SimpleJSON_NewModifiedModel()
			{
				ModelExampleSavable model = new ModelExampleSavable()
				{
					myIntValue = 3,
					myFloatValue = 2.6f,
					myDoubleValue = 3.2d,
					myStringValue = "another string value",
					myStringArray = new string[2] { "string array value 0", "string array value 1" },
					myIntArray = new int[2] { 4, 2 },
					myStringList = new List<string> { "string list value 0", "string list value 1" },
					myStringDict = new Dictionary<string, string>
					{
						{ "ModifiedKey0", "ModifiedValue0" },
						{ "ModifiedKey1", "ModifiedValue1" },
					},
					myInputBinding = new Dictionary<KeyCode, string>
					{
						{ KeyCode.Z, "ModifiedForward"},
						{ KeyCode.F, "ModifiedRight"},
					},
					exampleClass = new ModelExampleSavable.ExampleClass()
					{
						name = "Modified name",
						index = 42
					}
				};

				string filename = string.Format("{0} - SaveSystem_SimpleJSON_NewModifiedModel", SaveSystemHelper.SAVE_FILENAME);

				bool result = SaveSystem_SimpleJSON.Save(model, filename);
				string savegamePath = SaveSystemHelper.FormatFilePath(filename);

				Assert.IsTrue(result);
				Assert.IsTrue(File.Exists(savegamePath));

				model = new ModelExampleSavable();
				ModelExampleSavable wrongComparer = new ModelExampleSavable();

				result = SaveSystem_SimpleJSON.Load(model, filename);
				Assert.IsTrue(result);
				Assert.IsTrue(model.myIntValue == 3);
				Assert.IsTrue(model.myFloatValue == 2.6f);
				Assert.IsTrue(model.myDoubleValue == 3.2d);

				Assert.IsTrue(model.myStringValue == "another string value");
				Assert.IsTrue(model.myStringValue.Equals("another string value"));
				Assert.IsTrue(model.myStringValue.CompareTo("another string value") == 0);
				Assert.IsFalse(model.myStringValue == wrongComparer.myStringValue);

				Assert.IsTrue(model.myStringArray.Length == 2);
				Assert.IsTrue(string.Compare(model.myStringArray[0], "string array value 0") == 0, model.myStringArray[0]);
				Assert.IsTrue(model.myStringArray[0].Equals("string array value 0"), model.myStringArray[0]);
				Assert.IsTrue(model.myStringArray[0].CompareTo("string array value 0") == 0, model.myStringArray[0]);
				Assert.IsTrue(model.myStringArray[1] == "string array value 1", model.myStringArray[1]);
				Assert.IsFalse(model.myStringArray[1] == wrongComparer.myStringArray[1]);

				Assert.IsNotNull(model.myIntArray);
				Assert.IsTrue(model.myIntArray.Length == 2);
				Assert.IsTrue(model.myIntArray[0] == 4);
				Assert.IsTrue(model.myIntArray[1] == 2);
				Assert.IsFalse(model.myIntArray[1] == wrongComparer.myIntArray[1]);

				Assert.IsTrue(model.myStringList.Count == 2);
				Assert.IsTrue(model.myStringList[0] == "string list value 0");
				Assert.IsTrue(model.myStringList[1] == "string list value 1");
				Assert.IsFalse(model.myStringList[1] == wrongComparer.myStringList[1]);

				Assert.IsTrue(model.myStringDict.Count == 2);
				Assert.IsNotNull(model.myStringDict["ModifiedKey0"]);
				Assert.IsTrue(model.myStringDict["ModifiedKey0"] == "ModifiedValue0");
				Assert.IsNotNull(model.myStringDict["ModifiedKey1"]);
				Assert.IsTrue(model.myStringDict["ModifiedKey1"] == "ModifiedValue1");

				Assert.IsTrue(model.myInputBinding.Count == 2);
				Assert.IsNotNull(model.myInputBinding[KeyCode.Z]);
				Assert.IsTrue(model.myInputBinding[KeyCode.Z] == "ModifiedForward");
				Assert.IsNotNull(model.myInputBinding[KeyCode.F]);
				Assert.IsTrue(model.myInputBinding[KeyCode.F] == "ModifiedRight");

				Assert.IsNotNull(model.exampleClass);
				Assert.IsTrue(model.exampleClass.name == "Modified name");
				Assert.IsTrue(model.exampleClass.index == 42);
				Assert.IsFalse(model.exampleClass.Equals(wrongComparer));
			}
		}
	}
}
