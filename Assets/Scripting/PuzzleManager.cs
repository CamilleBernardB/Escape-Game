using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;


public class PuzzleManager : MonoBehaviour
{
    public int gridSize = 4;
    public GameObject tilePrefab;
    public Transform puzzleParent;
    public Sprite[] tileSprites;

    public TMP_Text solvedText;

    private List<Tile> tiles = new List<Tile>();
    private int emptyIndex;

    void Start()
    {
        SetupGrid();
        CreateGrid();
        Shuffle(100); 
    }

    void SetupGrid()
    {
        GridLayoutGroup grid = puzzleParent.GetComponent<GridLayoutGroup>();
        RectTransform rt = puzzleParent.GetComponent<RectTransform>();

        // subtract padding
        float panelWidth = rt.rect.width - (grid.padding.left + grid.padding.right);
        float panelHeight = rt.rect.height - (grid.padding.top + grid.padding.bottom);

        float cellWidth = panelWidth / gridSize;
        float cellHeight = cellWidth * (192f / 256f);

        if (cellHeight * gridSize > panelHeight)
        {
            cellHeight = panelHeight / gridSize;
            cellWidth = cellHeight * (256f / 192f);
        }

        grid.cellSize = new Vector2(cellWidth, cellHeight);
    }

    void CreateGrid()
{
    tiles.Clear();

    for (int i = 0; i < gridSize * gridSize; i++)
    {
        GameObject tileObj = Instantiate(tilePrefab, puzzleParent);
        Image img = tileObj.GetComponent<Image>();
        Button btn = tileObj.GetComponent<Button>();
        Tile tile = tileObj.GetComponent<Tile>();

        if (i == gridSize * gridSize - 1)
        {
            // EMPTY TILE
            img.sprite = null;
            img.color = new Color(0, 0, 0, 0); // fully transparent
            btn.interactable = false;

            emptyIndex = i;
            tiles.Add(null);
        }
        else
        {
            // NORMAL TILE
            img.color = Color.white;
            img.sprite = tileSprites[i];

            tile.Init(i, this);
            tile.currentIndex = i;

            btn.interactable = true;
            tiles.Add(tile);
        }
    }
}


    bool IsSolved()
{
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] == null)
                continue;

            if (tiles[i].currentIndex != tiles[i].correctIndex)
                return false;
        }
        return true;
}

    public void TryMove(Tile tile)
{
    int tileIndex = tiles.IndexOf(tile);

    if (IsAdjacent(tileIndex, emptyIndex))
    {
        Swap(tileIndex, emptyIndex);

        if (IsSolved())
        {
            OnPuzzleSolved();
        }
    }
}
    void OnPuzzleSolved()
    {
        Debug.Log("Puzzle Solved!");
        solvedText.gameObject.SetActive(true);
    }

    bool IsAdjacent(int a, int b)
    {
        int ax = a % gridSize;
        int ay = a / gridSize;
        int bx = b % gridSize;
        int by = b / gridSize;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by) == 1;
    }


    void Swap(int a, int b)
{
    Transform tileA = puzzleParent.GetChild(a);
    Transform tileB = puzzleParent.GetChild(b);

    tileA.SetSiblingIndex(b);
    tileB.SetSiblingIndex(a);

    Tile temp = tiles[a];
    tiles[a] = tiles[b];
    tiles[b] = temp;

    emptyIndex = a;

    if (tiles[b] != null)
        tiles[b].currentIndex = b;
}



    List<int> GetNeighbors(int index)
    {
        List<int> result = new List<int>();
        int x = index % gridSize;
        int y = index / gridSize;

        if (x > 0) result.Add(index - 1);
        if (x < gridSize - 1) result.Add(index + 1);
        if (y > 0) result.Add(index - gridSize);
        if (y < gridSize - 1) result.Add(index + gridSize);

        return result;
    }

    void Shuffle(int moves = 100)
{
    for (int i = 0; i < moves; i++)
    {
        List<int> neighbors = GetNeighbors(emptyIndex);
        int rand = neighbors[Random.Range(0, neighbors.Count)];
        Swap(rand, emptyIndex);
    }
}

    public void Reshuffle()
{
    solvedText.gameObject.SetActive(false);


    for (int i = 0; i < puzzleParent.childCount; i++)
    {
        puzzleParent.GetChild(i).SetSiblingIndex(i);
    }

    tiles.Clear();
    emptyIndex = -1;

    for (int i = 0; i < puzzleParent.childCount; i++)
    {
        Tile tile = puzzleParent.GetChild(i).GetComponent<Tile>();

        if (tile == null || !puzzleParent.GetChild(i).GetComponent<Button>().interactable)
        {
            tiles.Add(null);
            emptyIndex = i;
        }
        else
        {
            tile.currentIndex = i;
            tiles.Add(tile);
        }
    }


    Shuffle(150);
}






}
