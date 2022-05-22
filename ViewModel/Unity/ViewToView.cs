using System;
using UnityEngine;
using Utilities.CodeExtensions;
using Utilities.Unity.Extensions;

namespace ViewModel.Unity
{
    public class ViewToView<TView> : MonoBehaviour
        where TView : class
    {
        [SerializeField] private ViewModelDataProvider<ViewViewModelData<TView>> _viewModelDataProvider;

        private TView _view;
        
        private void OnEnable()
        {
            var data = _viewModelDataProvider.GetData();
            data.OnUpdateView += OnUpdateView;
            OnUpdateView();
        }

        private void OnDisable()
        {
            var data = _viewModelDataProvider.GetData();
            data.OnUpdateView -= OnUpdateView;
        }

        private void OnUpdateView()
        {
            UpdateView(_viewModelDataProvider.GetData().Request());
        }

        private void UpdateView(TView view)
        {
            if (_view is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _view = view;
            if (_view is Component component)
            {
                component.gameObject.SetActive(true);
                component.transform.SetParent(transform);
            }
            else if (_view is GameObject gameObject)
            {
                gameObject.SetActive(true);
                gameObject.transform.SetParent(transform);
            }
            else if(_view.IsNotNullInUnity())
            {
                throw new Exception("View isn't mono");
            }
        }
    }

    public class ViewToView : ViewToView<IViewModel>
    {
        
    }
}