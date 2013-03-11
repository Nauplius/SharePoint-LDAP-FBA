<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Assembly Name="Microsoft.SharePoint.IdentityModel, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="Microsoft.SharePoint.WebControls" %>

<%@ Register Tagprefix="SharePoint" 
    Namespace="Microsoft.SharePoint.WebControls" 
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" 
    Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" 
    Inherits="Nauplius.ADLDS.FBA.Layouts.Nauplius.ADLDS.FBA.login" MasterPageFile="~/_layouts/simple.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderPageTitle"
    runat="server">
    <SharePoint:EncodedLiteral runat="server"
        EncodeMethod="HtmlEncode" ID="ClaimsFormsPageTitle"
        Visible="false" />
    Forms Login
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea"
    runat="server">
    <SharePoint:EncodedLiteral runat="server"
        EncodeMethod="HtmlEncode" ID="ClaimsFormsPageTitleInTitleArea"
        Visible="false" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSiteName"
    runat="server" />
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMain"
    runat="server">
    <SharePoint:EncodedLiteral runat="server"
        EncodeMethod="HtmlEncode" ID="ClaimsFormsPageMessage"
        Visible="false" />
    <asp:Login ID="signInControl" FailureText="<%$Resources:wss,login_pageFailureText%>"
        runat="server" Width="100%" DisplayRememberMe="false" />
</asp:Content>