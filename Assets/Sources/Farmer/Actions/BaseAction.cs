using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sources.Farmer.Actions
{
    public abstract class BaseAction
    {
        public Sprite ActionSprite { get; }
        protected Action _finishCallback;
        
        public BaseAction(Sprite actionSprite, Action finishCallback = null)
        {
            ActionSprite = actionSprite;
            _finishCallback = finishCallback;
        }

        public abstract UniTask PerformAction(CancellationToken cancellationToken);
    }
}