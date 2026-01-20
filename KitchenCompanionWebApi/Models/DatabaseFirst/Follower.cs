using System;
using System.Collections.Generic;

namespace KitchenCompanionWebApi.Models.DatabaseFirst;

public partial class Follower
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int FollowerId { get; set; }
}
