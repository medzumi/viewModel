﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace ViewModel
{
    [Serializable]
    public class CollectionData : IViewModelData
    {
        public event Action OnUpdate;
        
        private Func<int, IViewModel> _cocnreteFillAction;
        private ICollection _collectionReference = null;

        public int Count => _collectionReference != null ? _collectionReference.Count : 0;

        public void Fill<TCollection, TData>(TCollection collection, Func<TData, IViewModel> fillAction, bool forceUpdate = false)
            where TCollection : class, IList<TData>, ICollection
        {
            //ToDo Fix ForceUpdate
            if (!object.ReferenceEquals(collection, _collectionReference) || forceUpdate)
            {
                _collectionReference = collection;
                    //ToDo : if need present different compoents
                _cocnreteFillAction = (i) => fillAction?.Invoke(collection[i]);
                OnUpdate?.Invoke();
            }
        }

        public IViewModel FillViewModel(int id)
        {
            return _cocnreteFillAction?.Invoke(id);
        }
    }
}