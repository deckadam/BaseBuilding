using Deck.Component;
using Deck.Data.Damage;

namespace Deck.Components
{
    public class DeckDamageDealerComponent : IDeckComponent
    {
        private DeckComponentHolder _componentHolder;
        private DeckDataDamage _dataDamage;

        public void DealDamage()
        {
        }

        public void Initialize(DeckComponentHolder holder)
        {
            _componentHolder = holder;
            _dataDamage = _componentHolder.GetData<DeckDataDamage>();
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