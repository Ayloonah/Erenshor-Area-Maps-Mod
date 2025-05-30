using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Erenshor_Area_Maps_Mod
{
    public class MapCamera : MonoBehaviour
    {
        public static MapCamera Instance {get; private set;}
        private Camera mapCam;
        private RenderTexture mapRT;
        private float zoomLvl = 500f;
        private GameObject mapCamObj;
        private List<(GameObject obj, int originalLayer)> movedFoliage = new List<(GameObject, int)>();
        public RenderTexture GetRenderTexture() => mapRT;

        // Ensuring camera object is created: 
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoCreate()
        {
            if (Instance == null)
            {
                var obj = new GameObject("MapCameraHolder");
                Instance = obj.AddComponent<MapCamera>();
                DontDestroyOnLoad(obj);
            }
        }

        // Singleton pattern to ensure only one instance of MapCamera exists:
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            mapCamObj = new GameObject("MapCamera");
            mapCamObj.SetActive(false);
            CreateMapCamera();
            HideFoliage();
            Debug.LogError("Awake: MapCamera singleton initialized.");
        }


        private void CreateMapCamera()
        {
            // Creating and setting up the map camera:
            mapCam = mapCamObj.AddComponent<Camera>();

            // Map position and rotation:
            mapCamObj.transform.position = new Vector3(200f, 350f, 400f);
            mapCam.orthographic = true;
            mapCam.orthographicSize = zoomLvl;
            mapCamObj.transform.rotation = Quaternion.Euler(90f, 91.3131f, 0f);

            // Other map settings:
            mapCam.clearFlags = CameraClearFlags.SolidColor;
            mapCam.aspect = 1.7778f;
            mapCam.farClipPlane = 1000f;

            // Displaying all layers except the NoCollision layer: 
            mapCam.cullingMask = ~(1 << LayerMask.NameToLayer("NoCollision"));

            // Setting up the camera's render texture:
            mapRT = new RenderTexture(1024, 1024, 16)
            {
                name = "MapCameraRenderTexture"
            };
            mapCam.targetTexture = mapRT;

            Debug.LogError($"CreateMapCamera: Map camera was created.");
        }

        // Capturing the mapCam's view and freezing it for display:
        public IEnumerator CaptureAndFreeze()
        {
            // Rendering the camera's texture for 2 frames:
            yield return null;
            yield return null;

            // Freezing the camera by disabling it:
            gameObject.SetActive(false);
            Debug.LogError($"CaptureAndFreeze: Map camera was frozen.");

            // Moving foliage back into the correct layer:
            RestoreFoliage();
        }

        // Enabling the camera: 
        public void EnableCamera()
        {
            mapCamObj.SetActive(true);
            Debug.LogError($"EnableCamera: Camera was enabled.");
            StartCoroutine(CaptureAndFreeze());
            Debug.LogError($"EnableCamera: Coroutine is done.");
        }

        // Adding foliage to the NoCollision layer:
        private void HideFoliage()
        {
            movedFoliage.Clear();
            foreach (var obj in GameObject.FindObjectsOfType<GameObject>())
            {
                if (obj.name.ToLower().Contains("tree") || obj.name.ToLower().Contains("bush"))
                {
                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        // Storing the foliage obj in an array to be able to reapply them:
                        movedFoliage.Add((obj, obj.layer));

                        // Moving the foliage to the NoCollision layer:
                        obj.layer = LayerMask.NameToLayer("NoCollision");
                    }
                    Debug.LogError($"HideFoliage: Foliage moved to the No Colision layer.");
                }
            }
        }

        // Restoring foliage to its original layer: 
        public void RestoreFoliage()
        {
            foreach (var (obj, originalLayer) in movedFoliage)
            {
                if (obj != null)
                {
                    obj.layer = originalLayer;
                }
            }
            movedFoliage.Clear();
            Debug.LogError($"RestoreFoliage: Foliage was restored to the default layer.");
        }

        // Adjusting the zoom level of the map camera:
        public void SetZoom()
        {
            zoomLvl = Mathf.Clamp(zoomLvl, 10f, 1000f);
            if (mapCam != null)
            {
                mapCam.orthographicSize = zoomLvl;
            }
        }

        // Grabbing the zoom value:
        public float GetZoom()
        {
            return zoomLvl;
        }
    }
}