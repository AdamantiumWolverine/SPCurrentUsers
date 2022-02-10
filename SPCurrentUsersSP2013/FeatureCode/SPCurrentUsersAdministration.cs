////////////////////////////////////////////////////////////////////////////////////////////////////
// File:	FeatureCode\SPCurrentUsersAdministration.cs
//
// Summary:	Implements the sp current users administration class
// This is the server side code behind for the SPCurrentUsersAdministration.aspx page.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Data;

namespace SPCurrentUsers
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  SPCurrentUsersAdministration: This is the server side code behind for the SPCurrentUsersAdministration.aspx page. </summary>
    ///
    /// <remarks>   William.chung, 9/15/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SPCurrentUsersAdministration : LayoutsPageBase
    {
        /// <summary>   The text box minutes per session control. </summary>
        protected TextBox TextBoxMinutesPerSession;
        /// <summary>   The label minutes per session control. </summary>
        protected Label LabelMinutesPerSession;
        /// <summary>   The link button update control. </summary>
        protected Button LinkButtonUpdate;
        /// <summary>   The label results control. </summary>
        protected Label LabelResults;
        /// <summary>   The label update results control. </summary>
        protected Label lblUpdateResults;
        /// <summary>   The label page overview control. </summary>
        protected Label LabelPageOverview;
        /// <summary>   The grid view pages. </summary>
        protected GridView GridViewPages;

        //protected HyperLink HyperLinkModifyPages;
        /// <summary>   List of hl view user trackings. </summary>
        protected HyperLink hlViewUserTrackingList;
        //protected HyperLink hlUserList;


        /// <summary>   The display current users in site actions control. </summary>
        protected CheckBox cbDisplayCurrentUsersInSiteActions;
        /// <summary>   The debug mode control. </summary>
        protected CheckBox cbDebugMode;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by Page for pre init events. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Page_PreInit(object sender, EventArgs e)
        {
            Page.MasterPageFile = SPContext.Current.Web.MasterUrl;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Raises the init event. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="e">    Event information to send to registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void OnInit(EventArgs e)
        {
            // Make sure we have our child controls
            EnsureChildControls();

            // Hook up the update LinkButton
            LinkButtonUpdate.Click += new EventHandler(LinkButtonUpdate_Click);
            SPWeb web = SPContext.Current.Web.Site.RootWeb;

            // Next, try reading the current session duration...
            TextBoxMinutesPerSession.Text = SPCurrentUsersHelper.GetPageSessionTimeout(web).ToString();

            //web.Properties["SPCurrentUsersDefaultSessionDuration"];

            cbDisplayCurrentUsersInSiteActions.Checked = SPCurrentUsersHelper.GetDisplayCurrentUsersInSiteActions(web);

            cbDebugMode.Checked = SPCurrentUsersHelper.GetDebugModeSetting(web);

            // ...and if it is not set, use the default.
            if (string.IsNullOrEmpty(TextBoxMinutesPerSession.Text))
            {
                TextBoxMinutesPerSession.Text = "15";
            }
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates the counts. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void UpdateCounts()
        {
                        // Get a reference to the current web
            SPWeb web = SPContext.Current.Web.Site.RootWeb;

           

          //  Boolean bDisplayCurrentUsersInSession = SPCurrentUsersHelper.Get


            // Create a new DataTable to be source for GridView
            DataTable dt = new DataTable();
            // Add some columns
            dt.Columns.Add("Title", typeof(String));
           // dt.Columns.Add("Timeout");
            dt.Columns.Add("Logged in users", typeof(Int32));
            dt.Columns.Add("Anonymous users", typeof(Int32));
           
            /*
            // This method runs very slow especially when the number of pages in the pages list get's pretty big.
             
            // Grab the Page list...
            SPList pagesList = web.Lists["SPCurrentUsers URLs"];
            // ...and iterate through the list.
            foreach (SPListItem page in pagesList.Items)
            {
                // This is where we get the number of users on the page. 
                // See explanation below
                int currentUsers =
                    SPCurrentUsersHelper.GetNumberOfCurrentUsersOnPage(web, page.Title);
                int anonUsers = SPCurrentUsersHelper.GetNumberOfAnonymousUsersOnPage(web, page.Title);

                // Add a new DataRow for each page in the list
                DataRow dr = dt.NewRow();
                dr[0] = page["Title"].ToString();
               // dr[1] = page["Page Session Timeout"].ToString();
                dr[1] = currentUsers.ToString();
                dr[2] = anonUsers.ToString();
                dt.Rows.Add(dr);
            }
            
             //Rather than use the above method, we will query the Current Users and Anonymous Users Tables
             */


            //Query the user table for accesses within the last 15 minutes
            SPListItemCollection users = SPCurrentUsersHelper.GetAllAnonymousAndLoggedInCurrentUsers(web);

            //Initialize LabelResults
            LabelResults.Text = "";
                int iUserCount = 0;
                int iAnonymousCount = 0;
            try
            {
                //Since the URLs are ordered, we just need to add an item when the URL changes.
                string strLastURL = "";
                DataRow currentRow = null;
           

                foreach (SPListItem user in users)
                {
                    if (currentRow==null || (user["LastPageHitUrl"]!=null && strLastURL.ToLower().Trim() != user["LastPageHitUrl"].ToString().ToLower().Trim()) )
                    {
                        if (user["LastPageHitUrl"] != null)
                        {
                           

                            DataRow dr = dt.NewRow();

                            dr[0] = user["LastPageHitUrl"].ToString();
                            // dr[1] = page["Page Session Timeout"].ToString();

                            if (user["UserName"] == null || user["UserName"].ToString() == "")
                            {
                                //anonymous user
                                dr["Logged in users"] = "0";
                                dr["Anonymous users"] = "1";
                                iAnonymousCount++;
                            }
                            else
                            {
                                //logged in user
                                dr["Logged in users"] = "1";
                                dr["Anonymous users"] = "0";
                                iUserCount++;
                            }

                            
                            dt.Rows.Add(dr);

                            currentRow = dr;
                            strLastURL = user["LastPageHitUrl"].ToString();
                        }
                        else
                        {
                            LabelResults.Text += "Field LastPageHitUrl was not found in the User list.<br />";
                        }
                    }
                    else if (currentRow != null)
                    {

                        if (user["UserName"] == null || user["UserName"].ToString() == "")
                        {
                            //anonymous user
                            int iCount = int.Parse(currentRow["Anonymous users"].ToString());
                            iCount = iCount + 1;
                            currentRow["Anonymous users"] = iCount;
                            iAnonymousCount++;
                        }
                        else
                        {
                            //logged in user
                            int iCount = int.Parse(currentRow["Logged in users"].ToString());
                            iCount = iCount + 1;
                            currentRow["Logged in users"] = iCount;
                            iUserCount++;
                        }
                    
                    }
                }


            }
            catch (Exception ex)
            {
                LabelResults.Text += "Error occurred counting user page hits. " + ex.Message + "<br />";
            }




            /*
            //Don't have to do this separately now that we are using one table to store both
            //anonymous and logged in user info.
             
            //Query the user table for accesses within the last 15 minutes
            SPListItemCollection anonymousUsers = SPCurrentUsersHelper.GetCurrentAnonymousUsers(web);

          

            try
            {
                //Since the URLs are ordered, we just need to add an item when the URL changes.
                string strLastURL = "";
                DataRow currentRow = null;
                foreach (SPListItem user in anonymousUsers)
                {
                    if (currentRow == null || (user["LastPageHitUrl"] != null && strLastURL != user["LastPageHitUrl"].ToString()))
                    {
                        if (user["LastPageHitUrl"] != null)
                        {
                            DataRow[] foundRows = dt.Select("Title = '" + PrepForStore(user["LastPageHitUrl"].ToString()) + "'");
                            if (foundRows.Length > 0)
                            {
                                currentRow = foundRows[0];
                                int iCount = int.Parse(currentRow["Anonymous users"].ToString());
                                currentRow["Anonymous users"] = iCount + 1;
                            }
                            else
                            {

                                DataRow dr = dt.NewRow();

                                dr[0] = user["LastPageHitUrl"].ToString();
                                // dr[1] = page["Page Session Timeout"].ToString();
                                dr[1] = "0";
                                dr[2] = "1";
                                dt.Rows.Add(dr);

                                currentRow = dr;
                            }
                        }
                        else
                        {
                            LabelResults.Text += "Field LastPageHitUrl was not found in the User list.<br />";
                        }
                    }
                    else if (currentRow != null)
                    {
                        int iCount = int.Parse(currentRow["Anonymous users"].ToString());
                        currentRow["Anonymous users"] = iCount + 1;
                    }
                }


            }
            catch (Exception ex)
            {
                LabelResults.Text += "Error occurred counting user page hits. " + ex.Message + "<br />";
            }
            */

            // Hook the DataTable to the GridView...
            DataView dv = dt.DefaultView;
            dv.Sort = "[Logged in users] desc, [Anonymous users] desc, Title";
            GridViewPages.DataSource = dv;
            // ...and bind the data.
            GridViewPages.DataBind();

            //Update total
            LabelResults.Text += "<hr />"+  iUserCount + " logged in users, and " + iAnonymousCount + " anonymous users.";


            try
            {

               // SPList userList = web.SiteUserInfoList;
                SPList UserTrackingList = web.Lists["SPCurrentUsers User Tracker"];

                //Not supporting individual page time outs anymore
                //HyperLinkModifyPages.NavigateUrl = pagesList.DefaultViewUrl;
                //HyperLinkModifyPages.Visible = false;

                hlViewUserTrackingList.NavigateUrl = UserTrackingList.DefaultViewUrl;
               // hlUserList.NavigateUrl = userList.DefaultViewUrl;
               // hlUserList.Visible = false;
            }
            catch (Exception ex)
            {
                LabelResults.Text += "One of the critical lists for this application was not found. " + ex.Message + "<br />";
            }
       
        }

       ////////////////////////////////////////////////////////////////////////////////////////////////////
       /// <summary>    Raises the load event. </summary>
       ///
       /// <remarks>    William.chung, 9/15/2016. </remarks>
       ///
       /// <param name="e"> Event information to send to registered event handlers. </param>
       ////////////////////////////////////////////////////////////////////////////////////////////////////

       protected override void  OnLoad(EventArgs e)
        {
 	           base.OnLoad(e);
               UpdateCounts();
        }

        //Replaces single quotes with two single quotes

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Prep for store. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="str">  The string. </param>
        ///
        /// <returns>   A string. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        string PrepForStore(string str)
        {
            string strRet = str.Replace("'", "''");
            return strRet;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by LinkButtonUpdate for click events. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void LinkButtonUpdate_Click(object sender, EventArgs e)
        {

            lblUpdateResults.Text = "";
            EnsureChildControls();
            SPWeb web = SPContext.Current.Web.Site.RootWeb;

            double dValidMinutes =0.0;

            try
            {
                dValidMinutes = double.Parse(TextBoxMinutesPerSession.Text);
                if (dValidMinutes < 1)
                {
                    dValidMinutes = 1;
                }
            }
            catch
            {
                
                dValidMinutes = 15;
                lblUpdateResults.Text += "<div style=\"color: red; padding: 5px; border: solid 1px silver;\">Invalid value entered for default session timeout.  Using default of 15 minutes.</div>";
            }
            if (web.Properties["SPCurrentUsersDefaultSessionDuration"] != null)
            {
                web.Properties["SPCurrentUsersDefaultSessionDuration"] = dValidMinutes.ToString();
            }
            else
            {
                web.Properties.Add("SPCurrentUsersDefaultSessionDuration", dValidMinutes.ToString());
            }

            if (web.Properties["SPCurrentUsersDisplayCurrentUsersInSiteActions"] != null)
            {
                web.Properties["SPCurrentUsersDisplayCurrentUsersInSiteActions"] = cbDisplayCurrentUsersInSiteActions.Checked.ToString();
            }
            else
            {
                web.Properties.Add("SPCurrentUsersDisplayCurrentUsersInSiteActions", cbDisplayCurrentUsersInSiteActions.Checked.ToString());
                
            }


            if (web.Properties["SPCurrentUsersDebugMode"] != null)
            {
                web.Properties["SPCurrentUsersDebugMode"] = cbDebugMode.Checked.ToString();
            }
            else
            {
                web.Properties.Add("SPCurrentUsersDebugMode", cbDebugMode.Checked.ToString());

            }

            web.Properties.Update();

            lblUpdateResults.Text += "<div style=\"color: green; border: solid 1px silver; padding: 5px;\">Settings Updated, "+DateTime.Now.ToString()+"</div>";
        }
    }

    

}
