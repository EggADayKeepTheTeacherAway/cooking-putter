
using UnityEngine;

public class SortingOrderManager : MonoBehaviour
{
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();        
    }

    private void LateUpdate()
    {
        sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10);
    }
}
