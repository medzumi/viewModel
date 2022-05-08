using System;
using UnityEngine;

namespace ViewModel
{
    [Serializable]
    public class ViewModelDataProvider<T>
        where T : IViewModelData
    {
        private T _cachedData;

        [SerializeField] private MonoViewModel _monoViewModel;
        [SerializeField] private string _key;
        
        public T GetData()
        {
            return _cachedData ??= GetDataFromViewModel();
        }

        private T GetDataFromViewModel()
        {
            return _monoViewModel.GetViewModelData<T>(_key);
        }
    }
}