using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FencingManager : MonoBehaviour {
    [System.Serializable]
    public struct PlayerSettings {
        public KeyCode keyMoveRight, keyMoveLeft, keyUp, keyDown, keyJump, keyAttack;
        public Color color;
    }

    public static FencingManager instance = null;

    public float groundOffset;
    public PlayerSettings playerSet1, playerSet2;
    public Vector2[] swordOffsets;
    public Vector2 swordAttackVector;
    public float swordAttackDuration;
    public Fencer player1, player2;
    [HideInInspector] public Vector2 camBoundMin, camBoundMax;

    private void Awake() {
        if(instance == null) instance = this;
        else if(instance != this) Destroy(gameObject);

        Vector2[] camBounds = Utils.GetCameraBounds();
        camBoundMin = camBounds[0];
        camBoundMax = camBounds[1];
    }

    private void Update() {
        // queue fencer flipping to make them face each other
        Fencer pLeft, pRight;
        if(player1.transform.position.x != player2.transform.position.x) {
            if(player1.transform.position.x < player2.transform.position.x) {
                pLeft = player1; pRight = player2;
            } else {
                pLeft = player2; pRight = player1;
            }
            if(!pLeft.IsFacingRight) pLeft.QueueFlip();
            else pLeft.UnqueueFlip();

            if(pRight.IsFacingRight) pRight.QueueFlip();
            else pRight.UnqueueFlip();
        }
    }
}