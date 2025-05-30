using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Erenshor_Area_Maps_Mod
{
    public class MapUIManager : MonoBehaviour
    {
        private readonly string mapFramePath = "UI/UIElements/Map/Map (1)";
        private const float WIDTH_SCALE = 0.9f;
        private const float HEIGHT_SCALE = 0.85f;
        private AreaMaps areaMaps;
        private RawImage mapCameraDisplayRawImage;
        private RectTransform mapFrameRect;
        private int lastScreenWidth;
        private int lastScreenHeight;
        private bool initialized = false;
        private string sceneName;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!initialized)
            {
                // Grabbing the scene:
                sceneName = scene.name;

                // Discarding the main menu and loading scenes:
                if (sceneName == "Menu" || sceneName == "LoadScene")
                {
                    return;
                }

                InitializeMapFrame();
            }
            else
            {
                return;
            }
        }

        // Resizing the map frame if the screen size has changed when player next opens the map:
        public void ResizeMapFrame()
        {
            if (mapFrameRect == null)
            {
                return;
            }

            if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
            {
                mapFrameRect.anchorMin = new Vector2(0.5f, 0.5f);
                mapFrameRect.anchorMax = new Vector2(0.5f, 0.5f);
                mapFrameRect.pivot = new Vector2(0.5f, 0.5f);
                mapFrameRect.localPosition = Vector3.zero;
                mapFrameRect.localScale = Vector3.one;

                float newWidth;
                float newHeight;

                if (areaMaps.showingWorldMap && mapCameraDisplayRawImage != null && mapCameraDisplayRawImage.texture != null)
                {
                    // Resizing to the png size:
                    newWidth = mapCameraDisplayRawImage.texture.width;
                    newHeight = mapCameraDisplayRawImage.texture.height;
                }
                else
                {
                    // Resizing to allow for more room for the area map:
                    newWidth = Screen.width * WIDTH_SCALE;
                    newHeight = Screen.height * HEIGHT_SCALE;
                }


                mapFrameRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
                mapFrameRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

                if (mapCameraDisplayRawImage != null)
                {
                    mapCameraDisplayRawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
                    mapCameraDisplayRawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
                }

                CacheScreenSize();
                Debug.Log($"ResizeMapFrame: Resized map frame to {newWidth}x{newHeight}");
            }
        }

        // Caching screen size to only resize if necessary:
        private void CacheScreenSize()
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            Debug.LogWarning($"CacheScreenSize: Screen size cached");
        }

        // Initializes the map frame object after we are the the right scene:
        private void InitializeMapFrame()
        {
            if (initialized)
            {
                return;
            }

            var mapFrameObj = GameObject.Find(mapFramePath);
            if (mapFrameObj == null)
            {
                Debug.LogWarning($"InitializeMapFrame: Could not find map frame at path '{mapFramePath}'");
                return;
            }

            mapFrameRect = mapFrameObj.GetComponent<RectTransform>();
            if (mapFrameRect == null)
            {
                Debug.LogWarning("InitializeMapFrame: Map frame GameObject found but has no RectTransform component!");
                return;
            }

            Debug.Log($"MapFrame anchors: min {mapFrameRect.anchorMin}, max {mapFrameRect.anchorMax}, pivot {mapFrameRect.pivot}");

            mapCameraDisplayRawImage = mapFrameObj.GetComponent<RawImage>();
            if (mapCameraDisplayRawImage == null)
            {
                Debug.LogWarning("InitializeMapFrame: Could not find RawImage inside the map frame!");
            }

            // Resizing the map frame:
            ResizeMapFrame();

            initialized = true;
        }
    }
}
