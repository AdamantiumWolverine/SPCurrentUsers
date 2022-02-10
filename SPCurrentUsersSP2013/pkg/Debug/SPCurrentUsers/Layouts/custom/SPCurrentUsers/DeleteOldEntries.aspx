<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DeleteOldEntries.aspx.cs" MasterPageFile="~/_layouts/application.master"
Inherits="DeleteOldEntries"  %>

<asp:Content ID="Content_Title" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
Delete Old Entries
    <asp:Label ID="lblWebAppTitle" runat="server" Text=""></asp:Label>
</asp:Content>

<asp:Content ID="Content_PlaceHolderMain" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">                
<div style="text-align: center;">
<h1>SPCurrentUser List Management</h1><br />

    <asp:HyperLink ID="hlBack" runat="server">Back to SPCurrentUser Admin Page</asp:HyperLink>
    </div>
<br />
<br />
This web page allows you to clean up all SPCurrentUserTrackerList.  This helps speed things up if your list gets too large.<br />
<br />


    <div style="text-align: left;">

        Delete records older than
        <asp:DropDownList ID="ddlNumYears" runat="server">
        <asp:ListItem Value="3"></asp:ListItem>
        <asp:ListItem Value="2"></asp:ListItem>
        <asp:ListItem Value="1"></asp:ListItem>
        </asp:DropDownList>
        Years.
        <br />
          <br />
        Delete Up To
        <asp:DropDownList ID="ddlRowLimit" runat="server">
         <asp:ListItem Value="5"></asp:ListItem>
        <asp:ListItem Value="10"></asp:ListItem>
        <asp:ListItem Value="50"></asp:ListItem>
        <asp:ListItem Value="100"></asp:ListItem>
        <asp:ListItem Value="1000"></asp:ListItem>
        </asp:DropDownList>
        records per site collection.
        <br />
        <br />

       <asp:Button ID="btnDelete" runat="server" Text="Delete Old Entries" OnClick="Delete_Old_Entries_Clicked" OnClientClick="PleaseWait();" />

        <script language="javascript">
            function PleaseWait() {
                var btn = document.getElementById("ctl00_PlaceHolderMain_btnDelete");
                btn.enabled = false;
                btn.value = "Please Wait....Running Script....Decrease the Row Limit for faster page response times...";
            }
        </script>

        <hr />
        <asp:Label ID="lblOutput" runat="server" Text="" ></asp:Label>
    </div>
    
</asp:Content>
