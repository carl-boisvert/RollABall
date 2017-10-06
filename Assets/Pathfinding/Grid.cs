using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public bool displayGridGizmos;

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    public int MaxSize{
        get{
            return gridSizeX * gridSizeY;
        }
    }

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        this.gridSizeX = Mathf.RoundToInt(this.gridWorldSize.x / this.nodeDiameter);
        this.gridSizeY = Mathf.RoundToInt(this.gridWorldSize.y / this.nodeDiameter);
        CreateGrid();
    }

    void CreateGrid(){
        this.grid = new Node[this.gridSizeX,this.gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * this.gridWorldSize.x / 2 - Vector3.forward * this.gridWorldSize.y / 2;

        for (int x = 0; x < this.gridSizeX; x++){
			for (int y = 0; y < this.gridSizeY; y++)
			{
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * this.nodeDiameter + this.nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, this.nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node){
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++){
			for (int y = -1; y <= 1; y++)
			{
                if(x == 0 && y == 0){
                    continue;
                } else{
                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    if(checkX>=0 && checkX<gridSizeX && checkY >= 0 && checkY < gridSizeY){
                        neighbours.Add(grid[checkX,checkY]);
                    }
                }
			}
        }

        return neighbours;
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition){
        float percentX = Mathf.Clamp01((worldPosition.x + this.gridWorldSize.x/2) / this.gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.z + this.gridWorldSize.y / 2) / this.gridWorldSize.y);

        int x = Mathf.RoundToInt((this.gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((this.gridSizeY - 1) * percentY);
        return this.grid[x,y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(this.gridWorldSize.x, 1, this.gridWorldSize.y));

        if (this.grid != null && this.displayGridGizmos)
		{
			foreach (Node n in grid)
			{
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				Gizmos.DrawCube(n.position, Vector3.one * (nodeDiameter - .1f));
			}
		}
    }
}
