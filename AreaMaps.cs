using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        private U

        private void Update()
        {
            // Finding the instance of the map object:

            
            // Checking the scene we're in:
            sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "Menu" || sceneName == "LoadScene")
            {
                return;
            }

            // Cross checking with the game scenes list to see what we're doing with it and catch errors:
            if (!mapAreas.Contains(sceneName))
            {
                return;
            }
            else if (mapAreas.IndexOf(sceneName) >= 22) {
            {

            }
        }
    }
}
