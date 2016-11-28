﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WasteAway.Models;

namespace WasteAway.ViewModels
{
    public class ChangeAddressViewModel
    {
        [Required]
        [Display(Name = "Street Address Line*")]
        public string StreetAddressOne { get; set; }

        [Display(Name = "Street Address Line Cont.")]
        public string StreetAddressTwo { get; set; }

        [Required]
        [Display(Name = "State*")]
        public int StateId { get; set; }
        public IEnumerable<State> States { get; set; }

        [Required]
        [Display(Name = "City*")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Zipcode*")]
        public string ZipcodeId { get; set; }

        public void ChangeAddress(ChangeAddressViewModel model, ApplicationDbContext context)
        {
            var city = new City
            {
                StateId = model.StateId,
                Name = model.City
            };
            context.Cities.Add(city);
            context.SaveChanges();

            var zipcode = new Zipcode
            {
                Name = model.ZipcodeId,
            };
            context.Zipcodes.Add(zipcode);
            context.SaveChanges();

            var address = new Address
            {
                StreetAddressOne = model.StreetAddressOne,
                StreetAddressTwo = model.StreetAddressTwo,
                CityId = city.Id,
                ZipcodeId = zipcode.Id
            };
            context.Addresses.Add(address);
            context.SaveChanges();
        }
    }
}