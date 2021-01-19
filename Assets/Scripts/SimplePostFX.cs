using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePostFX : MonoBehaviour
{
    /// <summary>
    /// 스크린 효과를 위한 머터리얼
    /// </summary>
    public Material transitionMaterial;

    /// <summary>
    /// 진행도
    /// </summary>
    public float Progress = 1;

    /// <summary>
    /// 코루틴 아이디
    /// </summary>
    private int routineId = 0;

    void Awake()
    {
        transitionMaterial = new Material(Shader.Find("Hidden/TransitionEffect"));
        transitionMaterial.hideFlags = HideFlags.HideAndDontSave;
    }

    public IEnumerator PlayFxRoutine(float start, float target, float duration)
    {
        float timePassed = 0f;
        int currentRoutineId = ++routineId;

        while (timePassed <= duration)
        {
            // 다른 코루틴이 실행되면 종료한다.
            if (currentRoutineId != routineId)
            {
                yield break;
            }

            timePassed += Time.unscaledDeltaTime;
            Progress = Mathf.Lerp(start, target, timePassed / duration);

            yield return null;
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        transitionMaterial.SetFloat("_Progress", Progress);
        Graphics.Blit(src, dest, transitionMaterial);
    }
}
