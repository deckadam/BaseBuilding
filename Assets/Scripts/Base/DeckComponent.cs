using System;
using Deck.UI.Stats;
using UnityEngine;

namespace Deck.Base
{
    public abstract class DeckComponent : MonoBehaviour, IDeckComponent
    {
        public Action<DeckStatGroup> OnStatsChanged;

        protected DeckAgent agent { get; private set; }

        public void PreInitialize(DeckAgent holder)
        {
            agent = holder;
            InternalPreInitialize();
        }

        public void PostInitialize()
        {
            InternalPostInitialize();
        }

        public virtual void OnDestroy()
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

        public virtual void Release()
        {
        }

        public virtual void Possess()
        {
        }

        public DeckAgent GetAgent() => agent;

        public virtual object GetData()
        {
            return null;
        }

        public virtual void LoadData(string value)
        {
        }

        public virtual DeckStatGroup GetStatGroup()
        {
            return default;
        }
    }

    [Serializable]
    public class DeckComponentSaveData
    {
        public string id;
        public string data;
    }
}