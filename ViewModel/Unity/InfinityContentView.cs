using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ViewModel.Unity
{
    public class InfinityContentView : MonoBehaviour
    {
        [SerializeField] private ViewModelDataProvider<CollectionData> _collectionData;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private RectTransform _content;

        [SerializeField] private bool _isLooped;
        [SerializeField] private Direction _direction;
        
        private int _startIndex = -1;
        private int _endIndex = -1;

        private readonly LinkedList<IViewModel> _viewModels = new LinkedList<IViewModel>();

        private enum Direction
        {
            Horizontal,
            Vertical
        }
        
        private void Awake()
        {
        }

        private void OnEnable()
        {
            _startIndex = -1;
            _endIndex = -1;
        }

        private void UpdateHandler()
        {
        }

        private void Update()
        {
            var data = _collectionData.GetData();
            if (_endIndex >= data.Count)
            {
                _endIndex = data.Count;
            }
            
            var newIndexes = Update(_startIndex, _endIndex);
            while (_startIndex != newIndexes.Item1 || _endIndex != newIndexes.Item2)
            {
             if (_startIndex > newIndexes.Item1 && newIndexes.Item1 > -1)
             {
                 var viewModel = data.FillViewModel(newIndexes.Item1);
                 if (viewModel is MonoBehaviour monoBehaviour)
                 {
                     monoBehaviour.gameObject.SetActive(true);
                     monoBehaviour.transform.SetParent(_content);
                     monoBehaviour.transform.SetAsFirstSibling();
                 }
                 else
                 {
                     throw new Exception("Not mono view model");
                 }

                 _viewModels.AddFirst(viewModel);
             }
             else if(_startIndex < newIndexes.Item1 && newIndexes.Item1 < data.Count)
             {
                 _viewModels.First.Value.Dispose();
                 _viewModels.RemoveFirst();
             }
             _startIndex = newIndexes.Item1;
             
             if (_endIndex > newIndexes.Item2 && newIndexes.Item2 > -1)
             {
                 _viewModels.Last.Value.Dispose();
                 _viewModels.RemoveLast();
                 
             }
             else if(_endIndex < newIndexes.Item2 && newIndexes.Item2 < data.Count)
             {
                 var viewModel = data.FillViewModel(newIndexes.Item2);
                 if (viewModel is MonoBehaviour monoBehaviour)
                 {
                     monoBehaviour.gameObject.SetActive(true);
                     monoBehaviour.transform.SetParent(_content);
                     monoBehaviour.transform.SetAsLastSibling();
                 }
                 else
                 {
                     throw new Exception("Not mono view model");
                 }

                 _viewModels.AddLast(viewModel);
             }
             _endIndex = newIndexes.Item2;
            }
        }

        private (int, int)  Update(int from, int to)
        {
            var availableSpace = _viewport.rect.size;
            var viewPortScale = _viewport.lossyScale;
            var viewPortPosition = Devide(_viewport.position, viewPortScale);
            if (_viewModels.Count != 0)
            {
                if (_viewModels.First.Value is MonoBehaviour monoBehaviour)
                {
                    var viewModelRect = monoBehaviour.transform as RectTransform;
                    var viewModelPosition = Devide(viewModelRect.position, viewPortScale);
                    if (viewPortPosition.x + _viewport.rect.xMin >
                        viewModelPosition.x - viewModelRect.rect.width / 2f)
                    {
                        from++;
                    }
                    else
                    {
                        from--;
                    }
                }
            }

            if (_viewModels.Count != 0)
            {
                if (_viewModels.Last.Value is MonoBehaviour monoBehaviour2)
                {
                    var viewModelRect = monoBehaviour2.transform as RectTransform;
                    var viewModelPosition = Devide(viewModelRect.position, viewPortScale);

                    if (viewPortPosition.x + _viewport.rect.xMax < viewModelPosition.x + viewModelRect.rect.width / 2f)
                    {
                        to--;
                    }
                    else
                    {
                        to++;
                    }
                }
            }
            else
            {
                to++;
            }

            return (from, to);
        }

        private Vector2 Devide(Vector2 a, Vector2 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y);
        }
    }
}