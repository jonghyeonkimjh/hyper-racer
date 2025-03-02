using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class GameTestScript
{
    private CarController _carController;
    private GameObject _leftMoveButton;
    private GameObject _rightMoveButton;
    
    // A Test behaves as an ordinary method
    [Test]
    public void GameTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator GameTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.

        
        // 타임 스케일 변경
        Time.timeScale = 10f;
        
        
        // 씬 로드하기
        SceneManager.LoadScene("Scenes/Game", LoadSceneMode.Single);
        yield return WaitForSceneLoad();


        // 필수 오브젝트 확인 
        var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Assert.IsNotNull(gameManager, "gameManager is Null");
        
        var startButton = GameObject.Find("Start Button");
        Assert.IsNotNull(startButton, "startButton is null");
        
        // 게임 실행
        startButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
        
        // 플레이어 자동차
        _carController = GameObject.Find("Car(Clone)").GetComponent<CarController>();
        Assert.IsNotNull(_carController, "carController is null");

        //게임 제어 관련 버튼 확인
        _leftMoveButton = GameObject.Find("LeftMoveButton");
        Assert.IsNotNull(_leftMoveButton, "leftButton is null");
        _rightMoveButton = GameObject.Find("RightMoveButton");
        Assert.IsNotNull(_rightMoveButton, "rightButton is null");
        
        // 가스의 스폰 위치 파악 하기
        var leftPosition = new Vector3(-1f, 0.2f, -3f);
        var rightPosition = new Vector3(1f, 0.2f, -3f);
        var centerPosition = new Vector3(0, 0.2f, -3f);

        var rayDistance = 10f;
        var rayDirection = Vector3.forward;
        
        // 플레이 시간
        float elapsedTime = 0f;
        float targetTime = 10f;

        
        //반복
        while (gameManager.GameState == GameManager.State.Play)
        {
            RaycastHit hit;
            if (Physics.Raycast(leftPosition, rayDirection, out hit, rayDistance,
                    LayerMask.GetMask("Gas")))
            {
                Debug.Log("left");
                MoveCar(hit.point);
            }
            else if (Physics.Raycast(rightPosition, rayDirection, out hit, rayDistance,
                          LayerMask.GetMask("Gas")))
            {
                Debug.Log("right");
                MoveCar(hit.point);
            }
            else if (Physics.Raycast(centerPosition, rayDirection, out hit, rayDistance,
                         LayerMask.GetMask("Gas")))
            {
                Debug.Log("center");
                MoveCar(hit.point);
            }
            else
            {
                Debug.Log("None");
                MoveButtonUp(_leftMoveButton);
                MoveButtonUp(_rightMoveButton);
            }
            
            
            Debug.DrawRay(leftPosition, rayDirection*rayDistance, Color.red);
            Debug.DrawRay(rightPosition, rayDirection*rayDistance, Color.green);
            Debug.DrawRay(centerPosition, rayDirection*rayDistance, Color.blue);

            // 시간 체크
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        if (elapsedTime < targetTime)
        {
            Assert.Fail("Game Time is too short");
        }
        
        Time.timeScale = 1f;
    }

    private IEnumerator WaitForSceneLoad()
    {
        while (SceneManager.GetActiveScene().buildIndex > 0)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Move Button 다운
    /// </summary>
    /// <param name="moveButton"></param>
    private void MoveButtonDown(GameObject moveButton)
    {
        var pointerEventData = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(moveButton, pointerEventData, ExecuteEvents.pointerDownHandler);
    }
    
    /// <summary>
    /// Move Button Up
    /// </summary>
    /// <param name="moveButton"></param>
    private void MoveButtonUp(GameObject moveButton)
    {
        var pointerEventData = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(moveButton, pointerEventData, ExecuteEvents.pointerUpHandler);
    }

    /// <summary>
    /// 플레이어 자동차 이동
    /// </summary>
    /// <param name="targetPosition">이동 타겟 위치</param>
    private void MoveCar(Vector3 targetPosition)
    {
        if (Mathf.Abs(targetPosition.x - _carController.transform.position.x) < 0.1f)
        {
            MoveButtonUp(_rightMoveButton);
            MoveButtonUp(_leftMoveButton);
            return;
        }
        if (targetPosition.x < _carController.transform.position.x)
        {
            // 왼쪽으로 이동
            MoveButtonDown(_leftMoveButton);
            MoveButtonUp(_rightMoveButton);

        }
        else if (targetPosition.x > _carController.transform.position.x)
        {
            // 오른쪽으로 이동
            MoveButtonDown(_rightMoveButton);
            MoveButtonUp(_leftMoveButton);
        }
        else
        {
            MoveButtonUp(_rightMoveButton);
            MoveButtonUp(_leftMoveButton);
        }
    }
}
