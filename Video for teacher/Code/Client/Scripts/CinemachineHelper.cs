using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineHelper : MonoBehaviour
{

    public GameObject Player;
    public float DivisionFactor;

    void Update()
    {
        transform.position = new Vector2(Player.GetComponent<Transform>().position.x, Player.GetComponent<Transform>().position.y / DivisionFactor);
    }
}
