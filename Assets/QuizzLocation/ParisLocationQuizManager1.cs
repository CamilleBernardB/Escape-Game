using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class IleDeLaCiteQuizManager : MonoBehaviour
{
    // Position test du joueur : devant Notre-Dame
    private float playerLatitude = 48.852968f;
    private float playerLongitude = 2.349902f;
    private float detectionRadiusMeters = 30f; // rayon précis pour l'île de la Cité

    [Header("UI Elements")]
    public TMP_Text locationText;
    public TMP_Text questionText;
    public TMP_Text feedbackText;
    public Button yesButton;
    public Button noButton;
    public GameObject quizPanel;

    private int currentLocationIndex = -1;
    private int currentQuestionIndex = 0;

    private List<Location> locations = new List<Location>()
    {
        new Location("Cathédrale Notre-Dame", 48.852968f, 2.349902f, new Question[]{
            new Question("La cathédrale a-t-elle été construite au XIIe siècle ?", true),
            new Question("La flèche actuelle a-t-elle été détruite lors de l’incendie de 2019 ?", true),
            new Question("Les tours de Notre-Dame mesurent environ 70 mètres ?", true)
        }),

        new Location("Sainte-Chapelle", 48.855375f, 2.344961f, new Question[]{
            new Question("Sainte-Chapelle a-t-elle été construite par Saint Louis pour abriter la Sainte Couronne d’Épines ?", true),
            new Question("A-t-elle été achevée au XIIIe siècle ?", true),
            new Question("Les vitraux représentent-ils des scènes bibliques ?", true)
        }),

        new Location("Conciergerie", 48.856011f, 2.345495f, new Question[]{
            new Question("La Conciergerie a-t-elle servi de prison pendant la Révolution française ?", true),
            new Question("Marie-Antoinette y a-t-elle été détenue ?", true),
            new Question("La Conciergerie faisait-elle partie du Palais de la Cité ?", true)
        }),

        new Location("Palais de Justice", 48.855932f, 2.345317f, new Question[]{
            new Question("Le Palais de Justice a-t-il remplacé le Palais royal médiéval ?", true),
            new Question("La Sainte-Chapelle se trouve-t-elle dans l’enceinte du Palais de Justice ?", true),
            new Question("Le Palais de Justice abrite-t-il la Cour de cassation française ?", true)
        }),

        new Location("Place Dauphine", 48.856687f, 2.342318f, new Question[]{
            new Question("La Place Dauphine a-t-elle été créée sous le règne d’Henri IV ?", true),
            new Question("La place a-t-elle une forme triangulaire ?", true),
            new Question("A-t-elle été nommée en hommage au fils du roi, le Dauphin ?", true)
        }),

        new Location("Pont Neuf", 48.856697f, 2.340841f, new Question[]{
            new Question("Le Pont Neuf est-il le plus ancien pont de Paris encore en place ?", true),
            new Question("Était-il le premier pont à ne pas comporter de maisons ?", true),
            new Question("Relie-t-il la rive gauche et la rive droite via l’Île de la Cité ?", true)
        }),

        new Location("Marché aux Fleurs – Place Louis Lépine", 48.855145f, 2.347666f, new Question[]{
            new Question("Le Marché aux Fleurs existe-t-il depuis 1808 ?", true),
            new Question("Peut-on y acheter uniquement des fleurs et plantes ?", false),
            new Question("Le marché est-il ouvert tous les jours de la semaine ?", true)
        }),

        new Location("Mémorial des Martyrs de la Déportation", 48.851757f, 2.352444f, new Question[]{
            new Question("Le mémorial a-t-il été inauguré en 1962 ?", true),
            new Question("Rend-il hommage aux victimes françaises déportées pendant la Seconde Guerre mondiale ?", true),
            new Question("Se situe-t-il sur l’Île de la Cité, le long de la Seine ?", true)
        }),

        new Location("Square Jean XXIII", 48.852408f, 2.350872f, new Question[]{
            new Question("Le square Jean XXIII se situe-t-il derrière Notre-Dame ?", true),
            new Question("A-t-il été aménagé en 1963 ?", true),
            new Question("Est-il l’un des plus grands squares de Paris ?", false)
        }),

        new Location("Crypte archéologique de Notre-Dame", 48.853650f, 2.347715f, new Question[]{
            new Question("La crypte archéologique montre-t-elle des vestiges de l’ancienne Lutèce romaine ?", true),
            new Question("Est-elle située sous le parvis de Notre-Dame ?", true),
            new Question("La crypte est-elle ouverte au public ?", true)
        }),

        new Location("Pont Saint-Louis", 48.852763f, 2.352705f, new Question[]{
            new Question("Le pont relie-t-il l’Île de la Cité à l’Île Saint-Louis ?", true),
            new Question("Est-il exclusivement piéton ?", true),
            new Question("Le pont actuel date-t-il du XIXe siècle ?", false)
        }),

        new Location("Hôtel-Dieu de Paris", 48.854695f, 2.348663f, new Question[]{
            new Question("L’Hôtel-Dieu est-il le plus ancien hôpital de Paris ?", true),
            new Question("Est-il toujours en fonctionnement comme hôpital aujourd’hui ?", true),
            new Question("Se situe-t-il juste à côté de Notre-Dame ?", true)
        })
    };

    void Start()
    {
        quizPanel.SetActive(false);
        DetectPlayerLocation();
    }

    void DetectPlayerLocation()
    {
        float closestDistance = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < locations.Count; i++)
        {
            float distance = HaversineDistance(playerLatitude, playerLongitude,
                locations[i].latitude, locations[i].longitude);

            if (distance < closestDistance && distance <= detectionRadiusMeters)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        if (closestIndex != -1)
        {
            currentLocationIndex = closestIndex;
            locationText.text = "Lieu : " + locations[currentLocationIndex].nom;
            currentQuestionIndex = 0;
            ShowQuestion();
        }
        else
        {
            locationText.text = "Lieu inconnu.";
        }
    }

    void ShowQuestion()
    {
        quizPanel.SetActive(true);
        feedbackText.text = "";

        Question q = locations[currentLocationIndex].questions[currentQuestionIndex];
        questionText.text = q.texte;

        // Corriger le texte des boutons
        yesButton.GetComponentInChildren<TMP_Text>().text = "Oui";
        noButton.GetComponentInChildren<TMP_Text>().text = "Non";

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() => CheckAnswer(true));
        noButton.onClick.AddListener(() => CheckAnswer(false));
    }

    void CheckAnswer(bool choix)
    {
        Question q = locations[currentLocationIndex].questions[currentQuestionIndex];

        if (choix == q.reponseOui)
        {
            currentQuestionIndex++;
            feedbackText.text = "Bonne réponse !";

            if (currentQuestionIndex >= locations[currentLocationIndex].questions.Length)
            {
                feedbackText.text = $"Lieu terminé : {locations[currentLocationIndex].nom}";
                quizPanel.SetActive(false);
            }
            else
            {
                ShowQuestion();
            }
        }
        else
        {
            feedbackText.text = "Mauvaise réponse, réessaie.";
        }
    }

    private float HaversineDistance(float lat1, float lon1, float lat2, float lon2)
    {
        float R = 6371000f;
        float dLat = Mathf.Deg2Rad * (lat2 - lat1);
        float dLon = Mathf.Deg2Rad * (lon2 - lon1);

        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos(Mathf.Deg2Rad * lat1) * Mathf.Cos(Mathf.Deg2Rad * lat2) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        return R * c;
    }

    private class Location
    {
        public string nom;
        public float latitude;
        public float longitude;
        public Question[] questions;

        public Location(string nom, float lat, float lon, Question[] questions)
        {
            this.nom = nom;
            this.latitude = lat;
            this.longitude = lon;
            this.questions = questions;
        }
    }

    private class Question
    {
        public string texte;
        public bool reponseOui;

        public Question(string texte, bool reponseOui)
        {
            this.texte = texte;
            this.reponseOui = reponseOui;
        }
    }
}
