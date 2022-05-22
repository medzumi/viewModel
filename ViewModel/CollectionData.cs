using System;
using System.Collections;
using System.Collections.Generic;

namespace ViewModel
{
    [Serializable]
    public class CollectionData<TView> : IViewModelData, IResetable
        where TView : class
    {
        private static readonly List<object> _nullList = new List<object>();
        
        public event Action OnUpdateCollection;

        private Func<int, TView> _cocnreteFillAction;
        private ICollection _collectionReference = _nullList;

        public int Count => _collectionReference != null ? _collectionReference.Count : 0;

        public void Fill(ICollection collection, Func<int, TView> fillAction, bool forceUpdate = false)
        {
            //ToDo : add update view resolving
            if (fillAction != _cocnreteFillAction)
            {
                _cocnreteFillAction = fillAction;
            }
            
            //ToDo Fix ForceUpdate
            if (!object.ReferenceEquals(collection, _collectionReference) || forceUpdate)
            {
                _collectionReference = collection;
                OnUpdateCollection?.Invoke();
            }
        }

        public TView RequestView(int id)
        {
            return _cocnreteFillAction?.Invoke(id);
        }

        public void Reset()
        {
            Fill(_nullList, null, true);   
        }
    }
    
    [Serializable]
    public class CollectionData : CollectionData<IViewModel>
    {

    }
}