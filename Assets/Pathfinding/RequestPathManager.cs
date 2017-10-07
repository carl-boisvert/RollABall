using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestPathManager : MonoBehaviour {

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static RequestPathManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback){
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        print(instance.pathRequestQueue.Count);
        Debug.Log("Is processing path: "+instance.isProcessingPath.ToString());
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0){
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindingPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success){
        Debug.Log("PATH FOUND FINISHED");
        isProcessingPath = false;
        currentPathRequest.callback(path, success);
        this.TryProcessNext();
    }

    struct PathRequest{
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback){
            this.pathStart = start;
            this.pathEnd = end;
            this.callback = callback;
        }
    }
}
