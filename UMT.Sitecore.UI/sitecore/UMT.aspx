<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="UMT.Sitecore.Pipelines.ExtractTemplates" %>
<%@ Import Namespace="Sitecore.Pipelines" %>
<%@ Import Namespace="UMT.Sitecore.Configuration" %>
<%@ Import Namespace="UMT.Sitecore.Pipelines.ExtractItems" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
    <script runat="server">
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var channels = UMTConfigurationManager.ChannelMapping.Channels;
                foreach(var channel in channels)
                {
                    var listItem = new ListItem
                    {
                        Value = channel.ChannelGUID.ToString(),
                        Text = channel.ChannelDisplayName
                    };

                    Channel.Items.Add(listItem);
                }
            }
        }
    
        protected void Button1_Click(object sender, EventArgs e)
        {
            ListBox1.Items.Clear();
            ListBox1.Items.Add("Pipeline triggered");
            var args = new ExtractTemplatesArgs();
            CorePipeline.Run("extractTemplates", args);
            ListBox1.Items.Add(args.TargetTemplates.Count + " templates mapped");

            var itemsArgs = new ExtractItemsArgs
            {
                Channel = UMTConfigurationManager.ChannelMapping.Channels.FirstOrDefault(x => x.ChannelGUID.ToString() == Channel.SelectedValue),
                ContentPaths = new List<string> { TextBox1.Text }
            };
            CorePipeline.Run("extractItems", itemsArgs);
            ListBox1.Items.Add(itemsArgs.TargetItems.Count + " items mapped");
        }
    
    </script>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="lblChannel" Text="Channel" AssociatedControlID="Channel" runat="server">
            <asp:DropDownList ID="Channel" runat="server" Width="229px" />
        </asp:Label>
        <br/>
        <asp:TextBox ID="TextBox1" runat="server" Width="229px"></asp:TextBox>
        <br />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Run export" Width="234px" />
    
        <br />
        <asp:ListBox ID="ListBox1" runat="server" Height="314px" Width="557px" />
    
    </div>
    </form>
</body>
</html>
