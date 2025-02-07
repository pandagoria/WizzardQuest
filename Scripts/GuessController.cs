using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GuessController : MonoBehaviour
{
    public string correctWord;
    public Sprite clearedWordBoxSprite;
    // List with all the words
    private List<string> dictionary = new List<string>();
    // List with words that can be chosen as correct words
    private List<string> guessingWords = new List<string>();
    // All wordboxes
    public List<Transform> wordBoxes = new List<Transform>();
    // Current wordbox
    private int currentWordBox = 0;
    private int currentRow = 0;
    // colors
    private Color colorCorrect = new Color(0.09019608f, 0.4470588f, 0.2705882f);
    private Color colorIncorrectPlace = new Color(0.6705883f, 0.628282f, 0.345098f);
    private Color colorUnused = new Color(0.1634834f, 0.1065326f, 0.1698113f);
    public AnimationCurve wordBoxInteractionCurve;
    SaveSystem saveSystem;

    void Start() {
        saveSystem = new SaveSystem("wordsSave.pndgoria");
        string[] wordsList = null;
        // Populate the dictionary
        AddWordsToList("Assets/Scripts/Resources/can_guess.txt", dictionary);
        Debug.Log(PlayerPrefs.GetInt("newGame", 0));
        if (PlayerPrefs.GetInt("newGame", 1) == 0)
            wordsList = saveSystem.GetWordsDone();
        PlayerPrefs.SetInt("newGame", 0);
        // Populate the guessing words
        AddWordsToList("Assets/Scripts/Resources/correct_guesses.txt", guessingWords);
        if (wordsList != null) {
            foreach (string word in wordsList) {
            if (guessingWords.Contains(word)) {
                guessingWords.Remove(word);
                Debug.Log("Removed word " + word);
            }
        }
        }
        // Choose a random correct word
        correctWord = GetRandomWord();
    }


    void AddWordsToList(string path, List<string> listOfWords) {
        // Read the text from the file
        StreamReader reader = new StreamReader(path);
        string text = reader.ReadToEnd();
        // Separate them for each ',' character
        char[] separator = { ',' };
        string[] singleWords = text.Split(separator);

        // Add everyone of them to the list provided as a variable
        foreach (string newWord in singleWords)
        {
            listOfWords.Add(newWord);
        }
        reader.Close();
    }
    
    string GetRandomWord() {
        string randomWord = guessingWords[Random.Range(0, guessingWords.Count)];
        Debug.Log(randomWord);
        return randomWord;
    }

    public bool SubmitWord(string guess)
    {
        // Check if the word exists in the dictionary 
        bool wordExists = false;
        foreach (var word in dictionary)
        {
            if (guess == word)
            {
                wordExists = true;
                break;
            }
        }
        if (wordExists == false)
            return wordExists;

        // Output the guess to the console
        Debug.Log("Player guess:" + guess);

        // If the guess was correct, output that the player has won into the console
        if (guess == correctWord)
            Debug.Log("Correct word!");
        else
            Debug.Log("Wrong, guess again!");
        return true;
    }

    public Status AddLetterToWordBox(string letter) {
        letter = letter.ToLower();
        if (currentWordBox < 30 && SubmitWord(letter)) {
            string upper = letter.ToUpper();
            for (int i = 0; i < 5; i++) {
                wordBoxes[currentWordBox].GetChild(0).GetComponent<TextMeshProUGUI>().text = upper[i].ToString();
                StartCoroutine(AnimateWordboxRoutine((wordBoxes[currentWordBox])));
                ++currentWordBox;
            }
            CheckWord(letter);
            ++currentRow;
        }
        if (letter == correctWord) {
            saveSystem.SaveWordsDone(correctWord);
            return Status.Win;
        } else if (currentWordBox == 30)
            return Status.Lose;
        return  Status.Continue;
    }

        
    void CheckWord(string guess)
    {
        // Set up variables
        char[] playerGuessArray = guess.ToCharArray();
        string tempPlayerGuess = guess;
        char[] correctWordArray = correctWord.ToCharArray();
        string tempCorrectWord = correctWord;

        // Swap correct characters with '0'
        for (int i = 0; i < 5; i++)
        {
            if (playerGuessArray[i] == correctWordArray[i])
            {
                // Correct place
                playerGuessArray[i] = '0';
                correctWordArray[i] = '0';
            }
        }

        // Update the information
        tempPlayerGuess = "";
        tempCorrectWord = "";
        for (int i = 0; i < 5; i++)
        {
            tempPlayerGuess += playerGuessArray[i];
            tempCorrectWord += correctWordArray[i];
        }

        // Check for characters in wrong place, but correct letter
        for (int i = 0; i < 5; i++)
        {
            if (tempCorrectWord.Contains(playerGuessArray[i].ToString()) && playerGuessArray[i] != '0')
            {
                char playerCharacter = playerGuessArray[i];
                playerGuessArray[i] = '1';
                tempPlayerGuess = "";
                for (int j = 0; j < 5; j++)
                {
                    tempPlayerGuess += playerGuessArray[j];
                }

                int index = tempCorrectWord.IndexOf(playerCharacter, 0);
                correctWordArray[index] = '.';
                tempCorrectWord = "";
                for (int j = 0; j < 5; j++)
                {
                    tempCorrectWord += correctWordArray[j];
                }
            }
        }

        // Set the fallback color to gray
        Color newColor = colorUnused;

        for (int i = 0; i < 5; i++)
        {

            if (tempPlayerGuess[i] == '0')
                newColor = colorCorrect;
            else if (tempPlayerGuess[i] == '1')
                newColor = colorIncorrectPlace;
            else
                newColor = colorUnused;

            Image currentWordboxImage = wordBoxes[i + (currentRow * 5)].GetComponent<Image>();
            currentWordboxImage.sprite = clearedWordBoxSprite;
            currentWordboxImage.color = newColor;
        }
    }

    IEnumerator AnimateWordboxRoutine(Transform wordboxToAnimate)
    {
        // Our timer
        float timer = 0f;

        // Duration of the animation
        float duration = 0.15f;

        //Set up startscale and end-scale of the wordbox
        Vector3 startScale = Vector3.one;

        // End-scale is just a little bit bigger than the original scale
        Vector3 scaledUp = Vector3.one * 1.2f;

        // Set the wordbox-scale to the starting scale, in case we're entering in the middle of another transition
        wordboxToAnimate.localScale = Vector3.one;

        // Loop for the time of the duration
        while (timer <= duration)
        {
            // This will go from 0 to 1 during the time of the duration
            float value = timer / duration;

            // LerpUnclamped will return a value above 1 and below 0, regular Lerp will clamp the value at 1 and 0
            // To have more freedom when animating, LerpUnclamped can be used instead
            wordboxToAnimate.localScale = Vector3.LerpUnclamped(startScale, scaledUp, wordBoxInteractionCurve.Evaluate(value));

            // Increase the timer by the delta time
            timer += Time.deltaTime;
            yield return null;
        }

        // Since we're checking if the timer is smaller and/or equals to the duration in the loop above,
        // the value might go above 1 which would give the wordbox a scale that is not equals to the desired scale.
        // To prevent slightly scaled wordboxes, we set the scale of the wordbox to the startscale
        wordboxToAnimate.localScale = startScale;
    }
}
