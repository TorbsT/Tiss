using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    private enum Group
    {
        none,
        player,
        zombies
    }
    [SerializeField] private Group team;

    public bool CanInjure(Team other)
    {
        return other.team != team;
    }
}
