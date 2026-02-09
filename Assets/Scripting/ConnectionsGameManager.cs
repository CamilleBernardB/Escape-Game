using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ConnectionsGameManager : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private Transform gridContainer;
    [SerializeField] private WordCell cellPrefab;

    [Header("UI")]
    [SerializeField] private Button submitButton;
    [SerializeField] private Transform solvedCategoriesContainer;
    [SerializeField] private CategoryBlock categoryBlockPrefab;

    [Header("End of game")]
    [SerializeField] private TMP_Text endMessageText;          // <- un TMP_Text dans la scène
    [SerializeField] private string mainSceneName = "SampleScene"; // <- ta scène map (dans Scene List)
    [SerializeField] private float returnDelaySeconds = 2.0f;

    private List<WordData> allWords;
    private readonly List<WordCell> selectedCells = new();
    private int solvedCount = 0;
    private bool gameEnded = false;

    void Start()
    {
        if (endMessageText != null)
            endMessageText.gameObject.SetActive(false);

        CreateWordData();
        SpawnGrid();

        submitButton.onClick.AddListener(OnSubmit);
        UpdateSubmitButton();
    }

    void UpdateSubmitButton()
    {
        submitButton.interactable = (!gameEnded && selectedCells.Count == 4);
    }

    public void OnSubmit()
    {
        if (gameEnded) return;
        if (selectedCells.Count != 4) return;

        string cat = selectedCells[0].Data.categoryId;

        bool ok = true;
        for (int i = 1; i < selectedCells.Count; i++)
        {
            if (selectedCells[i].Data.categoryId != cat)
            {
                ok = false;
                break;
            }
        }

        if (ok)
        {
            Debug.Log("✅ Correct group: " + cat);

            // créer la ligne de mots
            string wordsLine = "";
            for (int i = 0; i < selectedCells.Count; i++)
            {
                wordsLine += selectedCells[i].Data.word;
                if (i < selectedCells.Count - 1) wordsLine += ", ";
            }

            // spawn le bloc catégorie
            CategoryBlock block = Instantiate(categoryBlockPrefab, solvedCategoriesContainer);
            block.Init(cat, wordsLine);

            // supprimer les 4 cells de la grille
            foreach (var cell in selectedCells)
                Destroy(cell.gameObject);

            selectedCells.Clear();

            solvedCount++;
            if (solvedCount >= 4)
            {
                EndGame();
                return;
            }
        }
        else
        {
            Debug.Log("❌ Wrong group");
            // optionnel : feedback UI ici
            selectedCells.Clear(); // si tu veux forcer à re-sélectionner
        }

        UpdateSubmitButton();
    }

    private void EndGame()
    {
        gameEnded = true;

        // Désactiver UI de jeu
        submitButton.interactable = false;

        // Afficher message
        if (endMessageText != null)
        {
            endMessageText.text = "Bien joué ! Passe à la prochaine énigme !";
            endMessageText.gameObject.SetActive(true);
        }

        // Retour scène principale
        Invoke(nameof(ReturnToMainScene), returnDelaySeconds);
    }

    private void ReturnToMainScene()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    void CreateWordData()
    {
        allWords = new List<WordData>
        {
            new WordData("EXPERT", "experience"),
            new WordData("INTERMEDIATE", "experience"),
            new WordData("NOVICE", "experience"),
            new WordData("PROFICIENT", "experience"),

            new WordData("ARMCHAIR", "furniture"),
            new WordData("BOOKCASE", "furniture"),
            new WordData("CONSOLE", "furniture"),
            new WordData("FOOTSTOOL", "furniture"),

            new WordData("CEFTAZIDIME", "coins"),
            new WordData("HEADQUARTER", "coins"),
            new WordData("MONEYPENNY", "coins"),
            new WordData("PUMPERNICKEL", "coins"),

            new WordData("AGREEMENT", "promise"),
            new WordData("COMPACT", "promise"),
            new WordData("HANDSHAKE", "promise"),
            new WordData("UNDERSTANDING", "promise"),
        };
    }

    void SpawnGrid()
    {
        Shuffle(allWords);

        foreach (var word in allWords)
        {
            WordCell cell = Instantiate(cellPrefab, gridContainer);
            cell.Init(word, this);
        }
    }

    public void TryToggleCell(WordCell cell)
    {
        if (gameEnded) return;

        if (cell.IsSelected)
        {
            cell.SetSelected(false);
            selectedCells.Remove(cell);
            UpdateSubmitButton();
            return;
        }

        if (selectedCells.Count >= 4)
            return;

        cell.SetSelected(true);
        selectedCells.Add(cell);
        UpdateSubmitButton();
    }

    void Shuffle(List<WordData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
