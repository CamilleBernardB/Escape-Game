using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public int gridSize = 4;
    public GameObject tilePrefab;
    public Transform puzzleParent;
    public Sprite[] tileSprites;

    public TMP_Text solvedText;

    [Header("End of game")]
    [SerializeField] private string mainSceneName = "SampleScene";
    [SerializeField] private float returnDelaySeconds = 2.0f;

    private List<Tile> tiles = new List<Tile>();
    private int emptyIndex;
    private bool hasWon = false;

    void Start()
    {
        SetupGrid();
        CreateGrid();
        Shuffle(100);
        if (solvedText != null) solvedText.gameObject.SetActive(false);
    }

    void SetupGrid()
    {
        GridLayoutGroup grid = puzzleParent.GetComponent<GridLayoutGroup>();
        RectTransform rt = puzzleParent.GetComponent<RectTransform>();

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
                img.sprite = null;
                img.color = new Color(0, 0, 0, 0);
                btn.interactable = false;

                emptyIndex = i;
                tiles.Add(null);
            }
            else
            {
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
            if (tiles[i] == null) continue;
            if (tiles[i].currentIndex != tiles[i].correctIndex) return false;
        }
        return true;
    }

    public void TryMove(Tile tile)
    {
        if (hasWon) return;

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
        if (hasWon) return;
        hasWon = true;

        Debug.Log("Puzzle Solved!");
        if (solvedText != null)
        {
            solvedText.text = "Bien joué ! Passe à la prochaine énigme !";
            solvedText.gameObject.SetActive(true);
        }

        Invoke(nameof(ReturnToMainScene), returnDelaySeconds);
    }

    private void ReturnToMainScene()
    {
        SceneManager.LoadScene(mainSceneName);
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
        hasWon = false;
        CancelInvoke(nameof(ReturnToMainScene));

        if (solvedText != null) solvedText.gameObject.SetActive(false);

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
