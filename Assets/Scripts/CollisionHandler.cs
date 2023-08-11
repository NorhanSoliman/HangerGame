using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionHandler : MonoBehaviour
{
    public string pinTag = "Pin";

    // Hanger Pivots
    private HingeJoint2D hingeJoint;
    [SerializeField]
    private Transform rightPivot;
    [SerializeField]
    private Transform leftPivot;
    [SerializeField]
    private Transform originalPivot;

    // UI Screens
    public GameOverScreen YouWinScreen;
    public GameOverScreen GameOverScreen;
    public GameObject requirementsImage;
    int childIndexToActivate = 0;


    // Collectible Flying
    public float moveDuration = 1.0f; // Time in seconds to move the collectible
    public Vector3 upPosition; // Position where the collectible moves

    private bool isMoving = false;

    //Pin Destroying
    public Sprite changedShapeSprite;
    private GameObject pinObject;
    private SpriteRenderer pinSpriteRenderer;

    // Music
    AudioSource audioSource;

    public AudioClip NailClick;
    public AudioClip HangerBreak;
    public AudioClip Collectible;
    public AudioClip HangerBreakSound;
    public AudioClip YouLoseSound;

    private PolygonCollider2D polygonCollider;

    private void Start()
    {
        hingeJoint = GetComponent<HingeJoint2D>();
        audioSource = GetComponent<AudioSource>();
        polygonCollider = GetComponent<PolygonCollider2D>();

    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Main controller of all triggers with the hanger
    private void OnTriggerEnter2D(Collider2D other)
    {
        //nail position
        if (other.gameObject.CompareTag(pinTag))
        {
            //Debug.Log("Pin Trigger entered");

            PlaySound(NailClick);


            Vector3 childPosition = other.gameObject.transform.GetChild(0).transform.position;
            //Debug.Log(childPosition);

            hingeJoint.connectedAnchor = childPosition; //connected anchor is always the nailPivot

            //hingeJoint.connectedAnchor = collisionPoint;

            float distanceToRight = Vector2.Distance(childPosition, rightPivot.position);
            float distanceToLeft = Vector2.Distance(childPosition, leftPivot.position);
            float distanceToOriginal = Vector2.Distance(childPosition, originalPivot.position);

            float minDistance = Mathf.Min(distanceToRight, distanceToLeft, distanceToOriginal);

            if (minDistance == distanceToRight)
            {
                Vector3 PivotToNail = childPosition - rightPivot.position;
                //PivotToNail.x += 0.2f;
                PivotToNail.y += 0.2f;
                //repeat

                transform.position += PivotToNail;
                //hingeJoint.connectedAnchor = rightPivot.position;
                hingeJoint.anchor = hingeJoint.transform.InverseTransformPoint(rightPivot.position);

                //hingeJoint.anchor = rightPivot.position;
                //Debug.Log("Minimum distance to Right Pivot");
            }
            else if (minDistance == distanceToLeft)
            {
                Vector3 PivotToNail = childPosition - leftPivot.position;
                transform.position += PivotToNail;
                //hingeJoint.connectedAnchor = leftPivot.position;
                hingeJoint.anchor = hingeJoint.transform.InverseTransformPoint(leftPivot.position);
                //hingeJoint.anchor = leftPivot.position;
                //Debug.Log("Minimum distance to Left Pivot");
            }
            else if (minDistance == distanceToOriginal)
            {
                Vector3 PivotToNail = childPosition - originalPivot.position;
                transform.position += PivotToNail;
                //hingeJoint.connectedAnchor = originalPivot.position;
                hingeJoint.anchor = hingeJoint.transform.InverseTransformPoint(originalPivot.position);
                //hingeJoint.anchor = originalPivot.position;

                //Debug.Log("Minimum distance to Original Pivot");
            }


        }

        if (other.gameObject.CompareTag("Collectible") && !isMoving)
        {
            PlaySound(Collectible);
            isMoving = true;
            StartCoroutine(MoveToPosition(other.gameObject, upPosition));

            Transform requirementsTransform = requirementsImage.transform;
            int childCount = requirementsTransform.childCount;

            GameObject childObject = requirementsImage.transform.GetChild(childIndexToActivate).gameObject;

            //GameObject childObject = requirementsTransform.GetChild(childCount).gameObject;
            //GameObject childToActivate = gameObject.GetChild(childIndexToActivate);

            childObject.SetActive(true);
            if (childIndexToActivate == 4)
            {
                YouWinScreen.SetUp();   
            }
            else
            {
                childIndexToActivate += 1;
            }
        }       

        if (other.gameObject.CompareTag("Blanket"))
        {
            Debug.Log("OOOPPS!Hanger is dead ;( ");

            PlaySound(HangerBreakSound);

            GameOverScreen.SetUp();

            StopBackgroundMusic();
            audioSource.PlayOneShot(YouLoseSound);

            // Deactivate the collider to prevent the hanger from further losing or moving
            polygonCollider.enabled = false;

        }
    }

    //Moves the collectible to a specific position 
    private IEnumerator MoveToPosition(GameObject collectible, Vector3 targetPosition)
    {
        Vector3 initialPosition = collectible.transform.position;
        float elapsedTime = 0f; // Initialize the elapsed time to track the progress of the movement

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime; // Increment the elapsed time by the time passed since the last frame
            float t = Mathf.Clamp01(elapsedTime / moveDuration); // The normalized time value from 0 to 1

            // Interpolate the position of the collectible from its initial position to the target position
            // using a smooth Lerp function and apply the desired movement speed
            collectible.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            yield return null;
        }
        Destroy(collectible);
        isMoving = false;
    }

    //when the hanger exits the trigger of the nail, nail's shape changes
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Pin"))
        {
            pinObject = other.gameObject;
            //Debug.Log("Pin broken");
            other.enabled = false; // Disable the collider of the nail
            ChangePinShape();

        }
    }

    // Replaces the sprite of the nail to the broken one
    private void ChangePinShape()
    {
        pinSpriteRenderer = pinObject.GetComponent<SpriteRenderer>();
        pinSpriteRenderer.sprite = changedShapeSprite;
    }

    // Called on losing to stop BG music
    public void StopBackgroundMusic()
    {
        GameObject backgroundMusicObject = GameObject.Find("BackgroundMusic");

        if (backgroundMusicObject != null)
        {
            AudioSource backgroundMusicAudioSource = backgroundMusicObject.GetComponent<AudioSource>();
            if (backgroundMusicAudioSource != null)
            {
                backgroundMusicAudioSource.Stop();
            }
        }
    }
}