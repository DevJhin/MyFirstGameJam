﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// 카메라 로직들은 전부 여기서 조작
/// </summary>
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Camera")]
    /// <summary>
    /// 디버그 모드
    /// </summary>
    public bool DebugMode = false;

    /// <summary>
    /// 따라갈 타겟
    /// </summary>
    public Transform FollowTarget;

    [Header("Camera Follow Variables")]
    /// <summary>
    /// 타겟 Y Offset
    /// </summary>
    public float verticalOffset;

    /// <summary>
    /// 카메라가 움직일 방향으로 타겟보다 얼마나 떨어져있을 것인지?
    /// </summary>
    public float lookAheadDstX;

    /// <summary>
    /// 카메라가 타겟을 쫓아가는데 걸리는 시간 X
    /// </summary>
    public float xFollowDuration;

    /// <summary>
    /// 카메라가 타겟을 쫓아가는데 걸리는 시간 Y
    /// </summary>
    public float yFolowDuration;

    /// <summary>
    /// 카메라가 타겟을 따라가기 시작하는 카메라와 타겟의 거리 XY
    /// </summary>
    public Vector2 focusAreaSize;

    /// <summary>
    /// 따라다니는 타겟의 Bound
    /// </summary>
    private Bounds targetFocusBound = new Bounds();

    /// <summary>
    /// targetFocusBound의 크기
    /// </summary>
    public float targetFocusBoundSize = 0.5f;

    /// <summary>
    /// 화면 전환 이펙트 조절
    /// </summary>
    private SimplePostFX transitionFX;

    [Header("Camera Boundaries")]
    /// <summary>
    /// 카메라 Boundaries
    /// </summary>
    public float leftLimit;

    public float rightLimit;
    public float bottomLimit;
    public float topLimit;

    /// <summary>
    /// Letterbox 관련 설정
    /// </summary>
    [Header("Letter box")]

    /// <summary>
    /// 게임 보여주는 카메라
    /// </summary>
    public Camera MainGameCamera;

    /// <summary>
    /// 레터박스 전용 카메라
    /// </summary>
    public Camera LetterBoxCamera;

    /// <summary>
    /// 희망하는 해상도 X값
    /// </summary>
    public float desiredXRatio = 4;

    /// <summary>
    /// 희망하는 해상도 Y값
    /// </summary>
    public float desiredYRatio = 3;

    private Vector3 finalPos;

    //Follow Variables
    private FocusArea focusArea;
    private Vector3 prevPlayerPos;
    private float currentLookAheadX;
    private float targetLookAheadX;
    private float lookAheadDirX;
    private float smoothLookVelocityX;
    private float smoothVelocityY;
    private bool lookAheadStopped;
    private bool initialized;

    //Screen Shake Variables
    private float shakeTimeRemaining;
    private float shakePower;
    private float shakeFadeTime;
    private float shakeRotation;
    private float rotationMultiplier;
    private bool isRotating;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        transitionFX = GetComponentInChildren<SimplePostFX>();
    }

    void Start()
    {
        Rect rect = MainGameCamera.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / (desiredXRatio / desiredYRatio); // (가로 / 세로)
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }
        MainGameCamera.rect = rect;
    }

    void OnDestroy()
    {
        Instance = null;
    }

    //Stage에서 실행
    public void Setup(Transform followTarget)
    {
        FollowTarget = followTarget.transform;
        targetFocusBound.center = FollowTarget.position;
        targetFocusBound.size = new Vector3(targetFocusBoundSize, targetFocusBoundSize, 1);

        focusArea = new FocusArea(targetFocusBound, focusAreaSize);
        initialized = true;
    }

    void LateUpdate()
    {
        if (initialized)
        {
            Follow();

            Clamp();

            ScreenShake();

            if (transform.position != finalPos)
            {
                transform.position = finalPos;
            }
        }
    }

    //LateUpdate에서 실행
    private void Follow()
    {
        // 타겟 없으면 안 따라감.
        if (FollowTarget == null)
            return;

        targetFocusBound.center = FollowTarget.position;
        targetFocusBound.size = new Vector3(targetFocusBoundSize, targetFocusBoundSize, 1);
        focusArea.Update(targetFocusBound);
        Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;
        if (!Mathf.Approximately(focusArea.velocity.x, 0))
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            float playerDirX = Mathf.Sign(FollowTarget.position.x - prevPlayerPos.x);
            prevPlayerPos = FollowTarget.position;
            if (Mathf.Approximately(playerDirX, Mathf.Sign(focusArea.velocity.x)) &&
                !Mathf.Approximately(playerDirX, 0))
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDstX;
            }
            else
            {
                if (!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                }
            }
        }

        currentLookAheadX =
            Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, xFollowDuration);
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, yFolowDuration);
        focusPosition += Vector2.right * currentLookAheadX;
        finalPos = (Vector3) focusPosition + Vector3.forward * -10;
    }

    //LateUpdate에서 실행
    private void Clamp()
    {
        finalPos = new Vector3(Mathf.Clamp(finalPos.x, leftLimit, rightLimit),
            Mathf.Clamp(finalPos.y, bottomLimit, topLimit), finalPos.z);
    }

    //LateUpdate에서 실행
    private void ScreenShake()
    {
        if (shakeTimeRemaining > 0)
        {
            shakeTimeRemaining -= Time.deltaTime;
            //Random shake 방향
            float xAmount = Random.Range(-0.1f, 0.1f) * shakePower;
            float yAmount = Random.Range(-0.1f, 0.1f) * shakePower;
            //Transform XY + Rotation Z 움직임
            finalPos += new Vector3(xAmount, yAmount, 0);
            transform.rotation = Quaternion.Euler(0, 0, shakeRotation * Random.Range(-1, 1));
            //Shake timer 줄면서 shake power도 줄게됨
            shakePower = Mathf.MoveTowards(shakePower, 0, shakeFadeTime * Time.deltaTime);
            shakeRotation = Mathf.MoveTowards(shakeRotation, 0, shakeFadeTime * rotationMultiplier * Time.deltaTime);
        }
        //Rotation reset
        else if (isRotating)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            isRotating = false;
        }
    }

    /// <summary>
    /// Length: Shake 기간 | Power: Shake 힘 | rotPower: 회전 shake 힘 |
    /// 예: CameraManager.Instance.StartShake(0.4f, 0.4f, 0.3f);
    /// </summary>
    /// <param name="length"></param>
    /// <param name="power"></param>
    /// <param name="rotPower"></param>
    public void StartShake(float length, float power, float rotPower)
    {
        shakeTimeRemaining = length;
        shakePower = power;
        shakeFadeTime = power / length;
        rotationMultiplier = rotPower;
        shakeRotation = power * rotationMultiplier;
        isRotating = true;
    }

    public void StartTransition(float target, float duration)
    {
        StartCoroutine(transitionFX.PlayFxRoutine(transitionFX.Progress, target, duration));
    }

    struct FocusArea
    {
        public Vector2 centre;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;
            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }

            left += shiftX;
            right += shiftX;
            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }

            top += shiftY;
            bottom += shiftY;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
}
