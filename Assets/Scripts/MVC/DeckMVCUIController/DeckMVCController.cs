using System;
using Deck.MVC.DeckMVCController;

namespace Deck.Test.MVC.DeckMVCController
{
    public class DeckMVCController<T> where T : class
    {
        private Action<IDeckModel<T>> _modelListener;
        private IDeckModel<T> _model;

        public void SetModel(IDeckModel<T> model)
        {
            _model = model;
            _model.Register(OnDataChanged);
        }

        public void ResetModel()
        {
            _modelListener = null;
            _model = null;
        }

        private void OnDataChanged(IDeckModel<T> obj)
        {
            _modelListener?.Invoke(obj);
        }

        public void AddModelListener(Action<IDeckModel<T>> listener)
        {
            _modelListener += listener;
        }

        public void RemoveModelListener(Action<IDeckModel<T>> listener)
        {
            _modelListener -= listener;
        }

        public IDeckModel<T> RequestData()
        {
            return _model;
        }

        public void Clear()
        {
        }
    }

    
}