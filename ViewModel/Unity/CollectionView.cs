using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.CodeExtensions;
using Utilities.Unity.Extensions;

namespace ViewModel.Unity
{
    //ToDo : add refiling with new views
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
                .OnUpdateCollection += UpdateCollectionHandler;
            UpdateCollectionHandler();
        }

        private void OnDisable()
        {
            for (int i = 0; i < _currentCount; i++)
            {
                var valueTuple = _viewCollection[i];
                valueTuple.Item1.Dispose();
                valueTuple.Item2.gameObject.SetActive(false);
            }

            _currentCount = 0;
        }

        private void UpdateCollectionHandler()
        {
            var collectionData = _collectionDataProvider.GetData();
            for (int i = _currentCount; i < collectionData.Count; i++)
            {
                var viewModel = collectionData.RequestView(i);
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
                else if(viewModel.IsNotNullInUnity())
                {
                    throw new Exception("Isn't monobehaviour");
                }
            }

            for (int i = _currentCount; i > collectionData.Count; i--)
            {
                var tuple = _viewCollection[i - 1];
                tuple.Item1.Dispose();
                tuple.Item2.gameObject.SetActive(false);
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