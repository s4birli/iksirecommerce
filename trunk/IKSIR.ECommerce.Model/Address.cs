﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IKSIR.ECommerce.Model
{
    class Address : ModelBase
    {
        #region Properties
        public int Type { get; set; }
        public Country Country { get; set; }
        public City City { get; set; }
        public District District { get; set; }
        public string Description { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public User User { get; set; }
        #endregion

        #region Constructors
        public Address(int id, int createUserId, DateTime createDate, int editUserId, DateTime editDate, int type, Country country, City city, District district, string description, string postalCode, string phone, User user)
            : base(id, createUserId, createDate, editUserId, editDate)
        {
            this.Type = type;
            this.Country = country;
            this.City = city;
            this.District = district;
            this.Description = description;
            this.PostalCode = postalCode;
            this.Phone = phone;
            this.User = user;
        }
        #endregion
    }
}
