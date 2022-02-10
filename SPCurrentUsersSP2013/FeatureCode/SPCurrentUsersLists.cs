////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	FeatureCode\SPCurrentUsersLists.cs
//
// summary:	Implements the sp current users lists class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace SPCurrentUsers
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A sp current users lists feature receiver.  When the SPCurrentUsersLists feature is activated, this code will ensure that the SPCurrentUsers User Tracking List exists and all columns required for the current version are in the list. </summary>
    ///
    /// <remarks>   William.chung, 9/15/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    class SPCurrentUsersLists : SPFeatureReceiver
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
            // Get reference to  Root web
            //SPWeb web = SPContext.Current.Web.Site.RootWeb;

            SPSite site = properties.Feature.Parent as SPSite;
            SPWeb web = site.RootWeb;

            //*****************************************
            /*
            // Grab the newly created list.
            // Note that we reference the title, not the Url of the list
            SPList pageList = web.Lists["SPCurrentUsers URLs"];
            // Declare a new column to add later
            SPFieldNumber pageSessionTimeout = default(SPFieldNumber);

            // Add session timeout column only if not present
            if (!pageList.Fields.ContainsField("Page Session Timeout"))
            {
                // Add a new Number field to our list.
                pageList.Fields.Add("SPCurrentUserPageTimeout", SPFieldType.Number, false);
                // Get a reference to the new column
                pageSessionTimeout = (SPFieldNumber)pageList.Fields["SPCurrentUserPageTimeout"];
                // Set default value...
                pageSessionTimeout.DefaultValue = "15";
                // ...and title...
                pageSessionTimeout.Title = "Page Session Timeout";
                // ...before saving changes
                pageSessionTimeout.Update();
            }
            else
            {
                pageSessionTimeout = (SPFieldNumber)pageList.Fields["Page Session Timeout"];
            }
            

            // Get reference to default view.
            SPView defaultView = pageList.DefaultView;

            if (!SPCurrentUsersHelper.IsFieldInView(defaultView, "Page Session Timeout"))
            {
                // Add the new column to the view...
                defaultView.ViewFields.Add(pageSessionTimeout);
                // ...and save changes
                defaultView.Update();
            }
            // Hide the list...
            pageList.Hidden = true;
            // ...and save changes
            pageList.Update();
            */



            //*****************************************

            // Grab the newly created list.
            // Note that we reference the title, not the Url of the list
            SPList IPList = web.Lists["SPCurrentUsers User Tracker"];
            // Declare a new column to add later
            SPFieldDateTime lastHitTime = default(SPFieldDateTime);
            SPFieldText lastPageHitUrl = default(SPFieldText);
            SPFieldText webUrl = default(SPFieldText);
            SPFieldText serverNameField = default(SPFieldText);
            SPFieldText userName = default(SPFieldText);
            SPFieldText authenticationTypeField = default(SPFieldText);
            SPFieldUser user = default(SPFieldUser);

            // Add session timeout column only if not present
            // Add required fields to IP List

            // Same for Url

            //We'll use the list Title field and store the IP there, so no need for column for IP address


            // only if not already present
            if (!IPList.Fields.ContainsField("UserName"))
            {
                // Pattern follows pattern from PageList
                IPList.Fields.Add("UserName", SPFieldType.Text, false);
                userName = (SPFieldText)IPList.Fields["UserName"];                
                userName.Title = "UserName";
                // Remember to save changes
                userName.Update();
            }
            else
            {
                userName = (SPFieldText)IPList.Fields["UserName"];
            }

            // only if not already present
            if (!IPList.Fields.ContainsField("User"))
            {
                // Pattern follows pattern from PageList
                IPList.Fields.Add("User", SPFieldType.User, false);
                user = (SPFieldUser)IPList.Fields["User"];
                user.Title = "User";
                // Remember to save changes
                user.Update();
            }
            else
            {
                user = (SPFieldUser)IPList.Fields["User"];
            }

            // only if not already present
            if (!IPList.Fields.ContainsField("Last Page Hit Time"))
            {
                // Pattern follows pattern from PageList
                IPList.Fields.Add("LastPageHitTime", SPFieldType.DateTime, false);
                lastHitTime = (SPFieldDateTime)IPList.Fields["LastPageHitTime"];
                lastHitTime.DisplayFormat = SPDateTimeFieldFormatType.DateTime;
                lastHitTime.Title = "Last Page Hit Time";
                // Remember to save changes
                lastHitTime.Update();
            }
            else
            {
                lastHitTime = (SPFieldDateTime)IPList.Fields["Last Page Hit Time"];
            }

            // Same for Url
            if (!IPList.Fields.ContainsField("Last Page Hit Url"))
            {
                IPList.Fields.Add("LastPageHitUrl", SPFieldType.Text, false);
                lastPageHitUrl = (SPFieldText)IPList.Fields["LastPageHitUrl"];
                lastPageHitUrl.Title = "Last Page Hit Url";
                // Remember to save changes
                lastPageHitUrl.Update();
            }
            else
            {
                lastPageHitUrl = (SPFieldText)IPList.Fields["Last Page Hit Url"];
            }

            // Same for Url
            if (!IPList.Fields.ContainsField("WebUrl"))
            {
                IPList.Fields.Add("WebUrl", SPFieldType.Text, false);
                webUrl = (SPFieldText)IPList.Fields["WebUrl"];
                webUrl.Title = "WebUrl";
                // Remember to save changes
                webUrl.Update();
            }
            else
            {
                webUrl = (SPFieldText)IPList.Fields["WebUrl"];
            }

            if (!IPList.Fields.ContainsField("ServerName"))
            {
                IPList.Fields.Add("ServerName", SPFieldType.Text, false);
                serverNameField = (SPFieldText)IPList.Fields["ServerName"];
                serverNameField.Title = "ServerName";
                // Remember to save changes
                serverNameField.Update();
            }
            else
            {
                serverNameField = (SPFieldText)IPList.Fields["ServerName"];
            }

            if (!IPList.Fields.ContainsField("AuthenticationType"))
            {
                IPList.Fields.Add("AuthenticationType", SPFieldType.Text, false);
                authenticationTypeField = (SPFieldText)IPList.Fields["AuthenticationType"];
                authenticationTypeField.Title = "AuthenticationType";
                // Remember to save changes
                authenticationTypeField.Update();
            }
            else
            {
                authenticationTypeField = (SPFieldText)IPList.Fields["AuthenticationType"];
            }




            // Get reference to default view.
            SPView defaultIPView = IPList.DefaultView;
            if (!SPCurrentUsersHelper.IsFieldInView(defaultIPView, "UserName"))
            {

                // Add the new column to the view...
                defaultIPView.ViewFields.Add(userName);

               
              
            }

            if (!SPCurrentUsersHelper.IsFieldInView(defaultIPView, "User"))
            {

                // Add the new column to the view...
                defaultIPView.ViewFields.Add(user);



            }

            if (!SPCurrentUsersHelper.IsFieldInView(defaultIPView, "Last Page Hit Url"))
            {

                // Add the new column to the view...
                defaultIPView.ViewFields.Add(lastPageHitUrl);
                    
               
                
            }
            if (!SPCurrentUsersHelper.IsFieldInView(defaultIPView, "Last Page Hit Time"))
            {

                // Add the new column to the view...
              
                defaultIPView.ViewFields.Add(lastHitTime);
             

            }

            if (!SPCurrentUsersHelper.IsFieldInView(defaultIPView, "WebUrl"))
            {
                defaultIPView.ViewFields.Add(webUrl);
            }


            if (!SPCurrentUsersHelper.IsFieldInView(defaultIPView, "ServerName"))
            {
                defaultIPView.ViewFields.Add(serverNameField);
            }

            if (!SPCurrentUsersHelper.IsFieldInView(defaultIPView, "AuthenticationType"))
            {
                defaultIPView.ViewFields.Add(authenticationTypeField);
            }


            defaultIPView.Query = "<OrderBy><FieldRef Name=\"LastPageHitTime\" Ascending=\"FALSE\" /><FieldRef Name=\"LastPageHitUrl\" Ascending=\"TRUE\" /><FieldRef Name=\"UserName\" Ascending=\"TRUE\" /></OrderBy>";
             // ...and save changes
            defaultIPView.Update();

            // Hide the list...
            IPList.Hidden = true;
            // ...and save changes
            IPList.Update();


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
