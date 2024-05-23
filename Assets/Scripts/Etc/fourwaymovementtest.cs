using UnityEngine;

public class fourwaymovementtest : MonoBehaviour
{
    public float speed;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Animator animator;
    public VectorValue startingPosition;

    Vector3 currentPosition, lastPosition;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        transform.position = startingPosition.initialValue;
        if(GameManager.instance.nextSpawnPoint != "")
        {
            GameObject spawnPoint = GameObject.Find(GameManager.instance.nextSpawnPoint);
            transform.position = spawnPoint.transform.position;

            GameManager.instance.nextSpawnPoint = "";
        }
        else if(GameManager.instance.lastHeroPosition != Vector3.zero)
        {
            transform.position = GameManager.instance.lastHeroPosition;
            GameManager.instance.lastHeroPosition = Vector3.zero;
        }
        
    }

    void FixedUpdate()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        UpdateAnimationAndMove();
        currentPosition = transform.position;
        if(currentPosition == lastPosition)
        {
            GameManager.instance.isWalking = false;
        }
        else
        {
            GameManager.instance.isWalking = true;
        }
        lastPosition = currentPosition;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void UpdateAnimationAndMove()
    {
        if (change != Vector3.zero)
        {
            MoveCharacter();
            animator.SetFloat("movex", change.x);
            animator.SetFloat("movey", change.y);
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }
    void MoveCharacter()
    {
        myRigidbody.MovePosition(transform.position + change.normalized * speed * Time.deltaTime);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "teleporter")
        {
            CollisionHandler col = other.gameObject.GetComponent<CollisionHandler>();
            GameManager.instance.nextSpawnPoint = col.spawnPointName;
            GameManager.instance.sceneToLoad = col.sceneToLoad;
            GameManager.instance.LoadNextScene();
        }
        //if (other.tag == "Enter")
        //{
        //    CollisionHandler col = other.gameObject.GetComponent<CollisionHandler>();
        //    GameManager.instance.nextHeroPosition = col.spawnPoint.transform.position;
        //    GameManager.instance.sceneToLoad = col.sceneToLoad;
        //    GameManager.instance.LoadNextScene();
        //}

        //if (other.tag == "Leave")
        //{
        //    CollisionHandler col = other.gameObject.GetComponent<CollisionHandler>();
        //    GameManager.instance.nextHeroPosition = col.spawnPoint.transform.position;
        //    GameManager.instance.sceneToLoad = col.sceneToLoad;
        //    GameManager.instance.LoadNextScene();
        //}

        if (other.tag == "EncounterZone")
        {
            RegionData region = other.gameObject.GetComponent<RegionData>();
            GameManager.instance.currentRegion = region;
        }
        
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "EncounterZone")
        {
            GameManager.instance.canGetEncounter = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "EncounterZone")
        {
            GameManager.instance.canGetEncounter = false;
        }
    }

    
    
}
