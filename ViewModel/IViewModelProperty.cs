using System;

namespace ViewModel
{
    public interface IViewModelData
    {
        
    }

    public interface IViewModelEvent<T> : IViewModelData
    {
        IDisposable Subscribe(Action<T> action);

        void SetValue(T value);
    }

    public interface IViewModelProperty
    {
        void Reset();
    }
    
    public interface IViewModelProperty<T> : IViewModelEvent<T>, IViewModelProperty
    {
        T GetValue();
    }
}