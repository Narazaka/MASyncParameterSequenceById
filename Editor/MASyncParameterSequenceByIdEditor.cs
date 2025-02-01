using UnityEditor;

namespace Narazaka.VRChat.MASyncParameterSequenceById.Editor
{
    [CustomEditor(typeof(MASyncParameterSequenceById))]
    class MASyncParameterSequenceByIdEditor : UnityEditor.Editor
    {
        MASyncParameterSequenceByIdSetting Setting;
        SerializedObject SettingObject;
        SerializedProperty BaseDirectory;

        void OnEnable()
        {
            Setting = MASyncParameterSequenceByIdSetting.instance;
            SettingObject = new SerializedObject(Setting);
            BaseDirectory = SettingObject.FindProperty(nameof(MASyncParameterSequenceByIdSetting.BaseDirectory));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.HelpBox("Make Synced Params asset when 'Build & Publish'", MessageType.Info);

            SettingObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(BaseDirectory);
            if (SettingObject.ApplyModifiedProperties())
            {
                Setting.Save();
            }
        }
    }
}
