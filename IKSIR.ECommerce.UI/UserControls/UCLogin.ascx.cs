﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IKSIR.ECommerce.Infrastructure.DataLayer.MembershipDataLayer;
using IKSIR.ECommerce.Model.MembershipModel;

namespace IKSIR.ECommerce.UI.UserControls
{
    public partial class UCLogin : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LOGIN_USER"] != null)
            {
                pnlLogin.Visible = false;
                pnlLogout.Visible = true;

                User loginUser = (User)Session["LOGIN_USER"];
                lblUserTitle.Text = "Sayın " + loginUser.FirstName + " " + loginUser.LastName;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Toolkit.Utility.isEmail(txtEmail.Text))
            {
                lblAlert.Text = "Geçerli bir e-posta giriniz.";
                lblAlert.ForeColor = System.Drawing.Color.Red;
                return;
            }
            if (LoginUser())
            {
                pnlLogin.Visible = false;
                pnlLogout.Visible = true;
                //Response.Redirect("Default.aspx");
            }
            else
            {
                lblAlert.Text = "Kullanıcı adı ve/veya şifreniz hatalı.";
                lblAlert.ForeColor = System.Drawing.Color.Red;
            }
        }

        private bool LoginUser()
        {
            bool retValue = false;
            var user = UserData.Get(txtEmail.Text, txtPassword.Text);
            if (user != null)
            {
                Session.Add("LOGIN_USER", user);
                lblUserTitle.Text = "Sayın " + user.FirstName + " " + user.LastName;
                retValue = true;
            }
            return retValue;
        }

        protected void lbtnLogout_Click(object sender, EventArgs e)
        {
            Session.Remove("LOGIN_USER");
            Session.Remove("USER_BASKET");
            pnlLogin.Visible = true;
            pnlLogout.Visible = false;
            txtEmail.Text = "E-posta Adresiniz";
        }
    }
}