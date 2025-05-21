using System.Collections.Generic;
using System.IO;
using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Erenshor_Area_Maps_Mod {
    public class AreaMaps : MonoBehaviour {
        private readonly string mapAssetsPath = Path.Combine(Paths.PluginPath, "Ayloonah.Erenshor-Area-Maps-Mod", "Assets");
        private List<string> mapAreas = new List<string>
    {   // We have maps for index 0 - 20: 
        "Azure",            //Port Azure
        "Azynthi",          //Azynthi's Garden dark map
        "AzynthiClear",     //Azynthi's Garden grey map
        "Blight",           //The Blight
        "Brake",            //Faerie's Brake
        "Braxonian",        //Braxonian Desert
        "Duskenlight",      //The Duskenlight Coast
        "FernallaField",    //Fernalla's Revival Plains
        "Hidden",           //Hidden Hills
        "Loomingwood",      //Loomingwood Forest
        "Malaroth",         //Malaroth's Nesting Grounds
        "Ripper",           //Ripper's Keep
        "SaltedStrand",     //Blacksalt Strand
        "ShiveringStep",    //Shivering Step
        "ShiveringTomb",    //Island Tomb
        "Silkengrass",      //Silkengrass Meadowlands
        "Soluna",           //Soluna's Landing
        "Stowaway",         //Stowaway's Step
        "Tutorial",         //Island Tomb
        "Vitheo",           //Vitheo's Watch
        "Windwashed",       //Windwashed Pass

        // No map for index 21 - 35, will display world map instead:
        "Abyssal",          //Abyssal Lake
        "Bonepits",         //The Bonepits
        "Braxonia",         //Fallen Braxonia
        "DuskenPortal",     //Mysterious Portal
        "Elderstone",       //The Elderstone Mines
        "FernallaPortal",   //Mysterious Portal
        "Jaws",             //Jaws of Sivakaya
        "Krakengard",       //Old Krakengard
        "PrielPlateau",     //Prielian Cascade
        "RipperPortal",     //Mysterious Portal
        "Rockshade",        //Rockshade Hold
        "Rottenfoot",       //Rottenfoot
        "Undercity",        //Lost Cellar
        "Underspine",       //Underspine Hollow
        "VitheosEnd"        //Vitheo's Rest 
    };
        private Dictionary<string, Texture2D> mapCache = new Dictionary<string, Texture2D>();
        private string sceneName;
        private GameObject mapCanvas;
        private string lastLoadedMap = "";
        private Button toggleButton;
        private bool showingWorldMap = false;
        private Text toggleButtonText;

        private void Awake() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // The actual mod content:
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            // Grabbing the scene:
            sceneName = scene.name;

            // Discarding the main menu and loading scenes:
            if (sceneName == "Menu" || sceneName == "LoadScene") {
                return;
            }

            // Finding the instance of the map object:
            if (mapCanvas == null) {
                foreach (var obj in Resources.FindObjectsOfTypeAll<RectTransform>()) {
                    if (obj.name == "Map" && obj.GetComponent<Image>() != null) {
                        mapCanvas = obj.gameObject;
                        break;
                    }
                }
            }

            // If we can't find the map object, we don't need to do anything:
            if (mapCanvas == null) {
                return;
            }
            
            // Displaying a toggle button to change the displayed map:
            CreateToggleButton();

            // Finding the image component of the map object:
            var mapImage = mapCanvas.GetComponent<Image>();

            // If the image is not found, we don't need to do anything:
            if (mapImage == null) {
                return;
            }

            // Making sure we actually need to apply the texture:
            if (lastLoadedMap != sceneName) {
                // Cross checking with the game scenes list to see which map image will be displayed:
                if (!mapAreas.Contains(sceneName) || (mapAreas.IndexOf(sceneName) >= 21)) {
                    ApplyMapTexture("MapRoutes", true);
                    ButtonSetter(-2, false);
                } else {
                    ApplyMapTexture(sceneName, false);
                    ButtonSetter(mapAreas.IndexOf(sceneName));
                }
            }
        }

        // Creating a button to toggle between the world map and the area map, if there's an area map:
        private void CreateToggleButton() {
            // Skipping if the map window is not open or if the button already exists:
            if (mapCanvas == null || toggleButton != null) {
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
        private void ToggleMapView() {
            // If the map is not open, skip:
            if (mapCanvas == null) {
                return;
            }

            // Getting the map image component:
            var mapImage = mapCanvas.GetComponent<Image>();
            
            // If it's not found, skip:
            if (mapImage == null) {
                return;
            }

            // If we're currently seeing the world map:
            if (showingWorldMap) {
                // Try to switch to an area map, if one exists:
                if (mapAreas.Contains(sceneName) && (mapAreas.IndexOf(sceneName) < 21)) {
                    ApplyMapTexture(sceneName, false);
                    ButtonSetter(mapAreas.IndexOf(sceneName));
                } else {
                    return;
                }
            // If we're currently seeing the area map:
            } else {
                // Showing the world map:
                ApplyMapTexture("MapRoutes", true);
                ButtonSetter(-1);
            }
        }

        // Grabbing the map texture based on the scene the player's in:
        private Texture2D GetMapTexture(string sceneName) {
            // Checking if the map is already cached:
            if (mapCache.ContainsKey(sceneName)) {
                return mapCache[sceneName];
            }

            // Creating the path to the required texture:
            string path = Path.Combine(mapAssetsPath, sceneName + ".png");
            if (sceneName == "MapRoutes") {
                path = Path.Combine(mapAssetsPath, "MapRoutes.png");
            }

            // Caching the map for later use:
            try {
                byte[] data = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                texture.LoadImage(data);
                mapCache[sceneName] = texture;
                return texture;
            } catch (IOException e) {
                Debug.LogWarning($"[AreaMaps] Failed to load map image {sceneName}: {e.Message}");
                return null;
            }
        }

        // Applying the map texture to the map object:
        private void ApplyMapTexture(string mapKey, bool isWorldMap) {
            // Accessing the map image component:
            var mapImage = mapCanvas?.GetComponent<Image>();
            if (mapImage == null) {
                return;
            }

            // Grabbing the right texture:
            Texture2D texture = GetMapTexture(mapKey);
            
            // Applying the texture and updating relevant variables:
            mapImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            showingWorldMap = isWorldMap;
            lastLoadedMap = sceneName;
        }

        // Sets the button to active or not, and updates the text on the button:
        private void ButtonSetter(int index, bool hasAreaMap = true) {
            // If there is no area map for this scene:
            if (!hasAreaMap && (index == -2)) {
                toggleButton?.gameObject.SetActive(false);
                // If the world map is showing:
            } else if ((index == -1) || (index >= 21)) {
                if (toggleButtonText != null) {
                    toggleButtonText.text = "Area Map";
                }
                toggleButton?.gameObject.SetActive(true);
                // If an area map is showing:
            } else {
                if (toggleButtonText != null) {
                    toggleButtonText.text = "World Map";
                }
                toggleButton?.gameObject.SetActive(true);
            }
        }
    }
}
