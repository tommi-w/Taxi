using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour {

    public float directdamageFactor = 1f;
    public float indirectdamageFactor = .25f;
    public int damageTreshold = 1;
    public string lastPlayerTouched = "";
    public int health = 100;
    public float velocityTreshold = 5f;

    private Rigidbody2D rb;
    private FrictionJoint2D myJoint;
    public bool tryingToCapture = false;
    public bool isCaptured = false;

    private void Awake()
    {
        myJoint = GetComponent<FrictionJoint2D>();
        rb = GetComponent<Rigidbody2D>();
    }


    private int energy;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (GameManager.instance.isGameOver)
            return;
        if (health <= 0)
            return;

        if (other.relativeVelocity.magnitude >= (1.6f * velocityTreshold) && isCaptured)
            StartCoroutine(ReleaseAliveCustomer());

        if (other.gameObject.tag == "Player" && other.gameObject.name =="cage" )
        {
            //Debug.Log("A player hit " + gameObject.name);
            //Debug.Log("Other collider is  " + other.gameObject.name);
            Debug.Log("velocity in collision  " + other.relativeVelocity.magnitude);
            if (other.relativeVelocity.magnitude < velocityTreshold && !tryingToCapture && !isCaptured)
            {
                myJoint.connectedBody = other.rigidbody;
                myJoint.enabled = true;
                StartCoroutine(TryToCaptureCustomer(other));
            }
            else
            {
                lastPlayerTouched = "Player";
                GrantInfamy(other, directdamageFactor, true);
            }
        }
        else if (other.gameObject.GetComponent<DestroyableController>() != null)
        {
            if (other.gameObject.GetComponent<DestroyableController>().lastPlayerTouched == "Player")
            {
                lastPlayerTouched = "Player";
                GrantInfamy(other, indirectdamageFactor, true);
            }
        }
        else if (lastPlayerTouched == "Player")
        {
            GrantInfamy(other, indirectdamageFactor, false);
        }
    }

    IEnumerator TryToCaptureCustomer(Collision2D other)
    {
        tryingToCapture = true;

        while (tryingToCapture)
        {
            if (other.relativeVelocity.magnitude < velocityTreshold)
            {
                Debug.Log("distance at try to capture: " + Vector2.Distance(rb.position, other.rigidbody.position).ToString());
                if (Vector2.Distance(rb.position, other.rigidbody.position) < .3f)
                {
                    Debug.Log("Adding Impulse towards cage");
                    Vector2 force = new Vector2(30f * rb.mass * (other.rigidbody.position.x - rb.position.x), 100f * rb.mass * (other.rigidbody.position.y - rb.position.y));
                    rb.AddForce(force, ForceMode2D.Force);
                    yield return new WaitForFixedUpdate();
                    if (Vector2.Distance(rb.position, other.rigidbody.position) <= .02f)
                    {
                        myJoint.enableCollision = true;
                        tryingToCapture = false;
                        isCaptured = true;
                        StartCoroutine(GrantFame());
                        break;
                    }
                }
                else if (Vector2.Distance(rb.position, other.rigidbody.position) >= .6f)
                {
                    tryingToCapture = false;
                    myJoint.enabled = false;
                    myJoint.connectedBody = null;
                    break;
                }
                    
            }
            else
            {
                Debug.Log("Adding impulse away from cage");
                rb.AddForce(1f * rb.mass * -(other.rigidbody.position - rb.position), ForceMode2D.Impulse);
                tryingToCapture = false;
                myJoint.enabled = false;
                myJoint.connectedBody = null;
                break;
            }
            yield return null;
        }
        yield return null;
    }


    IEnumerator ReleaseDeadCustomer()
    {
        myJoint.enableCollision = false;
        Debug.Log("RELEASING DEAD CUSTOMER");
        while (isCaptured)
        {
            //Debug.Log("distance at time after DEATH: " + Vector2.Distance(rb.position, myJoint.connectedBody.position).ToString());
            if (Vector2.Distance(rb.position, myJoint.connectedBody.position) > .7f)
            {
                isCaptured = false;
                myJoint.enableCollision = true;
                myJoint.enabled = false;
                myJoint.connectedBody = null;
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator ReleaseAliveCustomer()
    {
        myJoint.enableCollision = false;
        Debug.Log("RELEASING ALIVE CUSTOMER");
        while (isCaptured)
        {
            //Debug.Log("distance at time after DEATH: " + Vector2.Distance(rb.position, myJoint.connectedBody.position).ToString());
            if (Vector2.Distance(rb.position, myJoint.connectedBody.position) > .7f)
            {
                isCaptured = false;
                myJoint.enabled = false;
                myJoint.connectedBody = null;
            }
            yield return new WaitForSeconds(.1f);
        }
    }


    IEnumerator GrantFame()
    {
        int points = 0;
        int startingHealth = health;
        Vector2 lastPosition = rb.position;
        while (isCaptured)
        {
            yield return new WaitForSeconds(1.0f);
            points = (int)((float)health / (float)startingHealth * Vector2.Distance(lastPosition,rb.position)*10);
            if (points >= 2)
            {
                points *= 100;
                GameManager.instance.GivePlayerFame(points);
                GameManager.instance.gui.ActivatePointsPopup(this.transform, points);
            }
            points = 0;
            lastPosition = rb.position;
        }
        yield return null;
    }

    private void GrantInfamy(Collision2D other, float damageFactor, bool isDestroyable)
    {
        if (isDestroyable)
            energy = (int)(0.5 * (other.rigidbody.mass + gameObject.GetComponent<Rigidbody2D>().mass) / 2 * other.relativeVelocity.magnitude * other.relativeVelocity.magnitude);
        else
            energy = (int)(0.5 * gameObject.GetComponent<Rigidbody2D>().mass * other.relativeVelocity.magnitude * other.relativeVelocity.magnitude);
        //if (energy >= 40)
        //Debug.Log("Energy of hit: " + energy);
        if (energy <= damageTreshold)
            return;

        health -= energy;
        if (health <= 0 && isCaptured)
            StartCoroutine(ReleaseDeadCustomer());

        if (energy > 100)
            GameManager.instance.StartSlowMotion();

        energy = 10 * (int)(damageFactor * energy);
        GameManager.instance.GivePlayerInfamy(energy);
        GameManager.instance.gui.ActivatePointsPopup(this.transform, energy);
    }
}
