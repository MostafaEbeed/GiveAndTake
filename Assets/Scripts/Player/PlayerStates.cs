using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    [SerializeField] PlayerProgressionMathSo playerProgressionMathSo;

    [SerializeField] int rank = 1;
    [SerializeField] float speed;

    public int Rank => rank;

    public float Speed
    {
        get { return playerProgressionMathSo.speedRankRelation.Evaluate(rank); }
    }

    public void BoostSpeedByRank(int _rank)
    {
        rank = _rank;
    }
}
