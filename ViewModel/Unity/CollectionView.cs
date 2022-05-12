using System;
using System.Collections.Generic;
using UnityEngine;

namespace ViewModel.Unity
{
    public class CollectionView : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private ViewModelDataProvider<CollectionData> _collectionDataProvider;

        private int _currentCount = 0;
        private readonly List<(IViewModel, MonoBehaviour)> _viewCollection = new List<(IViewModel, MonoBehaviour)>();

        private void OnEnable()
        {
            _currentCount = 0;
            _collectionDataProvider
                .GetData()
                .OnUpdate += UpdateHandler;
            UpdateHandler();
        }

        private void OnDisable()
        {
            foreach (var valueTuple in _viewCollection)
            {
                valueTuple.Item2.gameObject.SetActive(false);
                valueTuple.Item1.Dispose();
            }

            _currentCount = 0;
        }

        private void UpdateHandler()
        {
            var collectionData = _collectionDataProvider.GetData();
            for (int i = _currentCount; i < collectionData.Count; i++)
            {
                var viewModel = collectionData.FillViewModel(i);
                if (viewModel is MonoBehaviour monoBehaviour)
                {
                    monoBehaviour.gameObject.SetActive(true);
                    monoBehaviour.transform.SetParent(_content);

                    if (_currentCount >= _viewCollection.Count)
                    {
                        _viewCollection.Add((viewModel, monoBehaviour));
                    }
                    else
                    {
                        _viewCollection[_currentCount] = (viewModel, monoBehaviour);
                    }

                    _currentCount++;
                }
                else
                {
                    throw new Exception("Isn't monobehaviour");
                }
            }

            for (int i = collectionData.Count; i > _currentCount; i--)
            {
                var tuple = _viewCollection[i - 1];
                tuple.Item2.gameObject.SetActive(false);
                tuple.Item1.Dispose();
                _viewCollection[i - 1] = (null, null);
                _currentCount--;
            }
        }

        private void Reset()
        {
            _content = transform;
        }
    }
}