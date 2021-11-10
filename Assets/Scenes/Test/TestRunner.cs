using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Wormhole;

[ExecuteInEditMode]
public class TestRunner : MonoBehaviour
{
    private SimplePortalSpace _portals;

    private Transform _portalA;
    private Transform _portalB;
    private Camera _portalCamera;

    private MeshFilter _quadDst;
    private MeshFilter _quadSrc;

    public Camera mainCamera;
    public GameObject portalPrefab;
    public RenderTexture receiverTexture;

    public float clippingPlaneOffset = 0.001f;

    private void Awake()
    {
        GeneratePortals();

        RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;
    }

    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= RenderPipelineManager_beginCameraRendering;
    }

    private void RenderPipelineManager_beginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (!EnsureConrfigured())
        {
            return;
        }

        _portals.UpdateCameras(camera);

        _portalCamera.targetTexture = receiverTexture;
        //UniversalRenderPipeline.RenderSingleCamera(context, _portalCamera);
    }

    private bool EnsureConrfigured()
    {
        return mainCamera && receiverTexture && _portals != null;
    }

    private void GeneratePortals()
    {
        _portals?.Clear();

        if (!portalPrefab)
        {
            return;
        }
                
        _portals = new SimplePortalSpace(transform, portalPrefab);

        var p0 = _portals.AddPortal(null,  Vector3.forward, new Vector3(0, 0, -2.41f), null);
        var p1 = _portals.AddPortal(null, -Vector3.forward, new Vector3(0, 0,  1.50f), null);

        _portals.LinkPortals(p0, p1);

        _portalA = _portals[p0].transform;
        //_quadDst = _portalA.GetComponentInChildren<MeshFilter>();
        //_quadDst.mesh = Instantiate(_quadDst.sharedMesh);

        OverrideMaterial(_portalA.gameObject, "Assets/Scenes/Test/PortalATex.mat");

        _portalB = _portals[p1].transform;
        //_quadSrc = _portalB.GetComponentInChildren<MeshFilter>();
        //_quadSrc.mesh = Instantiate(_quadSrc.sharedMesh);

        OverrideMaterial(_portalB.gameObject, "Assets/Scenes/Test/PortalBChecker.mat");

        _portalCamera = _portalA.GetComponentInChildren<Camera>(true);
        _portalCamera.enabled = false;
        _portalCamera.gameObject.SetActive(true);

        void OverrideMaterial(GameObject go, string material)
        {
            var r = go.GetComponentInChildren<Renderer>();
            r.material = AssetDatabase.LoadAssetAtPath<Material>(material);
        }
    }

    private void OnGUI()
    {
        using (new EditorGUILayout.VerticalScope())
        {
            var lastEnabled = GUI.enabled;
            GUI.enabled = portalPrefab;

            try
            {
                if (GUILayout.Button("Re-generate Portals"))
                {
                    GeneratePortals();
                }
            }
            finally
            {
                GUI.enabled = lastEnabled;
            }
        }
    }
}
