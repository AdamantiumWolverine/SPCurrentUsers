<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WebAppStats.aspx.cs" MasterPageFile="~/_layouts/application.master"
Inherits="WebAppStats"  %>

<asp:Content ID="Content_Title" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
Current Users Summary for Web Application 
    <asp:Label ID="lblWebAppTitle" runat="server" Text=""></asp:Label>
</asp:Content>

<asp:Content ID="Content_PlaceHolderMain" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
     
           
    <div style="text-align: center;">
        <asp:Label ID="lblOutput" runat="server" Text=""></asp:Label>
    </div>
    
</asp:Content>