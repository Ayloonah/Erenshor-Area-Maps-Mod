using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Erenshor_Area_Maps_Mod
{
    [BepInPlugin("com.ayloonah.erenshor_area_maps_mod", "Erenshor Area Maps Mod", "1.0.0")]
    public class Mod : BaseUnityPlugin
    {
        private void Awake() {
            Logger.LogInfo("Erenshor Area Maps Mod loaded!");

            // Referring to the actual mod content
            GameObject go = new GameObject("AreaMaps");
            go.AddComponent<AreaMaps>();
            DontDestroyOnLoad(go);
        }

        public void OnDestroy()
        {
            Logger.LogInfo("Erenshor Area Maps Mod unloaded!");
        }
    }
}
