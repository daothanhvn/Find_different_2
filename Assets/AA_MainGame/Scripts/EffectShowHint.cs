﻿using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UniRx;
using UnityEngine;

namespace IceFoxStudio
{
    internal class ShowEffectHintMessage
    {
        public Vector3 Pos { get; set; }
    }

    public class EffectShowHint : MonoBehaviour
    {
        [SerializeField] private Transform startPos;
        [SerializeField] private float _duration = 1;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tween;

        private void Awake()
        {
            gameObject.SetActive(false);
            MessageBroker.Default.Receive<ShowEffectHintMessage>().TakeUntilDestroy(gameObject).Subscribe(mes =>
            {
                gameObject.SetActive(true);
                HandleHint(mes.Pos);
            });
        }

        private void HandleHint(Vector3 objPos)
        {
            var dis = Vector3.Distance(startPos.position, objPos);
            var duration = 1 * _duration;
            transform.position = startPos.position;
            _tween?.Kill();
            _tween = transform.DOMove(objPos, duration).OnComplete(() => gameObject.SetActive(false));
        }
    }
}