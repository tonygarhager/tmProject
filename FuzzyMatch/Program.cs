using Sdl.LanguagePlatform.TranslationMemory;
using System.Web.Script.Serialization;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace concordance_search
{
    class JsonResult
    {
        public int Match { get; set; }
        public string sourceTU { get; set; }
        public string targetTU { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //search_first(args);
            Console.OutputEncoding = Encoding.UTF8;
            searchFuzzy(args);
        }

        public static void concordance_search(string[] args)
        {
            FileBasedTMHelper fileBasedHelper = new FileBasedTMHelper();

            string tmPath = args[0];
            bool target = bool.Parse(args[1]); //false;// 
            SearchResults results = fileBasedHelper.ConcordanceSearch(tmPath, target, args[2], args[3], args[4]);

            List<JsonResult> jsonResults = new List<JsonResult>();
            foreach (SearchResult result in results)
            {
                JsonResult jsonResult = new JsonResult();
                jsonResult.Match = result.ScoringResult.Match;
                jsonResult.sourceTU = result.MemoryTranslationUnit.SourceSegment.ToPlain();
                jsonResult.targetTU = result.MemoryTranslationUnit.TargetSegment.ToPlain();
                jsonResults.Add(jsonResult);
            }
            
            string jsonString = new JavaScriptSerializer().Serialize(jsonResults);
            Console.Write(jsonString);

        }

        public static void searchFuzzy(string[] args)
        {
            FileBasedTMHelper fileBasedHelper = new FileBasedTMHelper();
            string tmPath = args[0]; //"E:\\2.sdltm"; //
            string query = args[1]; //query;// 

            SearchResults results = fileBasedHelper.FuzzySearch(tmPath, query, args[2], args[3]);
            List<JsonResult> jsonResults = new List<JsonResult>();
            foreach (SearchResult result in results)
            {
                JsonResult jsonResult = new JsonResult();
                jsonResult.Match = result.ScoringResult.Match;
                jsonResult.sourceTU = result.MemoryTranslationUnit.SourceSegment.ToPlain();
                jsonResult.targetTU = result.MemoryTranslationUnit.TargetSegment.ToPlain();
                jsonResults.Add(jsonResult);
            }
            string jsonString = new JavaScriptSerializer().Serialize(jsonResults);
            Console.Write(jsonString);
        }


        private static void UpdateResult(string v)
        {

        }
        /// <summary>
        /// This function returns further information on the given translation unit (TU).
        /// </summary>
        private static string GetTuInformation(SearchResult tuResult)
        {
            string tuInfo;

            /// The matching score
            tuInfo = "\nScore: " + tuResult.ScoringResult.Match.ToString() + "%\n";

            /// The source and target segments
            tuInfo += "Source: " + tuResult.MemoryTranslationUnit.SourceSegment.ToPlain() + "\n";
            tuInfo += "Target: " + tuResult.MemoryTranslationUnit.TargetSegment.ToPlain() + "\n";
            /*
            #region "date"
            /// The TU creation date.
            tuInfo += "Creation date: " + tuResult.MemoryTranslationUnit.SystemFields.CreationDate + "\n";
            #endregion

            #region "fields"
            /// Any field values (e.g. Customer, Project id, etc. associated with the
            /// given TU.
            foreach (FieldValue field in tuResult.MemoryTranslationUnit.FieldValues)
            {
                tuInfo += field.Name + ": " + field.ToString();
            }
            #endregion

            */
            tuInfo += "\n";

            return tuInfo;
        }

    }
}
