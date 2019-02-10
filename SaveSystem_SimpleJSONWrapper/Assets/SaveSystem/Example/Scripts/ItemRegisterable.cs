namespace SaveSystem.Example
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using global::SaveSystem;
	using global::SaveSystem.SimpleJSON;
	using TMPro;
	using UnityEngine.UI;

	public class ItemRegisterable : MonoBehaviour, ISavableRegistrable
	{
		[SerializeField] private TextMeshProUGUI _valueText = null;
		[SerializeField] private Button _addButton = null;
		[SerializeField] private Button _removeButton = null;

		private int _value = 0;
		private int _uniqueIndex = -1;
		private bool _isDirty = false;
		public int Value
		{
			get
			{
				return _value;
			}
			set
			{
				if (_value != value)
				{
					_value = value;
					UpdateText();
				}
			}
		}

		private void Start()
		{
			//SaveSystem.Register(this);
		}

		private void OnDestroy()
		{
			SaveSystem.Unregister(this);
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

		#region ISavable
		public JSONObject ToSave()
		{
			JSONObject jsonObject = new JSONObject();
			jsonObject.Add("value", Value);
			return jsonObject;
		}

		public bool IsDirty()
		{
			return _isDirty;
		}

		public void ResetDirty()
		{
			_isDirty = false;
		}

		public string GetIdentifier()
		{
			return string.Format("{0} {1}", GetType().Name, _uniqueIndex);
		}

		public void FromSave(JSONNode jsonSave)
		{
			Value = jsonSave["value"].AsInt;
		}
		#endregion ISavable

		#endregion Public

		#region Private

		private void Add()
		{
			Value++;
			_isDirty = true;
		}

		private void Remove()
		{
			Value--;
			_isDirty = true;
		}

		private void UpdateText()
		{
			_valueText.text = string.Format("{0} - {1}", _uniqueIndex, Value.ToString());
		}
		#endregion Private


	}
}