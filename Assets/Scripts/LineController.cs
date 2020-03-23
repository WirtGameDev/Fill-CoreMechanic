using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [SerializeField]
    private Line m_linePrefab;

    [SerializeField]
    private GameObject m_connectorPrefab;

    [SerializeField]
    private Transform m_lineRoot;

    [SerializeField]
    private LayerMask m_blockLayer;

    private List<Line> m_curLine;

    private List<GameObject> m_curConnect;

    private bool m_isDrawing;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isDrawing)
        {
            // Finish drawing
            if (Input.GetMouseButtonUp(0))
            {
                m_isDrawing = false;
            }

            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            // Detect the block pointed by mouse
            var hit = Physics2D.OverlapCircle(mousePos, 0.1f, m_blockLayer);
            if (hit != null)
            {
                var block = hit.GetComponent<Block>();

                if (block.IsOccupied)
                {
                    for (int i=0; i<m_curLine.Count; i++)
                    {
                        if (m_curLine[i].Coordinate == block.Coordinate)
                        {
                            if (i != m_curLine.Count - 1)
                            {
                                EraseLine(m_curLine[i]);
                            }

                        }
                    }
                }
                else
                {
                    DrawLine(block);
                }
            }
        }
    }

    private void Initialize()
    {
        m_curConnect = new List<GameObject>();
        RegisterExistedLine();
    }

    /// <summary>
    /// Detect the beginning line
    /// </summary>
    private void RegisterExistedLine()
    {
        m_curLine = new List<Line>();

        for(int i=0; i<m_lineRoot.childCount; i++)
        {
            var line = m_lineRoot.GetChild(i).GetComponent<Line>();
            var initCoordinate = line.Coordinate;
            m_curLine.Add(line);
            line.Initialize(this, initCoordinate);
            BlockManager.instance.GetBlockAt(initCoordinate).IsOccupied = true;
        }
    }

    public void LineGetClicked(Line line)
    {
        var curHead = m_curLine[m_curLine.Count - 1];
        if (line != curHead)
        {
            EraseLine(line);
        }
        else
        {
            //Ready to draw new line
            m_isDrawing = true;
        }
    }

    public void EraseLine(Line line)
    {
        var index = 0;
        for (int i = 0; i < m_curLine.Count; i++)
        {
            if (line == m_curLine[i])
            {
                index = i;
                break;
            }
        }

        for (int i = m_curLine.Count - 1; i > index; i--)
        {
            //Remove lines
            var deletedLine = m_curLine[i];
            BlockManager.instance.GetBlockAt(deletedLine.Coordinate).IsOccupied = false;
            m_curLine.RemoveAt(i);
            Destroy(deletedLine.gameObject);

            //Remove connectors
            var deletedConnect = m_curConnect[m_curConnect.Count - 1];
            m_curConnect.RemoveAt(m_curConnect.Count - 1);
            Destroy(deletedConnect);
        }
    }

    public void DrawLine(Block block)
    {
        var curHead = m_curLine[m_curLine.Count - 1];

        // Check whether two blocks are neighbor.
        if (Mathf.Abs(block.Coordinate.x - curHead.Coordinate.x) + Mathf.Abs(block.Coordinate.y - curHead.Coordinate.y) != 1)
            return;

        var position = block.transform.position;
        var newLine = Instantiate<Line>(m_linePrefab, position, Quaternion.identity, m_lineRoot);
        newLine.Initialize(this, block.Coordinate);

        ShowConnection(curHead.transform.position, newLine.transform.position);

        m_curLine.Add(newLine);
        block.IsOccupied = true;


    }

    /// <summary>
    /// Put the connect sprite between two blocks.
    /// </summary>
    private void ShowConnection(Vector2 from, Vector2 to)
    {
        var connectPos = Vector3.zero;
        var rotation = Vector3.zero;

        if (from.x == to.x)
        {
            var posY = (to.y > from.y) ? (from.y + 0.9f) : (from.y - 0.9f);
            connectPos = new Vector3(from.x, posY, 0f);
            rotation.z = 90;
        }
        else
        {
            var posX = (to.x > from.x) ? (from.x + 0.9f) : (from.x - 0.9f);
            connectPos = new Vector3(posX, from.y, 0f);
        }

        var newConnect = Instantiate(m_connectorPrefab, connectPos, Quaternion.Euler(rotation));

        m_curConnect.Add(newConnect);
    }
}
