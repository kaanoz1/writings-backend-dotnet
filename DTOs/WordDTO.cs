using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using writings_backend_dotnet.Models;

namespace writings_backend_dotnet.DTOs
{
    public class WordSimpleDTO
    {
        public required short SequenceNumber { get; set; }
        public required string Text { get; set; }
        public string? TextNoVowel { get; set; }
        public string? TextSimplified { get; set; }
        public RootSimpleDTO? Root { get; set; }
    }

    public class WordRootDTO
    {
        public required short SequenceNumber { get; set; }
        public required string Text { get; set; }
        public string? TextNoVowel { get; set; }
        public string? TextSimplified { get; set; }
        public VerseSimpleDTO Verse { get; set; } = null!;
    }


    public static class WordExtensions
    {
        public static WordSimpleDTO ToWordSimpleDTO(this Word word)
        {
            return new WordSimpleDTO
            {
                SequenceNumber = word.SequenceNumber,
                Text = word.Text,
                TextNoVowel = word.TextNoVowel,
                TextSimplified = word.TextSimplified,
                Root = word.Root?.ToRootSimpleDTO() ?? null
            };
        }

        public static WordRootDTO ToWordRootDTO(this Word word)
        {
            return new WordRootDTO
            {
                SequenceNumber = word.SequenceNumber,
                Text = word.Text,
                TextNoVowel = word.TextNoVowel,
                TextSimplified = word.TextSimplified,
                Verse = word.Verse.ToVerseSimpleDTO()
            };
        }
    }
}