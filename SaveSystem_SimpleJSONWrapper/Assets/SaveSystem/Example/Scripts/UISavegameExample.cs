namespace SaveSystem.Example
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using SaveSystem;
	using SaveSystem.SimpleJSON;

	public class UISavegameExample : MonoBehaviour, ISavable
	{
		#region Fields
		#region Serialized
		[SerializeField] private Button _saveButton = null;
		[SerializeField] private Button _loadButton = null;

		// alternative version
		[SerializeField] private Button _altSaveButton = null;
		[SerializeField] private Button _altLoadButton = null;

		[SerializeField] private Transform _itemParent = null;
		#endregion Serialized

		#region Private
		private List<Item> _items = new List<Item>();
		private string _savegameFilename = "UISavegameExample";
		#endregion Private

		#endregion Fields

		#region Methods
		#region MonoBehaviour
		private void Awake()
		{
			int itemIndex = 0;
			foreach (Transform child in _itemParent)
			{
				Item item = child.GetComponent<Item>();
				if (item != null)
				{
					_items.Add(item);
					item.Initialize(itemIndex++);
				}
			}
		}

		private void OnEnable()
		{
			_saveButton.onClick.AddListener(SaveButton);
			_loadButton.onClick.AddListener(LoadButton);

			_altSaveButton.onClick.AddListener(AlternativeSaveButton);
			_altLoadButton.onClick.AddListener(AlternativeLoadButton);
		}

		private void OnDisable()
		{
			_saveButton.onClick.RemoveListener(SaveButton);
			_loadButton.onClick.RemoveListener(LoadButton);

			_altSaveButton.onClick.RemoveListener(AlternativeSaveButton);
			_altLoadButton.onClick.RemoveListener(AlternativeLoadButton);
		}
		#endregion MonoBehaviour

		#region ISavable

		public JSONObject ToSave()
		{
			JSONObject jsonObject = new JSONObject();
			JSONArray jsonArray = new JSONArray();
			for (int i = 0, length = _items.Count; i < length; i++)
			{
				jsonArray.Add(i.ToString(), _items[i].ToSave());
			}
			jsonObject.Add(GetType().Name, jsonArray);
			return jsonObject;
		}

		public void FromSave(JSONNode jsonSave)
		{
			JSONArray jsonArray = jsonSave[GetType().Name].AsArray;
			for (int i = 0, length = _items.Count; i < length; i++)
			{
				_items[i].FromSave(jsonArray[i]);
			}
		}

		private void SaveButton()
		{
			bool result = true;
			for (int i = 0, length = _items.Count; i < length; i++)
			{
				result |= SaveSystem_SimpleJSON.Save(this, _savegameFilename);
			}
			Debug.Log(result == true ? "Save success" : "Save fail");
		}

		private void LoadButton()
		{
			bool result = true;
			for (int i = 0, length = _items.Count; i < length; i++)
			{
				result |= SaveSystem_SimpleJSON.Load(this, _savegameFilename);
			}
			Debug.Log(result == true ? "Load success" : "Load fail");
		}

		#endregion ISavable

		#region Alternative
		private void AlternativeSaveButton()
		{
			JSONObject jsonObject = new JSONObject();
			JSONArray itemArray = new JSONArray();
			for (int i = 0, length = _items.Count; i < length; i++)
			{
				JSONObject content = new JSONObject();
				content.Add(FormatItemKey(i), _items[i].Value);
				itemArray.Add(content);
			}

			jsonObject.Add("UISavegameExample", itemArray);
			SaveSystem_SimpleJSON.Save(jsonObject, "Savegame - UISavegameExample");
		}

		private void AlternativeLoadButton()
		{
			JSONObject jsonObject;
			bool result = SaveSystem_SimpleJSON.Load(out jsonObject, "Savegame - UISavegameExample");
			if (result == false)
			{
				Debug.LogError("Fail to read Savegame - UISavegameExample file");
				return;
			}

			JSONArray jsonArray = jsonObject["UISavegameExample"].AsArray;
			if (jsonArray == null)
			{
				Debug.LogError("Fail to parse UISavegameExample array");
				return;
			}

			for (int i = 0, length = jsonArray.Count; i < length; i++)
			{
				_items[i].Value = jsonArray[i][FormatItemKey(i)].AsInt;
			}
		}

		#endregion Alternative

		#region Utils
		private string FormatItemKey(int index)
		{
			return string.Format("Item {0}", index.ToString());
		}
		#endregion Utils
		#endregion Methods
	}
}
