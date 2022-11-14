using System;
using UnityEngine;

namespace Sources
{
    public class GameTimer : MonoBehaviour
    {
        public event Action<float> OnTickedEvent;
        public bool IsActive { get; private set; } = true;

        public void Pause() => IsActive = false;
        
        public void UnPause() => IsActive = true;

        public double GetCurrentTime => Time.timeSinceLevelLoadAsDouble;

        private void Update()
        {
            if (IsActive) OnTickedEvent?.Invoke(Time.deltaTime);
        }
    }
}