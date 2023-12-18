using UnityEngine;

public class VisorController : MonoBehaviour
{
    public Camera visorCamera;
    public GameObject target;
    public GameObject rocket;

    private float startingDistance = 0f;
    
    void FixedUpdate()
    {
        if (target && rocket)
        {
            if (CrosshairsController.getRocketStatus())
            {
                if (startingDistance == 0f)
                    startingDistance = Vector3.Distance(rocket.transform.position, target.transform.position);
                
                float percentage = Mathf.InverseLerp(0, startingDistance, Vector3.Distance(rocket.transform.position, target.transform.position));
                percentage = 1 - percentage;

                visorCamera.focalLength = 2200 * percentage + 350;
            }
            
            Vector3 targetPos = target.transform.position;
            visorCamera.transform.LookAt(targetPos);
        }
    }
    
}
