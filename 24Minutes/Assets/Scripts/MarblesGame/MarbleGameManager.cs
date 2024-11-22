using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MarbleGameManager : MonoBehaviour
{
    public GameObject ground;
    public GameObject playerMarble; // La canica del jugador (Roja)
    public GameObject aiMarble; // La canica de la IA (Azul)
    public List<GameObject> neutralMarbles; // Lista de canicas neutrales (Grises)
    public GameObject youwin;
    public GameObject youlose;
    
    public Color gris = Color.gray;
    public Color azul = Color.blue;
    public Color rojo = Color.red;
    
    public bool isPlayerTurn = true;
    public bool isGameOver = false;

    void Start()
    {
        InitializeMarbles();
    }

    void InitializeMarbles()
    {
        /*Rigidbody rb = playerMarble.GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;*/
        playerMarble.GetComponent<Renderer>().material.color = rojo;
        aiMarble.GetComponent<Renderer>().material.color = azul;

        foreach (var neutralMarble in neutralMarbles)
        {
            neutralMarble.GetComponent<Renderer>().material.color = gris;
        }
    }

    private void FixedUpdate()
    {
            if (playerMarble != null)
            {
                Rigidbody rb = playerMarble.GetComponent<Rigidbody>();
                if (rb.velocity.magnitude > 200.0f) // Ajusta el límite según sea necesario
                {
                    rb.velocity = rb.velocity.normalized * 200.0f;
                }
                if (playerMarble != null && ground != null) // Aseguramos que las referencias existan
                {
                    // Obtenemos el tamaño del área del objeto Ground
                    Collider groundCollider = ground.GetComponent<Collider>();
                    if (groundCollider != null)
                    {
                        Bounds groundBounds = groundCollider.bounds;

                        // Obtenemos la posición actual de la canica
                        Vector3 position = playerMarble.transform.position;

                        // Limitamos la posición X y Z dentro de los límites del objeto Ground
                        position.x = Mathf.Clamp(position.x, groundBounds.min.x, groundBounds.max.x);
                        position.z = Mathf.Clamp(position.z, groundBounds.min.z, groundBounds.max.z);

                        // Actualizamos la posición de la canica
                        playerMarble.transform.position = position;
                    }
                }
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
        float force = swipeVector.magnitude * 0.1f;
        if (force > 200.0f)
        {
            force = 200.0f;
        }
        Debug.Log($"Ball shot with force of {force}");

        Vector3 shootDirection = new Vector3(swipeVector.x, 0, swipeVector.y).normalized;
        playerMarble.GetComponent<Rigidbody>().AddForce(shootDirection * force, ForceMode.Impulse);
        
        yield return new WaitForSeconds(3.0f);
        isPlayerTurn = false;
        
        
        CheckMarbleColors();
        if (!isGameOver) StartCoroutine(AITurn());
    }

    IEnumerator AITurn()
    {
        yield return new WaitForSeconds(1.0f); // Pausa antes de lanzar la canica de la IA

        GameObject target = GetBestTarget();
        if (target != null)
        {
            Vector3 direction = (target.transform.position - aiMarble.transform.position).normalized;
            float force = 45f;
            aiMarble.GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(1.5f);

        CheckMarbleColors();
        if (!isGameOver) isPlayerTurn = true;
    }

    GameObject GetBestTarget()
    {
        float closestDistance = Mathf.Infinity;
        GameObject bestTarget = null;

        foreach (var neutralMarble in neutralMarbles) // Neutral marbles or gray marbles?
        {
            if (neutralMarble.CompareTag("NeutralMarble"))
            {
                float distance = Vector3.Distance(aiMarble.transform.position, neutralMarble.transform.position);

                // Nos aseguramos de que solo seleccione canicas grises o rojas
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

        if (redCount == neutralMarbles.Count)
        {
            EndGame(true); 
            youwin.SetActive(true);
        }
        else if (blueCount == neutralMarbles.Count) EndGame(false); youwin.SetActive(false);
        
    }

    void EndGame(bool playerWins)
    {
        isGameOver = true;
        string resultMessage = playerWins ? "Player Wins!" : "AI Wins!";
        Debug.Log(resultMessage);
    }
}

