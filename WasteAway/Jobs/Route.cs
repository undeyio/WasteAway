﻿using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using WasteAway.Models;

namespace WasteAway.Jobs
{
    public class Route : IJob
    {
        public decimal PickupCost;
        private readonly ApplicationDbContext _context;

        public Route()
        {
            _context = new ApplicationDbContext();
            PickupCost = (decimal)5.99;
        }

        public void Execute(IJobExecutionContext context)
        {
            ResetTruckPickupList();
            ClearPickupsTable();
            SetTruckPickups(CreatePickupList());
        }

        private List<ApplicationUser> CreatePickupList()
        {
            var pickups = new List<ApplicationUser>();
            var date = DateTime.Today;
            var currentDay = date.DayOfWeek.ToString();

            var results = _context.Users
                .Where(a => a.AlternatePickupWeekday.Name == currentDay)
                .Where(a => a.LeaveDateId != null)
                .Where(a => date < LeaveOfAbsence(a.LeaveDateId))
                .ToList();

            foreach (var user in results)
            {
                pickups.Add(user);
                user.AlternatePickupWeekdayId = null;
                user.Balance += PickupCost;
            }

            results = _context.Users
                .Where(a => a.PickupWeekday.Name == currentDay)
                .Where(a => a.AlternatePickupWeekdayId == null)
                .Where(a => date < LeaveOfAbsence(a.LeaveDateId))
                .ToList();

            foreach (var user in results)
            {
                pickups.Add(user);
                user.Balance += PickupCost;
            }

            return pickups;
        }

        private DateTime LeaveOfAbsence(int? dateId)
        {
            var year = _context.Dates
                .Where(a => a.Id == dateId)
                .Select(a => a.YearId)
                .Single();

            var month = _context.Dates
                .Where(a => a.Id == dateId)
                .Select(a => a.MonthId)
                .Single();

            var day = _context.Dates
                .Where(a => a.Id == dateId)
                .Select(a => a.DayId)
                .Single();

            return new DateTime(year, month, day);
        }

        private void SetTruckPickups(List<ApplicationUser> users)
        {
            if (users == null) throw new ArgumentNullException(nameof(users));

            foreach (var user in users)
            {
                var pickup = new Pickup
                {
                    UserId = user.Id,
                };

                var zipcodeId = GetPickupAddressZipcodeId(user);
                SetPickupToTruckList(pickup, zipcodeId);
                _context.Pickups.Add(pickup);
                _context.SaveChanges();
            }
        }

        private void SetPickupToTruckList(Pickup pickup, int zipcodeId)
        {
            foreach (var truck in _context.Trucks)
            {
                if (truck.ZipcodeId != zipcodeId) continue;
                pickup.TruckId = truck.Id;
                truck.Pickups.Add(pickup);
            }
        }

        private int GetPickupAddressZipcodeId(ApplicationUser user)
        {
            var result = _context.Addresses
                .Where(a => a.Id == user.PickupAddressId)
                .Select(z => z.ZipcodeId)
                .Single();

            return result;
        }

        private void ClearPickupsTable()
        {
            var data = _context.Pickups.ToList();
            _context.Pickups.RemoveRange(data);
            _context.SaveChanges();
        }

        private void ResetTruckPickupList()
        {
            foreach (var truck in _context.Trucks)
            {
                truck.Pickups = new List<Pickup>();
            }
        }
    }
}