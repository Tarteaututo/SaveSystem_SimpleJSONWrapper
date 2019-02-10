namespace SaveSystem
{
	using SimpleJSON;

	public interface ISavable
	{
		JSONObject ToSave();
		void FromSave(JSONNode jsonSave);
	}

	/// <summary>
	/// Purpose : enforce the common use of registerable version with standard array
	/// </summary>
	/// 
	public interface ISavableRegistrable
	{
		JSONObject ToSave();
		void FromSave(JSONNode jsonSave);
		bool IsDirty();
		void ResetDirty();
		string GetIdentifier();
	}
}
