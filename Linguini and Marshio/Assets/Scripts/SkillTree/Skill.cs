using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static SkillTree;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public int id;

    public TMP_Text TitleText;
    public TMP_Text DescriptionText;

    public int[] ConnectedUpgrades;

    public void UpdateUI()
    {
        TitleText.text = $"{skillTree.SkillLevels[id]}/{SkillTree.skillTree.SkillCaps[id]}\n{skillTree.SkillNames[id]}";
        DescriptionText.text = $"{skillTree.SkillDescription[id]}\nCost: 1/{skillTree.SkillPoint} SP";

        //GetComponent<Image>().color = skillTree.SkillLevels[id] >= skillTree.SkillCaps[id] ? Color.yellow
        //    : skillTree.SkillPoint >= 1 ? Color.green : Color.white;


        foreach (var connectedSkill in ConnectedUpgrades)
        {
            skillTree.SkillList[connectedSkill].gameObject.SetActive(skillTree.SkillLevels[id] > 0);
            skillTree.ConnectorList[connectedSkill].SetActive(skillTree.SkillLevels[id] > 0);
        }
    }

    public void Buy()
    {
        if (skillTree.SkillPoint < 1 || skillTree.SkillLevels[id] >= skillTree.SkillCaps[id])
        {
            return;
        }
        
        skillTree.SkillPoint -= 1;
        skillTree.SkillLevels[id]++;
        var purchasedSkills = skillTree.SkillNames[id];
        CheckPurchasedSkill(purchasedSkills);

        skillTree.UpdateAllSkillUI();
    }

    public void CheckPurchasedSkill(string skillName)
    {
        if (skillName == "More HP")
        {
            StatsController.MaxHealth += 10;
            StatsController.Health += 10;
        }
        if (skillName == "More attack")
        {
            StatsController.AttackDamage += 5;
        }
    }

}
