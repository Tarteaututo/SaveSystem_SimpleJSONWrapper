namespace SaveSystem
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using System.IO;
	using SimpleJSON;

	public static class SaveSystem_JsonUtility
	{
		public static bool ToJson_JsonUtility(object obj, string filename = "")
		{
			if (obj == null)
			{
				Debug.LogWarningFormat("SaveSystem : Trying to write an object to json but the object is empty.");
				return false;
			}

			string json = JsonUtility.ToJson(obj, true);

			if (json == string.Empty)
			{
				Debug.LogWarningFormat("SaveSystem : Trying to write {0} to json but the result of ToJson is empty.", obj.GetType().Name);
				return false;
			}

			if (string.IsNullOrEmpty(filename) == true)
			{
				filename = SaveSystemHelper.SAVE_FILENAME;
			}

			string path = SaveSystemHelper.FormatSavegameDirectoryPath();
			if (Directory.Exists(path) == false)
			{
				if (Directory.CreateDirectory(path) == null)
				{
					Debug.LogWarningFormat("SaveSystem : unable to create directory at path {0}.", path);
					return false;
				}
			}

			path = SaveSystemHelper.FormatFilePath(filename);
			FileStream file = File.Create(path);

			file.Dispose();
			File.WriteAllText(path, json);

			return File.Exists(path);
		}

		public static bool FromJson_JsonUtility<T>(string jsonPath, out T result)
		{
			if (jsonPath == string.Empty)
			{
				Debug.LogWarningFormat("SaveSystem : Trying to write a string to json but the string is empty.");
				result = default;
				return false;
			}

			string json = File.ReadAllText(jsonPath);
			if (json == string.Empty)
			{
				Debug.LogWarningFormat("SaveSystem : Trying to read a file at path {0} to json but the resulted string is empty.", jsonPath);
				result = default;
				return false;
			}

			result = (T)JsonUtility.FromJson(json, typeof(T));
			return result != null;
		}
	}

	/// <summary>
	/// TODO: JSONArray wrapper to simplify calls
	/// </summary>
	public static class SaveSystem_SimpleJSON
	{
		public static bool Save(ISavable target, string filename = "")
		{
			JSONObject model = target.ToSave();
			if (string.IsNullOrEmpty(filename) == true)
			{
				filename = SaveSystemHelper.SAVE_FILENAME;
			}

			string path = SaveSystemHelper.FormatSavegameDirectoryPath();
			if (Directory.Exists(path) == false)
			{
				if (Directory.CreateDirectory(path) == null)
				{
					Debug.LogWarningFormat("SaveSystem : unable to create directory at path {0}.", path);
					return false;
				}
			}

			path = SaveSystemHelper.FormatFilePath(filename);
			File.WriteAllText(path, model.ToString());
			return File.Exists(path);
		}

		public static bool Load(ISavable target, string filename = "")
		{
			if (string.IsNullOrEmpty(filename) == true)
			{
				filename = SaveSystemHelper.SAVE_FILENAME;
			}

			string path = SaveSystemHelper.FormatFilePath(filename);
			if (File.Exists(path) == false)
			{
				Debug.LogErrorFormat("SaveSystem : fail to load file at path {0}. File doesn't exist.", path);
				return false;
			}

			string saveContent = File.ReadAllText(path);
			JSONObject jsonSave = JSON.Parse(saveContent) as JSONObject;
			if (jsonSave == null)
			{
				Debug.LogErrorFormat("SaveSystem : fail to parse result from {0}.", path);
				return false;
			}

			target.FromSave(jsonSave);
			return true;
		}
	}

	public interface ISavable
	{
		JSONObject ToSave();
		void FromSave(JSONObject jsonSave);
	}

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
	}

	// object[] doesn't serialized by JsonUtility
	[System.Serializable]
	public class SaveModel
	{
		[SerializeField] private object[] _model = null;

		public SaveModel(object[] model)
		{
			_model = model;
		}
	}
}
