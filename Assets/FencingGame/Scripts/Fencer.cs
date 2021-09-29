using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Fencer : MonoBehaviour {
    public enum SwordPosture {
        Low, Medium, High
    }

    public SwordPosture Posture {
        get { return _posture; }
        set {
            _posture = value;
            ChangeSwordPosture(value);
        }
    }
    public bool IsFacingRight {
        get { return transform.localScale.x > 1; }
    }

    [SerializeField] bool isPlayer1;
    [SerializeField] float moveSpeed, inAirMoveSpeed, jumpStrength;
    [SerializeField] Transform _handle;
    FencingManager.PlayerSettings playerSettings;
    Rigidbody2D _rb;
    SpriteRenderer _rend;
    BoxCollider2D _colFeet, _colSword;
    PolygonCollider2D _colBody;
    float _halfWidth, _height;

    Vector2 movement = Vector2.zero;
    bool _isGrounded = true, _wasGrounded = true, _isAttacking = false;
    SwordPosture _posture = SwordPosture.Medium;
    bool _flipQueued = false;


    private void Awake() {
        playerSettings = isPlayer1 ? FencingManager.instance.playerSet1 : FencingManager.instance.playerSet2;

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach(Collider2D col in colliders) {
            if(col is PolygonCollider2D) {
                _colBody = col as PolygonCollider2D;
            } else if(col is BoxCollider2D) {
                _colSword = col as BoxCollider2D;
            }
        }
        _rb = GetComponentInChildren<Rigidbody2D>();
        _rend = GetComponentInChildren<SpriteRenderer>();
        _rend.color = playerSettings.color;
        _halfWidth = _rend.sprite.bounds.extents.x * _rend.transform.lossyScale.x;// * wrapper.localScale.x;
        _height = _rend.sprite.bounds.size.y * _rend.transform.lossyScale.y;// * wrapper.localScale.y;

        Posture = _posture;
    }

    private void Update() {
        DoTheMove();
        DoTheSword();
    }

    private void LateUpdate() {
        Vector3 pos = transform.position;
        //Debug.Log("pos: " + pos);
        pos.x = Mathf.Clamp(pos.x, FencingManager.instance.camBoundMin.x + _halfWidth, FencingManager.instance.camBoundMax.x - _halfWidth);
        pos.y = Mathf.Clamp(pos.y, FencingManager.instance.camBoundMin.y + FencingManager.instance.groundOffset, FencingManager.instance.camBoundMax.y - _height);
        transform.position = pos;

        _wasGrounded = _isGrounded;
        _isGrounded = pos.y <= FencingManager.instance.camBoundMin.y + FencingManager.instance.groundOffset;
    }

    void DoTheMove() {
        movement = _rb.velocity;
        string strVelocity = "Base: " + movement;

        float xInputRaw = GetMoveAxisRaw();
        movement.x = xInputRaw * (_isGrounded ? moveSpeed : inAirMoveSpeed);

        if(!_isGrounded) movement += Physics2D.gravity * Time.deltaTime;
        else if(!_wasGrounded) movement.y = 0;
        strVelocity += "\nAfter move: " + movement;

        if(_isGrounded && Input.GetKeyDown(playerSettings.keyJump)) {
            movement.y = jumpStrength;
            //Debug.Log("JUMP!");
        }
        strVelocity += "\nAfter jump: " + movement;

        _rb.velocity = movement;
        //Debug.Log(strVelocity);
    }

    void DoTheSword() {
        if(_isAttacking) return;

        if(Input.GetKeyDown(playerSettings.keyUp)) {
            ChangeSwordPosture(true);
        }
        if(Input.GetKeyDown(playerSettings.keyDown)) {
            ChangeSwordPosture(false);
        }

        if(Input.GetKeyDown(playerSettings.keyAttack)) {
            //start the attack
            Debug.Log("Attack !");
            StartCoroutine(Attack());
            return;
        }
    }

    IEnumerator Attack() {
        _isAttacking = true;
        // make the sword move forward then back again
        float halfDuration = FencingManager.instance.swordAttackDuration * 0.5f;
        Vector3 originHandlePos = _handle.localPosition;
        Vector3 maxHandlePos = originHandlePos + (Vector3)FencingManager.instance.swordAttackVector;
        for(float t=0; t < halfDuration; t += Time.deltaTime) {
            _handle.localPosition = Vector3.Lerp(originHandlePos, maxHandlePos, t / halfDuration);
            yield return null;
        }
        _handle.localPosition = maxHandlePos;
        for(float t = 0; t < halfDuration; t += Time.deltaTime) {
            _handle.localPosition = Vector3.Lerp(maxHandlePos, originHandlePos, t / halfDuration);
            yield return null;
        }
        _handle.localPosition = originHandlePos;

        _isAttacking = false;
        if(_flipQueued) Flip();
    }

    public void QueueFlip() {
        if(!_isAttacking) {
            UnqueueFlip();
            Flip();
        } else {
            _flipQueued = true;
        }
    }

    public void UnqueueFlip() {
        _flipQueued = false;
    }

    void Flip() {
        Vector3 scale = transform.localScale;
        scale.x = -scale.x;
        transform.localScale = scale;
    }

    void ChangeSwordPosture(bool up) {
        Posture = up ? Posture.GetPostureUp() : Posture.GetPostureDown();
    }
    void ChangeSwordPosture(SwordPosture newPosture) {
        _handle.localPosition = FencingManager.instance.swordOffsets[(int)newPosture];
    }

    float GetMoveAxisRaw() {
        return (Input.GetKey(playerSettings.keyMoveRight) ? 1 : 0) - (Input.GetKey(playerSettings.keyMoveLeft) ? 1 : 0);
    }
}
