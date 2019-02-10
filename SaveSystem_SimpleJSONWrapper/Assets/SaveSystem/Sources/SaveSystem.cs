namespace SaveSystem
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using System.IO;
	using SimpleJSON;

	#region First version : JsonUtility
	/// <summary>
	/// JSONUtility est la fonctionnalité de Unity permettant de transformer les données public ou serializé d'une class en données
	/// répondant au standard JSON.
	/// 
	/// Elle ne permet que de sauvegarder toutes les données public ou serializée d'une class en une seule fois et de réécrire ces
	/// données toujours en une seule fois.
	/// 
	/// Elle gère un nombre limité de types (pas de liste) et ne permet pas d'explorer le json pour pick une données précises.
	/// </summary>
	public static class SaveSystem_JsonUtility
	{
		public static bool Save(object obj, string filename = "")
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

		public static bool Load<T>(string jsonPath, out T result)
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
	#endregion First version : JsonUtility

	#region Second version SimpleJSON
	/// <summary>
	/// 
	/// Notes : SimpleJSON est un framework open source qui permet d'écrire et lire des objets répondant aux standards du JSON.
	/// Au contraire de JSONUility, il permet entre autre d'explorer un JSON pour récupérer des données précises.
	///		
	/// </summary>
	public static class SaveSystem_SimpleJSON
	{
		///
		/// Cette version est la manière la plus directe de créer des objets.
		/// On fournit à Save un JSONObject qui contient les données à écrire et il s'occupe du reste.
		///
		#region Save direct object
		public static bool Save(JSONObject jsonObject, string filename = "")
		{
			if (string.IsNullOrEmpty(filename) == true)
			{
				filename = SaveSystemHelper.SAVE_FILENAME;
			}
			string path = SaveSystemHelper.FormatFilePath(filename);
			File.WriteAllText(path, jsonObject.ToString());
			return File.Exists(path);
		}

		public static bool Load(out JSONObject jsonObject, string filename = "")
		{
			if (string.IsNullOrEmpty(filename) == true)
			{
				filename = SaveSystemHelper.SAVE_FILENAME;
			}
			string path = SaveSystemHelper.FormatFilePath(filename);
			string jsonFile = File.ReadAllText(path);
			jsonObject = JSON.Parse(jsonFile) as JSONObject;
			return jsonObject != null;
		}
		#endregion Save direct object

		///
		/// Cette version utilise l'interface ISavable pour communiquer directement avec les objets qui demandent à être sauvegarder.
		/// De cette manière, on peut automatiser et chainer les appels à FromSave() et ToSave() à partir de plusieurs class qui héritent de ISavable,
		/// ajouter ces données à un unique JSONObject et écrire le fichier en une seule fois sans risquer l'écrasement.
		/// En optimisation, il peut être intéressant de merge le fichier de sauvegarde déjà existant avec les nouvelles données.
		/// Avec le merge, on peut également flagger les classes qui ont été modifiées pour n'ajouter que les valeurs modifiées et réduire le nombre d'opérations.
		///
		#region ISavable
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

		#endregion ISavable
	}

	/// <summary>
	/// Afin d'augmenter les fonctionnalités de la sauvegarde, le save system a besoin de communiquer avec une variété d'objet qu'il ne connait pas à l'avance.
	/// C'est là l'utilité de l'interface : chaque objet qui doit sauvegarder quelques chose en hérite et le save system peut le reconnaitre.
	/// Celui-ci peut alors demander aux objets de construire des données à sauvegarder (via ToSave()) ou lui transmettre des données à charger (via FromSave(JSONNode jsonSave)).
	/// </summary>
	public interface ISavable
	{
		JSONObject ToSave();
		void FromSave(JSONNode jsonSave);
	}

	/// <summary>
	/// Helper : contient des fonctionnalités pour faciliter l'utilisation de SaveSystem. Pas nécessaire à son fonctionnement.
	/// </summary>
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
	}
	#endregion Second version SimpleJSON

	#region Example of pattern that doesn't work with JSONUtility
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
	#endregion Example of pattern that doesn't work with JSONUtility
}
