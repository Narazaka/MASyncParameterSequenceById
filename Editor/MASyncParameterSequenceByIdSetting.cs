using UnityEditor;

namespace Narazaka.VRChat.MASyncParameterSequenceById.Editor
{
    [FilePath("Assets/SyncedParamsSetting.asset", FilePathAttribute.Location.ProjectFolder)]
    public class MASyncParameterSequenceByIdSetting : ScriptableSingleton<MASyncParameterSequenceByIdSetting>
    {
        public string BaseDirectory = "Assets/SyncedParams";
        public string NameFormat = "%name% [%blueprint_id%] SyncedParams";
        public string PrefabNameMatchPattern = "";
        public string PrefabNameReplaceValue = "";

        public void Save()
        {
            Save(true);
        }

        public string GetPreferredAssetName(string prefabName, string blueprintId)
        {
            var name = string.IsNullOrEmpty(PrefabNameMatchPattern) ? prefabName : System.Text.RegularExpressions.Regex.Replace(prefabName, PrefabNameMatchPattern, PrefabNameReplaceValue);
            return NameFormat.Replace("%name%", name).Replace("%blueprint_id%", blueprintId);
        }
    }
}
