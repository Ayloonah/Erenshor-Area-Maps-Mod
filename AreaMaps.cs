using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Erenshor_Area_Maps_Mod
{
    public class AreaMaps : MonoBehaviour
    {
        private RawImage mapRawImage;
        private GameObject mapCanvas;
        private Button toggleButton;
        private Text toggleButtonText;
        private MapCamera mapCam;
        private MapUIManager uiManager;
        private string sceneName;
        public bool showingWorldMap = false;
        private bool mapInitialized = false;
        private bool isMapOpen = false;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var name in assembly.GetManifestResourceNames())
            {
                Debug.Log("Embedded Resource: " + name);
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Grabbing the scene:
            sceneName = scene.name;

            // Discarding the main menu and loading scenes:
            if (sceneName == "Menu" || sceneName == "LoadScene")
            {
                return;
            }

            // Finding the instance of the map object:
            if (mapCanvas == null)
            {
                foreach (var obj in Resources.FindObjectsOfTypeAll<RectTransform>())
                {
                    if (obj.name == "Map" && obj.GetComponent<Image>() != null)
                    {
                        mapCanvas = obj.gameObject;
                        break;
                    }
                }
                Debug.LogError($"OnSceneLoaded: Found map canvas.");
            }

            // If we can't find the map object, we don't need to do anything:
            if (mapCanvas == null)
            {
                return;
            }

            // Removing the image component from the map canvas:
            var oldImage = mapCanvas.GetComponent<Image>();
            if (oldImage != null)
            {
                GameObject.DestroyImmediate(oldImage);
                Debug.Log("OnSceneLoaded: Removed legacy Image component from mapCanvas");
            }

            // Setting up the mapCam and uiManager references if not already done:
            if (mapCam == null)
            {
                mapCam = MapCamera.Instance;
                if (mapCam == null)
                {
                    Debug.LogError("OnSceneLoaded: MapCamera not found in the scene. Please ensure it is present.");
                }
                else
                {
                    Debug.LogError($"OnSceneLoaded: MapCam is initialized.");
                }
            }
            if (uiManager == null)
            {
                uiManager = FindObjectOfType<MapUIManager>();
                if (uiManager == null)
                {
                    Debug.LogError("OnSceneLoaded: uiManager not found in the scene. Please ensure it is present.");
                }
                else
                {
                    Debug.LogError($"OnSceneLoaded: uiManager is initialized.");
                }
            }

            // Making sure mapRawImage was created:
            if (mapRawImage == null)
            {
                mapRawImage = mapCanvas.GetComponent<RawImage>();
                if (mapRawImage == null)
                {
                    mapRawImage = mapCanvas.AddComponent<RawImage>();
                    mapRawImage.enabled = false;
                }
            }

            // On scene change, if world map was last displayed, resets to area map:
            if (showingWorldMap)
            {
                if (mapInitialized)
                {
                    ToggleMapView();
                    Debug.LogError($"OnSceneLoaded: Toggled map view so area map is displayed.");
                }
                showingWorldMap = false;

                // Reset initialization flag to force setup on first map open
                mapInitialized = false;
            }
        }

        // On every frame:
        private void Update()
        {
            // Skipping if mapCanvas not found:
            if (mapCanvas == null)
            {
                return;
            }

            // Checking if the map is open:
            bool currentlyOpen = mapCanvas.activeInHierarchy;

            // Skipping if map not open, otherwise displaying appropriate map:
            if (currentlyOpen && !isMapOpen)
            {
                if (!mapInitialized)
                {
                    CreateToggleButton();
                    uiManager?.ResizeMapFrame();
                    mapInitialized = true;
                    Debug.LogError($"Update: Map is initialized.");
                }
                else
                {
                    // Subsequent map opens resize the map frame, but reuse the frozen mapcam frame for the area map:
                    uiManager?.ResizeMapFrame();
                }
            }

            isMapOpen = currentlyOpen;
        }

        // Creating a button to toggle between the world map and the area map, if there's an area map:
        private void CreateToggleButton()
        {
            // Skipping if the map window is not open or if the button already exists:
            if (mapCanvas == null || toggleButton != null)
            {
                return;
            }

            // Creating the button object:
            GameObject buttonObj = new GameObject("MapToggleButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonObj.transform.SetParent(mapCanvas.transform, false);

            // Set the button's position and size:
            RectTransform rt = buttonObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1f, 0f);
            rt.anchorMax = new Vector2(1f, 0f);
            rt.anchoredPosition = new Vector2(-60, 0);
            rt.sizeDelta = new Vector2(100, 40);

            // Set the button's appearance:
            Image img = buttonObj.GetComponent<Image>();
            img.color = new Color(3f / 255f, 54f / 255f, 72f / 255f, 1f);
            Outline outline = buttonObj.AddComponent<Outline>();
            outline.effectColor = new Color(71f / 255f, 175f / 255f, 180f / 255f, 1f);
            outline.effectDistance = new Vector2(1f, 1f);

            // Creating the actual button, its listener, and its label:
            Button btn = buttonObj.GetComponent<Button>();
            btn.onClick.AddListener(ToggleMapView);
            toggleButton = btn;
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObj.transform.SetParent(buttonObj.transform, false);

            RectTransform textRt = textObj.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            toggleButtonText = textObj.GetComponent<Text>();
            toggleButtonText.text = showingWorldMap ? "Area Map" : "World Map";
            toggleButtonText.alignment = TextAnchor.MiddleCenter;
            toggleButtonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            toggleButtonText.color = Color.white;
            toggleButtonText.fontSize = 18;
        }

        // Swapping the map displayed after pressing on the button:
        private void ToggleMapView()
        {
            // If the map is not open, skip:
            if (mapCanvas == null)
            {
                return;
            }

            // Getting the map image component:
            var mapImage = mapCanvas.GetComponent<Image>();

            // If it's not found, skip:
            if (mapImage == null)
            {
                return;
            }

            // Applying the map texture and the button text:
            ApplyMapTexture(!showingWorldMap);
        }

        // Applying the map texture to the map object:
        private void ApplyMapTexture(bool isWorldMap)
        {
            // Grabbing the map's image and raw image components, and skipping if it doesn't work:
            if (mapRawImage == null)
            {
                Debug.LogWarning("ApplyMapTexture: mapCanvas missing RawImage component");
                return;
            }

            // Tracking the map currently in focus and the one we're switching to:
            Texture current = mapRawImage.texture;
            Texture intended = null;

            // Figuring out the intended texture:
            if (isWorldMap)
            {
                // Grabs the embedded MapRoutes png:
                Texture2D tex = LoadEmbeddedTexture("MapRoutes.png");

                if (tex != null)
                {
                    intended = tex;
                }
                else if (mapCam != null)
                {
                    intended = mapCam.GetRenderTexture();
                }
            }
            else if (mapCam != null)
            {
                intended = mapCam.GetRenderTexture();
            }

            // Skipping the change if we're already showing the intended map:
            if (current == intended)
            {
                Debug.Log("ApplyMapTexture: Texture already matches, skipping update.");
                return;
            }

            // Applying the proper texture if necessary: 
            if (intended != null)
            {
                mapRawImage.texture = intended;
                mapRawImage.enabled = true;

                if (!isWorldMap && mapCam != null)
                {
                    mapCam.EnableCamera();
                    Debug.Log("ApplyMapTexture: Enabled area map camera");
                }
                else
                {
                    Debug.Log("ApplyMapTexture: Applied world map texture");
                }
            }

            // Updating showingWorldMap and toggle button text:
            showingWorldMap = isWorldMap;
            ButtonSetter();
        }

        // Sets the button to active or not, and updates the text on the button:
        private void ButtonSetter()
        {
            if (toggleButtonText != null)
            {
                toggleButtonText.text = showingWorldMap ? "Area Map" : "World Map";
                Debug.LogError($"ButtonSetter: Switched toggle button text.");
            }
        }

        // Loading embedded resources:
        public static Texture2D LoadEmbeddedTexture(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            string fullName = assembly.GetManifestResourceNames()
                .FirstOrDefault(name => name.EndsWith(resourceName));

            if (string.IsNullOrEmpty(fullName))
            {
                Debug.LogError($"LoadEmbeddedTexture: Embedded resource '{resourceName}' not found.");
                return null;
            }

            using (Stream stream = assembly.GetManifestResourceStream(fullName))
            {
                if (stream == null)
                {
                    Debug.LogError($"LoadEmbeddedTexture: Failed to open stream for resource '{fullName}'.");
                    return null;
                }

                byte[] data;
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    data = ms.ToArray();
                }

                Texture2D texture = new Texture2D(2, 2);
                if (!texture.LoadImage(data))
                {
                    Debug.LogError($"LoadEmbeddedTexture: Failed to load image data from resource '{fullName}'.");
                    return null;
                }
                Debug.LogError($"LoadEmbeddedTexture: Returning texture.");
                return texture;
            }
        }
    }
}