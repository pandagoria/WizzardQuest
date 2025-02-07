using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public TMP_InputField input;
    public GuessController guessController;
    public GameObject popup;
    private Coroutine popupRoutine;

    public void onEndEdit(string word) {
        Debug.Log(word);
        if (word.Length == 5) {
            Status status = guessController.AddLetterToWordBox(word);
            if (status == Status.Win)
            {
                SaveSystem save = new SaveSystem(PlayerPrefs.GetString("enemyName"));
                save.SaveEnemy(false, 0f, 0f);
                Debug.Log("Player Wins!");
                EndGame("Победа! Ура!");
            } else if (status == Status.Lose) {
                Debug.Log("Player Lost!");
                EndGame("Проигрыш...");
            }
        }
        input.text = "";
    }

    private void EndGame(string message) {
        ShowPopup(message, 10f, true);
        SceneManager.LoadScene(1);
    }

    void ShowPopup(string message, float duration, bool stayForever)
    {
        // If a popup routine exists, we should stop that first,
        // this makes sure that not 2 coroutines can run at the same time.
        // Since we are using the same popup for every message, we only want one of these coroutines to run at any time
        if (popupRoutine != null)
        {
            StopCoroutine(popupRoutine);
        }
        popupRoutine = StartCoroutine(ShowPopupRoutine(message, duration, stayForever));
    }

    IEnumerator ShowPopupRoutine(string message, float duration, bool stayForever = false)
    {
        // Set the message of the popup
        popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        // Activate the popup
        popup.SetActive(true);
        // If it should stay forever or not
        if (stayForever)
        {
            while (true)
            {
                yield return null;
            }
        }
        // Wait for the duration time
        yield return new WaitForSeconds(duration);
        // Deactivate the popup
        popup.SetActive(false);
    }
}
