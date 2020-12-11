using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellPanel : MonoBehaviour
{
    private int index = -1;
    private bool isHighlight = false;

    public void setIndex(int index)
    {
        this.index = index;
    }

    public void Highlight()
    {
        this.isHighlight = !isHighlight;
    }

    public int getIndex()
    {
        return this.index;
    }

    public bool getIsHighlighted()
    {
        return this.isHighlight;
    }
}
