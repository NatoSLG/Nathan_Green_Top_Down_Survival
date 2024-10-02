using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class edits the sorting order of the objects the 
/// script is assigned to.
/// This will change the priority of the layer depending on the
/// Y axis of the player.
/// </summary>
public class DynamicSortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Adjust the sorting order based on the y position
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
    }
}
