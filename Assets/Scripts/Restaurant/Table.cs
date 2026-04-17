using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Table : MonoBehaviour
{
    public List<Seat> seats { get; private set; }


    public bool isTaken { get; private set; } = false;


    private void Awake()
    {
        seats = GetComponentsInChildren<Seat>().ToList();

        RestaurantManager.Instance.RegisterTable(this);
    }

    public void Take() => isTaken = true;

    public void UnTake() => isTaken = false;


    public Seat GetRandomSeat()
    {
        if (seats.Count == 0)
            return null;

        int randomIndex = Random.Range(0, seats.Count);
        Seat chosenSeat = seats[randomIndex];
        seats.RemoveAt(randomIndex);
        return chosenSeat;
    }

    public void ReturnSeat(Seat returnSeat) => seats.Add(returnSeat);


}
