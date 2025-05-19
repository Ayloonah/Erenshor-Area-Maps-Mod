using BepInEx;
using UnityEngine;

namespace Erenshor_Area_Maps_Mod
{

    [BepInPlugin("com.ayloonah.erenshor_area_maps_mod", "Erenshor Area Maps Mod", "1.0.0")]
    public class Mod : BaseUnityPlugin
    {
        // This method is called when the mod is loaded
        private void Awake()
        {
            Logger.LogInfo("Erenshor Area Maps Mod loaded!");

            // Referring to the actual mod content
            GameObject go = new GameObject("AreaMaps");
            go.AddComponent<AreaMaps>();
            DontDestroyOnLoad(go);
        }

        // This method is called when the mod is unloaded
        public void OnDestroy()
        {
            Logger.LogInfo("Erenshor Area Maps Mod unloaded!");
        }

    }
}
