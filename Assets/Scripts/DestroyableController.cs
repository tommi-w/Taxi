using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyableController : MonoBehaviour {
    
    public float directdamageFactor = 1f;
    public float indirectdamageFactor = .25f;
    public int damageTreshold = 1;
    public string lastPlayerTouched = "";
    public int health = 100;

    private int energy;


    void OnCollisionEnter2D(Collision2D other)
    {
        if (GameManager.instance.isGameOver)
            return;
        if (health <= 0)
            return;

        if (other.gameObject.tag == "Player")
        {
            lastPlayerTouched = "Player";
            GrantInfamy(other, directdamageFactor, true);
        }
        else if(other.gameObject.GetComponent<DestroyableController>() != null)
        {
            if (other.gameObject.GetComponent<DestroyableController>().lastPlayerTouched == "Player")
            {
                lastPlayerTouched = "Player";
                GrantInfamy(other, indirectdamageFactor, true);
            }
        }
        else if (lastPlayerTouched == "Player")
        {
            GrantInfamy(other, indirectdamageFactor,false);
        }

    }

    private void GrantInfamy(Collision2D other,float damageFactor, bool isDestroyable)
    {
        if (isDestroyable)
            energy = (int)(0.5 * (other.rigidbody.mass + gameObject.GetComponent<Rigidbody2D>().mass)/2 * other.relativeVelocity.magnitude * other.relativeVelocity.magnitude);
        else
            energy = (int)(0.5 * gameObject.GetComponent<Rigidbody2D>().mass * other.relativeVelocity.magnitude * other.relativeVelocity.magnitude);
        //if (energy >= 40)
        //Debug.Log("Energy of hit: " + energy);
        if (energy <= damageTreshold)
            return;

        health -= energy;
        if (health <= 0)
            Destroy(gameObject, 2f);

        if (energy > 100)
            GameManager.instance.StartSlowMotion();

        energy = 10 * (int)(damageFactor * energy);
        GameManager.instance.GivePlayerInfamy(energy);
        GameManager.instance.gui.ActivatePointsPopup(this.transform, energy);
    }
}
