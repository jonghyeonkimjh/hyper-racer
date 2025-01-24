using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  // 프리팹
  [SerializeField] private GameObject carPrefab;
  [SerializeField] private GameObject roadPrefab;

  // UI 관련
  [SerializeField] private MoveButton leftMoveButton;
  [SerializeField] private MoveButton rightMoveButton;
  [SerializeField] private TMP_Text gasText;
  [SerializeField] private TMP_Text difficultyText;
  [SerializeField] private TMP_Text elapsedTimeText;
  [SerializeField] private GameObject startPanelPrefab;
  [SerializeField] private GameObject endPanelPrefab;
  [SerializeField] private Transform canvasTransform;
  
  // 자동차
  private CarController _carController;
  
  // 로드 오브젝트 풀
  private int _roadPoolSize = 3;
  private Queue<GameObject> _roadPool = new Queue<GameObject>();
  
  // 도로 이동
  private List<GameObject> _activeRoads = new List<GameObject>();
  
  // 만들어지는 도로의 index
  private int _roadIndex;
  
  // 상태
  public enum State
  {
    Start,Play,End
  }
  public State GameState { get; private set; } = State.Start;

  // 게임 진행 시간
  public float ElapsedTime { get; private set; }

  // 게임 난이도
  public int Difficulty { get; private set; }
  
  // 싱글턴
  private static GameManager _instance;
  public static GameManager Instance
  {
    get
    {
      if (_instance == null)
      {
        _instance = FindObjectOfType<GameManager>();
      }
      return _instance;
    }
  }

  private void Awake()
  {
    if (_instance != null && _instance != this)
    {
      Destroy(this.gameObject);
    }
    else
    {
      _instance = this;
    }

    // Time.timeScale = 5f; // 0 stop game
  }

  private void Start()
  {
    // Road 오브젝트 풀 초기화
    InitializeRoadPool();
    
    // 게임 상태를 Start로 변경
    GameState = State.Start;
    
    //Start Panel 표시
    ShowStartPanel();
  }

  private void Update()
  {
    switch (GameState)
    {
      case State.Start:
        break;
      case State.Play:
        // 게임 진행 시간이 증가 할수록 난이도 증가
        ElapsedTime += Time.deltaTime;
        if ((int)ElapsedTime % 10 == 0 && ElapsedTime > 0)
        {
          // 10초마다 난이도 증가, 최대 Level 20
          Difficulty = Math.Clamp((int)ElapsedTime / 10 + 1, 0, 20);
        }
        foreach (var activeRoad in _activeRoads)
        {
          // 게임 시간이 증가할수록 도로의 속도도 빨라짐
          activeRoad.transform.Translate(Time.deltaTime* Difficulty * Vector3.back);
        }
        if (_carController != null) gasText.text = $"{_carController.Gas}";
        if (_carController != null) elapsedTimeText.text = $"{(int)ElapsedTime}s";
        if (_carController != null) difficultyText.text = $"LEVEL {Difficulty}";
        break;
      case State.End:
        break;
    }
  }

  private void StartGame()
  {
    // _roadIndex 초기화
    Difficulty = 1;
    ElapsedTime = 0;
    _roadIndex = 0;

    // 도로 생성
    SpawnRoad(Vector3.zero);
    
    // 지동차 생성 -3unit (-3f) from z;
    _carController = Instantiate(carPrefab, new Vector3(0, 0, -3f), Quaternion.identity)
      .GetComponent<CarController>();
    
    // Left, Right move button에 자동차 컨트롤 기능 적용
    leftMoveButton.OnMoveButtonDown += _carController.MoveLeft;
    rightMoveButton.OnMoveButtonDown += _carController.MoveRight;
    
    // 게임 상태를 Play로 변경
    GameState = State.Play;
  }

  public void EndGame()
  {
    GameState = State.End;
    
    leftMoveButton.OnMoveButtonDown -= _carController.MoveLeft;
    rightMoveButton.OnMoveButtonDown -= _carController.MoveRight;
    
    //자동차 제거
    Destroy(_carController.gameObject);
    
    //도로 제거
    foreach (var activeRoad in _activeRoads)
    {
      activeRoad.SetActive(false);
    }

    // 게임 오버 화면 표시
    ShowEndPanel();
  }

  #region UI
  /// <summary>
  /// 시작 화면을 표시
  /// </summary>
  private void ShowStartPanel()
  {
    var startPanelController = Instantiate(startPanelPrefab, canvasTransform)
      .GetComponent<StartPanelController>();
    startPanelController.OnStartButtonClick += () =>
    {
      StartGame();
      Destroy(startPanelController.gameObject);
    };
  }

  private void ShowEndPanel()
  {
    var endPanelController = Instantiate(endPanelPrefab, canvasTransform)
      .GetComponent<StartPanelController>();
    endPanelController.OnStartButtonClick += () =>
    {
      Destroy(endPanelController.gameObject);
      ShowStartPanel();
    };
  }

  #endregion

  //도로 생성 및 관리
  #region 도로 생성 및 관리
  /// <summary>
  /// 도로 오브젝트 풀 초기화
  /// </summary>
  private void InitializeRoadPool()
  {
    for (var i = 0; i < _roadPoolSize; i++)
    {
      var road = Instantiate(roadPrefab);
      road.SetActive(false);
      _roadPool.Enqueue(road);
    }
  }
  
  /// <summary>
  /// 도로 오브젝트 풀에서 불러와 배치하는 함수
  /// </summary>
  /// <param name="position"></param>
  public void SpawnRoad(Vector3 position)
  {
    GameObject road;
    if (_roadPool.Count > 0)
    {
      road = _roadPool.Dequeue();
      road.transform.position = position;
      road.SetActive(true);
    }
    else
    {
      road = Instantiate(roadPrefab, position, Quaternion.identity);
    }
    
    //가스 아이템 및 적 차량 생성
    if (_roadIndex > 0 && _roadIndex%2 == 0)
    {
      road.GetComponent<RoadController>().SpawnGas();
    }
    
    if (_roadIndex > 0 && _roadIndex%3 == 0)
    {
      road.GetComponent<RoadController>().SpawnEnemy();
    }
    
    
    // 활성화 된 길을 움직이기 위해 List에 저장
    _activeRoads.Add(road);
    _roadIndex++;
  }
  

  /// <summary>
  /// 
  /// </summary>
  /// <param name="road"></param>
  public void DespawnRoad(GameObject road)
  {
    road.SetActive(false);
    _activeRoads.Remove(road);
    _roadPool.Enqueue(road);
  }
  #endregion
  
}
