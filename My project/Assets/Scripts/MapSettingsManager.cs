using UnityEngine;

public enum MapSeedOption { Random, Preset, MapOfTheDay }

public class MapSettingsManager : MonoBehaviour
{

    public static MapSeedOption SelectedMapSeedOption = MapSeedOption.Random;

    public void SetSeedMode(int mode)
    {
        SelectedMapSeedOption = (MapSeedOption)mode;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
