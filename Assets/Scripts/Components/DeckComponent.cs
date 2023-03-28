using System;
using Deck.Components;
using Deck;

namespace Deck.Components
{
    public abstract class DeckComponent : IDeckComponent
    {
        protected DeckAgent holder { get; private set; }

        public void PreInitialize(DeckAgent holder)
        {
            this.holder = holder;
            InternalPreInitialize();
        }

        public void PostInitialize()
        {
            InternalPostInitialize();
        }

        public virtual void OnDeath()
        {
        }

        protected virtual void InternalPreInitialize()
        {
        }

        protected virtual void InternalPostInitialize()
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

        public virtual void Release()
        {
        }

        public virtual void Possess()
        {
        }

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