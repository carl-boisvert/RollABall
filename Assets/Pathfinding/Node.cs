using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {

    public bool walkable;
    public Vector3 position;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;

    public Node(bool walkable, Vector3 position, int gridX, int gridY){
        this.walkable = walkable;
        this.position = position;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public int fcost{
        get {
            return this.gCost + this.hCost;
        }
    }

    public int HeapIndex{
        get{
            return this.heapIndex;
        }
        set{
            this.heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare){
        int compare = this.fcost.CompareTo(nodeToCompare.fcost);
        if(compare == 0){
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
