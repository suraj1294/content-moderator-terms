using System;
using System.Threading;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;

namespace content_moderator_terms
{
    public static class Termsmanagement
    {
        /// <summary>
        /// The minimum amount of time, in milliseconds, to wait between calls
        /// to the Content Moderator APIs.
        /// </summary>
        private const int throttleRate = 3000;
        /// <summary>
        /// The number of minutes to delay after updating the search index before
        /// performing image match operations against the list.
        /// </summary>
        private const double latencyDelay = 0.5;
        /// <summary>
        /// The language of the terms in the term lists.
        /// </summary>
        private const string lang = "eng";


        /// <summary>
        /// Creates a new term list.
        /// </summary>
        /// <param name="client">The Content Moderator client.</param>
        /// <returns>The term list ID.</returns>
        public static string CreateTermList(ContentModeratorClient client)
        {
            Console.WriteLine("Creating term list.");

            Body body = new Body("Term list name", "Term list description");
            TermList list = client.ListManagementTermLists.Create("application/json", body);
            if (false == list.Id.HasValue)
            {
                throw new Exception("TermList.Id value missing.");
            }
            else
            {
                string list_id = list.Id.Value.ToString();
                Console.WriteLine("Term list created. ID: {0}.", list_id);
                Thread.Sleep(throttleRate);
                return list_id;
            }
        }

        /// <summary>
        /// Add a term to the indicated term list.
        /// </summary>
        /// <param name="client">The Content Moderator client.</param>
        /// <param name="list_id">The ID of the term list to update.</param>
        /// <param name="term">The term to add to the term list.</param>
        public static void AddTerm(ContentModeratorClient client, string list_id, string term)
        {
            Console.WriteLine("Adding term \"{0}\" to term list with ID {1}.", term, list_id);
            client.ListManagementTerm.AddTerm(list_id, term, lang);
            Thread.Sleep(throttleRate);
        }

        /// <summary>
        /// Get all terms in the indicated term list.
        /// </summary>
        /// <param name="client">The Content Moderator client.</param>
        /// <param name="list_id">The ID of the term list from which to get all terms.</param>
        public static void GetAllTerms(ContentModeratorClient client, string list_id)
        {
            Console.WriteLine("Getting terms in term list with ID {0}.", list_id);
            Terms terms = client.ListManagementTerm.GetAllTerms(list_id, lang);
            TermsData data = terms.Data;
            foreach (TermsInList term in data.Terms)
            {
                Console.WriteLine(term.Term);
            }
            Thread.Sleep(throttleRate);
        }

        /// <summary>
        /// Update the information for the indicated term list.
        /// </summary>
        /// <param name="client">The Content Moderator client.</param>
        /// <param name="list_id">The ID of the term list to update.</param>
        /// <param name="name">The new name for the term list.</param>
        /// <param name="description">The new description for the term list.</param>
        public static void UpdateTermList(ContentModeratorClient client, string list_id, string name = null, string description = null)
        {
            Console.WriteLine("Updating information for term list with ID {0}.", list_id);
            Body body = new Body(name, description);
            client.ListManagementTermLists.Update(list_id, "application/json", body);
            Thread.Sleep(throttleRate);
        }

        /// <summary>
        /// Refresh the search index for the indicated term list.
        /// </summary>
        /// <param name="client">The Content Moderator client.</param>
        /// <param name="list_id">The ID of the term list to refresh.</param>
        public static void RefreshSearchIndex(ContentModeratorClient client, string list_id)
        {
            Console.WriteLine("Refreshing search index for term list with ID {0}.", list_id);
            client.ListManagementTermLists.RefreshIndexMethod(list_id, lang);
            Thread.Sleep((int)(latencyDelay * 60 * 1000));
        }

        /// <summary>
        /// Delete a term from the indicated term list.
        /// </summary>
        /// <param name="client">The Content Moderator client.</param>
        /// <param name="list_id">The ID of the term list from which to delete the term.</param>
        /// <param name="term">The term to delete.</param>
        public static void DeleteTerm(ContentModeratorClient client, string list_id, string term)
        {
            Console.WriteLine("Removed term \"{0}\" from term list with ID {1}.", term, list_id);
            client.ListManagementTerm.DeleteTerm(list_id, term, lang);
            Thread.Sleep(throttleRate);
        }

        /// <summary>
        /// Delete all terms from the indicated term list.
        /// </summary>
        /// <param name="client">The Content Moderator client.</param>
        /// <param name="list_id">The ID of the term list from which to delete all terms.</param>
        public static void DeleteAllTerms(ContentModeratorClient client, string list_id)
        {
            Console.WriteLine("Removing all terms from term list with ID {0}.", list_id);
            client.ListManagementTerm.DeleteAllTerms(list_id, lang);
            Thread.Sleep(throttleRate);
        }

        /// <summary>
        /// Delete the indicated term list.
        /// </summary>
        /// <param name="client">The Content Moderator client.</param>
        /// <param name="list_id">The ID of the term list to delete.</param>
        public static void DeleteTermList(ContentModeratorClient client, string list_id)
        {
            Console.WriteLine("Deleting term list with ID {0}.", list_id);
            client.ListManagementTermLists.Delete(list_id);
            Thread.Sleep(throttleRate);
        }

    }
}