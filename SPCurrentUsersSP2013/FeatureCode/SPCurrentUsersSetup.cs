////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	FeatureCode\SPCurrentUsersSetup.cs
//
// summary:	Implements the sp current users setup class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace SPCurrentUsers
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A sp current users setup feature receiver.  This feature receiver is will automatically activate the features required for SPCurrentUser to work. </summary>
    ///
    /// <remarks>   William.chung, 9/15/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    class SPCurrentUsersSetup : SPFeatureReceiver
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Occurs after a Feature is activated. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="properties">   An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"><
        ///                             /see>
        ///                              object that represents the properties of the event. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            // Grab current web...
            //SPWeb web = (SPWeb)properties.Feature.Parent;
            SPSite site = (SPSite)properties.Feature.Parent;
            SPWeb web = site.RootWeb;


            //Let's not mess with the MOSS 2007 User List
            //We'll just utilize our own list
            /*
            // ...and it's User Information List
            SPList userList = web.SiteUserInfoList;

            // Add required fields to user list
            // only if not already present
            if (!userList.Fields.ContainsField("Last Page Hit Time"))
            {
                // Pattern follows pattern from PageList
                userList.Fields.Add("LastPageHitTime", SPFieldType.DateTime, false);
                SPFieldDateTime lastPageHit = (SPFieldDateTime)userList.Fields["LastPageHitTime"];
                lastPageHit.DisplayFormat = SPDateTimeFieldFormatType.DateTime;
                lastPageHit.Title = "Last Page Hit Time";
                // Remember to save changes
                lastPageHit.Update();
            }
            
            // Same for Url
            if (!userList.Fields.ContainsField("Last Page Hit Url"))
            {
                userList.Fields.Add("LastPageHitUrl", SPFieldType.Text, false);
                SPFieldText lastPageHit = (SPFieldText)userList.Fields["LastPageHitUrl"];
                lastPageHit.Title = "Last Page Hit Url";
                // Remember to save changes
                lastPageHit.Update();
            }


            // Same for Url
            if (!userList.Fields.ContainsField("Last IPAddress"))
            {
                userList.Fields.Add("LastIPAddress", SPFieldType.Text, false);
                SPFieldText lastIP = (SPFieldText)userList.Fields["LastIPAddress"];
                lastIP.Title = "Last IP Address";
                // Remember to save changes
                lastIP.Update();
            }
            


            if (userList.DefaultView != null)
            {
                SPView defaultView = userList.DefaultView;
                if (!SPCurrentUsersHelper.IsFieldInView(userList.DefaultView, "Last Page Hit Url"))
                {
                    defaultView.ViewFields.Add("Last Page Hit Url");

                }
                if (!SPCurrentUsersHelper.IsFieldInView(userList.DefaultView, "Last Page Hit Url"))
                {
                    defaultView.ViewFields.Add("Last Page Hit Time");
                }
                               
                defaultView.Update();                               
            }
            */


            // Add properties to web
            if (web.Properties["SPCurrentUsersDefaultSessionDuration"] == null)
            {
                web.Properties.Add("SPCurrentUsersDefaultSessionDuration", "15");
            }

            if (web.Properties["SPCurrentUsersDisplayCurrentUsersInSiteActions"] == null)
            {
                web.Properties.Add("SPCurrentUsersDisplayCurrentUsersInSiteActions", "False");
            }

            web.Properties.Update();

            // Activate features
            // Administration

            

            if (site.Features[new Guid("f25b1dcc-90ae-46ec-b42a-c337a12795b7")] == null)
                site.Features.Add(new Guid("f25b1dcc-90ae-46ec-b42a-c337a12795b7"));
            // DelegateControl
            if (site.Features[new Guid("3c1cf600-289a-484a-b622-307f8e57cdaf")] == null)
                site.Features.Add(new Guid("3c1cf600-289a-484a-b622-307f8e57cdaf"));
            // Page List
            if (site.Features[new Guid("e90b462d-b808-44c5-b7b4-e39c6a4cce8f")] == null)
                site.Features.Add(new Guid("e90b462d-b808-44c5-b7b4-e39c6a4cce8f"));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Occurs when a Feature is deactivated. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="properties">   An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"><
        ///                             /see>
        ///                              object that represents the properties of the event. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Occurs after a Feature is installed. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="properties">   An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"><
        ///                             /see>
        ///                              object that represents the properties of the event. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Occurs when a Feature is uninstalled. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="properties">   An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"><
        ///                             /see>
        ///                              object that represents the properties of the event. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
        }
    }
}
