using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float MAX_DISTANCE = Mathf.Sqrt(1250); // Maximum distance two players can be from each other. Hard calculated assuming 25x25 arena.

    private float rewardToGive;
    
    public GameObject owner;
    public GameObject opponent;
    public Rigidbody rb;
    public float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rewardToGive = 0;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.forward * speed;  // Keep speed constant

        // Calculate reward if bullet can see opponent
        if (GetOpponentVisible())
        {
            float r = 1f - (Vector3.Distance(transform.position, opponent.transform.position) / MAX_DISTANCE);   // Closer the distance, the higher the reward
            if (r > rewardToGive) { rewardToGive = r; }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController ownerController = owner.GetComponent<PlayerController>();
        ownerController.bulletActive = false;
        ownerController.currBullet = null;

        if(other.gameObject == opponent)    // Hit opponent; End episode
        { 
            print(owner.name + " Opponent hit.");

            // Get owner script for owner.hitOpponent()
            AgentControllerLow ownerLow = owner.GetComponent<AgentControllerLow>();
            AgentControllerMedium ownerMedium = owner.GetComponent<AgentControllerMedium>();
            AgentControllerHigh ownerHigh = owner.GetComponent<AgentControllerHigh>();
            if (ownerLow != null)
            {
                ownerLow.hitOpponent();
            }
            else if (ownerMedium != null)
            {
                ownerMedium.hitOpponent();
            }
            else if (ownerHigh != null)
            {
                ownerHigh.hitOpponent();
            }
            else
            {
                Debug.Log("[Opponent hit] Owner Agent not found");
            }

            // Get owner script for opponent.hitByOpponent()
            AgentControllerLow opponentLow = opponent.GetComponent<AgentControllerLow>();
            AgentControllerMedium opponentMedium = opponent.GetComponent<AgentControllerMedium>();
            AgentControllerHigh opponentHigh = opponent.GetComponent<AgentControllerHigh>();
            if(opponentLow != null)
            {
                opponentLow.hitByOpponent();
            }
            else if (opponentMedium != null)
            {
                opponentMedium.hitByOpponent();
            }
            else if (opponentHigh != null)
            {
                opponentHigh.hitByOpponent();
            }
            else
            {
                Debug.Log("Opponent Agent not found");
            }
        }
        else    // Did not hit opponent; Update reward
        {
            // Get agent script for owner
            AgentControllerLow ownerLow = owner.GetComponent<AgentControllerLow>();
            AgentControllerMedium ownerMedium = owner.GetComponent<AgentControllerMedium>();
            AgentControllerHigh ownerHigh = owner.GetComponent<AgentControllerHigh>();
            if (ownerLow != null)
            {
                if (ownerLow.reward < rewardToGive) { ownerLow.reward = rewardToGive; }
            }
            else if (ownerMedium != null)
            {
                if (ownerMedium.reward < rewardToGive) { ownerMedium.reward = rewardToGive; }
            }
            else if (ownerHigh != null)
            {
                if (ownerHigh.reward < rewardToGive) { ownerHigh.reward = rewardToGive; }
            }
            else
            {
                Debug.Log("[Opponent not hit] Owner Agent not found");
            }
        }

        Destroy(gameObject);
    }

    // Template from AgentController
    private bool GetOpponentVisible()
    {
        float OFFSET = 0.5f;    // Hard-coded offset to check for the bounds of the opponent capsule
        float ADD_MAGNITUDE = 1f;    // Hard-coded magnitude addition to ensure rays can hit the capsule

        Vector3 bulletToOpponentOrigin = opponent.transform.position - transform.position;
        Vector3 bulletToOpponentRight = (opponent.transform.position + new Vector3(0f, 0f, OFFSET)) - transform.position;
        Vector3 bulletToOpponentLeft = (opponent.transform.position - new Vector3(0f, 0f, OFFSET)) - transform.position;
        Vector3 bulletToOpponentFront = (opponent.transform.position + new Vector3(OFFSET, 0f, 0f)) - transform.position;
        Vector3 bulletToOpponentBack = (opponent.transform.position - new Vector3(OFFSET, 0f, 0f)) - transform.position;

        Vector3[] playerToOpponent = new Vector3[] { bulletToOpponentOrigin, bulletToOpponentRight, bulletToOpponentLeft, bulletToOpponentFront, bulletToOpponentBack };

        bool output = false;

        foreach (Vector3 v in playerToOpponent)
        {
            //Debug.DrawRay(transform.position, v);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, v, out hit, v.magnitude + ADD_MAGNITUDE))
            {
                //Debug.Log(v - transform.position);
                //Debug.Log(hit.collider.gameObject.name);

                if (hit.collider.gameObject == opponent)
                {
                    output = true;
                    break;
                }
            }
        }

        // For Debug
        if (false)
        {
            foreach (Vector3 v in playerToOpponent)
            {
                Debug.DrawRay(transform.position, v);
            }

            //Debug.Log(output);
        }

        return output;
    }
}
