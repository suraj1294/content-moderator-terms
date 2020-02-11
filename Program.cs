using Microsoft.Azure.CognitiveServices.ContentModerator;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;


namespace content_moderator_terms
{
    class Program
    {
        private static readonly int throttleRate = 3000;
        // TEXT MODERATION
        // Name of the file that contains text
        private static readonly string TextFile = "TextFile.txt";
        // The name of the file to contain the output from the evaluation.
        private static string TextOutputFile = "TextModerationOutput.txt";
        static void Main(string[] args)
        {
            using (var client = Clients.NewClient())
            {
                string list_id = Termsmanagement.CreateTermList(client);

                Termsmanagement.UpdateTermList(client, list_id, "name", "description");
                Termsmanagement.AddTerm(client, list_id, "term1");
                Termsmanagement.AddTerm(client, list_id, "term2");

                Termsmanagement.GetAllTerms(client, list_id);

                // Always remember to refresh the search index of your list
                Termsmanagement.RefreshSearchIndex(client, list_id);

                string text = "This text contains the terms \"term1\" and \"term2\".";
                ScreenText(client, list_id, text);

                Termsmanagement.DeleteTerm(client, list_id, "term1");

                // Always remember to refresh the search index of your list
                Termsmanagement.RefreshSearchIndex(client, list_id);

                text = "This text contains the terms \"term1\" and \"term2\".";
                ScreenText(client, list_id, text);

                Termsmanagement.DeleteAllTerms(client, list_id);
                Termsmanagement.DeleteTermList(client, list_id);

                Console.WriteLine("Press ENTER to close the application.");
                Console.ReadLine();
            }
        }
        /// <summary>
        /// Screen the indicated text for terms in the indicated term list.
        /// </summary>
        /// <param name="client">The Content Moderator client.</param>
        /// <param name="list_id">The ID of the term list to use to screen the text.</param>
        /// <param name="text">The text to screen.</param>
        static void ScreenText(ContentModeratorClient client, string list_id, string text)
        {
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("TEXT MODERATION");
            Console.WriteLine();


            // Remove carriage returns
            text = text.Replace(Environment.NewLine, " ");
            // Convert string to a byte[], then into a stream (for parameter in ScreenText()).
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            MemoryStream stream = new MemoryStream(textBytes);


            Console.WriteLine("Screening text: \"{0}\" using term list with ID {1}.", text, list_id);
            Screen screen = client.TextModeration.ScreenText("text/plain", stream, "eng", false, false, list_id, false);
            if (null == screen.Terms)
            {
                Console.WriteLine("No terms from the term list were detected in the text.");
            }
            else
            {
                foreach (DetectedTerms term in screen.Terms)
                {
                    Console.WriteLine(String.Format("Found term: \"{0}\" from list ID {1} at index {2}.", term.Term, term.ListId, term.Index));
                }
            }
            Thread.Sleep(throttleRate);
        }


    }




}
