using System;
using UnityEngine;

namespace Deck.MVC
{
    public class DeckMVCController<T, J> where T : class where J : class
    {
        private Action<IDeckModel<T, J>> _modelListener;
        private IDeckModel<T, J> _model;

        public void SetModel(IDeckModel<T, J> model)
        {
            _model = model;
            _model.Register(OnDataChanged);

            OnDataChanged(model);
        }

        public IDeckModel<T, J> GetModel()
        {
            return _model;
        }

        public void ResetModel()
        {
            _modelListener = null;
            _model = null;
        }

        private void OnDataChanged(IDeckModel<T, J> obj)
        {
            _modelListener?.Invoke(obj);
        }

        public void AddModelListener(Action<IDeckModel<T, J>> listener)
        {
            _modelListener += listener;
        }

        public void RemoveModelListener(Action<IDeckModel<T, J>> listener)
        {
            _modelListener -= listener;
        }

        public IDeckModel<T, J> RequestData()
        {
            return _model;
        }

        public void Clear()
        {
        }
    }
}