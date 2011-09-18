﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IKSIR.ECommerce.Model.CommonModel
{
    public class Files : ModelBase
    {
        public EnumValue Type { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public int ProductId { get; set; }

        public Files(int id, int createUserId, DateTime createDate, int editUserId, DateTime editDate, EnumValue type,  string value, string title, string description, string filePath, int productId)
            : base(id, createUserId, createDate, editUserId, editDate)
        {
            this.Type = type;
            this.Value = value;
            this.Title = title;
            this.Description = description;
            this.FilePath = filePath;
            this.ProductId = productId;
        }
        public Files()
        {
        }
    }
}
