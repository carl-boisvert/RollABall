using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PlayerController : MonoBehaviour {

    public float speed;
    public Text counter;
    public Text deathCounter;
    public Text winText;
    public Text deathText;
    public Image overlay;
    public GameObject coloredCube;

    private int deathCount;
    private Rigidbody rb;
    private int count;
    private int totalPickups;
    private bool hasControl;
    private List<GameObject> collectedPickups;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        this.setCounterText();
        this.setWinScreen(false);
		this.winText.text = "YOU WIN";
        this.deathText.enabled = false;
        this.totalPickups = GameObject.FindGameObjectsWithTag("Pickup").Length;
        this.hasControl = true;
        this.collectedPickups = new List<GameObject>();
    }

    void FixedUpdate(){
		if (Input.GetKeyDown("r"))
		{
			this.reset();
		}
        if(this.hasControl){
			float moveHorizontal = Input.GetAxis("Horizontal");
			float moveVertical = Input.GetAxis("Vertical");
			
			Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
			
			rb.AddForce(movement*speed);
        } else{
            this.stopPlayer();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("BouncySphere")){
            this.stopPlayer();
            Vector3 normal = collision.contacts[0].normal;
            this.rb.AddForce(normal*this.speed*2, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Pickup")){
			other.gameObject.SetActive(false);
            this.collectedPickups.Add(other.gameObject);
            count++;
            this.setCounterText();
            if(count >= totalPickups){
                this.hasControl = false;
                this.setWinScreen(true);            
            }
        } else if(other.gameObject.CompareTag("Hole")) {
            StartCoroutine(this.playerDead("You fell into a hole"));
        } else if(other.gameObject.CompareTag("MagicBeans")){
            this.coloredCube.GetComponent<Renderer>().materials[0].color = other.gameObject.GetComponent<Renderer>().materials[0].color;
            Destroy(other.gameObject);
        } else if(other.gameObject.CompareTag("Enemy")){
            StartCoroutine(this.playerDead("Try avoiding the cat..."));
        }
    }

    public void setCounterText(){
        counter.text = "Count: " + count.ToString();
        deathCounter.text = "Death count: " + deathCount.ToString();
    }

    public void setWinScreen(bool show){
        this.overlay.enabled = show;
        this.winText.enabled = show;
    }

    private IEnumerator playerDead(string message){
        this.deathText.text = message;
        this.overlay.enabled = true;
        this.deathText.enabled = true;
        this.hasControl = false;
        this.stopPlayer();
        this.deathCount++;
        yield return new WaitForSeconds(2);
        this.overlay.enabled = false;
		this.deathText.enabled = false;
		this.hasControl = true;
        this.reset();
    }

    private void reset(){
        this.stopPlayer();
        this.transform.position = new Vector3(0, 0.5f, 0);
        this.count = 0;
        this.setCounterText();
		//this.overlay.enabled = false;
		//this.holeText.enabled = false;
        //this.winText.enabled = false;
		//this.hasControl = true;
        foreach(GameObject obj in this.collectedPickups){
            obj.SetActive(true);
        }
    }

    private void stopPlayer(){
		this.rb.velocity = Vector3.zero;
		this.rb.angularVelocity = Vector3.zero;
    }


    private void OnEnable()
    {
        if(File.Exists(Application.persistentDataPath + "playerInfo.dat")){
			BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "playerInfo.dat", FileMode.Open);
			
            PlayerData playerData = (PlayerData)bf.Deserialize(file);
			file.Close();

            this.deathCount = playerData.deathCount;
        }
    }

    private void OnDisable()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "playerInfo.dat", FileMode.OpenOrCreate);

        PlayerData data = new PlayerData();
        data.deathCount = this.deathCount;

        bf.Serialize(file, data);
        file.Close();
    }
}

[System.Serializable]
class PlayerData{
    public int deathCount;
}