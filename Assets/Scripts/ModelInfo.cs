using UnityEngine;

public class ModelInfo : MonoBehaviour
{
    //[SerializeField] private string modelName;

    //public string ModelName => modelName; //Getter
    //public string MyModelName { get => modelName; } //Getter
    //public string SetModelName { set => modelName = value; } //Setter
    //public string SelectedModelName { get => modelName; set => modelName = value; } //Getter & Setter

    //public string GetModelName()
    //{
    //    return modelName;
    //}

    //public void SetNewModelName(string newModelName)
    //{
    //    modelName = newModelName;
    //}

    [SerializeField] private ModelData modelData;
    public ModelData Data => modelData;
}
