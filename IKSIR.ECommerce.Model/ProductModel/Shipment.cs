﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IKSIR.ECommerce.Model.CommonModel;

namespace IKSIR.ECommerce.Model.ProductModel
{
    public class Shipment : ModelBase
    {
        public string Title { get; set; }
        public decimal UnitPrice { get; set; }

        public Shipment(int id, int createUserId, DateTime createDate, int editUserId, DateTime editDate, string title,
            decimal unitPrice)
            : base(id, createUserId, createDate, editUserId, editDate)
        {

            this.Title = title;
            this.UnitPrice = unitPrice;

        }
        public Shipment()
        {
            // TODO: Complete member initialization
        }
    }

}