using System;
using Utilities;

namespace ViewModel
{
    public interface IViewModel : IDisposeHandler, IDisposable
    {
        T GetViewModelData<T>(string propertyName) where T : IViewModelData;

        void SetViewModel(IViewModel viewModel, string key = null);
    }
}