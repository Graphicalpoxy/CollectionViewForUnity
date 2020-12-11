using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CollectionViewV : MonoBehaviour
{
    [SerializeField]
    private float cellHeight = 10;
    [SerializeField]
    private float cellWidth = 10;
    [SerializeField]
    private int spaceWidth = 0;
    [SerializeField]
    private int spaceHeight = 0;
    [SerializeField]
    private CollectionViewAligment aligment = CollectionViewAligment.Center;

    /// <summary>
    /// 表示するセルのプレハブ
    /// </summary>
    [SerializeField]
    private GameObject cellPref;

    /// <summary>
    /// 表示するセルの個数
    /// </summary>
    [SerializeField]
    private int cellCount = 1;


    [SerializeField]
    private RectTransform containtView;

    private RectTransform scrollView;

    private VerticalLayoutGroup vertical;

    /// <summary>
    /// 行数
    /// </summary>
    private int rowCount;
    /// <summary>
    /// 最終行のセル
    /// </summary>
    private int extraCellCount = 0;
    /// <summary>
    /// 行ごとのセルの個数
    /// </summary>
    private int cellCountPerRow;

    /// <summary>
    /// セルを格納したリスト。処理を行う場合はここから行うといい
    /// </summary>
    private List<CellPanel> cellList = new List<CellPanel>();

    private void Awake()
    {
        scrollView = this.GetComponent<RectTransform>();

        this.containtView.gameObject.AddComponent<VerticalLayoutGroup>();
        vertical = this.containtView.GetComponentInChildren<VerticalLayoutGroup>();
        vertical.childForceExpandHeight = true;
        vertical.childForceExpandWidth = false;
        vertical.childControlHeight = true;
        vertical.childControlWidth = false;
        switch (this.aligment)
        {
            case CollectionViewAligment.Center:
                vertical.childAlignment = TextAnchor.MiddleCenter;
                break;
            case CollectionViewAligment.Left:
                vertical.childAlignment = TextAnchor.MiddleLeft;
                break;
            case CollectionViewAligment.Right:
                vertical.childAlignment = TextAnchor.MiddleRight;
                break;
        }

        this.loadData();
    }

    /// <summary>
    /// 設定値からコレクションビューを設定
    /// </summary>
    private void loadData()
    {
        cellCountPerRow = this.getCellCountPerRow();
        rowCount = this.getRowCount();

        // ContaintViewのサイズを決定
        var containtViewHeight = (rowCount * (cellHeight + spaceHeight)) - spaceHeight;
        this.containtView.sizeDelta = new Vector2(containtView.rect.width, containtViewHeight);

        // コンテントViewに行を追加
        vertical.spacing = this.spaceHeight;
        // ボタンに設定するためのインデックス
        var buttonIndex = 0;
        for (int i = 0; i < rowCount; i++)
        {
            var currentRowIndex = i + 1;
            GameObject rowPanel = new GameObject("rowPanele" + i.ToString());
            rowPanel.AddComponent<RectTransform>();
            var rect = rowPanel.GetComponent<RectTransform>();
            var rowWidth = (cellCountPerRow * (cellWidth + spaceWidth) - spaceWidth);
            rect.sizeDelta = new Vector2(rowWidth, 0);
            rowPanel.AddComponent<HorizontalLayoutGroup>();
            var horizontal = rowPanel.GetComponent<HorizontalLayoutGroup>();
            horizontal.childForceExpandHeight = false;
            horizontal.childForceExpandWidth = false;
            horizontal.childControlHeight = false;
            horizontal.childControlWidth = false;
            horizontal.childAlignment = TextAnchor.MiddleLeft;

            rowPanel.transform.SetParent(containtView);
            horizontal.spacing = spaceWidth;

            // 行にセルを追加
            if (currentRowIndex != rowCount)
            {
                // 最後の行じゃない場合
                for (int j = 0; j < cellCountPerRow; j++)
                {
                    GameObject cellPanel = Instantiate(cellPref,
                        Vector3.zero,
                        Quaternion.identity);
                    cellPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(this.cellWidth, this.cellHeight);
                    cellPanel.transform.SetParent(rowPanel.transform);
                    var panel = cellPanel.GetComponent<CellPanel>();
                    panel.setIndex(buttonIndex);
                    buttonIndex++;
                    this.cellList.Add(panel);
                }
            }
            else
            {
                var count = extraCellCount;
                if (extraCellCount == 0)
                {
                    count = cellCountPerRow;
                }

                // 最終行の場合
                for (int j = 0; j < count; j++)
                {
                    GameObject cellPanel = Instantiate(cellPref,
                        Vector3.zero,
                        Quaternion.identity);
                    cellPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(this.cellWidth, this.cellHeight);
                    cellPanel.transform.SetParent(rowPanel.transform);
                    var panel = cellPanel.GetComponent<CellPanel>();
                    panel.setIndex(buttonIndex);
                    buttonIndex++;
                    this.cellList.Add(panel);
                }
            }
        }

        this.setCellButtons();
    }

    /// <summary>
    /// データの更新
    /// </summary>
    public void reloadData()
    {
        // 既存のViewを削除して再生成
        foreach (Transform trans in containtView)
        {
            GameObject.Destroy(trans.gameObject);
        }

        this.loadData();
    }

    /// <summary>
    /// 一行に何個のセルを表示できるか計算
    /// </summary>
    private int getCellCountPerRow()
    {
        var currentWidth = 0.0;
        var cells = 0;
        for (int i = 0; i < cellCount; i++)
        {
            if (i == 0)
            {
                currentWidth += this.cellWidth;
            }
            else
            {
                currentWidth += (this.cellWidth + spaceWidth);
            }

            // 横幅が超過していないかのチェック
            if (currentWidth > this.scrollView.rect.width)
            {
                break;
            }
            cells += 1;
        }
        return cells;
    }

    /// <summary>
    /// 行数を計算
    /// </summary>
    /// <returns></returns>
    private int getRowCount()
    {
        var rows = 0;

        rows = cellCount / cellCountPerRow;
        extraCellCount = cellCount % cellCountPerRow;

        if (extraCellCount != 0)
        {
            rows += 1;
        }

        return rows;
    }

    /// <summary>
    /// cellタップ時の処理を追加
    /// </summary>
    private void setCellButtons()
    {
        foreach (CellPanel cell in this.cellList)
        {
            var button = cell.GetComponent<Button>();
            button.onClick.AsObservable().Subscribe(_ => {
                cell.Highlight();
            });
        }
    }
}
