using UnityEngine;
using UnityEngine.UI;

namespace ViewModel.Unity
{
    public class StringReactiveText : MonoBehaviour
    {
        [SerializeField] private ViewModelDataProvider<UniTaskData.ViewModelProperty<string>> _data;

        [SerializeField] private Text _text;

        private void OnEnable()
        {
            _data.GetData()
                .Subscribe(OnChangeHandler);
        }

        private void OnChangeHandler(string obj)
        {
            _text.text = obj?.ToString();
        }
    }
}