using System.Collections.Generic;
using UnityEngine;

public class RhythmBattleSystem : MonoBehaviour
{
    // A Máquina de Estados: define exatamente o que está acontecendo no momento
    public enum BattleState { Waiting, PlayerTurn, RhythmMinigame, EnemyTurn }
    public BattleState currentState = BattleState.Waiting;

    [Header("Stats")]
    public int instrumentHealth = 100;
    public int wearDamage = 15;

    [Header("Rhythm Mechanics")]
    public float timeLimit = 6f;
    private float timer;
    public int sequenceLength = 5;

    // Lista para guardar as setas sorteadas e controlar onde o jogador está
    private List<KeyCode> currentSequence = new List<KeyCode>();
    private int inputIndex = 0;

    // As opções de setas disponíveis
    private KeyCode[] arrowKeys = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

    void OnTriggerEnter(Collider other)
    {
        // Se quem encostou tem a tag "Player" e o script estava apenas esperando
        if (other.CompareTag("Player") && currentState == BattleState.Waiting)
        {
            Debug.Log("BATTLE STARTED! Press SPACE to attack.");
            currentState = BattleState.PlayerTurn;
        }
    }

    void Update()
    {
        // 2. Controlando o fluxo baseado no Estado Atual
        if (currentState == BattleState.PlayerTurn)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartRhythmAttack();
            }
        }
        else if (currentState == BattleState.RhythmMinigame)
        {
            HandleRhythmInput();
        }
    }

    // 3. Preparando o Minigame
    void StartRhythmAttack()
    {
        currentState = BattleState.RhythmMinigame;
        timer = timeLimit;
        inputIndex = 0;
        currentSequence.Clear();

        // Sorteia as setas e guarda na nossa lista
        for (int i = 0; i < sequenceLength; i++)
        {
            currentSequence.Add(arrowKeys[Random.Range(0, arrowKeys.Length)]);
        }

        Debug.Log("6 SECONDS ON THE CLOCK! GO!");
        foreach (KeyCode key in currentSequence)
        {
            Debug.Log("Press: " + key); // Mostra a sequência no console para você testar
        }
    }

    // 4. Lendo as teclas do jogador
    void HandleRhythmInput()
    {
        // O cronômetro corre
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            FailAttack("Time out!");
            return;
        }

        // Se o jogador apertou QUALQUER tecla nesse frame
        if (Input.anyKeyDown)
        {
            KeyCode expectedKey = currentSequence[inputIndex];

            // Se for exatamente a tecla que ele precisava apertar agora
            if (Input.GetKeyDown(expectedKey))
            {
                Debug.Log("Hit: " + expectedKey);
                inputIndex++; // Avança para a próxima nota

                // Se passou por todas as notas, sucesso!
                if (inputIndex >= currentSequence.Count)
                {
                    SuccessfulAttack();
                }
            }
            // Se ele errou, mas a tecla que ele apertou foi uma das setas
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                     Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                FailAttack("Wrong note!");
            }
        }
    }

    void SuccessfulAttack()
    {
        Debug.Log("SUCCESS! Massive damage to the enemy!");
        EndTurn();
    }

    void FailAttack(string reason)
    {
        instrumentHealth -= wearDamage;
        Debug.Log("FAIL: " + reason + " | Instrument health: " + instrumentHealth);

        if (instrumentHealth <= 0)
        {
            Debug.Log("The instrument is broken!");
            // Lógica de Game Over ou perda de dano
        }
        EndTurn();
    }

    void EndTurn()
    {
        currentState = BattleState.EnemyTurn;
        Debug.Log("Enemy's Turn...");

        // Aqui você chamaria a lógica do inimigo, e depois voltaria para o PlayerTurn
    }
}
