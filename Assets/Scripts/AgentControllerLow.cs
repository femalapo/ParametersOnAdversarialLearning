using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;

public class AgentControllerLow : Agent
{
    private float MAX_DISTANCE = Mathf.Sqrt(1250); // Maximum distance two players can be from each other. Hard calculated assuming 25x25 arena.

    [SerializeField] private int MAX_STEPS = 3000;  // Maximum steps for an agent episode. Should be constant across agents.

    // Initilization values for OnEpsiodeBegin
    Vector3 startingPos;
    Quaternion startingRot;

    PlayerController playerController;
    GameObject opponent;

    [HideInInspector] public float reward;           // To be read and set by Bullet
    [HideInInspector] public float[] observations;   // To be read by HighObsAgent

    private void Start()
    {
        startingPos = transform.position;
        startingRot = transform.rotation;
        playerController = GetComponent<PlayerController>();
        opponent = playerController.opponent;
        observations = new float[4]; // Constant 4 for LowOb Agent
    }

    private void FixedUpdate()
    {
        // Interupt episode after max steps
        if(StepCount > MAX_STEPS)
        {
            Debug.Log($"{gameObject.name}: {reward}");

            SetReward(reward);
            EpisodeInterrupted();
        }
    }

    public override void OnEpisodeBegin()
    {
        // Reset bullet if active
        if (playerController.bulletActive)
        {
            playerController.bulletActive = false;
            Destroy(playerController.currBullet);
            playerController.currBullet = null;
        }

        transform.position = startingPos;
        transform.rotation = startingRot;
        playerController.bulletActive = false;
        reward = 0;
        
        Debug.Log(gameObject.name + ": Episode Begin");
    }

    // Use Player Input component to get actions
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        InputAction moveAction = InputSystem.actions.FindAction("Move");
        InputAction rotateAction = InputSystem.actions.FindAction("Rotate");
        InputAction shootAction = InputSystem.actions.FindAction("Shoot");

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        float rotateValue = rotateAction.ReadValue<float>();
        float shootValue = shootAction.ReadValue<float>();

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        // Offset by 1 for OnActionReceived
        discreteActions[0] = (int) Mathf.Round((moveValue.x + 1));  // Round to normalize InputAction values
        discreteActions[1] = (int) Mathf.Round((moveValue.y + 1));  // Round to normalize InputAction values
        discreteActions[2] = (int) (rotateValue + 1);
        discreteActions[3] = (int) shootValue;                      // Does not need to be offset

        //Debug.Log(discreteActions[0] + " " + discreteActions[1]);
        //Debug.Log(moveValue);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Offset by -1 for PlayerController compatibility
        int moveX = actions.DiscreteActions[0] - 1;
        int moveZ = actions.DiscreteActions[1] - 1;
        int rotation = actions.DiscreteActions[2] - 1;
        int shoot = actions.DiscreteActions[3];

        playerController.Move(new Vector2(moveX, moveZ).normalized);    // Normalize vector to magnitude of 1
        playerController.Rotate(rotation);
        if (shoot == 1) { playerController.Shoot(); }


        //Debug.Log(moveX + " " + moveZ + " " + rotation + " " + shoot);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float heading = GetHeadingObservation();
        float canShoot = playerController.bulletActive ? 0 : 1;
        float distance = Vector3.Distance(transform.position, opponent.transform.position) / MAX_DISTANCE; // Normalize value based on maximum possible distance
        float opponentVisible = GetOpponentVisible() ? 1 : 0;

        sensor.AddObservation(heading);
        sensor.AddObservation(canShoot);
        sensor.AddObservation(distance);
        sensor.AddObservation(opponentVisible);

        //Debug.Log(opponentVisible);

        // For HighObsAgent
        observations[0] = heading;
        observations[1] = canShoot;
        observations[2] = distance;
        observations[3] = opponentVisible;
    }

    // When hitting opponent
    public void hitOpponent()
    {
        Debug.Log($"{gameObject.name} hit {opponent.name}");

        SetReward(1f);
        EndEpisode();
    }

    // When hit by opponent
    public void hitByOpponent()
    {
        Debug.Log($"{gameObject.name} hit by {opponent.name}");

        SetReward(-1f);
        EndEpisode();
    }

    private float GetHeadingObservation()
    {
        Vector3 relativeOpponentPos3D = opponent.transform.position - transform.position;
        Vector2 relativeOpponentPos2D = new Vector2(relativeOpponentPos3D.x, relativeOpponentPos3D.z);
        float heading = Vector2.SignedAngle(new Vector2(transform.forward.x, transform.forward.z), relativeOpponentPos2D);
        heading = heading / 180f; // Normalize heading value
        return heading;
    }

    private bool GetOpponentVisible()
    {
        float OFFSET = 0.5f;    // Hard-coded offset to check for the bounds of the opponent capsule
        float ADD_MAGNITUDE = 1f;    // Hard-coded magnitude addition to ensure rays can hit the capsule

        Vector3 playerToOpponentOrigin = opponent.transform.position - transform.position;
        Vector3 playerToOpponentRight = (opponent.transform.position + new Vector3(0f, 0f, OFFSET)) - transform.position;
        Vector3 playerToOpponentLeft = (opponent.transform.position - new Vector3(0f, 0f, OFFSET)) - transform.position;
        Vector3 playerToOpponentFront = (opponent.transform.position + new Vector3(OFFSET, 0f, 0f)) - transform.position;
        Vector3 playerToOpponentBack = (opponent.transform.position - new Vector3(OFFSET, 0f, 0f)) - transform.position;

        Vector3[] playerToOpponent = new Vector3[] { playerToOpponentOrigin, playerToOpponentRight, playerToOpponentLeft, playerToOpponentFront, playerToOpponentBack };

        bool output = false;

        foreach(Vector3 v in playerToOpponent)
        {
            //Debug.DrawRay(transform.position, v);

            RaycastHit hit;
            if(Physics.Raycast(transform.position, v, out hit, v.magnitude + ADD_MAGNITUDE))
            {
                //Debug.Log(v - transform.position);
                //Debug.Log(hit.collider.gameObject.name);

                if(hit.collider.gameObject == opponent)
                {
                    output = true;
                    break;
                }
            }
        }

        // For Debug
        if (false)
        {
            Debug.DrawRay(transform.position, playerToOpponentOrigin);
            Debug.DrawRay(transform.position, playerToOpponentRight);
            Debug.DrawRay(transform.position, playerToOpponentLeft);
            Debug.DrawRay(transform.position, playerToOpponentFront);
            Debug.DrawRay(transform.position, playerToOpponentBack);

            //Debug.Log(output);
        }

        return output;
    }
}
