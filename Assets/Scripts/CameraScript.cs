using UnityEngine;

public class CameraScript : MonoBehaviour {
	
    public void GoToPivot(GameObject pivot){
        transform.position = pivot.transform.position;
        transform.rotation = pivot.transform.rotation;
    }
}
