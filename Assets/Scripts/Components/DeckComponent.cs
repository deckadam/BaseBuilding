using System;
using Deck.Component;
using Deck.Components.Operations;
using UnityEngine;

namespace Deck.Components
{
    public abstract class DeckComponent
    {
        protected DeckAgent holder { get; private set; }

        public void Initialize(DeckAgent holder)
        {
            this.holder = holder;
            Initialize();
        }

        protected virtual void Initialize()
        {
        }

        public virtual void DeInitialize()
        {
        }

        public virtual void Tick()
        {
        }


        public virtual void LoadData(string value)
        {
        }

        public abstract DeckCommandListener[] GetSupportedCommandTypes();
        public DeckAgent GetComponentHolder() => holder;

        public virtual object GetData()
        {
            return null;
        }
    }

    [Serializable]
    public class DeckComponentSaveData
    {
        public string id;
        public string data;
    }
}