namespace SaveSystem
{
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine;
	using Example;
	using SimpleJSON;

	/// <summary>
	/// float works badly with json : https://stackoverflow.com/questions/40423708/unity-floating-point-precision-when-using-json-strings
	/// double should work but tests says they have the same imprecision
	/// </summary>

	namespace Tests
	{
		public class SaveSystem_JsonUtility_Test
		{
			[Test]
			public void SaveSystem_JsonUtility_NewDefaultModel()
			{
				ModelExample model = new ModelExample();
				string filename = string.Format("{0} - SaveSystem_JsonUtility_NewDefaultModel", SaveSystemHelper.SAVE_FILENAME);
				bool result = SaveSystem_JsonUtility.Save(model, filename);

				string savegamePath = SaveSystemHelper.FormatFilePath(filename);

				Assert.IsTrue(result);
				Assert.IsTrue(File.Exists(savegamePath));
			}

			[Test]
			public void SaveSystem_JsonUtility_NewModifiedModel()
			{
				ModelExample model = new ModelExample();
				string filename = string.Format("{0} - SaveSystem_JsonUtility_NewModifiedModel", SaveSystemHelper.SAVE_FILENAME);

				model.myFloatValue = 2.9f;
				model.myDoubleValue = 2.9F;
				model.myIntValue = 5;
				model.myStringValue = "NewModifiedModel";
				model.myObjectArray = new object[3]
				{
					2.9f,
					2.9F,
					"NewModifiedModel"
				};

				model.myObjectList = new List<object>
				{
					2.9f,
					2.9F,
					"NewModifiedModel"
				};
				bool result = SaveSystem_JsonUtility.Save(model, filename);

				string savegamePath = SaveSystemHelper.FormatFilePath(filename);

				Assert.IsTrue(result);
				Assert.IsTrue(File.Exists(savegamePath));
			}

		}
	}
}
