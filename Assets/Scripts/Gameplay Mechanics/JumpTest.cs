using UnityEngine;
using System.Collections;

public class JumpTest : MonoBehaviour
{

    public bool twoButtonControl;
    public bool isGoalKeeper;

    public bool isGaolKeeperSkin;

    public float haciYatmazFactor;
    public float jumpFactor;
    public float tekmeFactor = 800;
    JointMotor2D motor2d = new JointMotor2D();
    public GameObject tekmebacak;
    public bool isCpu;
    public float BallDistance;
    GameObject ball;
    bool jumpOnCD;
    public bool leftOnGround;
    public bool rightOnGround;
    public bool topOnGround;
    GameObject Proxymity3;
    GameObject Proxymity2;
    GameObject Proxymity1;
    bool standupCD;
    bool sleeping;
    public GameObject pair;
    float randomtime;
    public bool canMove = true;

    public bool humanoid = true;

    public float legAngle = 90f;

    

    public GameObject Tshirt;
    public GameObject Shoe1;
    public GameObject Shoe2;

    public bool reset;

    Vector3 startPos;
    // Use this for initialization
    public void Awake()
    {

    }


    void Start()
    {
        motor2d.maxMotorTorque = 400;
        startPos = transform.position;

        if (isCpu) {
            randomtime = Random.Range(3f, 10f);
            StartCoroutine(RandomJump(randomtime));
            Proxymity3 = transform.Find("LeftLeg/Proxymity3").gameObject;
            Proxymity2 = transform.Find("RightLeg/Proxymity2").gameObject;
            Proxymity1 = transform.Find("Proxymity1").gameObject;
        }
    }

    void ButtonJumpPress() // GOALKEEPER
    {
        if (twoButtonControl) {
            if (isGoalKeeper)
                TekmeAt();
        } else
            TekmeAt();
    }
    void ButtonJumpRelease() // GOALKEEPER
    {
        if (twoButtonControl) {
            if (isGoalKeeper)
                TekmeCek();
        } else
            TekmeCek();
    }

    void ButtonJumpPress1()
    {
        TekmeAt();
    }
    void ButtonJumpRelease1()
    {
        TekmeCek();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(transform.rotation.z);
        HaciYatmaz();
        //SleepDetect();
    }
    public void resetPlayer()
    {
        Transform leftLeg = transform.Find("LeftLeg");
        Transform LeftArm = transform.Find("LeftArm");
        Transform RightArm = transform.Find("RightArm");
        Transform Head = transform.Find("Universal Player_Face");
        Transform Tail = transform.Find("Tail");

        if (Head == null)
            Head = transform.Find("Universal Player_Face Collider");

        if (leftLeg != null) {
            leftLeg.transform.localEulerAngles = Vector3.zero;
            leftLeg.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            leftLeg.GetComponent<Rigidbody2D>().angularVelocity = 0;
        }

        if (LeftArm != null) {
            LeftArm.transform.localEulerAngles = Vector3.zero;
            LeftArm.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            LeftArm.GetComponent<Rigidbody2D>().angularVelocity = 0;
        }

        if (RightArm != null) {
            RightArm.transform.localEulerAngles = Vector3.zero;
            RightArm.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            RightArm.GetComponent<Rigidbody2D>().angularVelocity = 0;
        }

        if (Head != null) {
            Head.transform.localEulerAngles = Vector3.zero;
            Head.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            Head.GetComponent<Rigidbody2D>().angularVelocity = 0;
        }

        if (Tail != null) {
            Tail.transform.localEulerAngles = Vector3.zero;
            Tail.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            Tail.GetComponent<Rigidbody2D>().angularVelocity = 0;
        }

        GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0;
        transform.position = startPos;
        transform.eulerAngles = Vector3.zero;
        canMove = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("JUMP INPUT");
            ButtonJumpPress();
        }

        if (Input.GetKeyUp(KeyCode.Space)) {
            Debug.Log("RELEASE");
            ButtonJumpRelease();
        }



        if (reset) {
            transform.Find("LeftLeg").transform.localEulerAngles = Vector3.zero;
            transform.Find("LeftArm").transform.localEulerAngles = Vector3.zero;
            transform.Find("RightArm").transform.localEulerAngles = Vector3.zero;
            GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            GetComponent<Rigidbody2D>().angularVelocity = 0;
            transform.position = startPos;
            transform.eulerAngles = Vector3.zero;
            reset = false;
        }
        
    }

    void HaciYatmaz()
    {
        if (leftOnGround || rightOnGround || topOnGround) {
            if (transform.rotation.eulerAngles.z > 0 && transform.rotation.eulerAngles.z < 100) {
                GetComponent<Rigidbody2D>().AddTorque(haciYatmazFactor * (transform.rotation.eulerAngles.z / 10));
            }
            if (transform.rotation.eulerAngles.z > 260 && transform.rotation.eulerAngles.z < 360) {
                GetComponent<Rigidbody2D>().AddTorque(-haciYatmazFactor * ((360 - transform.rotation.eulerAngles.z) / 10));
            }
            if (GetComponent<Rigidbody2D>().angularVelocity > 40) { GetComponent<Rigidbody2D>().angularVelocity = 40; }

            if (GetComponent<Rigidbody2D>().angularVelocity < -40) { GetComponent<Rigidbody2D>().angularVelocity = -40; }
        }
    }

    void SleepDetect()
    {
        if (!sleeping) {
            if ((transform.rotation.eulerAngles.z > 60 && transform.rotation.eulerAngles.z < 90) && (leftOnGround || rightOnGround || topOnGround)) {
                StartCoroutine(StandUp(true));
                sleeping = true;
            } else if ((transform.rotation.eulerAngles.z > 240 && transform.rotation.eulerAngles.z < 300) && (leftOnGround || rightOnGround || topOnGround)) {
                StartCoroutine(StandUp(false));
                sleeping = true;
            }
        }
    }

    public void TekmeAt()
    {
        if (!canMove)
            return;

        if (!jumpOnCD) {
            if (leftOnGround || rightOnGround || topOnGround) {
                
                GetComponent<Rigidbody2D>().AddForce(new Vector2(transform.up.x, Mathf.Abs(transform.up.y)) * jumpFactor);
                jumpOnCD = true;
                StartCoroutine(JumpCD());

                if (topOnGround)
                    GetComponent<Rigidbody2D>().AddTorque(150f * Mathf.Sign(transform.localScale.x));
            }
            if (isCpu)
                StartCoroutine(CpuTekmeCek());
        }


        motor2d.motorSpeed = -tekmeFactor;
        tekmebacak.GetComponent<HingeJoint2D>().motor = motor2d;
    }

    void TekmeCek()
    {

        motor2d.motorSpeed = tekmeFactor;
        tekmebacak.GetComponent<HingeJoint2D>().motor = motor2d;
    }

    void CpuPlayer()
    {
        //if (Proxymity1 == null) {
        //    if (isCpu) {
        //        randomtime = Random.Range(3f, 10f);
        //        StartCoroutine(RandomJump(randomtime));
        //        Proxymity3 = transform.Find("LeftLeg/Proxymity3").gameObject;
        //        Proxymity2 = transform.Find("RightLeg/Proxymity2").gameObject;
        //        Proxymity1 = transform.Find("Proxymity1").gameObject;
        //    }
        //}


        //bool ballisnear = false;

        //if (GameHandler.GameController.gameMode == GameController.GameMode.OnePlayerPlatform) {
        //    ballisnear = true;
        //} else {
        //    float speedMux = ball.GetComponent<Rigidbody2D>().velocity.magnitude / 8;

        //    if ((Proxymity1.transform.position - ball.transform.position).magnitude < 0.5f + 2 * speedMux) {
        //        ballisnear = true;
        //    }
        //    if ((Proxymity2.transform.position - ball.transform.position).magnitude < 0.5f + 1 * speedMux) {
        //        ballisnear = true;
        //    }
        //    if ((Proxymity3.transform.position - ball.transform.position).magnitude < 1f + 2 * speedMux) {
        //        ballisnear = true;

        //    }
        //}

        //if (ballisnear && !jumpOnCD) {
        //    float tm = Random.Range(0.05f, 0.3f);
        //    StartCoroutine(RandomReflex(tm));
        //}
    }

    IEnumerator JumpCD()
    {
        yield return new WaitForSeconds(0.1f);
        if (!isCpu)
            jumpOnCD = false;

        if (isCpu) {
            yield return new WaitForSeconds(0.3f);

            TekmeCek();
            yield return new WaitForSeconds(0.5f);
            jumpOnCD = false;
        }
    }

    IEnumerator CpuTekmeCek()
    {

        yield return new WaitForSeconds(0.3f);

        TekmeCek();
    }
    IEnumerator RandomJump(float time)
    {
        yield return new WaitForSeconds(time);
        TekmeAt();
        if (!twoButtonControl)
            pair.GetComponent<JumpTest>().TekmeAt();
        randomtime = Random.Range(3f, 10f);
        StartCoroutine(RandomJump(randomtime));
    }

    IEnumerator RandomReflex(float time)
    {
        yield return new WaitForSeconds(time);
        TekmeAt();

        if (!twoButtonControl)
            pair.GetComponent<JumpTest>().TekmeAt();
    }

    IEnumerator StandUp(bool side)
    {



        yield return new WaitForSeconds(1);
        if (side) {
            if ((transform.rotation.eulerAngles.z > 60 && transform.rotation.eulerAngles.z < 90) && (leftOnGround || rightOnGround))
                GetComponent<Rigidbody2D>().angularVelocity = -1200;
        } else {
            if ((transform.rotation.eulerAngles.z > 240 && transform.rotation.eulerAngles.z < 300) && (leftOnGround || rightOnGround))
                GetComponent<Rigidbody2D>().angularVelocity = 1200;
        }

        yield return new WaitForSeconds(2);
        sleeping = false;

    }
}
