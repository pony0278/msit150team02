﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjCatChaOnlineShop.Models;

public partial class GameAchievementList
{
    public int AchievementId { get; set; }

    public string AchievementName { get; set; }

    public int? AchievementRewardId { get; set; }

    public int? AchievementConditionId { get; set; }

    public virtual GameTaskConditionData AchievementCondition { get; set; }

    public virtual GameAchievementRewardList AchievementReward { get; set; }
}