using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPCamera : MonoBehaviour
{
    GameObject player;

    public float camDistanceX;
    public float camDistanceY;
    public float camDistanceZ;

    public float smoothEffect;

    GameObject minimapCamera;

    // Start is called before the first frame update
    void Start()
    {
        minimapCamera = GameObject.Find("MinimapCamera");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float tempDistX = player.transform.position.x - camDistanceX;
        float tempDistY = player.transform.position.y - camDistanceY;
        float tempDistZ = player.transform.position.z - camDistanceZ;

        Vector3 tempDistance = new Vector3(tempDistX, tempDistY, tempDistZ);

        Vector3 camDistance = Vector3.Lerp(transform.position, tempDistance, smoothEffect);

        minimapCamera.transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

        transform.position = camDistance;

        transform.LookAt(player.transform);
    }
}
