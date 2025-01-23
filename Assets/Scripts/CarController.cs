using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Start is called before the first frame update
    public void Move(float direction)
    {
        // wheel collider를 쓰는 방법과 velocity를 쓰는 방법 position을 변경하는 방법 
        transform.Translate(Vector3.right * (direction * Time.deltaTime));
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.5f,1.5f), transform.position.y, transform.position.z);
    }
}
