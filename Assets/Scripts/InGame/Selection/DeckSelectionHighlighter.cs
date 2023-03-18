using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Agent;
using UnityEngine;

namespace Deck.Map.Selection
{
    public class DeckSelectionHighlighter : MonoBehaviour
    {
        [SerializeField] private float distanceFromGround;
        private DeckAgent _target;
        private CancellationTokenSource _tokenSource;

        public void SetTarget(DeckAgent newTarget)
        {
            _target = newTarget;
            _target.OnAgentSizeChanged -= OnTargetSizeChanged;
            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
            Follow(_target, _tokenSource.Token);
        }

        public void ClearTarget()
        {
            _tokenSource?.Cancel();
            _target.OnAgentSizeChanged -= OnTargetSizeChanged;
            _target = null;
            transform.position = Vector3.down * 1000f;
        }

        private void OnTargetSizeChanged(float newSize)
        {
            transform.localScale = newSize * Vector3.one;
        }

        private async void Follow(DeckAgent target, CancellationToken token)
        {
            var cachedTransform = _target.transform;
            transform.localScale = target.GetSize() * Vector3.one;
            var destroyToken = target.gameObject.GetCancellationTokenOnDestroy();
            while (!token.IsCancellationRequested && !destroyToken.IsCancellationRequested)
            {
                var elevatedPos = cachedTransform.position;
                elevatedPos.y = distanceFromGround;
                transform.position = elevatedPos;
                await UniTask.NextFrame();
            }
        }
    }
}