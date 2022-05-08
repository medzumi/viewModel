using UnityEngine;
using UnityEngine.UI;

namespace ViewModel.Unity
{
    public class ReactiveText : MonoBehaviour
    {
        [SerializeField] private ViewModelDataProvider<UniTaskData.ViewModelProperty<int>> _data;

        [SerializeField] private Text _text;

        private void OnEnable()
        {
            _data.GetData()
                .Subscribe(OnChangeHandler);
        }

        private void OnChangeHandler(int obj)
        {
            _text.text = obj.ToString();
        }
    }
}