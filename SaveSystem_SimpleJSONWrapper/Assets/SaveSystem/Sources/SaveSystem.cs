namespace SaveSystem
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using System.IO;
	using SimpleJSON;

	/// <summary>
	/// Register version :
	/// 
	///		Pour chaque ISavableRegistrable
	///			se register à l'init
	///			classé par filename
	///			Set dirty if changed

	///		save filename :
	///			regarde isdirty
	///			load la save
	///			merge les changements
	///			save
	///		
	///		load filename :
	///			load la save
	///			regarde is dirty
	///			ecrase if dirty
	///			
	/// 
	///		Fonctionnalité manquante :
	///			Merge de json
	/// </summary>

	public static class SaveSystem
	{
		#region Fields
		public static Dictionary<string, List<ISavableRegistrable>> _savables = null;
		#endregion Fields

		#region Methods
		#region Public
		#region Save registered ISavable

		public static bool Register(ISavableRegistrable savable, string filename = "")
		{
			if (string.IsNullOrEmpty(filename) == true)
			{
				filename = SaveSystemHelper.SAVE_FILENAME;
			}
			if (_savables == null)
			{
				_savables = new Dictionary<string, List<ISavableRegistrable>>();
			}
			if (_savables.ContainsKey(filename) == false)
			{
				_savables.Add(filename, new List<ISavableRegistrable>());
			}
			if (_savables[filename].Contains(savable) == true)
			{
				return false;
			}
			_savables[filename].Add(savable);
			return true;
		}

		public static void Unregister(ISavableRegistrable savable, string filename = "")
		{
			if (string.IsNullOrEmpty(filename) == true)
			{
				filename = SaveSystemHelper.SAVE_FILENAME;
			}
			if (_savables == null || _savables.ContainsKey(filename) == false)
			{
				return;
			}
			_savables[filename].Remove(savable);
		}

		public static bool Save(string filename = "")
		{
			if (string.IsNullOrEmpty(filename) == true)
			{
				filename = SaveSystemHelper.SAVE_FILENAME;
			}

			if (TryCreateDirectoryIfDoesntExist() == false)
			{
				return false;
			}
			string path = SaveSystemHelper.FormatFilePath(filename);

			if (_savables.ContainsKey(filename) == false)
			{
				Debug.LogErrorFormat("SaveSystem : no savable has been registered to its filename.", path);
				return false;
			}

			List<ISavableRegistrable> savables = new List<ISavableRegistrable>(_savables[filename]);
			savables = savables.FindAll(item => item.IsDirty() == true);
			
			JSONObject jsonObject = new JSONObject();

			for (int i = 0, length = savables.Count; i < length; i++)
			{
				jsonObject.Add(savables[i].GetIdentifier(), savables[i].ToSave());
			}

			return WriteFile(path, jsonObject.ToString());
		}

		public static bool Load(string filename = "")
		{
			JSONObject jsonObject;
			bool result = LoadFromFile(filename, out jsonObject, false);
			if (result == true)
			{
				//

			}
			return result;
		}

		#endregion #region 

		#region Save ISavable on demand

		public static bool Save(ISavable target, string filename = "")
		{
			if (string.IsNullOrEmpty(filename) == true)
			{
				filename = SaveSystemHelper.SAVE_FILENAME;
			}

			if (TryCreateDirectoryIfDoesntExist() == false)
			{
				return false;
			}

			string path = SaveSystemHelper.FormatFilePath(filename);

			// Write
			return WriteFile(path, target.ToSave().ToString());
		}

		public static bool Load(ISavable target, string filename = "")
		{
			JSONObject jsonObject;
			bool result = LoadFromFile(filename, out jsonObject, false);
			if (result == true)
			{
				target.FromSave(jsonObject);
			}
			return result;
		}
		#endregion Save ISavable on demand

		#endregion Public
		#region Private

		private static bool LoadFromFile(string filename, out JSONObject jsonSave, bool silentMode = false)
		{
			if (string.IsNullOrEmpty(filename) == true)
			{
				filename = SaveSystemHelper.SAVE_FILENAME;
			}

			string path = GetPathFromFilename(filename, silentMode);
			if (path == string.Empty)
			{
				jsonSave = null;
				return false;
			}

			string saveContent = File.ReadAllText(path);
			jsonSave = JSON.Parse(saveContent) as JSONObject;
			if (jsonSave == null)
			{
				if (silentMode == false)
				{
					Debug.LogErrorFormat("SaveSystem : fail to parse result from {0}.", path);
				}
				return false;
			}

			return true;
		}

		private static bool TryCreateDirectoryIfDoesntExist()
		{
			string path = SaveSystemHelper.FormatSavegameDirectoryPath();
			if (Directory.Exists(path) == false)
			{
				if (Directory.CreateDirectory(path) == null)
				{
					Debug.LogWarningFormat("SaveSystem : unable to create directory at path {0}.", path);
					path = string.Empty;
					return false;
				}
			}
			return true;
		}

		private static bool WriteFile(string path, string content)
		{
			File.WriteAllText(path, content);
			return File.Exists(path);
		}

		private static string GetPathFromFilename(string filename, bool silentMode = false)
		{
			string path = SaveSystemHelper.FormatFilePath(filename);
			if (File.Exists(path) == false)
			{
				if (silentMode == false)
				{
					Debug.LogErrorFormat("SaveSystem : fail to load file at path {0}. File doesn't exist.", path);
				}
				path = string.Empty;
			}
			return path;
		}
		#endregion Private

		#endregion Methods
	}

	public static class JSONNodeExtension
	{
		#region Built in Array
		public static void AddArray<T>(this JSONObject jsonObject, string key, T[] array)
		{
			JSONArray jsonArray = new JSONArray();
			for (int i = 0, length = array.Length; i < length; i++)
			{
				// Consider using .ToString() before
				string value = (string)System.Convert.ChangeType(array[i], typeof(string));
				jsonArray.Add(value);
			}
			jsonObject.Add(key, jsonArray);
		}

		/// <summary>
		/// Notes :
		///		Activator.CreateInstance need a parameterless ctor. Some built in types doesn't have one (all ?).
		///		https://stackoverflow.com/questions/13636274/how-to-create-new-default-instances-of-objects-that-are-stored-in-object-variabl
		///		
		///		Does not read class array.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="jsonArray"></param>
		/// <returns></returns>

		public static T[] ReadArray<T>(this JSONArray jsonArray)
		{
			int arrayLength = jsonArray.Count;

			Type arrayType = typeof(T);
			T[] arrayBuffer = default;

			Array.Resize<T>(ref arrayBuffer, arrayLength);

			for (int i = 0; i < arrayLength; i++)
			{
				arrayBuffer[i] = (T)Convert.ChangeType(jsonArray[i].Value, typeof(T));
			}

			return arrayBuffer;
		}
		#endregion Built in Array

		#region List
		public static void AddList<T>(this JSONObject jsonObject, string key, List<T> list)
		{
			jsonObject.AddArray<T>(key, list.ToArray());

		}
		#endregion List
	}
}
