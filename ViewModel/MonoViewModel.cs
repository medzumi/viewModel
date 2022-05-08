﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.SerializeReferencing;

namespace ViewModel
{
    [DefaultExecutionOrder(-100)]
    public class MonoViewModel : MonoBehaviour, IViewModel
    {
        [Serializable]
        private class CustomPair
        {
            public string Key = string.Empty;

            [SerializeReference] [SerializeTypes(typeof(IViewModelData))]
            public IViewModelData Data = null;
        }
        
        [Serializable]
        private class ViewModelPlace
        {
            public string Key = string.Empty;
            public Transform Place;
            public IViewModel ViewModel;
        }

        [SerializeField] private List<CustomPair> _customPairs = new List<CustomPair>();
        [SerializeField] private List<ViewModelPlace> _viewModelPlaces = new List<ViewModelPlace>();
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private bool _isDisposed = false;

#if UNITY_EDITOR
        private Dictionary<int, IViewModelData> _viewModelDatas = new Dictionary<int, IViewModelData>();
        
        private void OnValidate()
        {
            _viewModelDatas.Clear();
            foreach (var customPair in _customPairs)
            {
                PropertyName propertyName = customPair.Key;
                _viewModelDatas.Add(propertyName.GetHashCode(), customPair.Data);
            }                
        }
        #endif
        public T GetViewModelData<T>(string key) where  T : IViewModelData
        {
            var data = GetViewModelDataHandler(key);
            return (T)data;
        }

        public object GetViewModelData(string key)
        {
            return GetViewModelDataHandler(key);
        }

        public T AddTo<T>(T disposable) where T : IDisposable
        {
            _disposables.Add(disposable);
            return disposable;
        }

        public void SetViewModel(IViewModel viewModel, string key = null)
        {
            var place = _viewModelPlaces.Single(modelPlace => string.Equals(key, modelPlace.Key));
            if (viewModel is MonoBehaviour monoBehaviour)
            {
                monoBehaviour.transform.SetParent(place.Place);
            }

            place.ViewModel = viewModel;
        }

        public void Dispose()
        {
            if(_isDisposed)
                return;

            _isDisposed = true;
            
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
            _disposables.Clear();
            
            foreach (var keyValuePair in _customPairs)
            {
                if (keyValuePair.Data is IViewModelProperty viewModelProperty)
                {
                    viewModelProperty.Reset();
                }
            }

            foreach (var viewModelPlace in _viewModelPlaces)
            {
                viewModelPlace.ViewModel?.Dispose();
            }
        }

        public void Reset()
        {
            _isDisposed = false;
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void AddViewModelDataHandler(string key, IViewModelData data)
        {
            if (GetViewModelDataHandler(key) == null)
            {
                _customPairs.Add(new CustomPair()
                {
                    Key =  key,
                    Data = data
                });
            }
        }

        private object GetViewModelDataHandler(string key)
        {
            var pair = _customPairs.FirstOrDefault(pair => string.Equals(key, pair.Key));
            return pair?.Data;
        }
    }
}