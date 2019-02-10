namespace SaveSystem
{
	using UnityEngine;
	using SimpleJSON;

	public static class SaveSystemHelper
	{
		public const string SAVE_DIRECTORYNAME = "Savegame";
		public const string SAVE_FILENAME = "savegame";
		public const string SAVE_EXTENSION = ".json";

		public static string FormatSavegameDirectoryPath()
		{
			return string.Format("{0}/{1}", Application.persistentDataPath, SAVE_DIRECTORYNAME);
		}

		public static string FormatFilePath(string fileName)
		{
			return string.Format("{0}/{1}{2}", FormatSavegameDirectoryPath(), fileName, SAVE_EXTENSION);
		}

		public static T[] ReadArray<T>(JSONNode jsonObject)
		{
			if (jsonObject.IsArray == false)
			{
				Debug.LogWarning("Failed attempt to read a jsonObject that is not an array.");
				return null;
			}
			return jsonObject.AsArray.ReadArray<T>();
		}
	}
}
