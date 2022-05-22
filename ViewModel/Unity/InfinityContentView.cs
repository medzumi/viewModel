using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Unity.Components;

namespace ViewModel.Unity
{
    [Obsolete("Doesnt work yet. Require custom scroll rect")]
    public class InfinityContentView : MonoBehaviour
    {
        [SerializeField] private ViewModelDataProvider<CollectionData> _collectionData;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private RectTransform _content;

        [SerializeField] private bool _isLooped;
        [SerializeField] private Direction _direction;
        
        private int _startIndex = -1;
        private int _endIndex = -1;

        private Vector2 _speed;

        private readonly LinkedList<IViewModel> _viewModels = new LinkedList<IViewModel>();

        private enum Direction
        {
            Horizontal,
            Vertical
        }

        private void Awake()
        {
            _collectionData.GetData().OnUpdateCollection += UpdateCollectionHandler;
        }

        private void OnEnable()
        {
            _startIndex = 0;
            UpdateCollectionHandler();
        }

        private void UpdateCollectionHandler()
        {
            var data = _collectionData.GetData();

            if (!_isLooped)
            {
                if (_startIndex > data.Count)
                {
                    _startIndex = data.Count - 1;
                }else if (_startIndex < 0)
                {
                    _startIndex = 0;
                }
            }

            foreach (var viewModel in _viewModels)
            {
                viewModel.Dispose();
            }
            
            _viewModels.Clear();

            if (data.Count == 0)
            {
                return;
            }
            
            var scale = _viewport.lossyScale;
            var viewPortPosition = Devide(_viewport.position, scale);

            _endIndex = _startIndex;
            for (int i = _startIndex; (i < data.Count && i >= 0) || _isLooped; i++)
            {
                var index = i;
                if (_isLooped)
                {
                    index %= data.Count;
                }
                
                var viewModel = data.RequestView(index);
                if (viewModel is MonoBehaviour monoBehaviour)
                {
                    monoBehaviour.gameObject.SetActive(true);
                    monoBehaviour.transform.SetParent(_content);
                    #if UNITY_EDITOR
                    monoBehaviour.name = $"Element {i}";
                    #endif
                    _viewModels.AddLast(viewModel);
                    var monoRect = monoBehaviour.transform as RectTransform;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
                    var monoPosition = Devide(monoRect.position, scale);
                    _endIndex++;
                    if (viewPortPosition.x + _viewport.rect.xMax < monoPosition.x + monoRect.rect.width / 2f)
                    {
                        break;
                    }
                }
                else
                {
                    throw new Exception("Not mono view model");
                }
            }
        }

        private void LateUpdate()
        {
            var delta = _content.position - (Vector3)_speed;
            _speed = delta* 0.5f + _content.position;
            if (_collectionData.GetData().Count != 0)
            {
                var scale = _viewport.lossyScale;
                var viewPortPosition = Devide(_viewport.position, scale);
                var scaledSpeed = Devide(_speed, scale);
                var firstElement = _viewModels.First.Value as MonoBehaviour;
                var firstRect = firstElement.transform as RectTransform;
                var firstPos = Devide(firstRect.position, scale);
                if (viewPortPosition.x + _viewport.rect.xMin > firstPos.x + firstRect.rect.xMax && _viewModels.Count > 1)
                {
                    _viewModels.First.Value.Dispose();
                    _viewModels.RemoveFirst();
                    _startIndex++;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
                }
                else if(viewPortPosition.x + _viewport.rect.xMin + scaledSpeed.x < firstPos.x + firstRect.rect.xMin)
                {
                    _startIndex--;
                    if (_startIndex < 0 && !_isLooped)
                    {
                        _startIndex = 0;
                    }
                    else
                    {
                        var viewModel = _collectionData.GetData()
                            .RequestView(_startIndex % _collectionData.GetData().Count);
                        _viewModels.AddFirst(viewModel);
                        if (viewModel is MonoBehaviour monoBehaviour)
                        {
#if UNITY_EDITOR
                            monoBehaviour.name = $"Element {_startIndex}";
#endif
                            monoBehaviour.gameObject.SetActive(true);
                            monoBehaviour.transform.SetParent(_content);
                            monoBehaviour.transform.SetAsFirstSibling();
                            LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
                        }
                        else
                        {
                            throw new Exception("Not mono view model");
                        }
                    }
                }
            }
            
            if (_viewModels.Count != 0)
            {
                var scale = _viewport.lossyScale;
                var viewPortPosition = Devide(_viewport.position, scale);
                var scaledSpeed = Devide(_speed, scale);
                var lastElement = _viewModels.Last.Value as MonoBehaviour;
                var lastRect = lastElement.transform as RectTransform;
                var lastPos = Devide(lastRect.position, scale);
                if (viewPortPosition.x + _viewport.rect.xMax < lastPos.x + lastRect.rect.xMin && _viewModels.Count > 1)
                {
                    _viewModels.Last.Value.Dispose();
                    _viewModels.RemoveLast();
                    _endIndex--;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
                }
                else if(viewPortPosition.x + _viewport.rect.xMax + scaledSpeed.x > lastPos.x + lastRect.rect.xMax)
                {
                    _endIndex++;
                    if (_collectionData.GetData().Count <= _endIndex && !_isLooped)
                    {
                        _endIndex = _collectionData.GetData().Count - 1;
                    }
                    else
                    {
                        var viewmodel = _collectionData.GetData()
                            .RequestView(_endIndex % _collectionData.GetData().Count);
                        _viewModels.AddLast(viewmodel);
                        if (viewmodel is MonoBehaviour monoBehaviour)
                        {
#if UNITY_EDITOR
                            monoBehaviour.name = $"Element {_endIndex}";
#endif
                            monoBehaviour.gameObject.SetActive(true);
                            monoBehaviour.transform.SetParent(_content);
                            monoBehaviour.transform.SetAsLastSibling();
                            LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
                        }
                        else
                        {
                            throw new Exception("Not mono view model");
                        }
                    }
                }   
            }
        }

        private Vector2 Devide(Vector2 a, Vector2 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y);
        }
    }
}