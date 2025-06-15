using UnityEngine;

public class ClickPosition : MonoBehaviour
{
    public RosPublisherExample publisher;
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

          
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 clickPosition = hit.point;
                Debug.Log("Position: " + clickPosition);
                publisher.sendGoal(clickPosition, Quaternion.identity);
            }
            else
            {
                Debug.Log("Did not detect anything");
            }
        }
    }
}
