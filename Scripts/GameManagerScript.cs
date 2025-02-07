using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField]
    public int gameStartScene = 1;
    public List<AudioClip> sounds;
    AudioSource source;
    SaveSystem saveSystem;

    void Start()
    {
        source = GetComponent<AudioSource>();
        saveSystem = new SaveSystem("volumeSave.pndgoria");
        source.volume = saveSystem.GetVolume();
    }

    public void StartGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("newGame", 1);
        PlayerPrefs.SetFloat("volume", source.volume);
        saveSystem.SaveVolume();
        SceneManager.LoadScene(gameStartScene);
    }
    public void LoadGame()
    {
        PlayerPrefs.SetInt("newGame", 0);
        PlayerPrefs.SetFloat("volume", source.volume);
        saveSystem.SaveVolume();
        SceneManager.LoadScene(gameStartScene);
    }

    public void QuitGame() {
        PlayerPrefs.SetFloat("volume", source.volume);
        saveSystem.SaveVolume();
        Application.Quit();
    }

    void OnDestroy() {
        PlayerPrefs.SetFloat("volume", source.volume);
        saveSystem.SaveVolume();
        PlayerPrefs.SetInt("lastLevel", SceneManager.GetActiveScene().buildIndex);
    }
    
    void Update()
    {
        if (!source.isPlaying) {
            source.clip = GetRandom();
            source.Play();
        }
    }

    AudioClip GetRandom() {
        return sounds[Random.Range(0, sounds.Count)];
    }
}
