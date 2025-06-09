using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject[] puzzle = new GameObject[16];
    public GameObject[] puzzlePLace = new GameObject[16];
    public GameObject baseSquare;
    public TMP_Text timerText;

    [Header("Input")]
    public InputActionAsset inputActions;

    private InputAction exitAction;

    private Transform draggingPiece = null;
    private Vector3 offset;
    private Vector3 puzzleLastPosition;
    private float width = 0.8f;
    private float height = 0.8f;
    private int points = 0;
    private SpriteRenderer baseRenderer;
    private AudioSource[] audioSources;
    private float gameTime;

    void Start()
    {
        gameTime = 0f;
        audioSources = GetComponents<AudioSource>();
        baseRenderer = baseSquare.GetComponent<SpriteRenderer>();
        foreach (GameObject piece in puzzle)
        {
            float x = Random.Range(baseSquare.transform.position.x - baseRenderer.size.x / 3,
                baseSquare.transform.position.x + baseRenderer.size.x / 3);
            float y = Random.Range(baseSquare.transform.position.y - baseRenderer.size.y / 3,
                baseSquare.transform.position.y + baseRenderer.size.y / 3);
            piece.transform.position = new Vector3(x, y, 1);
        }
    }

    private void OnEnable()
    {
        if (inputActions != null)
        {
            var uiMap = inputActions.FindActionMap("UI");
            exitAction = uiMap?.FindAction("Exit");

            if (exitAction != null)
            {
                exitAction.performed += OnExit;
                exitAction.Enable();
            }
        }
    }

    private void OnDisable()
    {
        if (exitAction != null)
        {
            exitAction.performed -= OnExit;
            exitAction.Disable();
        }
    }

    private void OnExit(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("CanteenScene");
    }

    void Update()
    {
        if (points >= 16)
        {
            timerText.color = new Color(0, 169, 0, 255);
            audioSources[2].Play();
        }
        else UpdateTimerDisplay();

        gameTime += Time.deltaTime;
       
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                draggingPiece = hit.transform;
                puzzleLastPosition = draggingPiece.position;
                SpriteRenderer spriteRenderer = draggingPiece.GetComponent<SpriteRenderer>();
                SortingGroup sortingGroup = draggingPiece.GetComponent<SortingGroup>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = 3;
                    sortingGroup.sortingOrder = 3;
                }
                offset = draggingPiece.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                offset += Vector3.back;
            }
        }

        if (draggingPiece && Input.GetMouseButtonUp(0))
        {
            SnapIfCorrect();
            draggingPiece.position += Vector3.forward;
            SpriteRenderer spriteRenderer = draggingPiece.GetComponent<SpriteRenderer>();
            SortingGroup sortingGroup = draggingPiece.GetComponent<SortingGroup>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 0;
                sortingGroup.sortingOrder = 0;
            }
            draggingPiece = null;
        }

        if (draggingPiece)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition += offset;
            draggingPiece.position = newPosition;
        }

    }

    private void SnapIfCorrect()
    {
        int puzzleIndex = 0;
        for (int i = 0; i < puzzle.Length; i++)
        {
            if (puzzle[i].transform == draggingPiece)
            {
                puzzleIndex = i;
                break;
            }
        }

        Vector2 targetPosition = puzzlePLace[puzzleIndex].transform.position;

        if (Mathf.Abs(draggingPiece.position.x - targetPosition.x) < width &&
            Mathf.Abs(draggingPiece.position.y - targetPosition.y) < height)
        {
            draggingPiece.DOMove(targetPosition, 0.2f);
            draggingPiece.GetComponent<BoxCollider2D>().enabled = false;
            points++;
            /*if (points >= 16)
            {
                timerText.color = new Color(0, 169, 0, 255);
                audioSources[2].Play();
                this.enabled = false;
            }*/
            audioSources[0].Play();
        }
        else if (draggingPiece.position.x < baseSquare.transform.position.x - baseRenderer.size.x / 2 ||
                 draggingPiece.position.x > baseSquare.transform.position.x + baseRenderer.size.x / 2 ||
                 draggingPiece.position.y < baseSquare.transform.position.y - baseRenderer.size.y / 2 ||
                 draggingPiece.position.y > baseSquare.transform.position.y + baseRenderer.size.y / 2)
        {
            draggingPiece.DOMove(puzzleLastPosition, 0.4f);
            audioSources[1].Play();
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        int milliseconds = Mathf.FloorToInt((gameTime * 1000) % 1000);
        string timeString = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        timerText.text = timeString;
    }
}
