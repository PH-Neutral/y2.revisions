using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    public static Fencer.SwordPosture[] swordPostures = new Fencer.SwordPosture[] { 
        Fencer.SwordPosture.Low, Fencer.SwordPosture.Medium, Fencer.SwordPosture.High };

    public static Fencer.SwordPosture GetPostureUp(this Fencer.SwordPosture currentPosture) {
        return (Fencer.SwordPosture)Mathf.Clamp((int)currentPosture + 1, 0, swordPostures.Length - 1);
    }
    public static Fencer.SwordPosture GetPostureDown(this Fencer.SwordPosture currentPosture) {
        return (Fencer.SwordPosture)Mathf.Clamp((int)currentPosture - 1, 0, swordPostures.Length - 1);
    }

    public static Vector2[] GetCameraBounds() {
        Camera cam = Camera.main;
        Vector2 halfSize = new Vector2(cam.orthographicSize * cam.aspect, cam.orthographicSize);

        return new Vector2[] { (Vector2)cam.transform.position - halfSize, (Vector2)cam.transform.position + halfSize };
    }
}