using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }
    public FitType fitType;
    public Vector2 cellSize;
    public Vector2 spacing;

    public int rows;
    public int columns;

    public bool fitX;
    public bool fitY;
    
    public override void CalculateLayoutInputHorizontal()
    {        
        base.CalculateLayoutInputHorizontal();

        if (fitType == FitType.Height || fitType == FitType.Width || fitType == FitType.Uniform)
        {
            fitX = true;
            fitY = true;
            float sqrtRt = Mathf.Sqrt(transform.childCount);
            rows = Mathf.CeilToInt(sqrtRt);
            columns = Mathf.CeilToInt(sqrtRt);
        }

        if (fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float) columns);
        }
        else if (fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            columns = Mathf.CeilToInt(transform.childCount / (float) rows);
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = parentWidth / columns - 2 * spacing.x /columns - padding.left / (float) columns - padding.right / (float) columns;
        float cellHeight = parentHeight / rows - 2 * spacing.y / rows - padding.top / (float) rows - padding.bottom / (float) rows;


        cellSize.x = fitX? cellWidth : cellSize.x;
        cellSize.y = fitY? cellHeight : cellSize.y;

        int columnCount, rowCount;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / columns;
            columnCount = i % columns;

            RectTransform item = rectChildren[i];

            float xPos = cellSize.x * columnCount + spacing.x * columnCount + padding.left;
            float yPos = cellSize.y * rowCount + spacing.y * rowCount + padding.top;
            
            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }
    }

    public override void CalculateLayoutInputVertical()
    {
        
    }

    public override void SetLayoutHorizontal()
    {

    }

    public override void SetLayoutVertical()
    {
        
    }
}
