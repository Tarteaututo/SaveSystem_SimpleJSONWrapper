namespace SaveSystem.Example
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using SaveSystem;
	using SaveSystem.SimpleJSON;
	using TMPro;
	using UnityEngine.UI;
	public class Item : MonoBehaviour, ISavable
	{
		[SerializeField] private TextMeshProUGUI _valueText = null;
		[SerializeField] private Button _addButton = null;
		[SerializeField] private Button _removeButton = null;

		private int _value = 0;
		private int _uniqueIndex = -1;

		public int Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				UpdateText();
			}
		}

		private void OnEnable()
		{
			_addButton.onClick.AddListener(Add);
			_removeButton.onClick.AddListener(Remove);
		}

		private void OnDisable()
		{
			_addButton.onClick.RemoveListener(Add);
			_removeButton.onClick.RemoveListener(Remove);
		}

		#region Public
		public void Initialize(int index)
		{
			_uniqueIndex = index;
			UpdateText();
		}

		public JSONObject ToSave()
		{
			JSONObject jsonObject = new JSONObject();
			jsonObject.Add("Item " + _uniqueIndex, Value);
			return jsonObject;
		}

		public void FromSave(JSONNode jsonSave)
		{
			string key = "Item " + _uniqueIndex;
			Value = jsonSave[key].AsInt;
		}
		#endregion Public

		#region Private

		private void Add()
		{
			Value++;
		}

		private void Remove()
		{
			Value--;
		}

		private void UpdateText()
		{
			_valueText.text = string.Format("{0} - {1}", _uniqueIndex, Value.ToString());
		}
		#endregion Private

		
	}
}
