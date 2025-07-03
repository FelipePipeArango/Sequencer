using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyDB", menuName = "ScriptableObjects/DifficultyDB", order = 1)]
public class Difficulties : ScriptableObject
{
    [SerializeField] Color tutorialDifficultyColor;
    [SerializeField] Color lowDifficultyColor;
    [SerializeField] Color mediumDifficultyColor;
    [SerializeField] Color highlDifficultyColor;
    [SerializeField] Color challengeDifficultyColor;

    public enum difficultyLevel
    {
        Tutorial,
        Low,
        Medium,
        High,
        Challenge
    }

    public Color AssignColorDifficulty (difficultyLevel recievedDifficulty)
    {
        Debug.Log("oe");
        switch (recievedDifficulty)
        {
            case difficultyLevel.Tutorial:
                return tutorialDifficultyColor;

            case difficultyLevel.Low:
                return lowDifficultyColor;

            case difficultyLevel.Medium:
                return mediumDifficultyColor;

            case difficultyLevel.High:
                return highlDifficultyColor;

            case difficultyLevel.Challenge:
                return challengeDifficultyColor;
        }

        return Color.white;
    }
}
