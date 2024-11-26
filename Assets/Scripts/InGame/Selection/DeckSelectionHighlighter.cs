using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Base;
using Deck.EventManager;
using Deck.Services.Selection.Events;
using UnityEngine;

namespace Deck.Selection
{
    public class DeckSelectionHighlighter : MonoBehaviour
    {
        [SerializeField] private float distanceFromGround;

        private DeckAgent _target;
        private CancellationTokenSource _tokenSource;

        private void OnEnable()
        {
            DeckEventManager.Register<DeckEventOnAgentSelected>(OnAgentSelected);
            DeckEventManager.Register<DeckEventOnAgentPossessed>(OnAgentPossessed);
            DeckEventManager.Register<DeckEventOnAgentReleased>(ClearTarget);
            DeckEventManager.Register<DeckEventOnSelectionReleased>(ClearTarget);
            _tokenSource = new CancellationTokenSource();
            Follow(_tokenSource.Token);
        }


        private void OnDisable()
        {
            DeckEventManager.Unregister<DeckEventOnAgentSelected>(OnAgentSelected);
            DeckEventManager.Unregister<DeckEventOnAgentPossessed>(OnAgentPossessed);
            DeckEventManager.Unregister<DeckEventOnAgentReleased>(ClearTarget);
            DeckEventManager.Unregister<DeckEventOnSelectionReleased>(ClearTarget);
            _tokenSource.Cancel();
            _tokenSource.Dispose();
        }

        private void OnAgentPossessed(DeckEventOnAgentPossessed obj)
        {
            SetTarget(obj.agent);
        }

        private void OnAgentSelected(DeckEventOnAgentSelected obj)
        {
            SetTarget(obj.agent);
        }

        private void SetTarget(DeckAgent obj)
        {
            _target = obj;
            var itemVisual = _target.GetItemVisual();
            var targetSize = 1f;
            if (itemVisual != null)
            {
                targetSize = itemVisual.GetSize();
            }

            transform.localScale = targetSize * Vector3.one;
        }

        private void ClearTarget(IDeckEvent obj)
        {
            _target = null;
            transform.position = Vector3.down * 1000f;
        }

        private void OnTargetSizeChanged(float newSize)
        {
            transform.localScale = newSize * Vector3.one;
        }

        private async void Follow(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_target == null)
                {
                    await UniTask.Yield();
                    continue;
                }

                var elevatedPos = _target.transform.position;
                elevatedPos.y = distanceFromGround;
                transform.position = elevatedPos;
                await UniTask.Yield();
            }
        }
    }
}