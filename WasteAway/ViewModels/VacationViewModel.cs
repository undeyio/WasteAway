﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WasteAway.Models;

namespace WasteAway.ViewModels
{
    public class VacationViewModel
    {

        [Required]
        [Display(Name = "Day")]
        public int DayId { get; set; }
        public IEnumerable<Day> Days { get; set; }

        [Required]
        [Display(Name = "Month")]
        public int MonthId { get; set; }
        public IEnumerable<Month> Months { get; set; }

        [Required]
        [Display(Name = "Year")]
        public int YearId { get; set; }
        public IEnumerable<Year> Years { get; set; }

        public DateTime Test()
        {
            var leaveDate = new DateTime(DayId, MonthId, YearId);

            return leaveDate;
        }

        public void SetLeaveDate(string userId, ApplicationDbContext context)
        {
            var leaveDate = new Date
            {
                DayId = DayId,
                MonthId = MonthId,
                YearId = YearId
            };
            context.Dates.Add(leaveDate);
            context.SaveChanges();

            var query = (from a in context.Users
                         where a.Id == userId
                         select new { a }).Single();
            var user = query.a;
            user.LeaveDateId = leaveDate.Id;
            context.SaveChanges();
        }

        public void SetReturnDate(string userId, ApplicationDbContext context)
        {
            var returnDate = new Date
            {
                DayId = DayId,
                MonthId = MonthId,
                YearId = YearId
            };
            context.Dates.Add(returnDate);
            context.SaveChanges();

            var query = (from a in context.Users
                         where a.Id == userId
                         select new { a }).Single();
            var user = query.a;
            user.LeaveDateId = returnDate.Id;
            context.SaveChanges();
        }
    }
}