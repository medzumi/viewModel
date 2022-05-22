using UnityEngine;
using ViewModel.UniTaskData;

namespace ViewModel.Unity
{
    public class NullInvoker : MonoBehaviour
    {
        [SerializeField] private ViewModelDataProvider<IViewModelEvent<NullData>> _data;

        public void Invoke()
        {
            _data.GetData()
                .SetValue(default);
        }
    }
}