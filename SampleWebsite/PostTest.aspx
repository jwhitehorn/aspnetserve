<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PostTest.aspx.cs" Inherits="PostTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="What is your name?"></asp:Label>
        <asp:TextBox ID="txtName" runat="server" Width="291px"></asp:TextBox>
        <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Submit" /><br />
        <hr />
        <br />
        <asp:Panel ID="Panel1" runat="server" Height="160px" Width="495px" Visible="false">
            <asp:Label ID="lblWelcome" runat="server" Text="Label"></asp:Label>
            <asp:LinkButton ID="btnClear" runat="server" OnClick="btnClear_Click">clear</asp:LinkButton></asp:Panel>
    
    </div>
    </form>
</body>
</html>
