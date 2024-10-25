using UnityEngine;
using System.Collections;

public class Jump : MonoBehaviour {

    public bool twoButtonControl;
    public bool isGoalKeeper;

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
    GameObject Proxymity3;
    GameObject Proxymity2;
    GameObject Proxymity1;
    bool standupCD;
    bool sleeping;
    public GameObject pair;
    float randomtime;

    public GameController.Player player;

    public GameObject Tshirt;
    public GameObject Shoe1;
    public GameObject Shoe2;
    
    public bool reset;

    Vector3 startPos;
	// Use this for initialization
   public void OnEnable()
    {
        if (isCpu)
        {
            randomtime = Random.Range(3f, 10f);
            StartCoroutine(RandomJump(randomtime));
            Proxymity3 = transform.Find("LeftLeg/Proxymity3").gameObject;
            Proxymity2 = transform.Find("RightLeg/Proxymity2").gameObject;
            Proxymity1 = transform.Find("Proxymity1").gameObject;
        }

        if (player == GameController.Player.Red)
        {

            if (!isGoalKeeper && twoButtonControl)
            {
                GameHandler.GameController.OnP1JumpPress1 += new EventHandler(ButtonJumpPress1);
                GameHandler.GameController.OnP1JumpRelease1 += new EventHandler(ButtonJumpRelease1);
            }
            else
            {
                GameHandler.GameController.OnP1JumpPress += new EventHandler(ButtonJumpPress);
                GameHandler.GameController.OnP1JumpRelease += new EventHandler(ButtonJumpRelease);
            }
        }
        if (player == GameController.Player.Blue)
        {
            if (!isGoalKeeper && twoButtonControl)
            {
                GameHandler.GameController.OnP2JumpPress1 += new EventHandler(ButtonJumpPress1);
                GameHandler.GameController.OnP2JumpRelease1 += new EventHandler(ButtonJumpRelease1);
            }
            else
            {

                GameHandler.GameController.OnP2JumpPress += new EventHandler(ButtonJumpPress);
                GameHandler.GameController.OnP2JumpRelease += new EventHandler(ButtonJumpRelease);
            }
        }
    }

    public void OnDestroy()
    {
        if (player == GameController.Player.Red)
        {

            if (!isGoalKeeper && twoButtonControl)
            {
                GameHandler.GameController.OnP1JumpPress1 -= new EventHandler(ButtonJumpPress1);
                GameHandler.GameController.OnP1JumpRelease1 -= new EventHandler(ButtonJumpRelease1);
            }
            else
            {
                GameHandler.GameController.OnP1JumpPress -= new EventHandler(ButtonJumpPress);
                GameHandler.GameController.OnP1JumpRelease -= new EventHandler(ButtonJumpRelease);
            }
        }
        if (player == GameController.Player.Blue)
        {
            if (!isGoalKeeper && twoButtonControl)
            {
                GameHandler.GameController.OnP2JumpPress1 -= new EventHandler(ButtonJumpPress1);
                GameHandler.GameController.OnP2JumpRelease1 -= new EventHandler(ButtonJumpRelease1);
            }
            else
            {

                GameHandler.GameController.OnP2JumpPress -= new EventHandler(ButtonJumpPress);
                GameHandler.GameController.OnP2JumpRelease -= new EventHandler(ButtonJumpRelease);
            }
        }
    }
	void Start () {
        ball = GameHandler.Ball;
        motor2d.maxMotorTorque = 400;
        startPos = transform.position;
        
	}

    void ButtonJumpPress()
    {
        if (twoButtonControl)
        {
            if(isGoalKeeper)
            TekmeAt();
        }
        else
            TekmeAt();
    }
    void ButtonJumpRelease()
    {
        if (twoButtonControl)
        {
            if (isGoalKeeper)
                TekmeCek();
        }
        else
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
	void FixedUpdate () {
       //Debug.Log(transform.rotation.z);
       HaciYatmaz();
       //SleepDetect();
	}
    public void resetPlayer()
    {
        GameObject leftLeg = transform.Find("LeftLeg").gameObject;
        GameObject LeftArm = transform.Find("LeftArm").gameObject;
        GameObject RightArm = transform.Find("RightArm").gameObject;
        GameObject Head = transform.Find("Universal Player_Face").gameObject;
        leftLeg.transform.localEulerAngles = Vector3.zero;
        leftLeg.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        leftLeg.GetComponent<Rigidbody2D>().angularVelocity = 0;
        LeftArm.transform.localEulerAngles = Vector3.zero;
        LeftArm.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        LeftArm.GetComponent<Rigidbody2D>().angularVelocity = 0;
        RightArm.transform.localEulerAngles = Vector3.zero;
        RightArm.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        RightArm.GetComponent<Rigidbody2D>().angularVelocity = 0;
        Head.transform.localEulerAngles = Vector3.zero;
        Head.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        Head.GetComponent<Rigidbody2D>().angularVelocity = 0;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0;
        transform.position = startPos;
        transform.eulerAngles = Vector3.zero;
    }
    void Update()
    {
        if (reset)
        {
            transform.Find("LeftLeg").transform.localEulerAngles = Vector3.zero;
            transform.Find("LeftArm").transform.localEulerAngles = Vector3.zero;
            transform.Find("RightArm").transform.localEulerAngles = Vector3.zero;
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            GetComponent<Rigidbody2D>().angularVelocity = 0;
            transform.position = startPos;
            transform.eulerAngles = Vector3.zero;
            reset = false;
        }
        if (!isCpu)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TekmeAt();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                TekmeCek();
            }
        }
        if (isCpu)
        {
            CpuPlayer();
        }
    }

    void HaciYatmaz()
    {
        if (leftOnGround || rightOnGround)
        {
            if (transform.rotation.eulerAngles.z > 0 && transform.rotation.eulerAngles.z < 100)
            {
                GetComponent<Rigidbody2D>().AddTorque(haciYatmazFactor * (transform.rotation.eulerAngles.z / 10));
            }
            if (transform.rotation.eulerAngles.z > 260 && transform.rotation.eulerAngles.z < 360)
            {
                GetComponent<Rigidbody2D>().AddTorque(-haciYatmazFactor * ((360 - transform.rotation.eulerAngles.z) / 10));
            }
            if (GetComponent<Rigidbody2D>().angularVelocity > 80) { GetComponent<Rigidbody2D>().angularVelocity = 80; }
            
            if (GetComponent<Rigidbody2D>().angularVelocity < -80) { GetComponent<Rigidbody2D>().angularVelocity = -80; }
        }
    }

    void SleepDetect()
    {
        

            

            if (!sleeping)
            {
                if ((transform.rotation.eulerAngles.z > 60 && transform.rotation.eulerAngles.z < 90) && (leftOnGround || rightOnGround))
                {
                    StartCoroutine(StandUp(true));
                    sleeping = true;
                }

                else if ((transform.rotation.eulerAngles.z > 240 && transform.rotation.eulerAngles.z < 300) && (leftOnGround || rightOnGround))
                {
                    StartCoroutine(StandUp(false));
                    sleeping = true;
                }
                
            }
            
        
    }

    public void TekmeAt()
    {
        if (!jumpOnCD)
        {
            if (leftOnGround || rightOnGround)
            {
                GameHandler.Effect.PlayJump();
                GetComponent<Rigidbody2D>().AddForce(new Vector2(transform.up.x, Mathf.Abs(transform.up.y)) * jumpFactor);
                jumpOnCD = true;
                StartCoroutine(JumpCD());
            }
            if(isCpu)
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
        if (ball == null)
        {
            ball = GameHandler.Ball;
        }
        if (Proxymity1 == null)
        {
            if (isCpu)
            {
                randomtime = Random.Range(3f, 10f);
                StartCoroutine(RandomJump(randomtime));
                Proxymity3 = transform.Find("LeftLeg/Proxymity3").gameObject;
                Proxymity2 = transform.Find("RightLeg/Proxymity2").gameObject;
                Proxymity1 = transform.Find("Proxymity1").gameObject;
            }
        }

        float speedMux = ball.GetComponent<Rigidbody2D>().velocity.magnitude / 8;
        bool ballisnear = false;
        if ((Proxymity1.transform.position - ball.transform.position).magnitude < 0.5f + 2 * speedMux)
        {
            ballisnear = true;
        }
        if ((Proxymity2.transform.position - ball.transform.position).magnitude < 0.5f + 1 * speedMux)
        {
            ballisnear = true;
        }
        if ((Proxymity3.transform.position - ball.transform.position).magnitude < 1f + 2 * speedMux)
        {
            ballisnear = true;

        }
        if (ballisnear && !jumpOnCD)
        {
            float tm = Random.Range(0.05f, 0.3f);
            StartCoroutine(RandomReflex(tm));
        }
    }

    IEnumerator JumpCD()
    {
        yield return new WaitForSeconds(0.1f);
        if(!isCpu)
        jumpOnCD = false;

        if (isCpu)
        {
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
        if(!twoButtonControl)
        pair.GetComponent<Jump>().TekmeAt();
        randomtime = Random.Range(3f, 10f);
        StartCoroutine(RandomJump(randomtime));
    }

    IEnumerator RandomReflex(float time)
    {
        yield return new WaitForSeconds(time);
        TekmeAt();

        if (!twoButtonControl)
        pair.GetComponent<Jump>().TekmeAt();
    }

    IEnumerator StandUp(bool side)
    {

       

        yield return new WaitForSeconds(1);
        if (side)
        {
            if ((transform.rotation.eulerAngles.z > 60 && transform.rotation.eulerAngles.z < 90) && (leftOnGround || rightOnGround))
                GetComponent<Rigidbody2D>().angularVelocity = -1200;
        }
        else
        {
            if ((transform.rotation.eulerAngles.z > 240 && transform.rotation.eulerAngles.z < 300) && (leftOnGround || rightOnGround))
            GetComponent<Rigidbody2D>().angularVelocity = 1200;
        }

        yield return new WaitForSeconds(2);
        sleeping = false;

    }
}
