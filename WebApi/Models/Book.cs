using MARC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class Book
    {
        /// <summary>
        /// 200 a. Título próprio
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 200 f. Primeiro responsável
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 210 c. Nome do editor
        /// </summary>
        public string Editor { get; set; }

        /// <summary>
        /// Tipo - etqr
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// 930 d. Quota
        /// </summary>
        public string Quota { get; set; }

        /// <summary>
        /// Takes a record and build a book with it
        /// </summary>
        /// <param name="r">The record</param>
        /// <returns>The book</returns>
        public static Book Parse(Record record) 
        {
            Book book = new Book();

            DataField df200 = record.GetDataFieldByTag("200");

            // this datafield is required, since the book title is in it.
            if (df200 == null) 
            {
                return null;
            }

            book.Title = df200.GetSubfieldOrEmpty(MarcSubfields.Title);
            book.Author = df200.GetSubfieldOrEmpty(MarcSubfields.Author);

            DataField df210 = record.GetDataFieldByTag("210");
            book.Editor = df210.GetSubfieldOrEmpty(MarcSubfields.Editor);

            DataField df930 = record.GetDataFieldByTag("930");
            book.Quota = df930.GetSubfieldOrEmpty(MarcSubfields.Quota);

            return book;
        }
    }

    public static class MarcSubfields
    {
        public const char Title = 'a';
        public const char Author = 'f';
        public const char Editor = 'c';
        public const char Quota = 'd';
    }
}