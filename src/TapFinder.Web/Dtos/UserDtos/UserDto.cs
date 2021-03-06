﻿using System.Collections.Generic;

namespace TapFinder.Web.Dtos.UserDtos
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string ImagePath { get; set; }
        public RankDto Rank { get; set; }
        public IList<BadgeDto> Badges { get; set; }
        public int Experience { get; set; }
        public IList<string> FavouritePlaces { get; set; }
    }
}