using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{

    [SerializeField]
    private LineController m_controller;

    public Vector2Int Coordinate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(LineController controller, Vector2Int coordinate)
    {
        m_controller = controller;
        Coordinate = coordinate;
    }

    private void OnMouseDown()
    {
        m_controller.LineGetClicked(this);
    }
}
