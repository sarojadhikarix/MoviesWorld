﻿using DataServiceLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataServiceLib.DataServices
{
    public class SearchDS : ISearchDS
    {
        private readonly MoviesContext ctx = new MoviesContext();
        public IList<StringSearch> Search(string keyword)
        {
            return ctx.Search(keyword);
        }

        public IList<StringSearch> ExactSearch(string firstKeyword, String secondKeyword)
        {
            return ctx.ExactSearch(firstKeyword, secondKeyword);
        }

        public IList<BestSearch> BestSearch(string firstKeyword, String secondKeyword, string thirdKeyword)
        {
            return ctx.BestSearch(firstKeyword, secondKeyword, thirdKeyword);
        }
        public IList<SearchHistory>  SearchHistory(string userName)
        {
            return ctx.searchHistories.Where(s => s.UserName == userName).ToList();
        }
    }
}
