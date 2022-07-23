using UnityEngine;

public enum Motions { None, Tap, Up, Down, Left, Right };
public class MotionCapture : MonoBehaviour
{
    [SerializeField]
    private float minSwipeLength = 200f;

    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    private Motions currentMotion = Motions.None;

    public Motions CurrentMotion { get { Motions tmp = currentMotion; currentMotion = Motions.None; return tmp; } }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        catchMotions();
    }
    private void catchMotions() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            if (currentSwipe.magnitude < minSwipeLength)
            {
                // Click
                currentMotion = Motions.Tap;
                return;
            }

            currentSwipe.Normalize();

            if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            {
                // Swipe up
                currentMotion = Motions.Up;
            }else if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            {
                // Swipe down
                currentMotion = Motions.Down;
            }else if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                // Swipe left
                currentMotion = Motions.Left;
            }else if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                // Swipe right
                currentMotion = Motions.Right;
            }
        }

    }

    public void resetMotion() 
    {
        currentMotion = Motions.None; 
    }

}
