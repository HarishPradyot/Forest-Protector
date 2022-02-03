using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrapLever : MonoBehaviour
{
    [SerializeField]
    private float fadeSpeed = 1f;

    [SerializeField]
    private float waitDuration=4f;  // Time for which chest should remain opened before closing again
    private Animator trapAnimator;
    private string OPEN_TRAP, CLOSE_TRAP;
    // Start is called before the first frame update
    void Start()
    {
        trapAnimator=GetComponent<Animator>();
        OPEN_TRAP="Base Layer.Trap Open";
        CLOSE_TRAP="Base Layer.Trap Close";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void openTrap()
    {
        Debug.Log("Long Presss Working");
        trapAnimator.Play(OPEN_TRAP);
        StartCoroutine(closeTrap());
    }
    IEnumerator closeTrap()
    {
        float startTime=Time.time;
        while(Time.time-startTime<=waitDuration)
            yield return null;
        trapAnimator.Play(CLOSE_TRAP);
        StartCoroutine("destroyTrap");
    }


    IEnumerator destroyTrap()
    {
        Material trap=gameObject.GetComponent<Renderer>().material;
        while(trap.color.a > 0.01f)
        {
            trap.color = new Color(trap.color.r, trap.color.g, trap.color.b, trap.color.a-fadeSpeed*Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }
}
