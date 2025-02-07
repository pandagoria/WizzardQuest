using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EnemyMovements : MonoBehaviour
{
    private Rigidbody2D rb;
    Animator animator;
    SaveSystem saveSystem;
    bool exists = true;
    public GameObject windowDialog;
    public TextMeshProUGUI textDialog;
    public string[] message;
    public int numberDialog = 0;
    private bool pressed = false;
    private string theName;
    private string activeEnemyName;

    private void Start() {
        theName = this.name + ".pndgoria";
        activeEnemyName = PlayerPrefs.GetString("enemyName", "");
        saveSystem = new SaveSystem(theName);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Loading();
    }

    void Loading() {
        float[] position = saveSystem.GetEnemy();
        if (position != null && PlayerPrefs.GetInt("newGame", 1) == 0) {
            if (saveSystem.IsEnemyAlive()) {
                if (position[0] != 0f && position[1] != 0f)
                    rb.MovePosition(new Vector2(position[0], position[1]));
                if (PlayerPrefs.GetInt("lastLevel") == 2 && activeEnemyName == theName) {
                    windowDialog.SetActive(true);
                    textDialog.text = "Ха-ха! Такого же тупицу мы уже лет 100 не видели";
                }
            } else {
                exists = false;
                Debug.Log("Enemy is dead");
                if (PlayerPrefs.GetInt("lastLevel") == 2 && activeEnemyName == theName) {
                    windowDialog.SetActive(true);
                    textDialog.text = "О нет! Как у тебя получилось...";
                    animator.SetBool("isAlive", false);
                }
                else
                    gameObject.SetActive(false);       
            }
        }
    }

    // Func triggers when animation starts
    void DyingAnimation() {
        StartCoroutine(TestFunctHui());
    }

    IEnumerator TestFunctHui() {
        yield return new WaitForSeconds(4);
        windowDialog.SetActive(false);
        gameObject.SetActive(false);
    }

    void Update() {
        if (pressed && exists && activeEnemyName != theName) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                NextDialog();
            }
            windowDialog.SetActive(true);
            textDialog.text = message[numberDialog];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && exists)
            pressed = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (windowDialog != null)
            windowDialog.SetActive(false);
        numberDialog = 0;
        pressed = false;
    }

    public void NextDialog()
    {
        if (numberDialog < message.Length - 1)
        {
            Debug.Log("Next Dialogue");
            numberDialog++;
            textDialog.text = message[numberDialog];
        }
        else {
            PlayerPrefs.SetString("enemyName", theName);
            SceneManager.LoadScene(2);
        }
    }

    void OnDestroy() {
        saveSystem.SaveEnemy(exists, transform.position.x, transform.position.y);
        Debug.Log("Enemy save data has been passed to save system");
    }
}
