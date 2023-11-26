using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class picture_manager : MonoBehaviour
{
    public Image playerFaceImage;
    public Image bossFaceImage;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI bossNameText;
    public void ChangePlayerFace(Sprite newSprite)
    {
        playerFaceImage.sprite = newSprite;
    }

    public void ChangeBossFace(Sprite newSprite)
    {
        bossFaceImage.sprite = newSprite;
    }

      public void SetPlayerName(string playerName)
    {
        playerNameText.text = playerName;
    }
    public void SetBossName(string bossName)
    {
        bossNameText.text = bossName;
    }
}
