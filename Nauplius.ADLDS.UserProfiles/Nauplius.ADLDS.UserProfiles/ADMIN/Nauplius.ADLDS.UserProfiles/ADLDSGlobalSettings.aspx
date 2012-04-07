<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ADLDSGlobalSettings.aspx.cs" Inherits="Nauplius.ADLDS.UserProfiles.Layouts.Nauplius.ADLDS.UserProfiles.ADLDSGlobalSettings" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<table>
    <tr
</table>
Claims Identifier: <asp:TextBox ID="tBCI" runat="server" /><br />
LDAP Filter: <asp:TextBox ID="tBLF" runat="server" /><br />
Delete Profiles? <asp:CheckBox ID="cBDelProf" runat="server" /><br />
Department: <asp:TextBox ID="tBDept" runat="server" /><br />
First Name: <asp:TextBox ID="tBFN" runat="server" /><br />
Last Name: <asp:TextBox ID="tBLN" runat="server" /><br />
Office: <asp:TextBox ID="tBOff" runat="server" /><br />
Preferred Name: <asp:TextBox ID="tBPN" runat="server" /><br />
Title: <asp:TextBox ID="tBTitle" runat="server" /><br />
Web Site: <asp:TextBox ID="tBWS" runat="server" /><br />
Work Email: <asp:TextBox ID="tBWE" runat="server" /><br />
Work Phone: <asp:TextBox ID="tBWP" runat="server" /><br />
<br />
<br />
<asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_OnUpdate" />
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Nauplius - AD LDS/ADAM User Profile Global Configuration
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
AD LDS/ADAM User Profile Global Configuration
</asp:Content>
