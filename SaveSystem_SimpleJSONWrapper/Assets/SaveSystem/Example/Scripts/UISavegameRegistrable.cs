namespace SaveSystem.Example
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class UISavegameRegistrable : MonoBehaviour
	{
		#region Fields
		#region Serialized
		[SerializeField] private Button _saveButton = null;
		[SerializeField] private Button _loadButton = null;

		[SerializeField] private Transform _itemParent = null;
		#endregion Serialized

		#region Private
		private List<ItemRegisterable> _items = new List<ItemRegisterable>();
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
				ItemRegisterable item = child.GetComponent<ItemRegisterable>();
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

		#region ISavable

		private void SaveButton()
		{
			bool result = SaveSystem.Save();
			Debug.Log(result == true ? "Save success" : "Save fail");
		}

		private void LoadButton()
		{
			bool result = SaveSystem.Load();
			Debug.Log(result == true ? "Load success" : "Load fail");
		}

		#endregion ISavable

		#endregion Methods
	}
}