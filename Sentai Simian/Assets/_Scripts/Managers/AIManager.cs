using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class AITokenPool {
    public const int INVALID_TOKEN_ID = -1;

    [SerializeField, Min(0)] private int totalTokens = 1;
    [SerializeField] private float tokenCooldown = 0.0f;

    private Queue<int> availableTokenIndices = new();
    private List<float> tokens = new();


    public void Initialize() {
        for (int i = 0; i < totalTokens; i++) {
            availableTokenIndices.Enqueue(i);
            tokens.Add(0.0f);
        }
    }

    public void Update() {
        foreach (var id in availableTokenIndices) {
            if (tokens[id] > 0f) {
                tokens[id] -= Time.deltaTime;
            }
        }
    }

    public bool TryClaimToken(out int tokenID) {
        if (availableTokenIndices.TryPeek(out var id) && tokens[id] <= 0.0f) {
            tokenID = availableTokenIndices.Dequeue();
            return true;
        }

        tokenID = INVALID_TOKEN_ID;
        return false;
    }

    public void ReturnToken(ref int tokenID) {
        if (tokenID == INVALID_TOKEN_ID) {
            return;
        }

        if (tokenID < 0 || tokenID >= tokens.Count) {
            Debug.LogError($"Trying to return an invalid AI token ID. TokenID: {tokenID}");
            return;
        }

        availableTokenIndices.Enqueue(tokenID);
        tokens[tokenID] = tokenCooldown;
        tokenID = INVALID_TOKEN_ID;
    }
}

public class AIManager : Singleton<AIManager> {
    public AITokenPool lightMeleeTokens;
    public AITokenPool lightRangedTokens;
    public AITokenPool heavyMeleeTokens;
    public AITokenPool heavyRangedTokens;

    private void Start() {
        lightMeleeTokens.Initialize();
        lightRangedTokens.Initialize();
        heavyMeleeTokens.Initialize();
        heavyRangedTokens.Initialize();
    }

    private void Update() {
        lightMeleeTokens.Update();
        lightRangedTokens.Update();
        heavyMeleeTokens.Update();
        heavyRangedTokens.Update();
    }
}
