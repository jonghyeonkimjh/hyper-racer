using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private int gas = 100;
    [SerializeField] private float moveSpeed = 1f;
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
            yield return new WaitForSeconds(1);
            if (gas <= 0) break;
        }
        GameManager.Instance.EndGame();
    }

    /// <summary>
    /// 자동차 이동 메소드
    /// </summary>
    /// <param name="direction"></param>
    private void Move(float direction)
    {
        // wheel collider를 쓰는 방법과 velocity를 쓰는 방법 position을 변경하는 방법 
        transform.Translate(Vector3.right * (direction * moveSpeed * Time.deltaTime));
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.5f,1.5f), transform.position.y, transform.position.z);
    }

    /// <summary>
    /// 자동차 왼쪽 이동 메소드
    /// </summary>
    public void MoveLeft()
    {
        Move(-1);
    }

    /// <summary>
    /// 자동차 오른쪽 이동 메소드
    /// </summary>
    public void MoveRight()
    {
        Move(1);
    }

    /// <summary>
    /// 가스 아이템 획득시 호출되는 메소드
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gas"))
        {
            gas += 100;
            
            //가스 아이템 숨기기
            other.gameObject.SetActive(false);
        }

        if (other.CompareTag("Enemy"))
        {
            // 게임 난이도가 증가할수록 피해량이 증가함
            gas -= 30 * GameManager.Instance.Difficulty;
            
            //척 차량 숨기기
            other.gameObject.SetActive(false);
        }
    }
}
