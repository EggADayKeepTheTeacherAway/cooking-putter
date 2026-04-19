using UnityEngine;

public enum SeatTypes
{
    Top,
    Bottom
}

public class Seat : MonoBehaviour
{
    public SeatTypes seatType;
    public Vector2 GetSeatPos() => transform.position;
}
