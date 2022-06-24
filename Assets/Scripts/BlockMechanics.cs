using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BlockMechanics : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem hitParticles;
    [SerializeField]
    private GameSettings gSettings;

    //Mouse Events
    private MotionCapture mCapture;

    [SerializeField]
    private Rigidbody m_rigidBody;
    [SerializeField]
    private GameObject centre;

    //initial Values
    private Vector3 initialPosition = Vector3.zero;
    private int initialFlipCount = -1;

    //Current Flips
    private int flipCount;


    //Used to calculate next flip rotation
    float currentTimelapse = 0f;
    Quaternion oldRotation;
    float flipTriggerAngle;

    ///Logics
    private bool rotateMe = false;
    private bool isMyTurn = false;
    private bool isDoneJob = false;
    private bool isDoneByPlayer = false;

    //If True Block need to be in correct flip count
    [SerializeField]
    private bool isNeedToCheck = true;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        mCapture = Camera.main.GetComponent<MotionCapture>();
    }

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        flipTriggerAngle = 360f / gSettings.MaxFlipCount * 1f;
        setInitialRotation();
    }
    private void Update()
    {
        if (isMyTurn && !isDoneJob && !rotateMe)
        {
            switch (mCapture.CurrentMotion)
            {
                case Motions.Tap:
                    MusicManager.Instance.SfxHandler.playClipSelf("Rotate");
                    rotateBlock();
                    break;
                case Motions.Down:
                    MusicManager.Instance.SfxHandler.playClipSelf("Slam");
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

    private void FixedUpdate()
    {
        if (rotateMe)
        {
            currentTimelapse += Time.fixedDeltaTime * gSettings.BlockSpinSpeed;
            float timelapse = Mathf.Lerp(0, 1, currentTimelapse);
            Quaternion now = Quaternion.Lerp(oldRotation, Quaternion.Euler(Vector3.forward * flipTriggerAngle * (flipCount + 1)), timelapse);
            this.transform.RotateAround(this.centre.transform.position, Vector3.forward, Quaternion.Angle(this.centre.transform.rotation, now));

            if (currentTimelapse >= 1f)
            {
                flipCount++;
                rotateMe = false;
                currentTimelapse = 0f;
                checkBlockStatus();
                mCapture.resetMotion();
                return;
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
        currentTimelapse = 0f;
        oldRotation = transform.rotation;
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
        m_rigidBody.useGravity = false;
        m_rigidBody.constraints = ~(RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ);
        
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
        m_rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        m_rigidBody.useGravity = true;
        m_rigidBody.AddForce(Vector3.down * gSettings.BlockForce, ForceMode.Impulse);
        this.isMyTurn = false;
        this.isDoneJob = true;
        yield return new WaitUntil(() => { return m_rigidBody.velocity.y < -0.1f; });
        yield return new WaitUntil(() => { return m_rigidBody.velocity.y > -0.001f; });
        hitParticles.Play(true);
        // TODO
        if (!isBlocksKiller()) 
        {
            Camera.main.GetComponent<GameController>().sendNextBlock();
        }
            
    }
}
