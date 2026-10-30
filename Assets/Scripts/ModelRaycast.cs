using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using TMPro;
using UnityEngine.UI;

public class ModelRaycast : MonoBehaviour
{
    [SerializeField] private Camera arCamera;

    [SerializeField] private TMP_Text modelName;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text categoryText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image iconImage;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
        {
            if (!touch.began)
                continue;

            Ray ray = arCamera.ScreenPointToRay(touch.screenPosition);

            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                ModelInfo modelInfo = hit.collider.GetComponent<ModelInfo>();

                if(modelInfo != null && modelInfo.Data != null)
                {
                    ShowInfo(modelInfo.Data);
                    //modelInfo.SetModelName = "Pan";
                    //Debug.Log($"Selected Model Name: {modelInfo.SelectedModelName}");
                    //modelInfo.SelectedModelName = "Bolillo";

                    //modelInfo.SetNewModelName("Concha");
                }
            }
        }
    }

    private void ShowInfo(ModelData data)
    {
        modelName.text = data.modelName;
        descriptionText.text = data.description;
        categoryText.text = data.category;
        levelText.text = "Nivel: " + data.level;
        iconImage.sprite = data.icon;

        Debug.Log("Objeto seleccionado: " + data);
    }
}
