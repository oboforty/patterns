using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MK.Prefab.Manager;
using MK.Prefab.API;
using UnityEngine.AI;

public class HarborScript : MonoBehaviour
{
    public GameObject workers_spawn;
    public GameObject items_spawn;
    public GameObject soldiers_spawn;

    public GameObject NavMeshObject;

    private List<GameObject> workers = new List<GameObject>();
    public List<GameObject> ships = new List<GameObject>();
    //private List<GameObject> soldiers = new List<GameObject>();

    class CarryTask
    {
        public GameObject Item;
        public GameObject Container;
    }

    private Queue<CarryTask> Tasks = new Queue<CarryTask>();


    void Start()
    {
        CreateItem();
        CreateItem();
        CreateItem();

        CreateWorker();

        SetupNavMesh();
        BakeNavMesh();
    }

    void Update()
    {
        // TODO: ITT: return worker to queue after he's done
        // TODO: ITT: extend script so that you can transform back items

        // TODO: spawn 5 workers
        // TODO: don't break when there's no worker available (queue items)

        if (Tasks.Count > 0)
        {
            var worker = workers.Find(w => w.GetComponent<WorkerScript>().CarriedItem == null);

            if (worker != null)
            {
                var task = Tasks.Dequeue();
                worker.GetComponent<WorkerScript>().AddTarget(task.Item, task.Container, worker.transform.position);

                BakeNavMesh();
            }
        }


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                var item = hit.transform.parent.gameObject;

                if (hit.transform != null && item.tag == "Item")
                {
                    var cont = hit.transform.parent.parent.gameObject;


                    CarryTask task = new CarryTask();

                    // From ship to container | from container to ship
                    task.Container = cont.tag == "Ship" ? items_spawn : ships[0];
                    task.Item = item;

                    Tasks.Enqueue(task);
                }
            }
        }
    }
    
    void BakeNavMesh()
    {
        var navMesh = NavMeshObject.GetComponent<NavMeshSurface>();
        navMesh.BuildNavMesh();
    }

    void SetupNavMesh()
    {
        var navMesh = NavMeshObject.GetComponent<NavMeshSurface>();
        navMesh.ignoreNavMeshAgent = false;
        navMesh.ignoreNavMeshObstacle = false;

        BakeNavMesh();

        foreach (var worker in workers)
        {
            var agent = worker.AddComponent<NavMeshAgent>() as NavMeshAgent;
        }
    }

    void CreateItem()
    {
        // place items in a center
        // @todo: later: have slots? idk -- cannons in grid, workers in random, boosters in circle
        Vector2 pos2 = Random.insideUnitCircle * 3f;

        var item = PrefabManager.Instance.Spawn(PrefabType.Item, items_spawn.transform, new Vector3(pos2.x, 0, pos2.y));
    }

    void CreateWorker()
    {
        // place items in a center
        // @todo: later: have slots? idk -- cannons in grid, workers in random, boosters in circle
        Vector2 pos2 = Random.insideUnitCircle * 3f;
        Vector3 pos = workers_spawn.transform.position;
        pos.x += pos2.x;
        pos.z += pos2.y;

        var worker = PrefabManager.Instance.Spawn(PrefabType.Worker, pos);
        //var wsc = worker.GetComponent<WorkerScript>();
        //wsc.PickupDistance = PickupDistance;

        workers.Add(worker);
    }

}
