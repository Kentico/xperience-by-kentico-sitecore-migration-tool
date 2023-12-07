<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="UMT.Sitecore.Pipelines.ExtractTemplates" %>
<%@ Import Namespace="Sitecore.Pipelines" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
    <script runat="server">
        protected void Button1_Click(object sender, EventArgs e)
        {
            ListBox1.Items.Clear();
            ListBox1.Items.Add("Pipeline triggered");
            var args = new ExtractTemplatesArgs();
            CorePipeline.Run("extractTemplates", args);
            ListBox1.Items.Add(args.TargetTemplates.Count + " templates mapped");
        }
    </script>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:TextBox ID="TextBox1" runat="server" Width="229px"></asp:TextBox>
        <br />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Run export" Width="234px" />
    
        <br />
        <asp:ListBox ID="ListBox1" runat="server" Height="314px" Width="557px"></asp:ListBox>
    
    </div>
    </form>
</body>
</html>
