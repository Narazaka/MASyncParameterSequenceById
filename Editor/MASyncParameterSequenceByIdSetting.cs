using UnityEditor;

namespace Narazaka.VRChat.MASyncParameterSequenceById.Editor
{
    [FilePath("Assets/SyncedParamsSetting.asset", FilePathAttribute.Location.ProjectFolder)]
    public class MASyncParameterSequenceByIdSetting : ScriptableSingleton<MASyncParameterSequenceByIdSetting>
    {
        public string BaseDirectory = "Assets/SyncedParams";

        public void Save()
        {
            Save(true);
        }
    }
}
