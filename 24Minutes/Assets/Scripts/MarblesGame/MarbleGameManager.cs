using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class MarbleGameManager : MonoBehaviour
{
    public GameObject ground;
    public GameObject playerMarble; // Canica del jugador (Roja)
    public GameObject aiMarble; // Canica de la IA (Azul)
    public List<GameObject> neutralMarbles; // Lista de canicas neutrales (Grises)
    public GameObject youwin;
    public GameObject youlose;

    public Color gris = Color.gray;
    public Color azul = Color.blue;
    public Color rojo = Color.red;

    public bool isPlayerTurn = true;
    public bool isGameOver = false;
    public int rounds = 7;
    public TextMeshProUGUI roundsText;
    
    private Vector3 playerShootDirection;
    private float playerShootForce;
    private bool isPlayerShooting = false;

    private Vector3 aiShootDirection;
    private float aiShootForce;
    private bool isAIShooting = false;

    void Start()
    {
        UpdateRoundsText();
        InitializeMarbles();
    }
    
    public void UpdateRoundsText()
    {
        if (roundsText != null)
        {
            roundsText.text = rounds.ToString(); // Convierte el número a texto y lo muestra
        }
    }

    void InitializeMarbles()
    {
        playerMarble.GetComponent<Renderer>().material.color = rojo;
        aiMarble.GetComponent<Renderer>().material.color = azul;

        foreach (var neutralMarble in neutralMarbles)
        {
            neutralMarble.GetComponent<Renderer>().material.color = gris;
        }
    }

    private void FixedUpdate()
    {
        if (isGameOver) return;

        // Limitar velocidad y posición de las canicas
        LimitMarbleVelocityAndPosition(playerMarble);
        LimitMarbleVelocityAndPosition(aiMarble);

        // Disparo del jugador en FixedUpdate
        if (isPlayerShooting)
        {
            playerMarble.GetComponent<Rigidbody>().AddForce(playerShootDirection * playerShootForce, ForceMode.Impulse);
            isPlayerShooting = false;
        }

        // Disparo de la IA en FixedUpdate
        if (isAIShooting)
        {
            aiMarble.GetComponent<Rigidbody>().AddForce(aiShootDirection * aiShootForce, ForceMode.Impulse);
            isAIShooting = false;
        }
    }

    void Update()
    {
        if (isGameOver) return;

        if (isPlayerTurn)
        {
            HandlePlayerInput();
        }
    }

    void HandlePlayerInput()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == playerMarble)
                {
                    StartCoroutine(PlayerShoot());
                }
            }
        }
    }

    IEnumerator PlayerShoot()
    {
        Vector2 startPos = Input.GetTouch(0).position;
        yield return new WaitUntil(() => Input.GetTouch(0).phase == TouchPhase.Ended);
        Vector2 endPos = Input.GetTouch(0).position;
        Vector2 swipeVector = endPos - startPos;

        playerShootForce = Mathf.Clamp(swipeVector.magnitude * 0.1f, 0, 200f);
        playerShootDirection = new Vector3(swipeVector.x, 0, swipeVector.y).normalized;
        isPlayerShooting = true;

        yield return new WaitForSeconds(2.0f);

        if (!isGameOver)
        {
            isPlayerTurn = false;
            StartCoroutine(AITurn());
        }
    }

    IEnumerator AITurn()
    {
        yield return new WaitForSeconds(2.0f);

        GameObject target = GetBestTarget();
        if (target != null)
        {
            aiShootDirection = (target.transform.position - aiMarble.transform.position).normalized;
            aiShootForce = 45f;
            isAIShooting = true;
        }

        yield return new WaitForSeconds(2.0f);

        if (rounds >1)
        {
            rounds--;
            UpdateRoundsText();
            isPlayerTurn = true;
        }
        else
        {
            CheckMarbleColors();
        }
    }

    void LimitMarbleVelocityAndPosition(GameObject marble)
    {
        if (marble != null && ground != null)
        {
            Rigidbody rb = marble.GetComponent<Rigidbody>();

            // Limitar velocidad
            if (rb.velocity.magnitude > 75.0f)
            {
                rb.velocity = rb.velocity.normalized * 75.0f;
            }

            // Limitar posición dentro del área del Ground
            Collider groundCollider = ground.GetComponent<Collider>();
            if (groundCollider != null)
            {
                Bounds groundBounds = groundCollider.bounds;

                Vector3 position = marble.transform.position;
                position.x = Mathf.Clamp(position.x, groundBounds.min.x, groundBounds.max.x);
                position.z = Mathf.Clamp(position.z, groundBounds.min.z, groundBounds.max.z);

                marble.transform.position = position;
            }
        }
    }

    GameObject GetBestTarget()
    {
        float closestDistance = Mathf.Infinity;
        GameObject bestTarget = null;

        foreach (var neutralMarble in neutralMarbles)
        {
            if (neutralMarble.CompareTag("NeutralMarble"))
            {
                float distance = Vector3.Distance(aiMarble.transform.position, neutralMarble.transform.position);
                var marbleColor = neutralMarble.GetComponent<Renderer>().material.color;
                if ((marbleColor == gris || marbleColor == rojo) && distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = neutralMarble;
                }
            }
        }

        return bestTarget;
    }

    void CheckMarbleColors()
    {
        int redCount = 0;
        int blueCount = 0;

        foreach (var marble in neutralMarbles)
        {
            Color marbleColor = marble.GetComponent<Renderer>().material.color;
            if (marbleColor == rojo) redCount++;
            else if (marbleColor == azul) blueCount++;
        }

        if (redCount > blueCount)
        {
            rounds--;
            EndGame(true);
            youwin.SetActive(true);
        }
        else if (blueCount >= redCount)
        {
            rounds--;
            EndGame(false);
            youlose.SetActive(true);
        }
    }

    void EndGame(bool playerWins)
    {
        isGameOver = true;
        string resultMessage = playerWins ? "Player Wins!" : "AI Wins!";
        Debug.Log(resultMessage);
    }
}


