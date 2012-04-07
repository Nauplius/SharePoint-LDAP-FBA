<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ADLDSWebApplicationSettings.aspx.cs" Inherits="Nauplius.ADLDS.UserProfiles.Layouts.Nauplius.ADLDS.UserProfiles.ADLDSWebApplicationSettings" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<SharePoint:WebApplicationSelector ID="ddlWebApp" runat="server" OnContextChange="WebAppSelector_OnChanged" />
AD LDS/ADAM Server Name: <asp:TextBox ID="tBSN" runat="server" /><br />
AD LDS/ADAM Port Number: <asp:TextBox ID="tBPrtNo" runat="server" /><br />
Valid values are: 389, 636, or 1024 through 65535<br />
AD LDS/ADAM Distinguished Name: <asp:TextBox ID="tBDNPath" runat="server" /><br />
Use SSL: <asp:CheckBox ID="cBUseSSL" runat="server" /><br />
AD LDS/ADAM User Login Attribute: <asp:TextBox ID="tBLoginAttrib" runat="server" /><br />
<br />
<br />
<asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_OnSave" />
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Nauplius - AD LDS/ADAM User Profile Web Application Configuration
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
AD LDS/ADAM User Profile Web Application Configuration
</asp:Content>
