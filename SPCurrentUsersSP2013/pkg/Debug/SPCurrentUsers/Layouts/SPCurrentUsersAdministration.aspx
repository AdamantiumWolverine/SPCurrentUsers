<%@ Assembly Name="SPCurrentUsers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=55ce97ca144856f2" %>

<%@ Page Language="C#" MasterPageFile="~/_layouts/application.master" Inherits="SPCurrentUsers.SPCurrentUsersAdministration" %>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div style="text-align: center; margin: 10px;">
         
    <%-- 
    <asp:HyperLink ID="HyperLinkModifyPages" runat="server" NavigateUrl="/Lists/SPCurrentUsersPages/AllItems.aspx">
     Modify page timeouts</asp:HyperLink>    
     <asp:HyperLink ID="hlUserList" runat="server" NavigateUrl="" Visible ="false">
     View User List</asp:HyperLink>       
     --%>
      <asp:HyperLink ID="hlViewUserTrackingList" runat="server" NavigateUrl="">
     View SPCurrentUser User Tracking List for this Site Collection</asp:HyperLink> 
     
      | <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/_layouts/custom/SPCurrentUsers/WebAppStats.aspx">View Stats For Entire Web Application</asp:HyperLink>
      | <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/_layouts/custom/SPCurrentUsers/DeleteOldEntries.aspx">Clean the SPCurrent User Tracker list</asp:HyperLink>
     
    
     
    </div>
      <asp:Label ID="lblUpdateResults" runat="server" Text=""></asp:Label>
    <asp:TextBox ID="TextBoxMinutesPerSession" runat="server" Width="50px">15</asp:TextBox>       
    <asp:Label ID="LabelMinutesPerSession" runat="server" Text="Default session timeout in minutes"></asp:Label>
    <br />
    
    <asp:CheckBox ID="cbDisplayCurrentUsersInSiteActions" runat="server" />
    <asp:Label
        ID="lblDisplayCurrentUsersInSiteActions" runat="server" Text="Display the Count of Current Users in the Site Actions menu"></asp:Label>
        
        <br />
        
        <asp:CheckBox ID="cbDebugMode" runat="server" />
    <asp:Label
        ID="lblDebugMode" runat="server" Text="Display debug output for the Delegate Control"></asp:Label>
        <br />
    <asp:Button ID="LinkButtonUpdate" runat="server" Text="Update"></asp:Button>
  <br />
  
    <br />
    <asp:Label ID="LabelResults" runat="server"></asp:Label>
    <br />
    <br />
    <asp:Label ID="LabelPageOverview" runat="server" Text="<strong>Pages and Current Users</strong>"></asp:Label>
    &nbsp; &nbsp;
   
    <asp:GridView ID="GridViewPages" runat="server" AutoGenerateColumns="True" Font-Size="X-Small">
    </asp:GridView>
</asp:Content>
