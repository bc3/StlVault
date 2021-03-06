﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using StlVault.Util.Collections;
using StlVault.ViewModels;

namespace StlVault.Services
{
    internal sealed class PreviewList : ObservableList<ItemPreviewModel>, IPreviewList
    {
        [NotNull] private readonly Action<PreviewList> _disposeCallback;
        [NotNull] private readonly Func<IReadOnlyCollection<ItemPreviewModel>, IReadOnlyCollection<ItemPreviewModel>> _filter;

        public PreviewList([NotNull] Action<PreviewList> disposeCallback, [NotNull] Func<IReadOnlyCollection<ItemPreviewModel>, IReadOnlyCollection<ItemPreviewModel>> filter)
        {
            _disposeCallback = disposeCallback ?? throw new ArgumentNullException(nameof(disposeCallback));
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public void AddFiltered(IReadOnlyCollection<ItemPreviewModel> previewInfos)
        {
            var filtered = _filter.Invoke(previewInfos);
            AddRange(filtered);
        }

        public void AddFiltered(ItemPreviewModel previewInfo)
        {
            var array = new[] {previewInfo};
            var filtered = _filter.Invoke(array);
            if (filtered.Count == 1) Add(previewInfo);
        }

        public void Dispose()
        {
            _disposeCallback.Invoke(this);
        }

        public void RemoveRange(HashSet<ItemPreviewModel> itemsToRemove)
        {
            using (EnterMassUpdate())
            {
                for (var i = Count - 1; i >= 0; i--)
                {
                    if (itemsToRemove.Contains(this[i])) RemoveAt(i);
                }
            }
        }
    }
}