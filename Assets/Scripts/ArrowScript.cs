using UnityEngine;

public enum ArrowType
{
    NoramlArrow, FireArrow, IceArrow, PoisonArrow
}

public class ArrowScript : MonoBehaviour
{
    float angle;
    Quaternion startRotation;
    public ArrowType arrowType;
    TrailRenderer tr;
    public Gradient normalGrad;
    public Gradient iceGrad;
    public Gradient fireGrad;
    public Gradient poisonGrad;
    public GameObject fireParticle;
    public GameObject iceParticle;
    public GameObject poisonParticle;

    void Start()
    {
        ManageArrowTrailer();

        Physics.IgnoreLayerCollision(8, 8);

        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v = GetComponent<Rigidbody>().velocity;
        angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = new Quaternion(transform.rotation.x, startRotation.y, transform.rotation.z, transform.rotation.w);

        if (transform.position.y < -20)
            Destroy(gameObject);
        //transform.forward = Vector3.Slerp(transform.forward, v.normalized, Time.deltaTime);
    }

    void ManageArrowTrailer()
    {
        switch (arrowType)
        {
            case ArrowType.NoramlArrow:
                ChangeTrailer(normalGrad);
                break;
            case ArrowType.IceArrow:
                //Ice trailer
                ChangeTrailer(iceGrad);
                break;
            case ArrowType.FireArrow:
                ChangeTrailer(fireGrad);
                break;
            case ArrowType.PoisonArrow:
                ChangeTrailer(poisonGrad);
                break;
        }
    }

    void ChangeTrailer(Gradient gradient)
    {
        tr = GetComponent<TrailRenderer>();
        tr.colorGradient = gradient;
    }

    void PlaceParticleEffect(Vector3 pos){
        Vector3 position = new Vector3(pos.x, 0, pos.z);
        switch (arrowType){
            case ArrowType.FireArrow:
                GameObject goFire = Instantiate(fireParticle, position, GameManager.instance.activeRoad.transform.rotation);
                goFire.transform.Rotate(Vector3.right, -90);
                goFire.GetComponent<ArrowParticleCollisionManager>().arrowType = arrowType;
                break;
            case ArrowType.IceArrow:
                GameObject goIce = Instantiate(iceParticle, position, GameManager.instance.activeRoad.transform.rotation);
                goIce.transform.Rotate(Vector3.right, -90);
                goIce.GetComponent<ArrowParticleCollisionManager>().arrowType = arrowType;
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ZombieHead")
        {
            collision.transform.GetComponentInParent<Enemy>().GetCritDamage(GameManager.instance.player.GetComponent<PlayerScript>().damage);
        }
        if (collision.gameObject.tag == "ZombieBody")
        {
            collision.transform.GetComponentInParent<Enemy>().GetDamage(GameManager.instance.player.GetComponent<PlayerScript>().damage);
        }
        if (collision.gameObject.tag == "ZombieLegs")
        {
            collision.transform.GetComponentInParent<Enemy>().GetKneeDamage(GameManager.instance.player.GetComponent<PlayerScript>().damage);
        }
        if(arrowType != ArrowType.NoramlArrow)
        PlaceParticleEffect(collision.contacts[0].point);
        Destroy(gameObject);
    }
}
