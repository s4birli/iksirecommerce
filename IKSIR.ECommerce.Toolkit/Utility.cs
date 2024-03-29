﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace IKSIR.ECommerce.Toolkit
{
    public class Utility
    {
        protected bool IsEmail(string mail)
        {
            string emailPattern = @"^(([^<>()[\]\\.,;:\s@\""]+"
            + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
            + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
            + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
            + @"[a-zA-Z]{2,}))$";

            return Regex.IsMatch(mail, emailPattern);
        }
        public static string Encrypt(string strValue)
        {
            return Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(strValue));
        }
        public static string Decrypt(string strValue)
        {
            return System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(strValue));
        }
        public static string CurrencyFormat(decimal currency)
        {
             
            string temp = "";
            if (currency < 1)
            {
                temp = "0" + currency.ToString("#,##.00");
            }
            else
            {
                temp = currency.ToString("#,##.00");
            }
            if (temp.Substring(temp.Length - 3, 1) != ",")
            {
                temp = temp.Replace(',', '+');
                temp = temp.Replace('.', ',');
                temp = temp.Replace('+', '.');
            }
            return NumberManipulation.ChangeNumberFormatByCulture(temp, new CultureInfo("tr-TR"), Thread.CurrentThread.CurrentCulture); 
        }
        public static bool ResizeImage(string OriginalFile, string NewFile, int NewWidth, int MaxHeight, bool OnlyResizeIfWider)
        {
            bool returnValue = true;
            try
            {
                System.Drawing.Image FullsizeImage = System.Drawing.Image.FromFile(OriginalFile);

                // Prevent using images internal thumbnail
                FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
                FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

                if (OnlyResizeIfWider)
                {
                    if (FullsizeImage.Width <= NewWidth)
                    {
                        NewWidth = FullsizeImage.Width;
                    }
                }

                int NewHeight = FullsizeImage.Height * NewWidth / FullsizeImage.Width;
                if (NewHeight > MaxHeight)
                {
                    // Resize with height instead
                    NewWidth = FullsizeImage.Width * MaxHeight / FullsizeImage.Height;
                    NewHeight = MaxHeight;
                }

                System.Drawing.Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);

                // Clear handle to original file so that we can overwrite it if necessary
                FullsizeImage.Dispose();

                // Save resized picture
                NewImage.Save(NewFile);
            }
            catch (Exception)
            {

                returnValue = false;
            }
            return returnValue;
        }
        public static void BindDropDownList(DropDownList dropDownList, object dataSource, string dataTextField, string dataValueField, string firstItemText = "Seçiniz", string firstItemValue = "-1")
        {
            dropDownList.DataSource = dataSource;
            dropDownList.DataTextField = dataTextField;
            dropDownList.DataValueField = dataValueField;
            dropDownList.DataBind();
            dropDownList.Items.Insert(0, new ListItem(firstItemText, firstItemValue));
        }

        public static bool isEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }
    }
}
