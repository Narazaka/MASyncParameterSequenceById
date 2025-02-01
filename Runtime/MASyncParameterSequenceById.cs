using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDKBase;

namespace Narazaka.VRChat.MASyncParameterSequenceById
{
    [AddComponentMenu("MASyncParameterSequenceById")]
    public class MASyncParameterSequenceById : MonoBehaviour, IEditorOnly
    {
        public ModularAvatarSyncParameterSequence.Platform PrimaryPlatform = ModularAvatarSyncParameterSequence.Platform.Android;
    }
}
