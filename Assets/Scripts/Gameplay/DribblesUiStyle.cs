using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "DribblesUiStyle", menuName = "Kicklings/UI/Dribbles UI Style")]
public sealed class DribblesUiStyle : ScriptableObject
{
    [SerializeField] private TMP_FontAsset mainMenuFont;
    [SerializeField] private TMP_FontAsset mainMenuOutlineFont;
    [SerializeField] private Sprite mainMenuButtonSprite;
    [SerializeField] private Sprite mainMenuButtonOutlineSprite;
    [SerializeField] private Sprite mainMenuPopupSprite;

    public TMP_FontAsset MainMenuFont => mainMenuFont;
    public TMP_FontAsset MainMenuOutlineFont => mainMenuOutlineFont;
    public Sprite MainMenuButtonSprite => mainMenuButtonSprite;
    public Sprite MainMenuButtonOutlineSprite => mainMenuButtonOutlineSprite;
    public Sprite MainMenuPopupSprite => mainMenuPopupSprite;
}
