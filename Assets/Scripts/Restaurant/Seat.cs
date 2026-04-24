using UnityEngine;

public enum SeatType
{
    Top,
    Bottom
}

public enum ApproachSide
{
    None,
    Left,
    Right
}

public class Seat : MonoBehaviour
{
    public SeatType seatType;
    public ApproachSide approachSide;
    public Vector2 GetSeatPos() => transform.position;
}
