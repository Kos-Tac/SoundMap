using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public Renderer textureRender;
    private Color[] colourMap;
    Texture2D texture;
    float soundSteps;
    int currentStep;
    private int mapIntervals;


    public AudioAnalyser audioAnalyser;

    public bool mapHungry;

    public void Start()
    {  
        mapHungry = false;
    }

    public void FixedUpdate()
    {
        System.Console.WriteLine("lapata");
        if (mapHungry)
        {
            int currentYLine = currentStep % mapWidth;
            float power = audioAnalyser.rmsValue * 4;
            //Comparing currentStep to max possible iterations of our map
            //to avoid getting out of range in case of an Euclidian divide problem
            if (currentStep < mapWidth * mapIntervals)
            {
                feedSoundMap(currentStep, currentYLine + mapIntervals / 2, mapHeight / mapIntervals / 2, power);
            }
            currentStep += 1;
        }
    }



    public void initializeSoundMap()//float[,] soundMap)
    {
        soundSteps = audioAnalyser.audioPlayer.getAudioDuration() / Time.fixedDeltaTime;
        int verticalMaxSteps = Convert.ToInt32(soundSteps % mapWidth);
        mapIntervals = GCD(verticalMaxSteps, mapHeight) - 1;
        currentStep = 0;
        texture = new Texture2D(mapWidth, mapHeight);

        colourMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                colourMap[y * mapWidth + x] = Color.white;
            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(mapWidth, 1, mapHeight);
    }

    public void feedSoundMap(int coordX, int lineY, int interval, float mapHeight)
    {
        for (int y = lineY - interval; y < lineY + interval; y++)
        {
            colourMap[interval * mapWidth + coordX] = Color.Lerp(Color.white, Color.black, mapHeight - Mathf.Abs(lineY - y)*0.1f );
        }
        texture.SetPixels(colourMap);
        texture.Apply();
    }

    public void startGeneration()
    {
        mapHungry = true;
    }

    public void stopGeneration()
    {
        mapHungry = false;
    }

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

}
