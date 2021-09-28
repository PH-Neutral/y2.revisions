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
    [HideInInspector] public Vector2 camBoundMin, camBoundMax;

    private void Awake() {
        if(instance == null) instance = this;
        else if(instance != this) Destroy(gameObject);

        Vector2[] camBounds = Utils.GetCameraBounds();
        camBoundMin = camBounds[0];
        camBoundMax = camBounds[1];
    }
}