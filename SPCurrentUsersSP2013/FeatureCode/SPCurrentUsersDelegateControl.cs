////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	FeatureCode\SPCurrentUsersDelegateControl.cs
//
// summary:	Implements the sp current users delegate control class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using System.Web.UI.WebControls;
using System.Web;
using System.Security.Principal;

namespace SPCurrentUsers
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   SPCurrentUserDelegate control is the control that tracks whenever anyone accesses a SharePoint page with the AdditionalPageHead delegate control on it. 
    ///             This delegate control is included in most SharePoint MasterPages. </summary>
    /// 
    ///
    /// <remarks>   William.chung, 9/15/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SPCurrentUsersDelegateControl : WebControl
    {

        /// <summary>   The label debug control. </summary>
        protected Label lblDebug;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or add IP to list. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">          The web. </param>
        /// <param name="IPAddress">    The IP address. </param>
        ///
        /// <returns>   Add IP to list. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected SPListItem GetOrAddIPToList(SPWeb web, string IPAddress, string strUser="")
        {
            SPList IPList = web.Lists["SPCurrentUsers User Tracker"];
            if (IPList == null)
                return null;

            SPListItem IPItem = null;

                 SPQuery query = new SPQuery();

                                // Build the CAML query
                                query.Query = @"
                            <Where>
                            <And>
                                <Eq>
                                    <FieldRef Name=""Title"" />
                                    <Value Type=""Text"">" + IPAddress + @"</Value>
                                </Eq>
                                <Eq>
                                    <FieldRef Name=""UserName"" />
                                    <Value Type=""Text"">" + strUser + @"</Value>
                                </Eq>                                
                            </And>
                            </Where>";
                                // And retrieve the results
                                SPListItemCollection listItems = IPList.GetItems(query);
                                if (listItems.Count < 1)
                                {
                                    //add to list
                                    IPItem = IPList.Items.Add();

                                    IPItem["Title"] = IPAddress;
                                    IPItem["UserName"] = strUser;

                                    try
                                    {
                                        web.AllowUnsafeUpdates = true;
                                        IPItem.Update();
                                        web.AllowUnsafeUpdates = false;
                                    }
                                    catch (Exception ex) {
                                        lblDebug.Text += "Error adding IP to list: " + ex.Message + "<br />";
                                        return null;
                                    }

                                    return IPItem;
                                }
                                else
                                {
                                    return listItems[0];
                                }

        }

        //For performance optimization, we are not doing anything with the page list any more.  

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds a page list entry to 'thisPage'. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">      The web. </param>
        /// <param name="thisPage"> this page. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void AddPageListEntry(SPWeb web, string thisPage)
        {
            /*
                         // Grab the Page list
            try
            {
                                SPList pageList = web.Lists["SPCurrentUsers URLs"];
                          // Create a query object to hold CAML query
                                SPQuery query = new SPQuery();

                                // Build the CAML query
                                query.Query = @"
                            <Where>
                                <Eq>
                                    <FieldRef Name=""Title"" />
                                    <Value Type=""Text"">" + thisPage + @"</Value>
                                </Eq>
                            </Where>";
                                // And retrieve the results
                                SPListItemCollection pageItems = pageList.GetItems(query);
                                if (pageItems.Count < 1)
                                {
                                    double defaultSessionLength;
                                    // Try reading the default session duration, with a bit of error handling.
                                    try
                                    {
                                        defaultSessionLength = double.Parse(
                                            web.Properties["SPCurrentUsersDefaultSessionDuration"]);
                                    }
                                    catch (Exception)
                                    {
                                        // If we can't read property, use default value
                                        defaultSessionLength = 15.0;
                                    }
                                    // Create a new list item in Pages list
                                    SPListItem newPageItem = pageList.Items.Add();
                                    // Set the properties
                                    newPageItem["Title"] = thisPage;
                                    newPageItem["Page Session Timeout"] = defaultSessionLength;
                                    // and then save. 
                                    // Note that we still need to use AllowUnsafeUpdates
                                    try
                                    {
                                        web.AllowUnsafeUpdates = true;
                                        newPageItem.Update();
                                        web.AllowUnsafeUpdates = false;
                                    }
                                    catch (Exception ex) { lblDebug.Text += "Error updating Page List: " + ex.Message + "<br />"; }
                                } //end if
            }
            catch (Exception ex)
            {
                lblDebug.Text += "Error adding Page List entry. " + ex.Message;
            }
                  */


        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" />
        ///  event.  This is where the main code thread for the control runs.  OnLoad the delegate control will add the user's access information in the SPCurrentUsers User Tracking list.
        /// </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="e">    The <see cref="T:System.EventArgs" />
        ///                      object that contains the event data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void OnLoad(EventArgs e)
        { 
            
            lblDebug = new Label();

            lblDebug.Text = "SPCurrentUsers Tracking is Enabled.<br /> ";
            try
            {
               

                // Get current web, use, user info list, and page list.
              //  SPWeb web = SPContext.Current.Web.Site.RootWeb;
                //SPUser spUser = SPContext.Current.Web.CurrentUser;
                IPrincipal webUser = HttpContext.Current.User;
                SPUser spCurrentUser = null;

                string strUser = "";

                string strAuthenticationType = "";

                if (webUser != null & webUser.Identity != null && webUser.Identity.IsAuthenticated && webUser.Identity.Name != null)
                {
                    strUser = webUser.Identity.Name;
                    strAuthenticationType = webUser.Identity.AuthenticationType;

                    spCurrentUser = SPContext.Current.Web.CurrentUser;

                }

                if (strUser != "")
                {
                    SPSecurity.RunWithElevatedPrivileges
                    (delegate()
                    {
                        try
                        {
                            using (SPSite elevatedSite = new SPSite(SPContext.Current.Site.ID))
                            {

                                using (SPWeb elevatedWeb = elevatedSite.OpenWeb(elevatedSite.RootWeb.ServerRelativeUrl))
                                {

                                    //If the site is read only, return do not attempt to write
                                    if (elevatedSite.ReadOnly)
                                    {
                                        lblDebug.Text = "<div style='background-color: #f8d7da; color: #721c24;' >This site is in Read Only Mode.</div>";
                                        return;
                                    }



                                   lblDebug.Visible = SPCurrentUsersHelper.GetDebugModeSetting(elevatedWeb);
                                    //spUser = elevatedWeb.CurrentUser;
                                    // Interestingly, the spUser inside the RunWithElevatedPrivileges section is the administrator account.


                                    lblDebug.Text += "User: " + strUser + "<br />";
                                    SPList userList = null;
                                    try
                                    {
                                        userList = elevatedWeb.Lists["SPCurrentUsers User Tracker"]; //elevatedWeb.SiteUserInfoList;
                                    }
                                    catch (Exception ex)
                                    {
                                        lblDebug.Text += "[SpCurrentUsers User Tracker] list was not found.  You may have to activate the SPCurrentUsers Setup Feature to create the list.<br />";
                                        return;

                                    }
                                    string strIPAddress = "No IP Address";

                                    if (!String.IsNullOrEmpty(HttpContext.Current.Request.UserHostAddress))
                                    {
                                        strIPAddress = HttpContext.Current.Request.UserHostAddress;
                                    }

                                    // Grab the user SPListItem from the user info list
                                    //SPListItem user = userList.Items.GetItemById(spUser.ID);

                                    SPListItem user = GetOrAddIPToList(elevatedWeb, strIPAddress, strUser);

                                    string thisPage = this.Page.Request.Path;

                                    if (user.Fields.ContainsField("LastPageHitTime"))
                                    {

                                        user["LastPageHitTime"] = DateTime.Now.ToString();
                                    }
                                    if (user.Fields.ContainsField("LastPageHitUrl"))
                                    {
                                        user["LastPageHitUrl"] = thisPage;
                                    }
                                    if (user.Fields.ContainsField("UserName"))
                                    {
                                        user["UserName"] = strUser;
                                    }

                                    if (user.Fields.ContainsField("User"))
                                    {
                                        string strUserName = strUser;
                                        try
                                        {
                                         
                                            if (strUserName.Contains("|"))
                                            {
                                                char[] sep = new char[] { '|' };

                                                string[] strUserLoginArray = strUserName.Split(sep);
                                                strUserName = strUserLoginArray[strUserLoginArray.Length - 1];//

                                            }
                                       
                                            
                                            //elevatedWeb.AllowUnsafeUpdates = true;
                                            //lblDebug.Text+=("Ensuring user: " + strUser + "<br />");
                                            //SPUser spuser = elevatedWeb.EnsureUser(strUserName);
                                            //elevatedWeb.AllowUnsafeUpdates = false;
                                            //lblDebug.Text+=("User found: " + spuser.ID.ToString() + ", " +spuser.LoginName + "<br />");
                                            if (spCurrentUser != null)
                                            {
                                                user["User"] = new SPFieldUserValue(elevatedWeb, spCurrentUser.ID, spCurrentUser.Name);
                                            }
                                        }
                                        catch (Exception exSPUser)
                                        {
                                            lblDebug.Text += "<div class='error'>Unable to resolve username " + strUserName + "<br />"+ exSPUser.ToString() + "</div>";
                                            user["User"] = null;
                                        }

                                    }

                                    if (user.Fields.ContainsField("AuthenticationType"))
                                    {
                                        user["AuthenticationType"] = strAuthenticationType;
                                    }

                                    if (user.Fields.ContainsField("WebUrl"))
                                    {
                                        user["WebUrl"] = SPContext.Current.Site.RootWeb.Url;
                                    }

                                    if (user.Fields.ContainsField("ServerName"))
                                    {

                                        user["ServerName"] = System.Environment.MachineName;
                                    }

                                    try
                                    {
                                        // AllowUnsafeUpdates required on GET requests
                                        elevatedWeb.AllowUnsafeUpdates = true;
                                        user.Update();
                                        elevatedWeb.AllowUnsafeUpdates = false;
                                        lblDebug.Text += "User page access has been logged successfully.<br />";
                                    }
                                    catch (Exception ex)
                                    {

                                        lblDebug.Text += "Error Updating User Info: " + ex.Message + "<br />";
                                    }

                                    // TODO: Add code to add page to PageList
                                    //    AddPageListEntry(elevatedWeb, thisPage);
                                  

                                } //end using SPWeb
                            } //end using SPSite
                        }
                        catch (Exception exElevatedLoggedIn)
                        {
                            lblDebug.Text += "Error occurred:" + exElevatedLoggedIn.Message + "<br />";
                        }

                    }); //End Delegate Run With Elevated Privileges


                }
                else
                {
                    lblDebug.Text += "User logged in anonymously.<br />";
                    SPSecurity.RunWithElevatedPrivileges
                   (delegate()
                   {
                       using (SPSite elevatedSite = new SPSite(SPContext.Current.Site.ID))
                       {

                           using (SPWeb elevatedWeb = elevatedSite.OpenWeb(elevatedSite.RootWeb.ServerRelativeUrl))
                           {
                               lblDebug.Visible = SPCurrentUsersHelper.GetDebugModeSetting(elevatedWeb);

                               //spUser = elevatedWeb.CurrentUser;
                               // Interestingly, the spUser inside the RunWithElevatedPrivileges section is the administrator account.


                             //  lblDebug.Text += "SPUser: " + spUser.Name + "<br />";
                               SPList userList = null;
                               try
                               {
                                   userList = elevatedWeb.Lists["SPCurrentUsers User Tracker"]; //elevatedWeb.SiteUserInfoList;
                               }
                               catch (Exception ex)
                               {
                                   lblDebug.Text += "[SpCurrentUsers User Tracker] list was not found.  You may have to activate the SPCurrentUsers Setup Feature to create the list.<br />";
                                   return;

                               }
                               // Grab the user SPListItem from the user info list


                               string strIPAddress = "No IP Address";

                               if (HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null && HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] !="")
                               {
                                   strIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                               }

                               SPListItem user = GetOrAddIPToList(elevatedWeb, strIPAddress);


                               string thisPage = this.Page.Request.Path;
                               if (user.Fields.ContainsField("LastPageHitTime"))
                               {
                                   user["LastPageHitTime"] = DateTime.Now.ToString();
                               }

                               if (user.Fields.ContainsField("LastPageHitUrl"))
                               {
                                   user["LastPageHitUrl"] = thisPage;
                               }

                               if (user.Fields.ContainsField("UserName"))
                               {
                                   user["UserName"] = "";
                               }

                               if (user.Fields.ContainsField("User"))
                               {
                                   user["User"] = "";
                               }

                               if (user.Fields.ContainsField("AuthenticationType"))
                               {
                                   user["AuthenticationType"] = "";
                               }


                               if (user.Fields.ContainsField("WebUrl"))
                               {
                                   user["WebUrl"] = SPContext.Current.Site.RootWeb.Url;
                               }


                               if (user.Fields.ContainsField("ServerName"))
                               {
                                    
                                   user["ServerName"] =  System.Environment.MachineName;
                               }
                               
                               try
                               {
                                   // AllowUnsafeUpdates required on GET requests
                                   elevatedWeb.AllowUnsafeUpdates = true;
                                   user.Update();
                                   elevatedWeb.AllowUnsafeUpdates = false;
                                   lblDebug.Text += "Anonymous User ("+strIPAddress+") page access logged successfully.<br />";
                               }
                               catch (Exception ex)
                               {

                                   lblDebug.Text += "Error Updating Anonymous User Info: " + ex.Message + "<br />";
                               }

                               // TODO: Add code to add page to PageList
                               // TODO: Add code to add page to PageList
                             //  AddPageListEntry(elevatedWeb, thisPage);
                           } //end using SPWeb
                       } //end using SPSite

                   }); //End Delegate Run With Elevated Privileges

                }//end if SPUser!=null
            }
            catch (Exception ex) { lblDebug.Text += "Error occurred in SPCurrentUser: " + ex.Message + "<br />"; }

              //Either set lblDebug.Visible to false or don't add it to the controls if you want to hide debug info.
            Controls.Add(lblDebug);
        }
      
    }
}
