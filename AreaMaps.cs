using System.Collections.Generic;
using System.IO;
using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Erenshor_Area_Maps_Mod
{
    public class AreaMaps : MonoBehaviour
    {
        private List<string> mapAreas = new List<string>
    {
        // We have maps for index 0 - 21: 
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
        "Rottenfoot",       //Rottenfoot
        "SaltedStrand",     //Blacksalt Strand
        "ShiveringStep",    //Shivering Step
        "ShiveringTomb",    //Island Tomb
        "Silkengrass",      //Silkengrass Meadowlands
        "Soluna",           //Soluna's Landing
        "Stowaway",         //Stowaway's Step
        "Tutorial",         //Island Tomb
        "Vitheo",           //Vitheo's Watch
        "Windwashed",       //Windwashed Pass

        // No map for index 22 - 35, will display world map instead:
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
        "Undercity",        //Lost Cellar
        "Underspine",       //Underspine Hollow
        "VitheosEnd"        //Vitheo's Rest
    };
        private string sceneName;
        private string sceneTemp;
        private GameObject mapCanvas;

        private void Update() {
            // Finding the instance of the map object:
            if (mapCanvas == null) {
                mapCanvas = GameObject.Find("Map");
            }

            // Grabbing the scene:
            sceneTemp = SceneManager.GetActiveScene().name;

            // Checking to see if the previous scene name matches the new one:
            if (sceneTemp == sceneName) {
                return;
            }
            else
            {
                // Updating sceneName:
                sceneName = sceneTemp;
                // Discarding the main menu and loading scenes:
                if (sceneName == "Menu" || sceneName == "LoadScene") { 
                    return;
                }

                // Some map texture variables:
                var mapImage = mapCanvas.GetComponent<Image>();
                var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);

                // Cross checking with the game scenes list to see which map image will be displayed:
                if (!mapAreas.Contains(sceneName) || (mapAreas.IndexOf(sceneName) >= 22)) {
                    texture.LoadImage(File.ReadAllBytes($"{Paths.PluginPath}/Ayloonah.Erenshor-Area-Maps-Mod/Assets/MapRoutes.png"));
                } else {
                    texture.LoadImage(File.ReadAllBytes($"{Paths.PluginPath}/Ayloonah.Erenshor-Area-Maps-Mod/Assets/" + sceneName + ".png"));
                }
            }
        }
    }
}
