using System;

namespace ViewModel
{
    [Serializable]
    public class ViewViewModelData<TView> : IViewModelData, IResetable
        where TView : class
    {
        public event Action OnUpdateView;

        private Func<TView> _concreteAction;

        public void Fill(Func<TView> func, bool isForce = false)
        {
            if (func != _concreteAction || isForce)
            {
                _concreteAction = func;
                OnUpdateView?.Invoke();
            }
        }

        public TView Request()
        {
            return _concreteAction?.Invoke();
        }

        public void Reset()
        {
            Fill(null);
        }
    }
    
    [Serializable]
    public class ViewViewModelData : ViewViewModelData<IViewModel>
    {
    }
}