using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public Renderer textureRender;
    private Mesh mesh;
    //public MeshRenderer meshRenderer;

    private Color[] colourMap;
    public TerrainType[] regions;
    public float heightMultiplicator;
    Texture2D texture;
    float soundSteps;
    int currentStep;
    private int mapIntervals;


    public AudioAnalyser audioAnalyser;

    public bool mapHungry;

    public enum DrawMode { LinearSoundTex, LinearSoundMesh };//, CircularSoundTex, CircularSoundMesh }; These are next in line
    public DrawMode drawMode;

    public void Start()
    {  
        mapHungry = false;
    }

    public void FixedUpdate()
    {
        if (drawMode == DrawMode.LinearSoundTex || drawMode == DrawMode.LinearSoundMesh)
            LinearGeneration();
    }


    private void LinearGeneration()
    {
        if (mapHungry)
        {
            int currentYLine = (currentStep / mapWidth) * mapIntervals;
            Debug.Log(currentYLine);
            float power = audioAnalyser.rmsValue * 4;
            //Comparing currentStep to max possible iterations of our map
            //to avoid getting out of range in case of an Euclidian divide problem
            if (currentStep < mapWidth * mapIntervals)
            {
                FeedSoundMap(currentStep, currentYLine + mapIntervals / 2, mapHeight / mapIntervals / 2 +1, power);
            }
            currentStep += 1;
            if (currentStep >= soundSteps)
                StopGeneration();
        }
    }


    public void InitializeSoundMap()//
    {
        soundSteps = audioAnalyser.audioPlayer.getAudioDuration() / Time.fixedDeltaTime;
        int verticalMaxSteps = Convert.ToInt32(soundSteps / mapWidth);
        Debug.Log(verticalMaxSteps);
        mapIntervals = GCD(verticalMaxSteps, mapHeight) - 1;
        currentStep = 0;
        texture = new Texture2D(mapWidth, mapHeight);

        colourMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                colourMap[y * mapWidth + x] = regions[0].colour;
            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();

        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.LinearSoundTex)
        {
            display.DrawTexture(texture);
        }
        else if (drawMode == DrawMode.LinearSoundMesh)
        {
            //display.DrawTexture(texture);
            GameObject.Find("MapTex").SetActive(false);
            mesh = display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapWidth, mapHeight), texture);
        }

    }

    public void FeedSoundMap(int coordX, int lineY, int interval, float height)
    {
        MapDisplay display = FindObjectOfType<MapDisplay>();
       
        float flatteningCoeff;
        for (int y = lineY - interval; y < lineY + interval; y++)
        {
            if (y >= 0 && y < mapHeight)
            {
                if (height >= 0.8)
                    flatteningCoeff = 0.17f;
                else
                    flatteningCoeff = 0.03f;

                float currentHeight = Mathf.Max(height - Mathf.Abs(lineY - y) * flatteningCoeff, 0);
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapWidth + coordX] = regions[i].colour;
                        if(drawMode == DrawMode.LinearSoundMesh)
                        {
                            Vector3[] verts = mesh.vertices;
                            verts[y * mapWidth + coordX].y = currentHeight * heightMultiplicator;
                            mesh.vertices = verts;
                            mesh.RecalculateBounds();
                            mesh.RecalculateNormals();
                        }
                        break;
                    }
                }
            }
            
        }
        texture.SetPixels(colourMap);
        texture.Apply();
    }

    public void StartGeneration()
    {
        mapHungry = true;
    }

    public void StopGeneration()
    {
        mapHungry = false;
    }


    /*-----------------------Utilitary functions---------------------------*/

    private static int GCD(int a, int b)
    {
        while (a != 0 && b != 0)
        {
            if (a > b)
                a %= b;
            else
                b %= a;
        }

        return a == 0 ? b : a;
    }


    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color colour;
    }


}
