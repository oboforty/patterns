using System;
using UnityEngine;

public class Minion : MonoBehaviour
{
    public MinionState State { get; protected set; } = MinionState.Idle;
    // Where the minion is moving to (free space)
    public Vector3Int MovingTo;
    // What the minion is intending to mine (existing block)
    public Block MiningTarget = null;
    // What the minion is carrying
    public BlockConfig carrying = null;

    public Animator anim;

    private void Awake()
    {
        //TryGetComponent(out anim);

        // @todo: set anim & shit
    }


    private void Update()
    {

        //switch(State)
        //{
        //    case MinionState.Idle:
        //        anim.SetBool("moving", false);
        //        anim.SetBool("mining", false);


        //        break;
        //    case MinionState.Mining:
        //        anim.SetBool("moving", false);
        //        anim.SetBool("mining", true);
        //        break;
        //    case MinionState.Moving:
        //        anim.SetBool("moving", true);
        //        anim.SetBool("mining", false);
        //        break;
        //}
    }
}