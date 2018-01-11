using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameWindow : SingletonWindow<GameWindow>
{

    public override string bundle
    {
        get
        {
            return "Game";
        }
    }

    public GameObject m_greenGo;
    public GameObject m_redGo;
    public GameObject m_orangeGo;
    public GameObject m_hightLightGo;
    public GameObject AddSquareBtn;

    public Transform leftParent;
    public Transform middleParent;
    public Transform rightParent;

    public List<List<int>> m_colorInfoList = new List<List<int>>();
    public List<List<GameObject>> m_colorGoList = new List<List<GameObject>>();
    public Dictionary<int, List<GameObject>> m_cacheList = new Dictionary<int, List<GameObject>>();

    public GameObject m_selectGo;
    public int m_selectColunmIndex;

    public float m_timer;

    public bool isGameOver = false;

    private float updateTime=2.5f;

    private GridLayoutGroup m_grid;

    public Text m_scoreTxt;

    private int m_score;
    private int m_comboScore = 10;

    private int m_comboLimit = 6;//超过6下不算连击
    private int m_operateCount = 6;//当前操作次数
    private int m_comboCount = 0;//已经连击的次数
    private float m_comboFactor = 1;//连击的加成系数


    public override void OnInit()
    {
        m_greenGo = Find("Container/GreenGo");
        m_redGo = Find("Container/RedGo");
        m_orangeGo = Find("Container/OrangeGo");
        m_hightLightGo = Find("Container/selectGo");
        leftParent = Find("Container/clickArea/LeftArea").transform;
        middleParent = Find("Container/clickArea/MiddleArea").transform;
        rightParent = Find("Container/clickArea/RightArea").transform;
        AddSquareBtn = Find("Container/Button");
        m_grid = leftParent.GetComponent<GridLayoutGroup>();

        Register(leftParent.gameObject).onClick = OnClickLeft;
        Register(middleParent.gameObject).onClick = OnClickMiddle;
        Register(rightParent.gameObject).onClick = OnClickRight;
        Register(AddSquareBtn).onClick = delegate { AddSquare(); };
        base.OnInit();
    }

    public override void OnShow(params object[] param)
    {
        base.OnShow(param);
        m_timer = 0;
        m_greenGo.SetActive(false);
        m_redGo.SetActive(false);
        m_orangeGo.SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            m_colorInfoList.Add(new List<int>());
            for (int j = 0; j < 2; j++)
            {
                m_colorInfoList[i].Add(Random.Range(1, 4));
            }
        }

        ShowSquare();
    }

    public void ShowSquare()
    {
        m_colorGoList.Clear();
        for (int i = 0; i < m_colorInfoList.Count; i++)
        {
            m_colorGoList.Add(new List<GameObject>());
            for (int j = 0; j < m_colorInfoList[i].Count; j++)
            {
                GameObject go = Get(m_colorInfoList[i][j]);
                switch (i)
                {
                    case 0:
                        go.transform.SetParent(leftParent);
                        break;
                    case 1:
                        go.transform.SetParent(middleParent);
                        break;
                    case 2:
                        go.transform.SetParent(rightParent);
                        break;
                }
                m_colorGoList[i].Add(go);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
            }
        }
    }

    public GameObject Get(int color)
    {
        GameObject go = null;
        if (m_cacheList.ContainsKey(color) == false)
            m_cacheList.Add(color, new List<GameObject>());
        switch (color)
        {
            case 1:
                go = GameObject.Instantiate(m_greenGo) as GameObject;
                m_cacheList[color].Add(go);
                break;
            case 2:
                go = GameObject.Instantiate(m_redGo) as GameObject;
                m_cacheList[color].Add(go);
                break;
            case 3:
                go = GameObject.Instantiate(m_orangeGo) as GameObject;
                m_cacheList[color].Add(go);
                break;
        }
        go.SetActive(true);
        return go;
    }



    public override void OnUpdate()
    {
        base.OnUpdate();
        if (isGameOver)
            return;

        m_timer += Time.deltaTime;
        if (m_timer >= updateTime)
        {
            AddSquare();
            m_timer = 0;
        }

        if (Input.GetKeyDown(KeyCode.D))
            AddSquare();

    }

    public void OnClickLeft(GameObject go, PointerEventData data)
    {
        if (isGameOver)
            return;

        m_operateCount--;
        if (m_selectGo == null)
        {
            if (m_colorGoList[0].Count <= 0)
                return;
            m_selectColunmIndex = 1;
            m_selectGo = m_colorGoList[m_selectColunmIndex - 1][m_colorGoList[m_selectColunmIndex - 1].Count - 1];
            SetIsSelect(m_selectGo, true);
        }
        else
        {
            SetIsSelect(m_selectGo, false);
            m_selectGo = null;
            if (m_selectColunmIndex == 1)
            {
                m_selectColunmIndex = 0;
            }
            else
            {
                GameObject obj = m_colorGoList[m_selectColunmIndex - 1][m_colorGoList[m_selectColunmIndex - 1].Count - 1];
                m_colorGoList[m_selectColunmIndex - 1].RemoveAt(m_colorGoList[m_selectColunmIndex - 1].Count - 1);

                int color = m_colorInfoList[m_selectColunmIndex - 1][m_colorInfoList[m_selectColunmIndex - 1].Count - 1];
                m_colorInfoList[m_selectColunmIndex - 1].RemoveAt(m_colorInfoList[m_selectColunmIndex - 1].Count - 1);

                m_selectColunmIndex = 1;
                m_selectGo = null;
                m_colorGoList[m_selectColunmIndex - 1].Add(obj);
                m_colorInfoList[m_selectColunmIndex - 1].Add(color);
                obj.transform.SetParent(leftParent);

                CheckCombo();
            }
        }
    }

    public void OnClickMiddle(GameObject go, PointerEventData data)
    {
        if (isGameOver)
            return;

        m_operateCount--;
        if (m_selectGo == null)
        {
            if (m_colorGoList[1].Count <= 0)
                return;
            m_selectColunmIndex = 2;
            m_selectGo = m_colorGoList[m_selectColunmIndex - 1][m_colorGoList[m_selectColunmIndex - 1].Count - 1];
            SetIsSelect(m_selectGo, true);
        }
        else
        {
            SetIsSelect(m_selectGo, false);
            m_selectGo = null;
            if (m_selectColunmIndex == 2)
            {
                m_selectColunmIndex = 0;
            }
            else
            {
                GameObject obj = m_colorGoList[m_selectColunmIndex - 1][m_colorGoList[m_selectColunmIndex - 1].Count - 1];
                m_colorGoList[m_selectColunmIndex - 1].RemoveAt(m_colorGoList[m_selectColunmIndex - 1].Count - 1);

                int color = m_colorInfoList[m_selectColunmIndex - 1][m_colorInfoList[m_selectColunmIndex - 1].Count - 1];
                m_colorInfoList[m_selectColunmIndex - 1].RemoveAt(m_colorInfoList[m_selectColunmIndex - 1].Count - 1);

                m_selectColunmIndex = 2;
                m_selectGo = null;
                m_colorGoList[m_selectColunmIndex - 1].Add(obj);
                m_colorInfoList[m_selectColunmIndex - 1].Add(color);
                obj.transform.SetParent(middleParent);
                CheckCombo();
            }
        }
    }

    public void OnClickRight(GameObject go, PointerEventData data)
    {
        if (isGameOver)
            return;

        m_operateCount--;
        if (m_selectGo == null)
        {
            if (m_colorGoList[2].Count <= 0)
                return;
            m_selectColunmIndex = 3;
            m_selectGo = m_colorGoList[m_selectColunmIndex - 1][m_colorGoList[m_selectColunmIndex - 1].Count - 1];
            SetIsSelect(m_selectGo, true);
        }
        else
        {
            SetIsSelect(m_selectGo, false);
            m_selectGo = null;
            if (m_selectColunmIndex == 3)
            {
                m_selectColunmIndex = 0;
            }
            else
            {
                GameObject obj = m_colorGoList[m_selectColunmIndex - 1][m_colorGoList[m_selectColunmIndex - 1].Count - 1];
                m_colorGoList[m_selectColunmIndex - 1].RemoveAt(m_colorGoList[m_selectColunmIndex - 1].Count - 1);

                int color = m_colorInfoList[m_selectColunmIndex - 1][m_colorInfoList[m_selectColunmIndex - 1].Count - 1];
                m_colorInfoList[m_selectColunmIndex - 1].RemoveAt(m_colorInfoList[m_selectColunmIndex - 1].Count - 1);

                m_selectColunmIndex = 3;
                m_selectGo = null;
                m_colorGoList[m_selectColunmIndex - 1].Add(obj);
                m_colorInfoList[m_selectColunmIndex - 1].Add(color);
                obj.transform.SetParent(rightParent);
                CheckCombo();
            }
        }
    }

    public void SetIsSelect(GameObject go, bool state)
    {
        m_hightLightGo.SetActive(state);
        if (state)
            m_hightLightGo.transform.position = m_selectGo.transform.position;
    }

    public void AddSquare()
    {
        if (isGameOver)
            return;

        for (int i = 0; i < 3; i++)
        {
            int color = Random.Range(1, 4);
            if (m_colorInfoList[i].Count > 0)
            {
                while (color == m_colorInfoList[i][0])
                {
                    color = Random.Range(1, 4);
                }
            }
            m_colorInfoList[i].Insert(0, color);
            GameObject go = Get(color);
            m_colorGoList[i].Insert(0, go);

            switch (i)
            {
                case 0:
                    go.transform.SetParent(leftParent);
                    break;
                case 1:
                    go.transform.SetParent(middleParent);
                    break;
                case 2:
                    go.transform.SetParent(rightParent);
                    break;
            }
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.SetAsFirstSibling();
        }
        if (m_selectGo != null)
        {
            m_hightLightGo.transform.localPosition -= new Vector3(0, m_grid.cellSize.y+m_grid.spacing.y,0);
        }
        CheckCombo();
        CheckIsDeath();
    }

    public void CheckIsDeath()
    {
        for (int i = 0; i < 3; i++)
        {
            if (m_colorInfoList[i].Count >= 12)
                isGameOver = true;
        }
    }

    public void CheckCombo()
    {
        for (int i = 0; i < 3; i++)
        {
            int color = 0;
            int count = 0;
            for (int j = 0; j < m_colorInfoList[i].Count; j++)
            {
                if (color != m_colorInfoList[i][j])
                {
                    color = m_colorInfoList[i][j];
                    count = 1;
                }
                else
                {
                    count++;
                    if (count == 3)
                    {
                        m_colorInfoList[i].RemoveAt(j);
                        GameObject.Destroy(m_colorGoList[i][j]);
                        m_colorGoList[i].RemoveAt(j);
                        j--;
                        m_colorInfoList[i].RemoveAt(j);
                        GameObject.Destroy(m_colorGoList[i][j]);
                        m_colorGoList[i].RemoveAt(j);
                        j--;
                        m_colorInfoList[i].RemoveAt(j);
                        GameObject.Destroy(m_colorGoList[i][j]);
                        m_colorGoList[i].RemoveAt(j);
                        j--;
                        if (m_operateCount >= 0) 
                            m_comboCount++;
                        else
                            m_comboCount = 0;

                        m_operateCount = m_comboLimit;
                      //  Util.LogError(m_comboCount.ToString());
                        m_score += m_comboScore+ (m_comboCount);
                        Util.LogError(m_score.ToString());
                    }
                }
            }
        }
      
    }   
}
