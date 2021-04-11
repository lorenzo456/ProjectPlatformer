using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Credits for inspiration = https://www.youtube.com/watch?v=11ofnLOE8pw&ab_channel=AlexanderZotov

public class BezierFollow : MonoBehaviour
{
    [SerializeField]
    private Transform[] routes = new Transform[0];
    private int currentRoute;
    private float tParam;
    private Vector3 objectPosition;
    private Vector3 p0;
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;

    public float speedModifier = 0.5f;
    public float waitTime = 3;

    void Start()
    {
        currentRoute = 0;
        tParam = 0f;
        transform.position = routes[0].GetChild(0).position;
        StartCoroutine(FollowRoute(currentRoute));
    }
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, objectPosition, 2 * Time.deltaTime);
    }
    private IEnumerator FollowRoute(int route, bool reversed = false)
    {
        if(route == 0 && Vector3.Distance(transform.position, routes[0].GetChild(0).position) < 0.05 
            || route == routes.Length -1 && Vector3.Distance(transform.position, routes[routes.Length -1].GetChild(3).position) < 0.05)
        {
            yield return new WaitForSeconds(waitTime);
        }

        if (reversed)
        {
            p0 = routes[route].GetChild(3).position;
            p1 = routes[route].GetChild(2).position;
            p2 = routes[route].GetChild(1).position;
            p3 = routes[route].GetChild(0).position;
        }
        else
        {
            p0 = routes[route].GetChild(0).position;
            p1 = routes[route].GetChild(1).position;
            p2 = routes[route].GetChild(2).position;
            p3 = routes[route].GetChild(3).position;
        }

        while(tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;
            objectPosition = Mathf.Pow(1 - tParam, 3) * p0 +
                3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                Mathf.Pow(tParam, 3) * p3;

            //transform.position = objectPosition;
            yield return new WaitForEndOfFrame();
        }

        tParam = 0;

        if (reversed)
        {
            currentRoute--;
        }
        else
        {
            currentRoute++;
        }

        if (currentRoute > routes.Length - 1)
        {
            reversed = true;
            currentRoute--;
        }else if(currentRoute < 0)
        {
            currentRoute++;
            reversed = false;
        }



        StartCoroutine(FollowRoute(currentRoute, reversed));

        yield break;
    }
}
