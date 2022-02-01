using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float fadeSpeed = 1f;
    public Material garbage;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Awake() {
        garbage=gameObject.GetComponent<Renderer>().material;
        garbage.color = new Color(garbage.color.r, garbage.color.g, garbage.color.b, 0);

    }
    void OnEnable() {
        StartCoroutine("awakeGarbage");
    }
    IEnumerator destroyGarbage()
    {
        
        while(garbage.color.a > 0.01f)
        {
            garbage.color = new Color(garbage.color.r, garbage.color.g, garbage.color.b, garbage.color.a-fadeSpeed*Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }
    IEnumerator awakeGarbage()
    {
        
        while(garbage.color.a < 1f)
        {
            garbage.color = new Color(garbage.color.r, garbage.color.g, garbage.color.b, garbage.color.a+fadeSpeed*Time.deltaTime);
            yield return null;
        }
    }

    public void destroy(){
        StartCoroutine("destroyGarbage");
    }
}
