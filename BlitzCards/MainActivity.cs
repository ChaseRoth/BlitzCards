using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using BlitzCards.Lang;

namespace BlitzCards
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        event System.Action NextCardEvent;
        private const string QUESTION_HEADER = "Question: ";
        private const byte ACRONYM_INDEX = 0;
        private const byte ACRONYM_DEFINITION_INDEX = 1;
        private const int DEFAULT_VOCAB_POS = 0;

        // Vocab & Definition
        readonly string[,] vocab = new string[,] {
            { "API (Application Programming Interface)", "Code that allows two software programs to communicate with each other." },
            { "HTTP (HyperText Transfer Protocol)", "Standard protocol for settting request and responses over the internet." },
            { "SSL (Secure Socket Protocol)", "Point to Point encryption." },
            { "SOAP (Simple Object Access Protocol)", " Provides simplified communications through proxies and firewalls. It is also used to package message request. " },
            { "REST (REpresentational State Transfer)", "A type of API that leverages HTTP/HTTPS exclusively to transfer hypermedia (hypertext and other formats) over the internet." },
            { "XML (eXtensible Markup Language)", "Use to encode all information passed between machines using the web service." },
            { "RSS (Rich Site Summary)", "A web feed that allows applications to get information about a aserver or service in a standardized format." },
            { "JSON (JavaScript Object Notation)", "A lightweight self-describing data-interchange format." },
            { "W3C (World Wide Web Consortium)", "An international community that develops standards for the web together." },
            { "Synchronous", "Client us 'blocked' until the server respondes or times out. Run in a linear fashion." },
            { "Asynchronous", "Server works in the background while waitingr for a responce." },
            { "Stateless", "No built-in transaction support, private (token) and supports load balancing Caching." },
            { "Stateful", "Remebers who you are between aapi calls. Harder to scale." }
        };

        // Index to manage position in vocab array
        private IndexInt VCardPos; 
        // Determines card visibility state
        bool isAwnserVisible;
        // References to views
        TextView cardTextView, questionNum;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // We divide by two because in our case our array is actually half as long as it says due to how we are using it
            VCardPos = new IndexInt(DEFAULT_VOCAB_POS, vocab.Length / 2 - 1);

            questionNum       = FindViewById<TextView>(Resource.Id.questionNum);
            cardTextView      = FindViewById<TextView>(Resource.Id.cardTextView);
            cardTextView.Text = vocab[VCardPos.Value, VCardPos.Value];

            NextCardEvent += UpdateQuestionHeader;
            NextCardEvent += UpdateVocabCard;            

            // Handles back button clicks
            FindViewById<ImageButton>(Resource.Id.backBtn).Click += delegate
            {
                VCardPos--;
                NextCardEvent?.Invoke();
            };

            // Handles next button clicks
            FindViewById<ImageButton>(Resource.Id.nextBtn).Click += delegate
            {
                VCardPos++;
                NextCardEvent?.Invoke();
            };

            // Handles flip button clicks
            FindViewById<ImageButton>(Resource.Id.flipBtn).Click += delegate
            {
                if (isAwnserVisible)
                {
                    // hide the answer
                    cardTextView.Text = vocab[VCardPos.Value, ACRONYM_INDEX];                    
                }
                else
                {
                    // show the answer
                    cardTextView.Text = vocab[VCardPos.Value, ACRONYM_DEFINITION_INDEX];
                }

                // inverts flag
                isAwnserVisible = !isAwnserVisible;
            };           
        }

        /// <summary>
        ///     Updates the question header text
        /// </summary>
        private void UpdateQuestionHeader()
        {
            questionNum.Text = QUESTION_HEADER + (VCardPos.Value + 1);
        }     
        
        /// <summary>
        ///     Updates the cards contents based off VcardPos (Position in array)
        /// </summary>
        private void UpdateVocabCard()
        {
            cardTextView.Text = vocab[VCardPos.Value, ACRONYM_INDEX];
            isAwnserVisible = false;
        }
    }
}