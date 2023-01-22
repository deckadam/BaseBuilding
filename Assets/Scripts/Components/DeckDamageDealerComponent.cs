using Deck.Component;
using Deck.Data.Damage;

namespace Deck.Components
{
    public class DeckDamageDealerComponent : IDeckComponent
    {
        private DeckComponentHolder _componentHolder;
        private DeckDamageData _damageData;

        public void DealDamage()
        {
        }

        public void Initialize(DeckComponentHolder holder)
        {
            _componentHolder = holder;
            _damageData = _componentHolder.GetData<DeckDamageData>();
        }

        public void DeInitialize()
        {
        }

        public void Tick()
        {
        }

        public DeckComponentHolder GetComponentOwner()
        {
            return _componentHolder;
        }

        public object GetData()
        {
            return new byte[0];
        }

        public void LoadData(string value)
        {
        }
    }
}