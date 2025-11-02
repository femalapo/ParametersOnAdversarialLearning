using UnityEngine;

public class Arena : MonoBehaviour
{
    // Get the bounds of the arena
    // [Min Z, Max Z, Min X, Max X]
    public float[] getBounds()
    {
        float[] output = new float[4];
        float scalingFactor = transform.localScale.z; // Scaling factor to adjust for arena scaling

        Transform backWall = transform.Find("Back Wall");
        output[0] = backWall.position.z + (backWall.localScale.z * scalingFactor); // Min Z

        Transform frontWall = transform.Find("Front Wall");
        output[1] = frontWall.position.z - (frontWall.localScale.z * scalingFactor); // Max Z

        Transform leftWall = transform.Find("Left Wall");
        output[2] = leftWall.position.x + (leftWall.localScale.x * scalingFactor); // Min X

        Transform rightWall = transform.Find("Right Wall");
        output[3] = rightWall.position.x - (rightWall.localScale.x * scalingFactor); // Max X

        return output;
    }
}
