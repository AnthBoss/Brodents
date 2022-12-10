using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firefly : MonoBehaviour
{

    private float rNumx;
    private float rNumy;

    private Vector2 startPosition;

    private bool canMove = true;
    [SerializeField] private float durationLow;
    [SerializeField] private float durationHigh;
    [SerializeField] private float range;

    private float randomDuration;

    void Update()
    {
        if(canMove)
        {
            startPosition = transform.position;
            canMove = false;
            StartCoroutine(Move());
        }
        
    }

    private IEnumerator Move()
    {
        float time = 0f;
        rNumx = Random.Range(-range, range);
        rNumy = Random.Range(-range, range);
        randomDuration = Random.Range(durationLow, durationHigh);
        while (time < randomDuration)
        {
            transform.position = Vector2.Lerp(startPosition, new Vector2(startPosition.x + rNumx, startPosition.y + rNumy), time / randomDuration);
            time += Time.deltaTime;
            yield return null;
        }
        canMove = true;
    }
}
