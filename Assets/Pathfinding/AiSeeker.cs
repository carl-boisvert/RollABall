using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSeeker : MonoBehaviour
{
    public GameObject magicBeans;
    public Transform target;
    public Animator animator;
    float speed = 5;
    Vector3[] path;
    int targetIndex;
    bool dropBeans = true;

    private void Start()
    {
        StartCoroutine("DropMagicBeans");
    }



    void Update()
    {
        RequestPathManager.RequestPath(transform.position, target.position, this.OnPathFound);
        if(GetComponent<Animator>()){
            this.animator = GetComponent<Animator>();
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown("r"))
        {
            this.transform.position = new Vector3(7, 0, 18);
            StopCoroutine("FollowPath");
            RequestPathManager.RequestPath(transform.position, target.position, this.OnPathFound);
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSucessful){
        if(pathSucessful){
            path = newPath;
            StopCoroutine("FollowPath");
            if(this.animator){
                this.animator.SetBool("isMoving",true);
                this.animator.Play("moving");
            }
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath(){
        Vector3 currentWaypoint = path[0];

        while(true){
            if(transform.position == currentWaypoint){
                targetIndex++;
                if(targetIndex >= path.Length){
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed*Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, (transform.position-currentWaypoint)*-1, speed * Time.deltaTime, 0.0F));
            yield return null;
        }
    }

	IEnumerator DropMagicBeans()
	{
        while(this.dropBeans){
            GameObject instance = Instantiate(magicBeans,new Vector3(this.transform.position.x, this.magicBeans.transform.position.y, this.transform.position.z), this.transform.rotation);
            instance.GetComponent<Renderer>().materials[0].color = new Color(Random.value, Random.value, Random.value, 1.0f);
            yield return new WaitForSeconds(3);
        }
        yield return null;
	}

    public void OnDrawGizmos()
    {
        if(path != null){
            for (int i = targetIndex; i < path.Length; i++){
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(path[i], Vector3.one);

                if(i == targetIndex){
                    Gizmos.DrawLine(transform.position, path[i]);
                } else{
                    Gizmos.DrawLine(path[i-1], path[i]);
                }
            }
        }
    }
}
