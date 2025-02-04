using UnityEditor;

namespace Narazaka.VRChat.MASyncParameterSequenceById.Editor
{
    [CustomEditor(typeof(MASyncParameterSequenceById))]
    class MASyncParameterSequenceByIdEditor : UnityEditor.Editor
    {
        MASyncParameterSequenceByIdSetting Setting;
        SerializedObject SettingObject;
        SerializedProperty BaseDirectory;
        SerializedProperty NameFormat;
        SerializedProperty PrefabNameMatchPattern;
        SerializedProperty PrefabNameReplaceValue;

        void OnEnable()
        {
            Setting = MASyncParameterSequenceByIdSetting.instance;
            SettingObject = new SerializedObject(Setting);
            BaseDirectory = SettingObject.FindProperty(nameof(MASyncParameterSequenceByIdSetting.BaseDirectory));
            NameFormat = SettingObject.FindProperty(nameof(MASyncParameterSequenceByIdSetting.NameFormat));
            PrefabNameMatchPattern = SettingObject.FindProperty(nameof(MASyncParameterSequenceByIdSetting.PrefabNameMatchPattern));
            PrefabNameReplaceValue = SettingObject.FindProperty(nameof(MASyncParameterSequenceByIdSetting.PrefabNameReplaceValue));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.HelpBox("Make Synced Params asset when 'Build & Publish'", MessageType.Info);

            SettingObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(BaseDirectory);
            EditorGUILayout.PropertyField(NameFormat);
            EditorGUILayout.PropertyField(PrefabNameMatchPattern);
            EditorGUILayout.PropertyField(PrefabNameReplaceValue);
            if (SettingObject.ApplyModifiedProperties())
            {
                Setting.Save();
            }
        }
    }
}
