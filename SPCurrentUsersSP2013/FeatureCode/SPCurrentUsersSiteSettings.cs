////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	FeatureCode\SPCurrentUsersSiteSettings.cs
//
// summary:	Implements the sp current users site settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Data;

namespace SPCurrentUsers
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Site Setting Web Control.  On SP2007 and SP2010, this control will be displayed in the user's Site Settings section.  It will display the total current users to administrators of the site. This control will display as a link to the SPCurrentUsers admin page if in SP 2013 or if you disable displaying current usage in the options under the admin page.</summary>
    ///
    /// <remarks>   William.chung, 9/15/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SPCurrentUsersSiteSettingsControl : WebControl
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based
        /// implementation to create any child controls they contain in preparation for posting back or
        /// rendering.
        /// </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void CreateChildControls()
        {
            SubMenuTemplate t = new SubMenuTemplate();
            string userCountText = "";
            // Grab the current web
            SPWeb rootweb = SPContext.Current.Web.Site.RootWeb;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb(site.RootWeb.ServerRelativeUrl))
                    {
                        // Create a new MenuItemTemplate to add to the menu
                        MenuItemTemplate menuItem = new MenuItemTemplate();

                        if (SPCurrentUsersHelper.GetDisplayCurrentUsersInSiteActions(web))
                        {



                            // Getting some more help from our helper class...
                            int userCount =
                                SPCurrentUsersHelper.GetNumberOfCurrentUsers(web);

                            int anonymousUserCount = SPCurrentUsersHelper.GetNumberOfCurrentAnonymousUsers(web);


                            // ...and start building a description
                            userCountText =
                                userCount.ToString() + " logged in, " + anonymousUserCount.ToString() + " anonymous.";


                            /*
                            //Too much info, dude!
                            // Thank the deities for that helper class.
                            int userCountPage =
                                SPCurrentUsersHelper.GetNumberOfCurrentUsersOnPage(
                                 this.Page.Request.Path
                                 );

                            int AnonUserCountPage = SPCurrentUsersHelper.GetNumberOfAnonymousUsersOnPage(this.Page.Request.Path);

                            userCountText += userCountPage.ToString() +
                                            " logged in, " + AnonUserCountPage.ToString() + " anonymous";
                             */
                        }
                        else
                        {
                            userCountText = "View the number of current users accessing this site.";
                        }
                        // Set properties for the MenuItemTemplate...
                        menuItem.Text = "Current users";
                        menuItem.Description = userCountText;
                        // ...and link to the custom application page
                        menuItem.ClientOnClickNavigateUrl = rootweb.Url +
                            "/_layouts/SPCurrentUsersAdministration.aspx";
                        // before we add it to the control
                        this.Controls.Add(menuItem);
                    }
                }
            });

        }
    }
}
