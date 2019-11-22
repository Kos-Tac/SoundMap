using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{

    private AudioSource audioSource;
    [SerializeField]
    private float audioDuration;
    //Play the music
    public bool m_Play;
    //Detect when you use the toggle, ensures music isn’t played multiple times
    public bool m_ToggleChange;

    private void Awake()
    {
        //Fetch the AudioSource from the GameObject
        audioSource = GetComponent<AudioSource>();
        //Ensure the toggle is set to true for the music to play at start-up
        audioDuration = audioSource.clip.length;
        m_Play = false;
    }

    void Update()
    {

        //Check to see if you just set the toggle to positive
        if (m_Play == true && m_ToggleChange == true)
        {
            //Play the audio you attach to the AudioSource component
            audioSource.Play();
            FindObjectOfType<MapGenerator>().InitializeSoundMap();
            FindObjectOfType<MapGenerator>().StartGeneration();
            //Ensure audio doesn’t play more than once
            m_ToggleChange = false;
        }
        //Check if you just set the toggle to false
        if (m_Play == false && m_ToggleChange == true)
        {
            //Stop the audio
            audioSource.Stop();
            FindObjectOfType<MapGenerator>().StopGeneration();
            //Ensure audio doesn’t play more than once
            m_ToggleChange = false;
        }
    }

    void OnGUI()
    {
        //Switch this toggle to activate and deactivate the parent GameObject
        m_Play = GUI.Toggle(new Rect(10, 10, 100, 30), m_Play, "Play Music");

        //Detect if there is a change with the toggle
        if (GUI.changed)
        {
            //Change to true to show that there was just a change in the toggle state
            m_ToggleChange = true;
        }
    }

    public float getAudioDuration()
    {
        return audioDuration;
    }
}
