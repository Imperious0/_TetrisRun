using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMechanics : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem hitParticles;

    [SerializeField]
    private GameObject centre;

    [SerializeField]
    private GameSettings gSettings;

    //Mouse Events
    private MotionCapture mCapture;

    //initial Values
    private Vector3 initialPosition = Vector3.zero;
    private int initialFlipCount = -1;

    //Current Flips
    private int flipCount;


    //Used to calculate next flip rotation
    float currentFlipAngle;
    float flipTriggerAngle;

    ///Logics
    [SerializeField]
    private bool rotateMe = false;
    [SerializeField]
    private bool isMyTurn = false;
    [SerializeField]
    private bool isDoneJob = false;
    [SerializeField]
    private bool isDoneByPlayer = false;

    //If True Block need to be in correct flip count
    [SerializeField]
    private bool isNeedToCheck = true;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        mCapture = Camera.main.GetComponent<MotionCapture>();
        flipTriggerAngle = 360f / gSettings.MaxFlipCount * 1f;
        setInitialRotation();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        if (rotateMe)
        {
            this.transform.RotateAround(this.centre.transform.position, Vector3.forward, gSettings.BlockTimeLapse);

            currentFlipAngle = flipTriggerAngle - (this.transform.rotation.eulerAngles.z % flipTriggerAngle);

            if (currentFlipAngle <= gSettings.BlockTimeLapse + 0.01f)
            {
                this.transform.RotateAround(this.centre.transform.position, Vector3.forward, currentFlipAngle);
                flipCount++;
                rotateMe = false;
                checkBlockStatus();
                return;
            }
            return;
        }
        if (isMyTurn && !isDoneJob)
        {
            switch (mCapture.CurrentMotion)
            {
                case Motions.Tap:
                    rotateBlock();
                    break;
                case Motions.Down:
                    isDoneJob = true;
                    isDoneByPlayer = true;
                    this.dieTie();
                    break;
                default:
                    //None
                    break;
            }
        }

    }
    private void setInitialRotation(int initialFlip = -1) 
    {
        if (initialFlip == -1) 
        {
            flipCount = Random.Range(0, gSettings.MaxFlipCount);
            initialFlipCount = flipCount;
        }
        else 
        {
            flipCount = initialFlip;
        }

        float angleInDegrees = 360f / gSettings.MaxFlipCount * 1f;

        angleInDegrees *= flipCount;

        this.transform.RotateAround(this.centre.transform.position, Vector3.forward, angleInDegrees);
        checkBlockStatus();
    }
    void rotateBlock() 
    {
        rotateMe = true;
    }
    public void setTurn(bool isTurn) 
    {
        isMyTurn = isTurn;
    }
    public bool isBlocksKiller() 
    {
        bool isKiller = true;
        if((flipCount == 0 && isDoneJob) || !isNeedToCheck) 
        {
            if (!isDoneByPlayer)
                isKiller = true;
            else
                isKiller = false;

        }

        return isKiller;
    }
    public bool isNaturalDeath() 
    {
        return !isDoneByPlayer;
    }
    void checkBlockStatus() 
    {
        flipCount %= gSettings.MaxFlipCount;
        foreach (Renderer rend in this.GetComponentsInChildren<Renderer>())
        {

            if (!isNeedToCheck) 
            {
                rend.material.SetColor("_Color", Color.green);
                continue;
            }
            if (flipCount == 0)
            {
                rend.material.SetColor("_Color", Color.green);
            }
            else if (flipCount == (gSettings.MaxFlipCount / 2))
            {
                rend.material.SetColor("_Color", Color.red);
            }
            else
            {
                rend.material.SetColor("_Color", Color.yellow);
            }

        }
    }
    public void resetBlock() 
    {
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<Rigidbody>().constraints = ~(RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ);
        
        this.transform.position = initialPosition;
        this.transform.rotation = Quaternion.identity;

        setInitialRotation(initialFlipCount);

        isMyTurn = false;
        rotateMe = false;
        isDoneJob = false;
        isDoneByPlayer = false;
    }
    public void dieTie() 
    {
        StartCoroutine(dieTieWorker());
    }
    private IEnumerator dieTieWorker()
    {
        Camera.main.GetComponent<CameraController>().ShakeIt();
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<Rigidbody>().AddForce(Vector3.down * gSettings.BlockForce, ForceMode.Impulse);
        this.isMyTurn = false;
        this.isDoneJob = true;
        yield return new WaitUntil(() => { return this.GetComponent<Rigidbody>().velocity.y < -0.1f; });
        yield return new WaitUntil(() => { return this.GetComponent<Rigidbody>().velocity.y > -0.001f; });
        hitParticles.Play(true);
        // TODO
        if (!isBlocksKiller()) 
        {
            Camera.main.GetComponent<GameController>().sendNextBlock();
        }
            
    }
}
