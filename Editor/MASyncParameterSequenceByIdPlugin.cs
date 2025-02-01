using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.Core;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase.Editor;

[assembly: ExportsPlugin(typeof(Narazaka.VRChat.MASyncParameterSequenceById.Editor.MASyncParameterSequenceByIdPlugin))]

namespace Narazaka.VRChat.MASyncParameterSequenceById.Editor
{
    class MASyncParameterSequenceByIdPlugin : Plugin<MASyncParameterSequenceByIdPlugin>
    {
        public override string DisplayName => nameof(MASyncParameterSequenceById);
        public override string QualifiedName => "net.narazaka.vrchat.ma-sync-parameter-sequence-by-id";

        protected override void Configure()
        {
            InPhase(BuildPhase.Resolving).Run($"{DisplayName} Add Component", ctx =>
            {
                var component = FetchComponent(ctx);
                if (component == null) return;

                var pipeline = ctx.AvatarDescriptor.GetComponent<PipelineManager>();
                if (pipeline == null)
                {
                    ErrorReport.ReportError(new Error("PipelineManager not found in the avatar descriptor."));
                    return;
                }
                if (string.IsNullOrWhiteSpace(pipeline.blueprintId))
                {
                    Undo.RecordObject(pipeline, "Assigning a new ID");
                    pipeline.AssignId();
                }
                var id = pipeline.blueprintId;

                var setting = MASyncParameterSequenceByIdSetting.instance;
                var directory = setting.BaseDirectory;
                var preferredAssetName = $"{IgnoreSuffix(ctx.AvatarRootObject.name)} [{id}] SyncedParams";

                var assetPaths = Directory.Exists(setting.BaseDirectory) ? Directory.EnumerateFiles(setting.BaseDirectory, $"*{id}*", SearchOption.AllDirectories).ToArray() : new string[0];
                if (assetPaths.Length > 1)
                {
                    foreach (var path in assetPaths.Skip(1))
                    {
                        AssetDatabase.DeleteAsset(path);
                    }
                }
                var assetPath = assetPaths.FirstOrDefault();
                if (assetPath == null)
                {
                    var asset = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                    asset.name = preferredAssetName;
                    assetPath = $"{directory}/{asset.name}.asset";
                    Directory.CreateDirectory(directory);
                    AssetDatabase.CreateAsset(asset, assetPath);
                    Debug.Log($"[{DisplayName}] Created new asset at \"{assetPath}\"");
                }
                if (CurrentPlatform == component.PrimaryPlatform)
                {
                    if (Path.GetFileNameWithoutExtension(assetPath) != preferredAssetName)
                    {
                        var newPath = $"{Path.GetDirectoryName(assetPath)}/{preferredAssetName}.asset";
                        AssetDatabase.MoveAsset(assetPath, newPath);
                        Debug.Log($"[{DisplayName}] Renamed asset \"{assetPath}\" to \"{newPath}\"");
                        assetPath = newPath;
                    }
                }
                var parameters = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(assetPath);

                var sync = ctx.AvatarRootObject.AddComponent<ModularAvatarSyncParameterSequence>();
                sync.PrimaryPlatform = component.PrimaryPlatform;
                sync.Parameters = parameters;

                ctx.GetState<State>().AssetPath = assetPath;
            });
            InPhase(BuildPhase.Transforming).AfterPlugin("nadena.dev.modular-avatar").Run($"{DisplayName} Clean", ctx =>
            {
                var component = FetchComponent(ctx);
                if (component == null) return;

                var state = ctx.GetState<State>();
                if (state == null) return;
                if (state.AssetPath == null) return;
                var assetPath = state.AssetPath;
                var parameters = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(assetPath);
                if (parameters == null) return;
                if (parameters.parameters.Length == 0)
                {
                    AssetDatabase.DeleteAsset(assetPath);
                    Debug.Log($"[{DisplayName}] Deleted asset \"{assetPath}\" because Empty");
                }

                Object.DestroyImmediate(component);
            });
        }

        MASyncParameterSequenceById FetchComponent(BuildContext ctx)
        {
            if (VRC_SdkBuilder.ActiveBuildType == VRC_SdkBuilder.BuildType.Test)
            {
                return null;
            }
            var components = ctx.AvatarRootTransform.GetComponentsInChildren<MASyncParameterSequenceById>(true);
            if (components.Length == 0) return null;
            if (components.Length > 1)
            {
                ErrorReport.ReportError(new Error("Multiple MASyncParameterSequenceById components found in the avatar. Only one is allowed."));
                return null;
            }
            var component = components[0];
            return component;
        }

        static Regex IgnoreSuffixRe = new Regex(@"\s*\(Clone\)$");

        static string IgnoreSuffix(string name) => IgnoreSuffixRe.Replace(name, "");

        private static ModularAvatarSyncParameterSequence.Platform? CurrentPlatform
        {
            get
            {
                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case BuildTarget.Android: return ModularAvatarSyncParameterSequence.Platform.Android;
                    case BuildTarget.iOS: return ModularAvatarSyncParameterSequence.Platform.iOS;
                    case BuildTarget.StandaloneWindows64: return ModularAvatarSyncParameterSequence.Platform.PC;
                    case BuildTarget.StandaloneLinux64: return ModularAvatarSyncParameterSequence.Platform.PC; // for CI
                    default: return null;
                }
            }
        }

        class State
        {
            public string AssetPath;
        }

        class Error : IError
        {
            public string Message { get; }

            public Error(string message)
            {
                Message = message;
            }

            public ErrorSeverity Severity => ErrorSeverity.NonFatal;

            public void AddReference(ObjectReference obj)
            {
                // ?
            }

            public VisualElement CreateVisualElement(ErrorReport report)
            {
                var root = new VisualElement();
                var label = new Label(ToMessage());
                label.style.color = new StyleColor(Color.red);
                root.Add(label);
                return root;
            }

            public string ToMessage() => Message;
        }
    }
}