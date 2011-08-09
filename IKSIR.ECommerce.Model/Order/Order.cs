﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IKSIR.ECommerce.Model.CommonModel;
using IKSIR.ECommerce.Model.MembershipModel;
using IKSIR.ECommerce.Model.Bank;

namespace IKSIR.ECommerce.Model.Order
{
    public class Order : ModelBase
    {
        public User User { get; set; }
        public Basket Basket { get; set; }
        public PaymetInfo PaymetInfo { get; set; }
        public EnumValue Status { get; set; }
        public decimal TotalRatedPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public Order(int id, int createUserId, DateTime createDate, int editUserId, DateTime editDate, User user, Basket basket, PaymetInfo paymetInfo, EnumValue status, decimal totalRatedPrice, decimal totalPrice)
            : base(id, createUserId, createDate, editUserId, editDate)
        {
            this.User = user;
            this.Basket = basket;
            this.PaymetInfo = paymetInfo;
            this.Status = Status;
            this.TotalPrice = totalPrice;
            this.TotalRatedPrice = totalRatedPrice;
        }
        public Order()
        {
        }
    }


}
