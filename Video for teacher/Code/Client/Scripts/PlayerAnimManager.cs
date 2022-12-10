using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float playerMoveSpeed;

    private Vector3 lastPosition;

    private void Start()
    {

    }

    public void AnimateBasedOnSpeed()
    {
        lastPosition.y = transform.position.y;
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        animator.SetBool("IsMoving", distanceMoved > 0.01f);

        lastPosition = transform.position;
    }

    public void SkunkPrimed(bool myBool)
    {
        animator.SetBool("IsPrimed", myBool);
    }
}
