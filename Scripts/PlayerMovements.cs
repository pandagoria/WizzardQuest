using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    ContactFilter2D movementFilter;
    Vector2 movementInput;
    public SpriteRenderer spriteRenderer;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    Rigidbody2D rb;
    Animator animator;
    AudioSource audioSteps;
    SaveSystem saveSystem;

    void Start()
    {
        saveSystem = new SaveSystem("playSave.pndgoria");
        rb = GetComponent<Rigidbody2D>();
        float[] position = saveSystem.GetPlayer();
        if (position != null && PlayerPrefs.GetInt("newGame", 1) == 0) {
            Debug.Log(position[0]);
            rb.MovePosition(new Vector2(position[0], position[1]));
        }
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSteps = GetComponent<AudioSource>();
    }

    private void FixedUpdate() {
        // If movement input is not 0, try to move
        // Debug.Log("Started up logging.");
        if(movementInput != Vector2.zero){
            bool is_moving = TryMove(movementInput);
            if (!is_moving)
                is_moving = TryMove(new Vector2(movementInput.x, 0));
            if (!is_moving)
                is_moving = TryMove(new Vector2(0, movementInput.y));
            animator.SetFloat("X", movementInput.x);
            animator.SetFloat("Y", movementInput.y);
            animator.SetBool("isMoving", is_moving);
        }
        else {
            if (audioSteps.isPlaying)
                audioSteps.Stop();
            animator.SetBool("isMoving", false);
        }
    }

    void OnMove(InputValue movementValue) {
        movementInput = movementValue.Get<Vector2>();
    }

    private bool TryMove(Vector2 direction) {
        if(direction != Vector2.zero) {
            int count = rb.Cast(
                direction, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
                movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
                castCollisions, // List of collisions to store the found collisions into after the Cast is finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset); // The amount to cast equal to the movement plus an offset

            if(count == 0){
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                if (!audioSteps.isPlaying)
                    audioSteps.Play();
                return true;
            } else {
                if (audioSteps.isPlaying)
                    audioSteps.Stop();
                return false;
            }
        } else {
            if (audioSteps.isPlaying)
                audioSteps.Stop();
            return false;
        }
    }

    void OnDestroy()
    {
        saveSystem.SavePlayer(transform.position.x, transform.position.y);
        Debug.Log("Player save data has been passed to save system");
    }
}
