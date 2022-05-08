using System;
using UniRx;
using UnityEngine;

namespace ViewModel.UniTaskData
{
    public class ViewModelEvent<T> : IViewModelEvent<T>
    {
        private readonly Subject<T> _subject = new Subject<T>();

        public IDisposable Subscribe(Action<T> action)
        {
            return UniRx.ObservableExtensions.Subscribe(_subject, action);
        }

        public void SetValue(T value)
        {
            _subject.OnNext(value);
        }
    }

    [Serializable]
    public class ViewModelProperty<T> : IViewModelProperty<T>
    {
        [SerializeField] private T _defaultValue;
        
        private readonly ReactiveProperty<T> _reactiveProperty = new ReactiveProperty<T>();
        public IDisposable Subscribe(Action<T> action)
        {
            return UniRx.ObservableExtensions.Subscribe(_reactiveProperty, action);
        }

        public void SetValue(T value)
        {
            _reactiveProperty.Value = value;
        }


        public T GetValue()
        {
            return _reactiveProperty.Value;
        }

        public void Reset()
        {
            _reactiveProperty.Value = _defaultValue;
        }
    }

    [Serializable]
    public class BoolViewModelProperty : ViewModelProperty<bool>
    {
    }

    [Serializable]
    public class IntViewModelProperty : ViewModelProperty<int>
    {
    }

    [Serializable]
    public class StringViewModelProperty : ViewModelProperty<string>
    {
    }

    [Serializable]
    public class FloatViewModelProperty : ViewModelProperty<float>
    {
    }

    [Serializable]
    public class NullViewModelEvent : ViewModelEvent<NullData>
    {
    }

    [Serializable]
    public class FloatViewModelEvent : ViewModelEvent<float>
    {
        
    }

    [Serializable]
    public class StringViewModelEvent : ViewModelEvent<string>
    {
        
    }

    [Serializable]
    public class IntViewModelEvent : ViewModelEvent<int>
    {
        
    }

    [Serializable]
    public class BoolViewModelEvent : ViewModelEvent<bool>
    {
        
    }
}