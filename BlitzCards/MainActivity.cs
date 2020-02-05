using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using BlitzCards.Lang;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

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
        private const string NADWORNY_URL = "https://www.nadworny.com/websvc/presidents.txt";
        private const int COLUMNS = 15;

        // Collection of presidents structured as a 2d array
        string[,] presidents;
        
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
        }

        protected async override void OnStart()
        {
            base.OnStart();

            // refs to ui textviews
            var frontDescription = FindViewById<TextView>(Resource.Id.frontDescription);
            var backDescription = FindViewById<TextView>(Resource.Id.backDescription);

            await Task.Run(() =>
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                Stream dataStream = client.OpenRead(NADWORNY_URL);
                var reader = new StreamReader(dataStream);

                string data = reader.ReadToEnd();

                string[] chunks = data.Split('\t', '\n');

                //string[] finalChunks = chunks.TakeWhile(chunk => string.IsNullOrEmpty(chunk) == true).ToArray();

                string[] finalChunks = chunks.Take(675).ToArray();

                //int endingIndicesToRemove = 0;

                //for (int i = 0; i < chunks.Length; i++)
                //{
                //    if (string.IsNullOrWhiteSpace(chunks[i]))
                //    {
                //        for (int index = i; index < chunks.Length; index++)
                //        {
                //            if (!string.IsNullOrWhiteSpace(chunks[index]))
                //            {
                //                // if a string wasn't empty, null, spaces then reset
                //                endingIndicesToRemove = 0;
                //                break;
                //            }
                //            endingIndicesToRemove++;
                //        }

                //        // checking to see if we have indexes from this index to remove
                //        if (endingIndicesToRemove != 0)
                //        {
                //            IEnumerable<string> query = chunks.TakeWhile(chunk => string.IsNullOrWhiteSpace(chunk) != true);
                //            break;
                //        }
                //    }
                //}

                // Calucating the number of rows
                int rowNumbers = finalChunks.Length / COLUMNS;

                // Instantiating our declaration to an actual array
                presidents = new string[rowNumbers - 1, 2];

                // Variable used for offseting to get specific columns
                int presidentOffset = 15;

                // Iterates through the strings based off the number of rows
                for (int i = 1; i < rowNumbers; i++)
                {
                    if (i == 0)
                    {
                        // Sets the title text
                        RunOnUiThread(() =>
                        {
                            var test = finalChunks[presidentOffset];
                            var test2 = finalChunks[1 + presidentOffset];

                            frontDescription.Text = $"Front: {finalChunks[0]}";
                            backDescription.Text = $"Back: {finalChunks[1]}";
                        });
                    }
                    else
                    {
                        // Assigning the indexed data into our 2d array
                        presidents[i - 1, 0] = finalChunks[presidentOffset];
                        presidents[i - 1, 1] = finalChunks[1 + presidentOffset];
                        // Applying a offset for our next iteration, so we get different columns
                        presidentOffset += COLUMNS;
                    }
                }
            });
            // We divide by two because in our case our array is actually half as long as it says due to how we are using it
            VCardPos = new IndexInt(DEFAULT_VOCAB_POS, presidents.Length / 2 - 1);

            questionNum = FindViewById<TextView>(Resource.Id.questionNum);
            cardTextView = FindViewById<TextView>(Resource.Id.cardTextView);
            cardTextView.Text = presidents[VCardPos.Value, VCardPos.Value];

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
                    cardTextView.Text = presidents[VCardPos.Value, ACRONYM_INDEX];
                }
                else
                {
                    // show the answer
                    cardTextView.Text = presidents[VCardPos.Value, ACRONYM_DEFINITION_INDEX];
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
            cardTextView.Text = presidents[VCardPos.Value, ACRONYM_INDEX];
            isAwnserVisible = false;
        }
    }
}