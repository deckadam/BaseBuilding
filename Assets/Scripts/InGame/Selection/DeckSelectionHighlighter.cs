using System.Threading;
using Base;
using Cysharp.Threading.Tasks;
using EventManager;
using Services.Selection.Events;
using UnityEngine;

namespace InGame.Selection
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
            DeckEventManager.Register<DeckEventOnAgentReleased>(OnAgentReleased);
            DeckEventManager.Register<DeckEventOnSelectionReleased>(OnSelectionReleased);
            _tokenSource = new CancellationTokenSource();
            Follow(_tokenSource.Token);
        }


        private void OnDisable()
        {
            DeckEventManager.Unregister<DeckEventOnAgentSelected>(OnAgentSelected);
            DeckEventManager.Unregister<DeckEventOnAgentPossessed>(OnAgentPossessed);
            DeckEventManager.Unregister<DeckEventOnAgentReleased>(OnAgentReleased);
            DeckEventManager.Unregister<DeckEventOnSelectionReleased>(OnSelectionReleased);
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

        private void OnAgentReleased(DeckEventOnAgentReleased obj)
        {
            ClearTarget();
        }
        
        private void OnSelectionReleased(DeckEventOnSelectionReleased obj)
        {
            ClearTarget();
        }
        
        private void ClearTarget()
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