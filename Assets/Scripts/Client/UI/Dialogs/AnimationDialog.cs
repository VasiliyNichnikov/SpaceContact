using System;
using System.Collections;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(Animation))]
    public class AnimationDialog : MonoBehaviour
    {
        [SerializeField] 
        private string _openingDialogAnimation = "OpeningDialog";

        [SerializeField] 
        private string _closingDialogAnimation = "ClosingDialog";
        
        private Animation _animation = null!;
        
        private IEnumerator? _currentAnimation;

        public bool CanPlayOpenAnimation() => CanPlayAnimation(_openingDialogAnimation);
        public bool CanPlayCloseAnimation() => CanPlayAnimation(_closingDialogAnimation);
        
        private bool CanPlayAnimation(string animationName) => _animation.GetClip(animationName) != null;
        
        public void Open(Action? onCompletedAnimation = null)
        {
            PlayAnimation(_openingDialogAnimation, onCompletedAnimation);
        }

        public void Close(Action? onCompletedAnimation = null)
        {
            PlayAnimation(_closingDialogAnimation, onCompletedAnimation);   
        }

        private void Awake()
        {
            _animation = GetComponent<Animation>();
            _animation.playAutomatically = false;
        }

        private void PlayAnimation(string clipName, Action? onCompletedAnimation)
        {
            var clip = _animation.GetClip(clipName);
            if (clip == null)
            {
                Debug.LogError($"AnimationDialog.PlayAnimation: clip with name {clipName} not found");
                onCompletedAnimation?.Invoke();
                return;
            }
            
            TryStopCurrentAnimation();
            
            _currentAnimation = WaitingForAnimationToFinish(clip, onCompletedAnimation);
            StartCoroutine(_currentAnimation);
        }

        private IEnumerator WaitingForAnimationToFinish(AnimationClip clip, Action? onCompletedAnimation)
        {
            _animation.clip = clip;
            _animation.Play();

            while (_animation.isPlaying)
            {
                yield return null;
            }

            // Жрет и правда много
            // лучше не вызывать в куротине колбек
            // использовать в экстренных случаях
            onCompletedAnimation?.Invoke();
        }
        
        private void OnDisable()
        {
            TryStopCurrentAnimation();
        }

        private void TryStopCurrentAnimation()
        {
            if (_currentAnimation == null)
            {
                return;
            }

            StopCoroutine(_currentAnimation);
            _currentAnimation = null;
        }
    }
}