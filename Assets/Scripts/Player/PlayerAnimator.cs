using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    //References
    Animator am;
    PlayerMovement pm;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        am = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
    }

    void PlayerMovement()
    {
        // Check if the player is running
        if (Input.GetKey(KeyCode.LeftShift) && (pm.movement.x != 0 || pm.movement.y != 0))
        {
            PlayerRun();
        }
        else if (pm.movement.x != 0 || pm.movement.y != 0)
        {
            PlayerWalk();
        }
        else
        {
            StopMovement();
        }
    }

    void PlayerWalk()
    {
        am.SetBool("PlayerWalk", true);
        am.SetBool("PlayerRun", false);  // Ensure running animation is not playing

        SpriteDirectionTracker();
    }

    void PlayerRun()
    {
        am.SetBool("PlayerRun", true);
        am.SetBool("PlayerWalk", false);  // Ensure walking animation is not playing

        SpriteDirectionTracker();
    }

    void StopMovement()
    {
        am.SetBool("PlayerWalk", false);
        am.SetBool("PlayerRun", false);
    }

    void SpriteDirectionTracker()
    {
        if (pm.lastHorizontalVector < 0)
        {
            //flips sprits facing left
            sr.flipX = true;
        }
        else
        {
            //keeps sprite facing right or flips sprite to face right
            sr.flipX = false;
        }
    }

}
