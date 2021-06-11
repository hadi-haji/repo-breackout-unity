using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    [SerializeField] Text nameText;
    [SerializeField] Text recoardText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    private int recoardNumber;
    
    private bool m_GameOver = false;


    private void SaveRecoard(int point)
    {
        DataRecoard dr = new DataRecoard();
        dr.recoard = point;
        string json = JsonUtility.ToJson(dr);
        File.WriteAllText(Application.persistentDataPath + "/DataRecoard.json", json);
    }


    int LoadReacord()
    {
        int recoard;
        string path = Application.persistentDataPath + "/DataRecoard.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            DataRecoard dr = JsonUtility.FromJson<DataRecoard>(json);
            recoard = dr.recoard;
        }
        else
        {
            recoard = 0;
        }
        return recoard;
    }

    public void GetName()
    {
        SaveName(nameText.text);
        SetRecoardText();
    }

    void SaveName(string name)
    {
        DataName dn = new DataName();
        dn.name = name;
        string json = JsonUtility.ToJson(dn);
        File.WriteAllText(Application.persistentDataPath + "/DataName.json", json);
    }


    string LoadName()
    {
        string name;
        string path = Application.persistentDataPath + "/DataName.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            DataName dn = JsonUtility.FromJson<DataName>(json);
            name = dn.name;
        }
        else
        {
            name = "Person";
        }
        return name;
    }


    private void Awake()
    {
        recoardNumber = LoadReacord();
        SetRecoardText();
    }

    void SetRecoardText()
    {
        recoardText.text = "Best Point : " + LoadName() + " : "+ recoardNumber;
    }

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
        CheckRecoard(m_Points);
    }

    void CheckRecoard(int point)
    {
        if(point > recoardNumber)
        {
            recoardNumber = point;
        }
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    public void Exit()
    {
        SaveRecoard(recoardNumber);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
    

    [System.Serializable]
    class DataName
    {
        public string name;
    }

    [System.Serializable]
    class DataRecoard
    {
        public int recoard;
    }
}
