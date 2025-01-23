using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private int gas = 100;
    [SerializeField] private float moveSpeed = 1f;
    // Start is called before the first frame update
    public int Gas => gas;

    private void Start()
    {
        StartCoroutine(GasCoroutine());
    }

    private IEnumerator GasCoroutine()
    {
        while (true)
        {
            gas -= 10;
            if (gas <= 0) break;
            yield return new WaitForSeconds(1);
        }
        //TODO: 게임 종료
    }

    /// <summary>
    /// 자동차 이동 메소드
    /// </summary>
    /// <param name="direction"></param>
    public void Move(float direction)
    {
        // wheel collider를 쓰는 방법과 velocity를 쓰는 방법 position을 변경하는 방법 
        transform.Translate(Vector3.right * (direction * moveSpeed * Time.deltaTime));
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.5f,1.5f), transform.position.y, transform.position.z);
    }

    /// <summary>
    /// 가스 아이템 획득시 호출되는 메소드
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gas"))
        {
            gas += 30;
            // TODO: 가스 아이템 제거
        }
    }
}
