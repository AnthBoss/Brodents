using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkunkSpray : MonoBehaviour
{

    private Vector2 targetPos;
    public float duration;

    public void Start()
    {
        StartCoroutine(LerpPositionSkunk(targetPos));
   
    }

    IEnumerator LerpPositionSkunk(Vector2 targetPosition)
    {
        float time = 0;
        Vector2 startPosition = transform.position;
        while (time < duration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }

    public void SetPositionToMoveTo(Vector2 pos)
    {
        targetPos = pos;
    }
}
