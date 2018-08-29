using System.Collections;
using UnityEngine;


[RequireComponent(typeof(HealthBarSystem))]
public class PlayerScript : MonoBehaviour
{
    public GameObject arrowPoint;
    public GameObject bowString;
    public GameObject spine;
    public GameObject arrowPF;
    public float attackCooldownTime;
    public float damage;
    public float force;
    public Transform target;
    bool rotateUp;
    bool rotateDown;
    public float rotationSpeed;
    bool canShoot;
    bool canRotate;
    LaunchArcRenderer launchArcRenderer;
    public HealthBarSystem healthBarSystem;

    public int normalArrows;
    public int iceArrows;
    public int poisonArrows;
    float maxAngle;
    float minAngle;
    public Animator animator;
    public GameObject arcPredictrer;

    public float maxHealth;
    public float currentHealth;
    bool dying;


    void Awake()
    {
        LoadArrows();
    }

   
    void Start()
    {
        canShoot = true;
        canRotate = true;
        currentHealth = maxHealth;
        healthBarSystem.SetMaxHelthValue(maxHealth);
        launchArcRenderer = arcPredictrer.GetComponent<LaunchArcRenderer>();
        maxAngle = spine.GetComponent<BowScript>().maxAngle;
        minAngle = spine.GetComponent<BowScript>().minAngle;
    }

    void Update()
    {
        if (!GameManager.instance.gameOver)
        {

            target = GameManager.instance.activeRoad.transform;

            Vector3 targetPostition = new Vector3(target.position.x,
                                            transform.position.y,
                                            target.position.z);
            transform.LookAt(targetPostition);
            //transform.Rotate(Vector3.up, -90);
            if (GameManager.instance.controlMode == ControlMode.Buttons)
            {
                if (rotateUp && spine.transform.eulerAngles.z > minAngle)
                {
                    spine.transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
                    launchArcRenderer.lineRenderer.enabled = true;
                    launchArcRenderer.drawRenderer = true;
                }
                else if (rotateDown && spine.transform.eulerAngles.z < maxAngle)
                {
                    spine.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
                    launchArcRenderer.lineRenderer.enabled = true;
                    launchArcRenderer.drawRenderer = true;
                }
                else
                {
                    StartCoroutine(launchArcRenderer.FadeAfterTime());
                }
            }
            if (GameManager.instance.controlMode == ControlMode.Joystick)
            {
                if (spine.transform.localEulerAngles.z > minAngle && GameManager.instance.joystick.Vertical>0)
                {
                    spine.transform.Rotate(Vector3.back * rotationSpeed * GameManager.instance.joystick.Vertical * Time.deltaTime);
                    launchArcRenderer.lineRenderer.enabled = true;
                    launchArcRenderer.drawRenderer = true;
                }
                if(spine.transform.localEulerAngles.z < maxAngle && GameManager.instance.joystick.Vertical < 0 ){
                    spine.transform.Rotate(Vector3.back * rotationSpeed * GameManager.instance.joystick.Vertical * Time.deltaTime);
                    launchArcRenderer.lineRenderer.enabled = true;
                    launchArcRenderer.drawRenderer = true;
                }
            }
        }
        else
        {
            canRotate = false;
            canShoot = false;
        }

        if(currentHealth<=0){
            if(!dying){
                animator.SetBool("death", true);
                GameManager.instance.gameOver = true;
                dying = true;
            }
        }
    }
    public void ShootArrow(ArrowType arrowType)
    {
        if (canShoot && InventoryManager.instance.GetArrowCount(arrowType) > 0)
            StartCoroutine(Shoot(arrowType));
        else
            print("Can't shoow selected arrow");
    }

    void LoadArrows()
    {
        normalArrows = InventoryManager.instance.GetArrowCount(ArrowType.NoramlArrow);
        iceArrows = InventoryManager.instance.GetArrowCount(ArrowType.IceArrow);
        poisonArrows = InventoryManager.instance.GetArrowCount(ArrowType.PoisonArrow);
    }

    public void RotateUp()
    {
        rotateUp |= canRotate;
    }

    public void RotateUpRelease()
    {
        rotateUp &= !canRotate;
    }

    public void RotateDown()
    {
        rotateDown |= canRotate;
    }

    public void RotateDownRelease()
    {
        rotateDown &= !canRotate;
    }

    public IEnumerator Shoot(ArrowType arrowType)
    {
        InventoryManager.instance.RemoveArrow(arrowType);
        launchArcRenderer.Hide();
        //GameObject arrow = Instantiate(arrowPF, arrowPoint.transform.position , Quaternion.identity );
        //Vector3 v = target.position - arrowPoint.transform.position;
        GameObject arrow = Instantiate(
            arrowPF,
            arrowPoint.transform.position, Quaternion.identity);
        arrow.transform.LookAt(target.transform.TransformPoint(Vector3.zero));
        arrow.GetComponent<ArrowScript>().arrowType = arrowType;
        arrow.transform.Rotate(Vector3.up, -90);
        arrow.GetComponent<Rigidbody>().AddForce(-arrowPoint.transform.up * force);
        animator.SetBool("shoot", true);
        LoadArrows();
        canShoot = false;
        yield return new WaitForSeconds(attackCooldownTime);
        canShoot = true;
    }

    public void ShootingDone(){
        animator.SetBool("shoot", false);
    }

    public void GetDamage(float amount){
        StartCoroutine(GetDamageSeq(amount));
    }

    IEnumerator GetDamageSeq(float amount){ 
        currentHealth -= amount;
        healthBarSystem.Hit();
        healthBarSystem.SetHelthValue(currentHealth);
        //Hit animation
        yield return new WaitForSeconds(1);
    }
}
