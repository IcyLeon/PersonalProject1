using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalOrb : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    void Explode()
    {

    }

    public IEnumerator MoveToTargetLocation(Vector3 target, float speed)
    {
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }
}
