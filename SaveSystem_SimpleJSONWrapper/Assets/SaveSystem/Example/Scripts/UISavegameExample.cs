﻿namespace SaveSystem.Example
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
		}

		private void OnDisable()
		{
			_saveButton.onClick.RemoveListener(SaveButton);
			_loadButton.onClick.RemoveListener(LoadButton);
		}
		#endregion MonoBehaviour

		#region Public
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
		#endregion Public

		#region Private
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

		private string FormatItemKey(int index)
		{
			return string.Format("Item {0}", index.ToString());
		}
		#endregion Private
		
		#endregion Methods
	}
}
