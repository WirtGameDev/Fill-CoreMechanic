    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;

    [SerializeField]
    private Transform m_blockRoot;

    [SerializeField]
    private Text m_gameStateText;

    private Dictionary<Vector2Int, Block> m_allBlocks;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        RegisterBlock();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (WinCheck())
        {
            m_gameStateText.text = "過關了！";
        }
        else
        {
            m_gameStateText.text = "解題中...";
        }
    }

    private void RegisterBlock()
    {
        m_allBlocks = new Dictionary<Vector2Int, Block>();

        for (int i = 0; i < m_blockRoot.transform.childCount; i++)
        {
            var block = m_blockRoot.GetChild(i).GetComponent<Block>();
            m_allBlocks.Add(block.Coordinate, block);
        }
    }


    public Vector3 GetBlockPosition(Vector2Int coordinate)
    {
        if (m_allBlocks.ContainsKey(coordinate) == false)
        {
            Debug.LogError("Block not found.");
            return Vector3.zero;
        }

        return m_allBlocks[coordinate].transform.position;
    }

    public Block GetBlockAt(Vector2Int coordinate)
    {
        return m_allBlocks[coordinate];
    }

    private bool WinCheck()
    {
        var isWin = true;

        foreach(var b in m_allBlocks)
        {
            if (b.Value.IsOccupied == false)
                isWin = false;
        }

        return isWin;
    }
}
