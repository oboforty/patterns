using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorkerScript : MonoBehaviour
{

    public enum WorkerState { 
        Idle = 0,

        Locating = 1,
        Finding = 2, PickingUp = 3,
        Carrying = 4, PuttingDown = 5,
        Returning = 6
    };

    private Queue<Vector3> Path = new Queue<Vector3>(4);
    public GameObject CarriedItem = null;
    public GameObject TargetContainer = null;

    public WorkerState State = WorkerState.Idle;

    private Animator anim;
    private NavMeshAgent agent;
    private Transform handBone;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        // Find hand's bone:
        handBone = transform.FindDeepChild("mixamorig:RightHand");

        agent.stoppingDistance = 1.1f;
        agent.angularSpeed = 168;
        //agent.baseOffset = 1;
    }

    void Update()
    {
        switch (State)
        {
            case WorkerState.Idle:
                break;
            case WorkerState.Locating:

                CarriedItem.GetComponent<NavMeshObstacle>().enabled = false;

                // animate
                agent.SetDestination(Path.Dequeue());
                anim.SetBool("Moving", true);
                anim.SetBool("Carrying", false);

                agent.speed = 2.3f;
                State += 1;
                break;
            case WorkerState.Finding:
                if (IsAtTarget())
                {
                    // if at target, initiate pickup animation
                    anim.SetTrigger("Gather");

                    //anim.SetBool("Moving", false);

                    State += 1;
                }
                break;
            case WorkerState.PickingUp:
                var animState = anim.GetCurrentAnimatorStateInfo(0);

                if (animState.IsName("Pickup"))
                {
                    // wait until animation is over
                    if (animState.normalizedTime >= 0.95f)
                    {
                        // lock picked up item:
                        SetParent(CarriedItem, transform, new Vector3(0, 0.7f, 0.55f));

                        //CarriedItem.GetComponent<NavMeshObstacle>().enabled = false;

                        anim.SetBool("Moving", true);
                        anim.SetBool("Carrying", true);

                        // Next target
                        agent.speed = 1.8f;
                        agent.SetDestination(Path.Dequeue());
                        State += 1;
                    }
                    else if (animState.normalizedTime >= 0.77f)
                    {
                        // animate picked up item too
                        CarriedItem.transform.localPosition = new Vector3(
                            CarriedItem.transform.localPosition.x,
                            handBone.transform.position.y - 0.3f,
                            CarriedItem.transform.localPosition.z
                        );
                    }
                }
                break;
            case WorkerState.Carrying:
                if (IsAtTarget())
                {
                    // if at target, initiate put down animation
                    anim.SetTrigger("Drop");

                    //anim.SetBool("Moving", false);
                    //anim.SetBool("Carrying", false);

                    State += 1;
                }
                break;
            case WorkerState.PuttingDown:
                var animState2 = anim.GetCurrentAnimatorStateInfo(0);

                if (animState2.IsName("Put Down 0") && animState2.normalizedTime >= 0.9f) {
                    // Leave carried item behind
                    CarriedItem.GetComponent<NavMeshObstacle>().enabled = true;
                    SetParent(CarriedItem, TargetContainer.transform);
                    CarriedItem = null;

                    // bugfix for y position (animation gets messy)
                    transform.position = new Vector3(transform.position.x, 0, transform.position.z);


                    // anim
                    anim.SetBool("Moving", true);
                    anim.SetBool("Carrying", false);

                    // Next target
                    agent.SetDestination(Path.Dequeue());
                    agent.speed = 2.3f;
                    State += 1;
                }
                else if (animState2.IsName("Put Down") && animState2.normalizedTime <= 0.77f)
                {
                    // animate picked up item too
                    CarriedItem.transform.localPosition = new Vector3(
                        CarriedItem.transform.localPosition.x,
                        handBone.transform.position.y - 0.3f,
                        CarriedItem.transform.localPosition.z
                    );
                }

                break;
            case WorkerState.Returning:
                if (IsAtTarget())
                {
                    anim.SetBool("Moving", false);
                    anim.SetBool("Carrying", false);

                    // Reset state
                    Path.Clear();
                    State = WorkerState.Idle;
                }
                break;
        }

        //float step = Speed * Time.deltaTime;
        //transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, step);
    }

    private void SetParent(GameObject item, Transform par, Vector3? RelPos = null)
    {
        item.transform.parent = par;

        item.transform.localPosition = RelPos != null ? (Vector3)RelPos : Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = Vector3.one;
    }

    public void AddTarget(GameObject obj, GameObject target, Vector3 position_return)
    {
        // Start moving towards object
        State = WorkerState.Locating;
        CarriedItem = obj;
        TargetContainer = target;

        // Full path of work:
        Path.Enqueue(obj.transform.position);
        Path.Enqueue(target.transform.position);
        Path.Enqueue(position_return);
        //agent.SetDestination()
    }

    public bool IsAtTarget()
    {
        //if (Path.Count == 0)
        //    return false;

        if (State != WorkerState.Carrying && State != WorkerState.Returning && State != WorkerState.Finding)
            return false;

        return (agent.remainingDistance <= agent.stoppingDistance && (agent.hasPath || agent.velocity.sqrMagnitude == 0f));
    }
}
