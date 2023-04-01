using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour, IHPListener, IEventListener
{
    public static Cam Instance { get; private set; }

    // Start is called before the first frame update
    private Transform following;
    private new Camera camera;
    private bool searching;

    [SerializeField] private Animator animator;
    [SerializeField] private AnimationCurve scrollCurve;
    [SerializeField, Range(0f, 10f)] private float panSpeed;
    [SerializeField, Range(0f, 10f)] private float scrollSpeed;
    [SerializeField] private AnimationCurve laggingSpeed;
    [SerializeField, Range(5f, 100f)] private float minZoom;
    [SerializeField, Range(1f, 5f)] private float maxZoom;

    private float shakeDuration = 0f;
    private Vector2 laggingFollowing;
    private float inputScroll;
    private Vector2 inputMousePos;

    private void Awake()
    {
        Instance = this;
        camera = GetComponent<Camera>();
    }

    void Start()
    {
        Search();
        EventSystem.AddEventListener(this, Event.ScreenFade);
    }

    private void Search()
    {
        searching = true;
        Player p = FindObjectOfType<Player>();
        if (p == null) Invoke(nameof(Search), 1f);
        else
        {
            p.GetComponent<HP>().AddListener(this);
            following = p.transform;
            searching = false;
        }
    }
    // Update is called once per frame

    void Update()
    {
        inputMousePos = Input.mousePosition;
        inputScroll += -Input.mouseScrollDelta.y;
    }
    void FixedUpdate()
    {
        if (following == null)
        {
            if (!searching) Search();
            return;
        }


        float scroll = inputScroll;
        scroll *= scrollSpeed;
        scroll *= scrollCurve.Evaluate((camera.orthographicSize-maxZoom) / (minZoom - maxZoom));
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize + scroll, maxZoom, minZoom);
        inputScroll = 0f;

        Vector2 mousePos = inputMousePos;
        Vector2 screenSize = new(Screen.width, Screen.height);
        Vector2 pan = new((mousePos.x - screenSize.x / 2f) / screenSize.x, (mousePos.y - screenSize.y / 2f) / screenSize.y);
        pan *= panSpeed;
        pan *= camera.orthographicSize;

        if (shakeDuration > 0f) shakeDuration -= Time.fixedDeltaTime;

        Vector2 followingPos = following.position;
        Vector2 lagDifference = (followingPos - laggingFollowing);
        Vector2 lagTravel = lagDifference * laggingSpeed.Evaluate((lagDifference).magnitude)*Time.fixedDeltaTime; 
        if (lagTravel.sqrMagnitude > lagDifference.sqrMagnitude)
        {
            lagTravel = lagDifference;
        }
        laggingFollowing += lagTravel;

        Vector3 newPos = laggingFollowing + new Vector2(pan.x+Mathf.Sin(shakeDuration*90f)*shakeDuration, pan.y);
        newPos.z = -10f;
        transform.position = newPos;
    }

    public void NewHP(float oldHP, float newHP)
    {
        if (newHP < oldHP) animator.SetTrigger("hurt");
    }
    public void Shake()
    {
        shakeDuration = Mathf.Max(1f, shakeDuration + 0.25f);
    }
    void IEventListener.EventDeclared(Event e)
    {
        if (e == Event.ScreenFade)
        {
            animator.SetTrigger("coolrotation");
        }
    }
}
