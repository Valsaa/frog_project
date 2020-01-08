using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class PathRequestManager : MonoBehaviour
{
    [Range(1, 5)]
    public int nbThreadPossible = 1;

    static PathRequestManager instance;

    Pathfinding pathfinding;
    Queue<PathRequest> requests = new Queue<PathRequest>();
    Queue<PathRequest> results = new Queue<PathRequest>();

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
        StopCoroutine(ProcessingPathThreadHandler());
        StartCoroutine(ProcessingPathThreadHandler());
    }

    private void Update()
    {
        if (results.Count > 0)
        {
            lock(results)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    PathRequest currentRequest = results.Dequeue();

                    currentRequest.callback(currentRequest);
                }
            }
        }
    }

    public static void RequestPath(PathRequest request)
    {
        lock (instance.requests)
        {
            Debug.Log("RequestPath");
            request.pipelineState = PathState.InQueue;
            instance.requests.Enqueue(request);
        }
    }

    IEnumerator ProcessingPathThreadHandler()
    {
        while(true)
        {
            if (requests.Count > 0)
            {
                int itemsInQueue = Mathf.Clamp(requests.Count, 1, nbThreadPossible);
                lock (requests)
                {
                    for (int i = 0; i < itemsInQueue; i++)
                    {
                        Debug.Log("Tread Created : " + i);
                        PathRequest currentRequest = requests.Dequeue();
                        currentRequest.pipelineState = PathState.InProcessing;
                        ThreadStart threadStart = delegate
                        {
                            instance.pathfinding.FindPath(currentRequest, FinishedProcessingPath);
                        };
                        //threadStart.Invoke();
                        Thread thread = new Thread(threadStart);
                        thread.IsBackground = true;
                        thread.Start();
                        yield return null;                        
                    }
                }
            }
            yield return null;
        }
    }

    public void FinishedProcessingPath(PathRequest request)
    {
        Debug.Log("FinishedProcessingPath");
        lock(request)
        {
            lock(results)
                results.Enqueue(request);
        }
    }
    

    void OnDrawGizmos()
    {
    }

}

public enum PathState
{
    InRequest,
    InQueue,
    InProcessing,
    Calculated,
    Error
}

public class PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;       
    public OnPathDelegate callback;

    public Vector3[] path { get; set; }
    public PathState pipelineState { get; set; }
    public Guid guid { get; private set; }

    public PathRequest(Vector3 _start, Vector3 _end, OnPathDelegate _callback)
    {
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;

        path = new Vector3[0];
        pipelineState = PathState.InRequest;
        guid = System.Guid.NewGuid();
    }
}

/* Delegate with on Path object as parameter.
* This is used for callbacks when a path has finished calculation.\n
* Example function:
* https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/using-delegates
*/
public delegate void OnPathDelegate(PathRequest request);
