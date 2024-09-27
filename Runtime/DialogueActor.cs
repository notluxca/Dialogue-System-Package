using System.Collections.Generic;
using UnityEngine;


namespace DialogueSystem
{
    using DialogueSystem.Enumerations;
    using ScriptableObjects;
    using UnityEngine.UI;
    using DG.Tweening;


    public class DialogueActor : MonoBehaviour
    {
        CharacterDialogueAnimations characterAnimations; // modificar para receber o speech animation diretamente do graphview como animation clip 
        private Animator animator;
        private CanvasGroup canvasGroup;
        public AnimationClip currentAnimationClip = null;
        private RectTransform rectTransform;
        private Image image;
        bool haveTalked;
        // Original color (white, fully visible)
        Color32 startColor = new Color32(255, 255, 255, 255);

        // Dark color (dark gray, fully visible) or another dark color of your choice
        Color32 darkColor = new Color32(50, 50, 50, 255);


        [SerializeField] private DSActor actor;
        private bool active = false;

        void Awake()
        {
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();
            canvasGroup.alpha = 0;


        }

        void OnEnable()
        {
            haveTalked = false;
            DialogueUIManager.OnDialogueChanged += OnDialogueChange;

        }

        void OnDialogueChange(DSActor Actor, string SpeechAnimation)
        {

            if (!active && this.actor == Actor)
            {
                //* first time on scene
                //* animate entrance
                //* rectTransform.sizeDelta = new Vector2(0, 0); modificando a width e height do rect transform
                active = true;
                canvasGroup.alpha = 1;
                image.color = new Color32(255, 255, 255, 255);

                SetAnimation(SpeechAnimation);

                // return;
            }
            if (this.actor == Actor)
            {
                // Animate the color from white to dark over time
                canvasGroup.alpha = 1;
                image.DOColor(startColor, 0.5f); // Transition to the dark color
                SetAnimation(SpeechAnimation);
            }
            else
            {


                image.DOColor(darkColor, 0.5f); // Transition to the dark color
                                                // image.color = new Color32(120, 120, 120, 255);
                SetAnimation("listening");

            }

            // ChangeCurrentAnimation(currentAnimationClip);
        }

        public void SetAnimation(string animation)
        {
            if (characterAnimations == null)
            {
                Debug.LogError("CharacterAnimations is not set! Make sure InitializeActor() was called.");
                return;
            }

            AnimationClip animationClip = characterAnimations.GetAnimationClip(this.actor, animation);
            if (animationClip == null)
            {
                Debug.LogError($"AnimationClip not found for {actor} with animation {animation}");
                return;
            }

            currentAnimationClip = animationClip;
            ChangeCurrentAnimation(animationClip);
        }

        public void InitializeActor(DSActor Actor, CharacterDialogueAnimations characterAnimations)
        {
            //Debug.Log("Actor tried to initialize");
            this.active = true;
            this.characterAnimations = characterAnimations;
            canvasGroup.alpha = 0;
            active = true;
            this.actor = Actor;
            SetAnimation("listening");
        }

        void ChangeCurrentAnimation(AnimationClip newClip)
        {
            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogError("Animator runtime controller is null!");
                return;
            }

            var overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

            if (overrideController.runtimeAnimatorController.animationClips.Length == 0)
            {
                Debug.LogError("No animation clips found in the animator controller!");
                return;
            }

            Debug.Log($"trying to change {newClip.name}");
            var currentClip = overrideController.runtimeAnimatorController.animationClips[0];
            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>
                {
                    new KeyValuePair<AnimationClip, AnimationClip>(currentClip, newClip)
                };

            overrideController.ApplyOverrides(overrides);
            animator.runtimeAnimatorController = overrideController;
            animator.Play("default", 0, 0);
            animator.Play("default", 0, 0);
            animator.Play("default", 0, 0);

        }

        void OnDisable()
        {
            canvasGroup.alpha = 0;
            DialogueUIManager.OnDialogueChanged -= OnDialogueChange;
        }

    }

}
