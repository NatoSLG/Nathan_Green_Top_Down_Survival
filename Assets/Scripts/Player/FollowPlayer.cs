using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Vector3 screen;
    public Transform player;//reference to targeting player

    void Start()
    {
        // Find the player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found in the scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + screen;
    }
}
