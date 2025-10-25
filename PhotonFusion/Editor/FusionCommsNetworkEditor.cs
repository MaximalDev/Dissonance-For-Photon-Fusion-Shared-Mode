#if UNITY_EDITOR
using Dissonance.Editor;
using UnityEditor;

namespace Dissonance.Integrations.PhotonFusion
{
    [CustomEditor(typeof(FusionCommsNetwork))]
    public class FusionCommsNetworkEditor
        : BaseDissonnanceCommsNetworkEditor<
            FusionCommsNetwork,
            FusionServer,
            FusionClient,
            FusionPeer,
            Unit,
            Unit
        >
    {
    }
}
#endif