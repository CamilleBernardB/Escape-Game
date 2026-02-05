using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public int correctIndex;
    public int currentIndex;
    private PuzzleManager puzzleManager;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void Init(int correctIndex, PuzzleManager manager)
    {
        this.correctIndex = correctIndex;
        this.puzzleManager = manager;
    }

    void OnClick()
    {
        puzzleManager.TryMove(this);
    }

    public void MoveToPosition(Vector3 newPosition)
    {
        transform.localPosition = newPosition; // for now, instant
    }
}
