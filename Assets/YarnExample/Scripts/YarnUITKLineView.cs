using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using UnityEngine.UI;
using UnityEngine.UIElements;

using Yarn.Unity;


    /// <summary>
    /// A Dialogue View that presents lines of dialogue, using Unity UI
    /// elements.
    /// </summary>
    public class YarnUITKLineView : DialogueViewBase
    {
        /// <summary>
        /// The canvas group that contains the UI elements used by this Line
        /// View.
        /// </summary>
        /// <remarks>
        /// If <see cref="useFadeEffect"/> is true, then the alpha value of this
        /// <see cref="CanvasGroup"/> will be animated during line presentation
        /// and dismissal.
        /// </remarks>
        /// <seealso cref="useFadeEffect"/>
    //    [SerializeField]
    //    internal CanvasGroup canvasGroup;

        /// <summary>
        /// Controls whether the line view should fade in when lines appear, and
        /// fade out when lines disappear.
        /// </summary>
        /// <remarks><para>If this value is <see langword="true"/>, the <see
        /// cref="canvasGroup"/> object's alpha property will animate from 0 to
        /// 1 over the course of <see cref="fadeInTime"/> seconds when lines
        /// appear, and animate from 1 to zero over the course of <see
        /// cref="fadeOutTime"/> seconds when lines disappear.</para>
        /// <para>If this value is <see langword="false"/>, the <see
        /// cref="canvasGroup"/> object will appear instantaneously.</para>
        /// </remarks>
        /// <seealso cref="canvasGroup"/>
        /// <seealso cref="fadeInTime"/>
        /// <seealso cref="fadeOutTime"/>
        [SerializeField]
        internal bool useFadeEffect = false;

        /// <summary>
        /// The time that the fade effect will take to fade lines in.
        /// </summary>
        /// <remarks>This value is only used when <see cref="useFadeEffect"/> is
        /// <see langword="true"/>.</remarks>
        /// <seealso cref="useFadeEffect"/>
        [SerializeField]
        [Min(0)]
        internal float fadeInTime = 0.25f;

        /// <summary>
        /// The time that the fade effect will take to fade lines out.
        /// </summary>
        /// <remarks>This value is only used when <see cref="useFadeEffect"/> is
        /// <see langword="true"/>.</remarks>
        /// <seealso cref="useFadeEffect"/>
        [SerializeField]
        [Min(0)]
        internal float fadeOutTime = 0.05f;



        /// Fade time for the options
        [SerializeField] float fadeTime = 0.1f;

        protected new static readonly string showingOptionsClassName = "showing-options";



        protected VisualElement documentRoot;

        /// <summary>
        /// The <see cref="TextMeshProUGUI"/> object that displays the text of
        /// dialogue lines.
        /// </summary>
        [SerializeField]
        protected VisualTreeAsset optionListItemTemplate;
        //internal TextMeshProUGUI lineText = null;

        protected TypewriterLabel lineText;

        protected Button continueButton;

        protected Label characterNameText;

        protected VisualElement optionsWrapper;
        protected VisualElement contentWrapper;

        protected VisualElement characterImage;

        protected List<TemplateContainer> optionViews = new List<TemplateContainer>();

        protected bool showUnavailableOptions = false;

        protected Action<int> OnOptionSelected;

        /// <summary>
        /// Controls whether the <see cref="lineText"/> object will show the
        /// character name present in the line or not.
        /// </summary>
        /// <remarks>
        /// <para style="note">This value is only used if <see
        /// cref="characterNameText"/> is <see langword="null"/>.</para>
        /// <para>If this value is <see langword="true"/>, any character names
        /// present in a line will be shown in the <see cref="lineText"/>
        /// object.</para>
        /// <para>If this value is <see langword="false"/>, character names will
        /// not be shown in the <see cref="lineText"/> object.</para>
        /// </remarks>
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("showCharacterName")]
        internal bool showCharacterNameInLineView = true;

        /// <summary>
        /// The <see cref="TextMeshProUGUI"/> object that displays the character
        /// names found in dialogue lines.
        /// </summary>
        /// <remarks>
        /// If the <see cref="LineView"/> receives a line that does not contain
        /// a character name, this object will be left blank.
        /// </remarks>
        //[SerializeField]
        //internal TextMeshProUGUI characterNameText = null;

        /// <summary>
        /// Controls whether the text of <see cref="lineText"/> should be
        /// gradually revealed over time.
        /// </summary>
        /// <remarks><para>If this value is <see langword="true"/>, the <see
        /// cref="lineText"/> object's <see
        /// cref="TMP_Text.maxVisibleCharacters"/> property will animate from 0
        /// to the length of the text, at a rate of <see
        /// cref="typewriterEffectSpeed"/> letters per second when the line
        /// appears. <see cref="onCharacterTyped"/> is called for every new
        /// character that is revealed.</para>
        /// <para>If this value is <see langword="false"/>, the <see
        /// cref="lineText"/> will all be revealed at the same time.</para>
        /// <para style="note">If <see cref="useFadeEffect"/> is <see
        /// langword="true"/>, the typewriter effect will run after the fade-in
        /// is complete.</para>
        /// </remarks>
        /// <seealso cref="lineText"/>
        /// <seealso cref="onCharacterTyped"/>
        /// <seealso cref="typewriterEffectSpeed"/>
        [SerializeField]
        internal bool useTypewriterEffect = false;

        /// <summary>
        /// A Unity Event that is called each time a character is revealed
        /// during a typewriter effect.
        /// </summary>
        /// <remarks>
        /// This event is only invoked when <see cref="useTypewriterEffect"/> is
        /// <see langword="true"/>.
        /// </remarks>
        /// <seealso cref="useTypewriterEffect"/>
        [SerializeField]
        internal UnityEngine.Events.UnityEvent onCharacterTyped;

        /// <summary>
        /// The number of characters per second that should appear during a
        /// typewriter effect.
        /// </summary>
        /// <seealso cref="useTypewriterEffect"/>
        [SerializeField]
        [Min(0)]
        internal float typewriterEffectSpeed = 0f;

        /// <summary>
        /// The game object that represents an on-screen button that the user
        /// can click to continue to the next piece of dialogue.
        /// </summary>
        /// <remarks>
        /// <para>This game object will be made inactive when a line begins
        /// appearing, and active when the line has finished appearing.</para>
        /// <para>
        /// This field will generally refer to an object that has a <see
        /// cref="Button"/> component on it that, when clicked, calls <see
        /// cref="OnContinueClicked"/>. However, if your game requires specific
        /// UI needs, you can provide any object you need.</para>
        /// </remarks>
        /// <seealso cref="autoAdvance"/>
    //    [SerializeField]
    //    internal GameObject continueButton = null;

        /// <summary>
        /// The amount of time to wait after any line
        /// </summary>
        [SerializeField]
        [Min(0)]
        internal float holdTime = 1f;

        /// <summary>
        /// Controls whether this Line View will wait for user input before
        /// indicating that it has finished presenting a line.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this value is true, the Line View will not report that it has
        /// finished presenting its lines. Instead, it will wait until the <see
        /// cref="UserRequestedViewAdvancement"/> method is called.
        /// </para>
        /// <para style="note"><para>The <see cref="DialogueRunner"/> will not
        /// proceed to the next piece of content (e.g. the next line, or the
        /// next options) until all Dialogue Views have reported that they have
        /// finished presenting their lines. If a <see cref="LineView"/> doesn't
        /// report that it's finished until it receives input, the <see
        /// cref="DialogueRunner"/> will end up pausing.</para>
        /// <para>
        /// This is useful for games in which you want the player to be able to
        /// read lines of dialogue at their own pace, and give them control over
        /// when to advance to the next line.</para></para>
        /// </remarks>
        [SerializeField]
        internal bool autoAdvance = false;
        
        /// <summary>
        /// The current <see cref="LocalizedLine"/> that this line view is
        /// displaying.
        /// </summary>
        LocalizedLine currentLine = null;

        /// <summary>
        /// A stop token that is used to interrupt the current animation.
        /// </summary>
        YarnUITKEffects.CoroutineInterruptToken currentStopToken = new YarnUITKEffects.CoroutineInterruptToken();


        internal bool haveSelectedableOptions = false;

        private void Awake()
        {

            UIDocument document = GetComponent<UIDocument>();
            documentRoot = document.rootVisualElement;
            lineText = documentRoot.Q<TypewriterLabel>(name: "line-text");
            characterNameText = documentRoot.Q<Label>(name: "name-label");
            continueButton = documentRoot.Q<Button>(name: "continue");

            contentWrapper = documentRoot.Q<VisualElement>(name: "yarn-content-wrappr");

            characterImage = documentRoot.Q<VisualElement>(name: "character-image");

            optionsWrapper = documentRoot.Q<VisualElement>(name: "options-wrapper"); 
            //optionsWrapper.style.display = DisplayStyle.None; 

            continueButton.clicked += () => OnContinueClicked();


        }

/*
        private void Reset()
        {
//            canvasGroup = GetComponentInParent<CanvasGroup>();
        }
*/
        /// <inheritdoc/>
        public override void DismissLine(Action onDismissalComplete)
        {
            currentLine = null;

            StartCoroutine(DismissLineInternal(onDismissalComplete));
        }

        private IEnumerator DismissLineInternal(Action onDismissalComplete)
        {
            // disabling interaction temporarily while dismissing the line
            // we don't want people to interrupt a dismissal
//            var interactable = canvasGroup.interactable;
//            canvasGroup.interactable = false;

            // If we're using a fade effect, run it, and wait for it to finish.
            if (useFadeEffect)
            {
                //yield return StartCoroutine(Effects.FadeAlpha(canvasGroup, 1, 0, fadeOutTime, currentStopToken));

                yield return StartCoroutine(YarnUITKEffects.FadeAlpha(contentWrapper, 1, 0, fadeOutTime, currentStopToken));
                //yield return null;
                currentStopToken.Complete();
            }
            
//            canvasGroup.alpha = 0;
//            canvasGroup.blocksRaycasts = false;
            // turning interaction back on, if it needs it
//            canvasGroup.interactable = interactable;
            onDismissalComplete();
        }

        /// <inheritdoc/>
        public override void InterruptLine(LocalizedLine dialogueLine, Action onInterruptLineFinished)
        {
            

            currentLine = dialogueLine;

            // Cancel all coroutines that we're currently running. This will
            // stop the RunLineInternal coroutine, if it's running.
            StopAllCoroutines();
            
            // for now we are going to just immediately show everything
            // later we will make it fade in

            int length;

            if (characterNameText == null)
            {
                if (showCharacterNameInLineView)
                {
                    lineText.Text = dialogueLine.Text.Text;
                    length = dialogueLine.Text.Text.Length;
                }
                else
                {
                    lineText.Text = dialogueLine.TextWithoutCharacterName.Text;
                    length = dialogueLine.TextWithoutCharacterName.Text.Length;
                }
            }
            else
            {
                characterNameText.text = dialogueLine.CharacterName;
                lineText.Text = dialogueLine.TextWithoutCharacterName.Text;
                length = dialogueLine.TextWithoutCharacterName.Text.Length;
            }

            // Show the entire line's text immediately.
            lineText.MaxVisibleCharacters = length;


            onInterruptLineFinished();
        }

        /// <inheritdoc/>
        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            // Stop any coroutines currently running on this line view (for
            // example, any other RunLine that might be running)
            StopAllCoroutines();

            // Begin running the line as a coroutine.
            StartCoroutine(RunLineInternal(dialogueLine, onDialogueLineFinished));
        }

        private IEnumerator RunLineInternal(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            IEnumerator PresentLine()
            {
            //    lineText.gameObject.SetActive(true);
//                canvasGroup.gameObject.SetActive(true);

                // Hide the continue button until presentation is complete (if
                // we have one).
                if (continueButton != null)
                {
                    //continueButton.style.display = DisplayStyle.None;// SetActive(false);
                    continueButton.style.visibility = Visibility.Hidden;
                }

                if (characterNameText != null)
                {
                    // If we have a character name text view, show the character
                    // name in it, and show the rest of the text in our main
                    // text view.
                    characterNameText.text = dialogueLine.CharacterName;
                    lineText.Text = dialogueLine.TextWithoutCharacterName.Text;
                }
                else
                {
                    // We don't have a character name text view. Should we show
                    // the character name in the main text view?
                    if (showCharacterNameInLineView)
                    {
                        // Yep! Show the entire text.
                        lineText.Text = dialogueLine.Text.Text;
                    }
                    else
                    {
                        // Nope! Show just the text without the character name.
                        lineText.Text = dialogueLine.TextWithoutCharacterName.Text;
                    }
                }

                if (useTypewriterEffect)
                {
                    // If we're using the typewriter effect, hide all of the
                    // text before we begin any possible fade (so we don't fade
                    // in on visible text).
                    lineText.MaxVisibleCharacters = 0;
                }
                else
                {
                    // Ensure that the max visible characters is effectively
                    // unlimited.
                    lineText.MaxVisibleCharacters = int.MaxValue;
                }

                // If we're using the fade effect, start it, and wait for it to
                // finish.
                if (useFadeEffect)
                {
//                    yield return StartCoroutine(Effects.FadeAlpha(canvasGroup, 0, 1, fadeInTime, currentStopToken));
                    yield return StartCoroutine( YarnUITKEffects.FadeAlpha(contentWrapper, 0, 1, fadeInTime, currentStopToken));
                    if (currentStopToken.WasInterrupted) {
                        // The fade effect was interrupted. Stop this entire
                        // coroutine.
                        yield break;
                    }
                }

                // If we're using the typewriter effect, start it, and wait for
                // it to finish.
                if (useTypewriterEffect)
                {
                    // setting the canvas all back to its defaults because if we didn't also fade we don't have anything visible
//                    canvasGroup.alpha = 1f;
//                    canvasGroup.interactable = true;
//                    canvasGroup.blocksRaycasts = true;
                    contentWrapper.style.opacity = 1f;


                    
                    /*
                    yield return StartCoroutine(
                        Effects.Typewriter(
                            lineText,
                            typewriterEffectSpeed,
                            () => onCharacterTyped.Invoke(),
                            currentStopToken
                        )
                    );
                    */

                    yield return StartCoroutine(
                        YarnUITKEffects.Typewriter(
                            lineText,
                            typewriterEffectSpeed,
                            () => onCharacterTyped.Invoke(),
                            currentStopToken
                        )
                    );

                    if (currentStopToken.WasInterrupted) {
                        // The typewriter effect was interrupted. Stop this
                        // entire coroutine.
                        yield break;
                    }
                }
            }
            currentLine = dialogueLine;

            // Run any presentations as a single coroutine. If this is stopped,
            // which UserRequestedViewAdvancement can do, then we will stop all
            // of the animations at once.
            yield return StartCoroutine(PresentLine());

            currentStopToken.Complete();

            // All of our text should now be visible.
            lineText.MaxVisibleCharacters = int.MaxValue;

            // Our view should at be at full opacity.
//            canvasGroup.alpha = 1f;
//           canvasGroup.blocksRaycasts = true;
            contentWrapper.style.opacity = 1f;

            // Show the continue button, if we have one.
            if (continueButton != null)
            {
                //continueButton.SetActive(true);
                //continueButton.style.display = DisplayStyle.Flex;
                continueButton.style.visibility = Visibility.Visible;
            }

            // If we have a hold time, wait that amount of time, and then
            // continue.
            if (holdTime > 0)
            {
                yield return new WaitForSeconds(holdTime);
            }

            if (autoAdvance == false)
            {
                // The line is now fully visible, and we've been asked to not
                // auto-advance to the next line. Stop here, and don't call the
                // completion handler - we'll wait for a call to
                // UserRequestedViewAdvancement, which will interrupt this
                // coroutine.
                yield break;
            }

            // Our presentation is complete; call the completion handler.
            onDialogueLineFinished();
        }

        /// <inheritdoc/>
        public override void UserRequestedViewAdvancement()
        {
            // We received a request to advance the view. If we're in the middle of
            // an animation, skip to the end of it. If we're not current in an
            // animation, interrupt the line so we can skip to the next one.

            // we have no line, so the user just mashed randomly
            if (currentLine == null)
            {
                return;
            }


            

            // we may want to change this later so the interrupted
            // animation coroutine is what actually interrupts
            // for now this is fine.
            // Is an animation running that we can stop?
            if (currentStopToken.CanInterrupt) 
            {
                // Stop the current animation, and skip to the end of whatever
                // started it.
                currentStopToken.Interrupt();
            }
            // No animation is now running. Signal that we want to
            // interrupt the line instead.

            //out "bounce" the dialog box
            BounceOutContent();
            

            requestInterrupt?.Invoke();
        }

        /// <summary>
        /// Called when the <see cref="continueButton"/> is clicked.
        /// </summary>
        public void OnContinueClicked()
        {
            // When the Continue button is clicked, we'll do the same thing as
            // if we'd received a signal from any other part of the game (for
            // example, if a DialogueAdvanceInput had signalled us.)
            UserRequestedViewAdvancement();
        }


        public void BounceOutContent(){

            var bounceIn = contentWrapper.experimental.animation
                 .Start( 1.03f , 1f , 200 , (ve,value) => ve.transform.scale = (Vector3.one * value) )
                 .Ease( UnityEngine.UIElements.Experimental.Easing.OutCubic )
                 .KeepAlive();
            bounceIn.Stop(); 

            var bounceOut = contentWrapper.experimental.animation
                 .Start( 1f , 1.03f , 100 , (ve,value) => ve.transform.scale = (Vector3.one * value ) )
                 .Ease( UnityEngine.UIElements.Experimental.Easing.InCubic )
                 .KeepAlive();

/// Weirdly - this doesn't work... just seems to be a wrapper for doing the above though.. 

/*            
            contentWrapper.style.scale = new Scale( Vector3.one );

            var bounceOut = contentWrapper.experimental.animation
                .Scale(1.5f, 300)
                .Ease( UnityEngine.UIElements.Experimental.Easing.InOutQuad ) // OutBounce InOutQuad
                .KeepAlive(); 


            var bounceIn = contentWrapper.experimental.animation
                .Scale(1.00f, 300)
                .Ease( UnityEngine.UIElements.Experimental.Easing.InOutQuad )
                .KeepAlive();

 */

            bounceOut.OnCompleted( ()=> bounceIn.Start() );
            bounceIn.OnCompleted( ()=> contentWrapper.transform.scale = Vector3.one );

            bounceOut.Start();

        }


        protected void clearOptionClickEvents(){
            foreach (TemplateContainer optionView in optionViews)
            {               
                Button button = optionView.Q<Button>(className: "option-button");
                button.clickable = null;

            }
        }


        //I want a differnt layout for when there are 2 options (my most common use case)
        protected void SetOptionsContainerClasses(int optionsCount){
            string optionTwoClassname = "options-2";
            if( optionsCount == 2)
            {
                if( !optionsWrapper.ClassListContains( optionTwoClassname )  )
                {
                    optionsWrapper.AddToClassList( optionTwoClassname );
                }
            }else{
                if( optionsWrapper.ClassListContains( optionTwoClassname )  )
                {
                    optionsWrapper.RemoveFromClassList( optionTwoClassname );
                }
            }
        }



        public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
        {
            // Hide all existing option views

            //optionsWrapper.style.display = DisplayStyle.Flex; // .style.display = value ? DisplayStyle.Flex 
            //add the "showing-options" class
            contentWrapper.AddToClassList(showingOptionsClassName);

            if( dialogueOptions.Length > 0)
            {
                haveSelectedableOptions = true;
            }

            SetOptionsContainerClasses( dialogueOptions.Length );

            foreach (TemplateContainer optionView in optionViews)
            {
               //optionView.gameObject.SetActive(false);
                optionView.style.display = DisplayStyle.None;
            }

            // If we don't already have enough option views, create more
            int childnum = 1;
            while (dialogueOptions.Length > optionViews.Count)
            {
                TemplateContainer optionView = optionListItemTemplate.Instantiate(); //doing this inline, 

                optionView.AddToClassList( "child-" + childnum ); // allows styling option children differently (padding / layout / margin)
                optionView.style.display = DisplayStyle.None;
                optionsWrapper.Add( optionView );

                Button button = optionView.Q<Button>(className: "option-button");//option-selection"); option-button
                button.clickable.clickedWithEventInfo += OptionViewWasSelected_WithEvent;


                optionViews.Add( optionView );
                childnum++;

            }

            // Set up all of the option views
            int optionViewsCreated = 0;

            for (int i = 0; i < dialogueOptions.Length; i++)
            {
                TemplateContainer optionView = optionViews[i];
                var option = dialogueOptions[i];

                if (option.IsAvailable == false && showUnavailableOptions == false)
                {
                    // Don't show this option.
                    continue;
                }

                //optionView.gameObject.SetActive(true);
                optionView.style.display = DisplayStyle.Flex;

                Button button = optionView.Q<Button>(className: "option-button");

                //TODO - there must be a good way to relate this button to an option... someone? 
                // obvous a custom button element that can store an option ( like optionView.Option = option; )
                // all we need is the index of the dialogueOptions so for now I'm setting it as the name - I know, I know!
                // storing the dialog id in the name .... uurrggh.. back to 1999 js!
                // Please send a PR with a better option :)
                button.name = "o_" + option.DialogueOptionID; 

                // you could use the below, assiging the clicked option and then "null"-fying it later (see clearOptionClickEvents) 
                // 
///                button.clicked += () => OptionViewWasSelected( option );
                

                
                Label optionViewText = optionView.Q<Label>(name: "option-label");
                optionViewText.text = option.Line.TextWithoutCharacterName.Text;


                //optionView.Option = option;


                // if you change to use radio buttons or something like that, this will be useful
                // commenting it out for now
/*
                // The first available option is selected by default
                if (optionViewsCreated == 0)
                {
                    optionView.Select();
                }
*/
                optionViewsCreated += 1;
            }

            //not using last line

/*
            // Update the last line, if one is configured
            if (lastLineText != null)
            {
                if (lastSeenLine != null) {
                    lastLineText.gameObject.SetActive(true);
                    lastLineText.text = lastSeenLine.Text.Text;
                } else {
                    lastLineText.gameObject.SetActive(false);
                }
            }
*/
            // Note the delegate to call when an option is selected
            OnOptionSelected = onOptionSelected;

            // Fade it all in
            if( useFadeEffect ){

                StartCoroutine(YarnUITKEffects.FadeAlpha( contentWrapper, 0, 1, fadeTime));

            }

/*
            /// <summary>
            /// Creates and configures a new <see cref="OptionView"/>, and adds
            /// it to <see cref="optionViews"/>.
            /// </summary>
            TemplateContainer CreateNewOptionView()
            {

                var optionView = Instantiate(optionViewPrefab);
                optionView.transform.SetParent(transform, false);
                optionView.transform.SetAsLastSibling();

                optionView.OnOptionSelected = OptionViewWasSelected;
                optionViews.Add(optionView);

                TemplateContainer optionView = optionListItemTemplate.Instantiate(); 
                optionViews.Add(optionView);

                return optionView;
            }
*/
            
/*
            /// <summary>
            /// Called by <see cref="OptionView"/> objects.
            /// </summary>
            void OptionViewWasSelected(DialogueOption option)
            {
                contentWrapper.RemoveFromClassList(showingOptionsClassName);
                clearOptionClickEvents();
                Debug.Log( "OptionViewWasSelected [" + haveSelectedableOptions + "] (" + option.DialogueOptionID + ") " + option.Line.TextWithoutCharacterName.Text );
               // OnOptionSelected(option.DialogueOptionID);
                
                StartCoroutine(OptionViewWasSelectedInternal(option));

                IEnumerator OptionViewWasSelectedInternal(DialogueOption selectedOption)
                {
                    Debug.Log( "Starting OptionViewWasSelectedInternal " + haveSelectedableOptions );
                    yield return StartCoroutine(YarnUITKEffects.FadeAlpha(contentWrapper, 1, 0, fadeTime));  
                    if( haveSelectedableOptions ){
                        OnOptionSelected(selectedOption.DialogueOptionID); // SelectedOption - creates an error... :(
                        haveSelectedableOptions = false;
                    }
                }
        
            }
*/

            void OptionViewWasSelected_WithEvent( EventBase evt ){

                contentWrapper.RemoveFromClassList(showingOptionsClassName);

                //get the index of the clicked button - the button is wrapped in a "TemplateContainer"
                int selectedID = -1;

                Button button = (Button)evt.target;
                Int32.TryParse( button.name.Substring(2) , out selectedID); // name is o_{NUM} - 

                // a fallback implementation, not keen on either of these but leaving them in
                if( selectedID < 0){

                    TemplateContainer buttonParent = (TemplateContainer)button.parent;

                    for(int i=0; i < optionsWrapper.childCount ; i++){
                        TemplateContainer element = (TemplateContainer)optionsWrapper.ElementAt(i);
                        if( element == buttonParent){
                            selectedID = i;
                            break;
                        }
                        
                    }
                }

                if( selectedID > -1){

                    StartCoroutine(OptionViewWasSelectedInternalEV(selectedID));
                }else{

                    StartCoroutine(OptionViewWasSelectedInternalEV(0));

                }

                IEnumerator OptionViewWasSelectedInternalEV(int selectedOptionID)
                {
                    if(useFadeEffect){

                        yield return StartCoroutine(YarnUITKEffects.FadeAlpha(contentWrapper, 1, 0, fadeTime));
                    }

                    if( haveSelectedableOptions ){
                        OnOptionSelected(selectedOptionID); 
                        haveSelectedableOptions = false;
                    }
                    
                }

            }
        }


    }

