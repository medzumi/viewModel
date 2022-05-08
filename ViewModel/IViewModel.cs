﻿using System;
using System.Threading;
using UnityEngine;

namespace ViewModel
{
    public interface IViewModel : IDisposable
    {
        T GetViewModelData<T>(string propertyName) where T : IViewModelData;

        T AddTo<T>(T disposable) where T : IDisposable;

        void SetViewModel(IViewModel viewModel, string key = null);
    }
}