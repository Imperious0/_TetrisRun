using System.Collections;
using System.Collections.Generic;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        catchMotions();
    }
    private void catchMotions() 
    {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP_8_1
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }

            if (t.phase == TouchPhase.Ended)
            {
                secondPressPos = new Vector2(t.position.x, t.position.y);
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                // Make sure it was a legit swipe, not a tap
                if (currentSwipe.magnitude < minSwipeLength)
                {
                    currentMotion = Motions.None;
                    return;
                }

                currentSwipe.Normalize();

                // Swipe up
                if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f) {
                    currentMotion = Motions.Up;
                } else if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f) {
                    // Swipe down
                    currentMotion = Motions.Down;
                } else if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) {
                    // Swipe left
                    currentMotion = Motions.Left;
                } else if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) {
                    // Swipe right
                    currentMotion = Motions.Right;
                }
            }
        }
        else
        {
            currentMotion = Motions.None;
        }
#else
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
#endif
    }

    public void resetMotion() 
    {
        currentMotion = Motions.None; 
    }

}
