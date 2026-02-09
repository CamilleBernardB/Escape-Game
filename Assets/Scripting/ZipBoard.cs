using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ZipBoard : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int width = 6;
    [SerializeField] private int height = 6;

    [Header("UI")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private CellView cellPrefab;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Win")]
    [SerializeField] private string winHint = "Indice : la prochaine machine est ...";
    [SerializeField] private string mainSceneName = "SampleScene";
    [SerializeField] private float returnDelaySeconds = 2.0f;
    private bool hasWon = false;

    // Level data
    private int[,] numbers;
    private bool[,] lit;
    private CellView[,] views;

    private Dictionary<int, Vector2Int> numToPos = new();
    private readonly List<Vector2Int> path = new();

    private int litCount;
    private int totalCells;
    private int maxNumber = 10;

    private void Start()
    {
        if (statusText != null) statusText.text = "";

        LoadLevel();
        BuildGrid();
        ResetBoard();
    }

    private void LoadLevel()
    {
        numbers = new int[width, height];

        numbers[0, 0] = 1;
        numbers[2, 1] = 2;
        numbers[4, 0] = 3;
        numbers[5, 2] = 4;
        numbers[3, 3] = 5;
        numbers[1, 2] = 6;
        numbers[0, 4] = 7;
        numbers[2, 5] = 8;
        numbers[4, 4] = 9;
        numbers[5, 4] = 10;

        lit = new bool[width, height];
        views = new CellView[width, height];

        numToPos.Clear();
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            int n = numbers[x, y];
            if (n > 0) numToPos[n] = new Vector2Int(x, y);
        }

        totalCells = width * height;
    }

    private void BuildGrid()
    {
        for (int i = gridParent.childCount - 1; i >= 0; i--)
            Destroy(gridParent.GetChild(i).gameObject);

        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                int cx = x;
                int cy = y;

                CellView view = Instantiate(cellPrefab, gridParent);
                views[cx, cy] = view;

                int n = numbers[cx, cy];
                view.SetNumber(n);
                view.SetLit(false);

                view.Init(() => OnCellClicked(new Vector2Int(cx, cy)));
            }
        }
    }

    private void OnCellClicked(Vector2Int pos)
    {
        // Après victoire : on ignore tout sauf reset via 1
        if (hasWon && numbers[pos.x, pos.y] != 1)
            return;

        // RESET : cliquer sur 1 réinitialise
        if (numbers[pos.x, pos.y] == 1)
        {
            ResetBoard();
            return;
        }

        // UNDO : cliquer sur la dernière case allumée l’éteint
        if (path.Count > 0 && pos == path[path.Count - 1])
        {
            UnlightLast();
            return;
        }

        if (lit[pos.x, pos.y]) return;
        if (path.Count == 0) return;

        Vector2Int last = path[path.Count - 1];
        if (!IsNeighbor4(pos, last)) return;

        int k = numbers[pos.x, pos.y];
        if (k > 1)
        {
            if (!numToPos.TryGetValue(k - 1, out var prevPos)) return;
            if (!lit[prevPos.x, prevPos.y]) return;
        }

        Light(pos);

        if (litCount == totalCells && !IsLastMoveMaxNumber())
        {
            if (statusText != null)
                statusText.text = $"Presque ! Termine sur le {maxNumber}.";
        }
        else if (litCount == totalCells && IsLastMoveMaxNumber())
        {
            Win();
        }
    }

    private void Light(Vector2Int pos)
    {
        lit[pos.x, pos.y] = true;
        litCount++;
        path.Add(pos);
        views[pos.x, pos.y].SetLit(true);
    }

    private void UnlightLast()
    {
        Vector2Int last = path[path.Count - 1];
        path.RemoveAt(path.Count - 1);

        lit[last.x, last.y] = false;
        litCount--;
        views[last.x, last.y].SetLit(false);

        if (statusText != null) statusText.text = "";
    }

    private void ResetBoard()
    {
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            lit[x, y] = false;
            views[x, y].SetLit(false);
        }

        path.Clear();
        litCount = 0;
        if (statusText != null) statusText.text = "";

        if (numToPos.TryGetValue(1, out var pos1))
            Light(pos1);

        hasWon = false;
        CancelInvoke(nameof(ReturnToMainScene));
    }

    private bool IsNeighbor4(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return (dx + dy) == 1;
    }

    private void Win()
    {
        hasWon = true;

        if (statusText != null)
            statusText.text = "Bien joué ! Passe à la prochaine énigme !\n" + winHint;

        Invoke(nameof(ReturnToMainScene), returnDelaySeconds);
    }

    private void ReturnToMainScene()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    private bool IsLastMoveMaxNumber()
    {
        if (path.Count == 0) return false;
        Vector2Int last = path[path.Count - 1];
        return numbers[last.x, last.y] == maxNumber;
    }
}
