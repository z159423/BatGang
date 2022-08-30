using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine;
using UnityEngine.EventSystems;

public class FixedTouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector]
    public Vector2 TouchDist;
    [HideInInspector]
    public Vector2 PointerOld;
    [HideInInspector]
    protected int PointerId;
    [HideInInspector]
    public bool Pressed;

    [SerializeField] private GameObject JoystickBody;
    [SerializeField] private GameObject JoystickHandle;

    [Space]

    [SerializeField] private RectTransform canvas;
    [SerializeField] private Camera mainCamera;


    private Vector2 touchStartPoint;
    private Vector2 currentTouchPoint;
    public Vector2 joystickDir;
    public float distBetweenJoystickBodyToHandle { get; set; }

    [SerializeField] private int clampDist = 130;

    // Update is called once per frame
    void Update()
    {
        if (Pressed)
        {
            if (PointerId >= 0 && PointerId < Input.touches.Length)
            {
                TouchDist = Input.touches[PointerId].position - PointerOld;
                PointerOld = Input.touches[PointerId].position;

                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            canvas, new Vector2(Input.mousePosition.x, Input.mousePosition.y),
                            mainCamera, out pos);

                currentTouchPoint = pos;

                //Vector2 clampJoystickDir = new Vector2(Mathf.Clamp(joystickDir.x, -100, 100), Mathf.Clamp(joystickDir.y, -100, 100));

                //JoystickHandle.transform.localPosition = touchStartPoint + clampJoystickDir;

                joystickDir = (currentTouchPoint - touchStartPoint);

                float joystickDist = Vector3.Distance(touchStartPoint, currentTouchPoint);

                if (joystickDist > clampDist)
                {
                    float fixPercent = (joystickDist - clampDist) / clampDist;

                    JoystickHandle.transform.localPosition = currentTouchPoint;

                    JoystickHandle.transform.localPosition = touchStartPoint + (joystickDir / (fixPercent + 1));

                    //print(joystickDist);
                }
                else
                {
                    JoystickHandle.transform.localPosition = currentTouchPoint;
                }

                joystickDist = Mathf.Clamp(joystickDist, 0, clampDist);
                distBetweenJoystickBodyToHandle = joystickDist / clampDist;


            }
            else
            {
                TouchDist = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - PointerOld;
                PointerOld = Input.mousePosition;
            }
        }
        else
        {
            TouchDist = new Vector2();

            distBetweenJoystickBodyToHandle = 0;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;
        PointerId = eventData.pointerId;
        PointerOld = eventData.position;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvas, new Vector2(Input.mousePosition.x, Input.mousePosition.y),
        mainCamera, out pos);

        JoystickBody.transform.localPosition = pos;
        JoystickHandle.transform.localPosition = pos;

        touchStartPoint = pos;

        JoystickBody.SetActive(true);
        JoystickHandle.SetActive(true);

        //print(pos);
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;

        joystickDir = Vector2.zero;

        JoystickBody.SetActive(false);
        JoystickHandle.SetActive(false);
    }

}