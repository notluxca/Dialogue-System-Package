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
        [Header("Animation settings")]
        [SerializeField] float Fadetime; // Actor fade in time
        [SerializeField] Color32 darkColor = new Color32(80, 80, 80, 255);
        Color32 startColor = new Color32(255, 255, 255, 255);
        public AnimationClip currentAnimationClip = null;
        [SerializeField] float EntranceSlideDuration = 1f;
        [SerializeField] float SlideDistance = 150f;


        CharacterDialogueAnimations characterAnimations; // modificar para receber o speech animation diretamente do graphview como animation clip 
        private Animator animator;
        private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rectTransform;
        private Image image;
        Vector3 startPosition;



        [SerializeField] public DSActor actor; //! 
        [SerializeField] private bool active = false;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            image = GetComponentInChildren<Image>();
            animator = GetComponentInChildren<Animator>();
            canvasGroup.alpha = 0;


        }

        void OnEnable()
        {
            DialogueUIManager.OnDialogueChanged += OnDialogueChange;

        }

        public void InitializeActor(DSActor Actor, CharacterDialogueAnimations characterAnimations)
        {
            // Debug.Log("Actor tried to initialize");
            this.active = true;
            this.characterAnimations = characterAnimations;
            canvasGroup.alpha = 0;
            active = false;
            this.actor = Actor;
            SetAnimation("listening");
        }

        void OnDialogueChange(DSActor Actor, string SpeechAnimation)
        {
            if (!active && this.actor == Actor)
            {
                // Primeira vez na cena
                active = true;

                // Faz fade in no CanvasGroup
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutSine);

                // Define a cor da imagem
                image.color = new Color32(255, 255, 255, 255);

                // Define a animação de fala
                SetAnimation(SpeechAnimation);

                // Pega a posição original do RectTransform
                RectTransform rectTransform = this.gameObject.GetComponent<RectTransform>();
                Vector3 originalPosition = rectTransform.anchoredPosition;

                // Define a posição de início fora da tela (vindo da esquerda)
                if (rectTransform.transform.localScale.x == -1)
                {
                    startPosition = originalPosition + new Vector3(150, 0, 0); ;
                }
                else
                {
                    startPosition = originalPosition - new Vector3(150, 0, 0); ;
                }
                // Move 150 unidades para a esquerda
                rectTransform.anchoredPosition = startPosition;

                // Anima a entrada do item até a posição original
                rectTransform.DOAnchorPos(originalPosition, EntranceSlideDuration).SetEase(Ease.OutQuint);


                return;
            }
            if (this.actor == Actor)
            {
                // Animate the color from white to dark over time
                canvasGroup.alpha = 1;
                image.DOColor(startColor, 0.5f); // Transition to the dark color
                SetAnimation(SpeechAnimation);
            }
            else if (active && this.actor != Actor && currentAnimationClip.name != "listening")
            {
                image.DOColor(darkColor, 0.5f); // Transition to the dark color
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

            // Debug.Log($"trying to change {newClip.name}");
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
            active = false;

        }

    }

}
