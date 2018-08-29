using UnityEngine;

public class ArrowParticleCollisionManager : MonoBehaviour {
    public ArrowType arrowType;

    void OnTriggerStay(Collider other)
    {
        if(other.tag.Contains("Zombie")){
            switch (arrowType){
                case ArrowType.FireArrow:
                    other.transform.GetComponentInParent<Enemy>().GetDamageTick(1);
                    break;
                case ArrowType.IceArrow:
                    other.transform.GetComponentInParent<Enemy>().StepOnIce();
                    break;
            }
        }

    }

}
