using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadController : MonoBehaviour
{
    [SerializeField] private GameObject[] gasObjects;
    [SerializeField] private GameObject[] enemyObjects;

    private void OnEnable()
    {
        // 모든 가스 아이템 비활성화
        foreach (var gasObject in gasObjects)
        {
            gasObject.SetActive(false);
        }
        
        // 모든 적 비활성화
        foreach (var enemyObject in enemyObjects)
        {
            enemyObject.SetActive(false);
        }
    }

    /// <summary>
    /// 플레이어 차량이 도로에 진입하면 다음 도로를 생성
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SpawnRoad(transform.position + new Vector3(0, 0, 10));
        }
    }

    /// <summary>
    ///  플레이어 차량이 도로를 벗어나면 해당 도로를 제거
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.DespawnRoad(gameObject);
        }
    }

    /// <summary>
    /// 랜덤으로 가스 아이템을 표시
    /// </summary>
    public void SpawnGas()
    {
        var randomIndex = Random.Range(0, gasObjects.Length);
        gasObjects[randomIndex].SetActive(true);
    }
    
    /// <summary>
    /// 랜덤으로 척 차량 표시
    /// </summary>
    public void SpawnEnemy()
    {
        var randomIndex = Random.Range(0, enemyObjects.Length);
        enemyObjects[randomIndex].SetActive(true);
    }
}
