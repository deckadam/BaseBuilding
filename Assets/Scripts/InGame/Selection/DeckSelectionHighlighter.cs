using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Agent;
using Deck.EventManager;
using Deck.Services;
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
            DeckEventManager.Register<DeckOnAgentSelectedEvent>(OnAgentSelected);
            DeckEventManager.Register<DeckOnAgentPossessedEvent>(OnAgentPossessed);
            DeckEventManager.Register<DeckOnAgentReleasedEvent>(ClearTarget);
            DeckEventManager.Register<DeckOnSelectionReleasedEvent>(ClearTarget);
            _tokenSource = new CancellationTokenSource();
            Follow(_tokenSource.Token);
        }


        private void OnDisable()
        {
            DeckEventManager.Unregister<DeckOnAgentSelectedEvent>(OnAgentSelected);
            DeckEventManager.Unregister<DeckOnAgentPossessedEvent>(OnAgentPossessed);
            DeckEventManager.Unregister<DeckOnAgentReleasedEvent>(ClearTarget);
            DeckEventManager.Unregister<DeckOnSelectionReleasedEvent>(ClearTarget);
            _tokenSource.Cancel();
            _tokenSource.Dispose();
        }

        private void OnAgentPossessed(DeckOnAgentPossessedEvent obj)
        {
            SetTarget(obj.agent);
        }

        private void OnAgentSelected(DeckOnAgentSelectedEvent obj)
        {
            SetTarget(obj.agent);
        }

        private void SetTarget(DeckAgent obj)
        {
            if (_target != null)
            {
                _target.OnAgentSizeChanged -= OnTargetSizeChanged;
            }

            _target = obj;
            transform.localScale = _target.GetSize() * Vector3.one;
            _target.OnAgentSizeChanged += OnTargetSizeChanged;
        }

        private void ClearTarget(DeckEvent obj)
        {
            if (_target != null)
            {
                _target.OnAgentSizeChanged -= OnTargetSizeChanged;
            }

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