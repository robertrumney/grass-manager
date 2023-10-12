using UnityEngine;
using UnityEditor;

public class GrassManagerWindow : EditorWindow
{
    private Color grassHealthyColor = Color.white;
    private Color grassDryColor = Color.grey;  //495706

    private Terrain[] terrains;
    private float grassMinWidth = 0.5f;
    private float grassMaxWidth = 1.0f;
    private float grassMinHeight = 1.0f;
    private float grassMaxHeight = 1.0f;
    private float grassNoiseSpread = 0.1f;

    private int detailDensity = 1;
    private int selectedGrassTextureIndex = 0;

    private Texture2D[] grassTextures = new Texture2D[4];

    [MenuItem("Window/Grass Manager")]
    public static void ShowWindow()
    {
        GetWindow<GrassManagerWindow>("Grass Manager");
    }
    
    private void OnEnable()
    {
        terrains = FindObjectsOfType<Terrain>();

        if (terrains != null && terrains.Length > 0 && terrains[0].terrainData.detailPrototypes.Length >= 4)
        {
            for (int i = 0; i < 4; i++)
            {
                grassTextures[i] = terrains[0].terrainData.detailPrototypes[i].prototypeTexture;
            }
        }
        else if (terrains != null && terrains.Length > 0)
        {
            // If the first terrain has less than 4 detail layers
            Debug.LogWarning("The first terrain does not have enough detail layers to populate all grass textures!");
        }
        else
        {
            Debug.LogError("No terrains found in the scene!");
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Select detail parameters here:", EditorStyles.boldLabel);

        grassMinWidth = EditorGUILayout.FloatField("Min Width", grassMinWidth);
        grassMaxWidth = EditorGUILayout.FloatField("Max Width", grassMaxWidth);
        grassMinHeight = EditorGUILayout.FloatField("Min Height", grassMinHeight);
        grassMaxHeight = EditorGUILayout.FloatField("Max Height", grassMaxHeight);
        grassNoiseSpread = EditorGUILayout.FloatField("Noise Spread", grassNoiseSpread);

        GUILayout.Label("Grass Colors", EditorStyles.boldLabel);
        grassHealthyColor = EditorGUILayout.ColorField("Healthy Color", grassHealthyColor);
        grassDryColor = EditorGUILayout.ColorField("Dry Color", grassDryColor);

        for (int i = 0; i < grassTextures.Length; i++)
        {
            grassTextures[i] = (Texture2D)EditorGUILayout.ObjectField($"Grass Texture {i + 1}", grassTextures[i], typeof(Texture2D), false);
        }

        // Optimise Terrain Settings
        if (GUILayout.Button("Click here to apply optimal terrain detail settings."))
        {
            OptimiseTerrainSettings();
        }

        // Remove All Detail Layers
        if (GUILayout.Button("Click here to remove all grass layers from terrains and start from scratch."))
        {
            RemoveAllDetailLayers();
        }

        // Apply Grass Details
        if (GUILayout.Button("Once you've chosen your 4 grass details, click here to apply to all terrains."))
        {
            ApplyGrassDetailSettings();
        }

        GUILayout.Space(10);
        GUILayout.Label("Terrains in Scene", EditorStyles.boldLabel);

        if (terrains != null)
        {
            foreach (Terrain terrain in terrains)
            {
                GUILayout.Label(terrain.name);
            }
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Click here to clear all placed grass."))
        {
            ClearAllGrass();
        }
        GUILayout.Space(10);

        if (grassTextures.Length > 0)
        {
            string[] grassTextureNames = new string[grassTextures.Length];

            for (int i = 0; i < grassTextures.Length; i++)
            {
                grassTextureNames[i] = grassTextures[i] != null ? grassTextures[i].name : "Not assigned";
            }
            selectedGrassTextureIndex = EditorGUILayout.Popup("Select Grass Texture", selectedGrassTextureIndex, grassTextureNames);
        }

        // Distribute Selected Grass
        if (GUILayout.Button("Click here to distribute selected grass over all terrains."))
        {
            DistributeGrassOverTerrains();
        }

        detailDensity = EditorGUILayout.IntField("Grass Density", detailDensity);
    }

    private void ApplyGrassDetailSettings()
    {
        foreach (Texture2D grassTexture in grassTextures)
        {
            if (grassTexture == null)
            {
                Debug.LogError("One of the Grass Textures is not assigned!");
                return;
            }

            foreach (Terrain terrain in terrains)
            {
                bool prototypeExists = false;
                foreach (DetailPrototype prototype in terrain.terrainData.detailPrototypes)
                {
                    if (prototype.prototypeTexture == grassTexture)
                    {
                        prototypeExists = true;
                        break;
                    }
                }

                if (!prototypeExists)
                {
                    DetailPrototype newPrototype = new DetailPrototype();
                    newPrototype.prototypeTexture = grassTexture;
                    newPrototype.minWidth = grassMinWidth;
                    newPrototype.maxWidth = grassMaxWidth;
                    newPrototype.minHeight = grassMinHeight;
                    newPrototype.maxHeight = grassMaxHeight;
                    newPrototype.noiseSpread = grassNoiseSpread;
                    newPrototype.renderMode = DetailRenderMode.GrassBillboard;

                    newPrototype.healthyColor = grassHealthyColor;
                    newPrototype.dryColor = grassDryColor;

                    DetailPrototype[] currentDetailPrototypes = terrain.terrainData.detailPrototypes;
                    DetailPrototype[] newDetailPrototypes = new DetailPrototype[currentDetailPrototypes.Length + 1];
                    currentDetailPrototypes.CopyTo(newDetailPrototypes, 0);
                    newDetailPrototypes[currentDetailPrototypes.Length] = newPrototype;

                    terrain.terrainData.detailPrototypes = newDetailPrototypes;
                }
            }
        }
    }

    private void ClearAllGrass()
    {
        foreach (Terrain terrain in terrains)
        {
            int detailLayerCount = terrain.terrainData.detailPrototypes.Length;
            int width = terrain.terrainData.detailWidth;
            int height = terrain.terrainData.detailHeight;

            for (int i = 0; i < detailLayerCount; i++)
            {
                int[,] emptyDetailLayer = new int[width, height];
                terrain.terrainData.SetDetailLayer(0, 0, i, emptyDetailLayer);
            }
        }
    }

    private void DistributeGrassOverTerrains()
    {
        Terrain[] terrains = FindObjectsOfType<Terrain>();

        foreach (Terrain terrain in terrains)
        {
            DistributeGrass(terrain);
        }
    }

    private void DistributeGrass(Terrain terrain)
    {
        // Ensure grass properties are valid
        int detailLayerCount = terrain.terrainData.detailPrototypes.Length;

        if (detailLayerCount == 0 || selectedGrassTextureIndex >= detailLayerCount)
        {
            Debug.LogWarning($"Invalid detail layer for terrain {terrain.name}. Please check your settings.");
            return;
        }

        int width = terrain.terrainData.detailWidth;
        int height = terrain.terrainData.detailHeight;

        int[,] newDetailLayer = new int[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                newDetailLayer[x, y] = detailDensity;
            }
        }

        terrain.terrainData.SetDetailLayer(0, 0, selectedGrassTextureIndex, newDetailLayer);
    }


    private void OptimiseTerrainSettings()
    {
        foreach (Terrain terrain in terrains)
        {
            terrain.terrainData.SetDetailResolution(1024, 128);
        }
    }

    private void RemoveAllDetailLayers()
    {
        foreach (Terrain terrain in terrains)
        {
            terrain.terrainData.detailPrototypes = new DetailPrototype[0];
        }
    }
}
