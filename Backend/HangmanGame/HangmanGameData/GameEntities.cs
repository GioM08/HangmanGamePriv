using System;
using System.Data.Linq.Mapping;

namespace HangmanGameData
{
    [Table(Name = "dbo.Categories")]
    public class Categories
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int CategoryId { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public string LanguageCode { get; set; }
    }

    [Table(Name = "dbo.Words")]
    public class Words
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int WordId { get; set; }

        [Column]
        public int CategoryId { get; set; }

        [Column]
        public string Text { get; set; }

        [Column(CanBeNull = true)]
        public string Hint { get; set; }

        [Column]
        public string LanguageCode { get; set; }

        [Column]
        public int Difficulty { get; set; }
    }

    [Table(Name = "dbo.Games")]
    public class Games
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int GameId { get; set; }

        [Column]
        public int CreatorId { get; set; }

        [Column(CanBeNull = true)]
        public int? RetadorId { get; set; }

        [Column]
        public int WordId { get; set; }

        [Column]
        public int Status { get; set; }

        [Column(CanBeNull = true)]
        public string Description { get; set; }

        [Column]
        public DateTime CreatedAt { get; set; }

        [Column(CanBeNull = true)]
        public DateTime? StartedAt { get; set; }

        [Column(CanBeNull = true)]
        public DateTime? FinishedAt { get; set; }

        [Column(CanBeNull = true)]
        public int? WinnerId { get; set; }

        [Column(CanBeNull = true)]
        public int? AbandonedByUserId { get; set; }
    }

    [Table(Name = "dbo.GameMoves")]
    public class GameMoves
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int MoveId { get; set; }

        [Column]
        public int GameId { get; set; }

        [Column]
        public int UserId { get; set; }

        [Column]
        public string Letter { get; set; }

        [Column]
        public bool IsCorrect { get; set; }

        [Column]
        public DateTime MoveDate { get; set; }
    }
}
