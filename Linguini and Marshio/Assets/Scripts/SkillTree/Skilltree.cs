using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public static SkillTree skillTree;
    private void Awake() => skillTree = this;

    public int[] SkillLevels;
    public int[] SkillCaps;
    public string[] SkillNames;
    public string[] SkillDescription;

    public List<Skill> SkillList;

    public GameObject SkillHolder;

    public List<GameObject> ConnectorList;
    public GameObject ConnectorHolder;

    public int SkillPoint;

    // Start is called before the first frame update
    void Start()
    {
        SkillPoint = 20;

        SkillLevels = new int[6];
        SkillCaps = new[] { 1, 5, 5, 2, 10, 10 };

        SkillNames = new[] { "More HP", "More attack", "Loot goblin", "'Quick learner", "Self defense", "God mode" };
        SkillDescription = new[]
        {
            "+10 Max HP",
            "+5 Attack damage",
            "Get more $ bills from enemies",
            "Get more XP from fights",
            "+10 armor",
            "U win, you are the strongest in the universe",
        };

        foreach (var skill in SkillHolder.GetComponentsInChildren<Skill>())
        {
            SkillList.Add(skill);
        }


        foreach (var connector in ConnectorHolder.GetComponentsInChildren<RectTransform>())
        {
            ConnectorList.Add(connector.gameObject);
        }

        for (var i = 0; i < SkillList.Count; i++)
        {
            SkillList[i].id = i;
        }

        SkillList[0].ConnectedUpgrades = new[] { 1, 2, 3 };
        SkillList[3].ConnectedUpgrades = new[] { 4, 5};

        UpdateAllSkillUI();
    }

    public void UpdateAllSkillUI()
    {
        foreach (var skill in SkillList)
        {
            skill.UpdateUI();
        }
    }
}
