using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NameCustomizingPanel : MonoBehaviour
{
    private const string PlayerNamePlayerPrefsKey = "PlayerName";

    [SerializeField] private TMP_InputField _nameField;
    [SerializeField] private TextMeshProUGUI _placeholderText;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private InputFieldEventForwarder _inputFieldEvents;
    [SerializeField] private CampaignStartScreen _campaignStartScreen;
    private void OnEnable()
    {
        _confirmButton.onClick.AddListener(OnConfirmClicked);
        _inputFieldEvents.InputFieldSelected += OnInputFieldSelected;
        _inputFieldEvents.InputFieldDeselected += OnInputFieldDeselected;
    }
    private void OnDisable()
    {
        _confirmButton.onClick.RemoveListener(OnConfirmClicked);
        _inputFieldEvents.InputFieldSelected -= OnInputFieldSelected;
        _inputFieldEvents.InputFieldDeselected -= OnInputFieldDeselected;
    }

    private void OnConfirmClicked()
    {
        if (string.IsNullOrEmpty(_nameField.text))
        {
            _nameField.placeholder.color = Color.red;
            _placeholderText.text = "Name can't be empty";
            return;
        }
        PlayerPrefs.SetString(PlayerNamePlayerPrefsKey, _nameField.text);
        _campaignStartScreen.NewCampaign();
    }
    private void OnInputFieldSelected()
    {
        _nameField.placeholder.color = Color.gray;
        _placeholderText.text = "...";
    }
    private void OnInputFieldDeselected()
    {

    }
}
