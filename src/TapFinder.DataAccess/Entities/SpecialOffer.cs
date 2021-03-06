﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TapFinder.DataAccess.Entities
{
    [Table("SpecialOffers")]
    public class SpecialOffer : LikeableItem
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }

        public string PlaceId { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
