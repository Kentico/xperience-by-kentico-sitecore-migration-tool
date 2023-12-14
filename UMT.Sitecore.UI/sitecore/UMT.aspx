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
                var channels = UMTConfiguration.ChannelMapping.ChannelMaps;
                foreach(var channel in channels)
                {
                    var listItem = new ListItem
                    {
                        Value = channel.Id.ToString(),
                        Text = channel.DisplayName
                    };

                    Channel.Items.Add(listItem);
                }

                var languages = UMTConfiguration.SitecoreLanguages;
                foreach (var language in languages)
                {
                    var listItem = new ListItem
                    {
                        Value = language.Origin.ItemId.Guid.ToString(),
                        Text = language.Name
                    };

                    Languages.Items.Add(listItem);
                }
            }
        }
    
        protected void Button1_Click(object sender, EventArgs e)
        {
            ListBox1.Items.Clear();
            ListBox1.Items.Add("Pipeline triggered");
            var sourceChannel = UMTConfiguration.ChannelMapping.ChannelMaps.FirstOrDefault(x => x.Id.ToString() == Channel.SelectedValue);
            var args = new ExtractTemplatesArgs{ SourceChannel = sourceChannel};
            CorePipeline.Run("extractTemplates", args);
            ListBox1.Items.Add(args.TargetTemplates.Count + " templates mapped");

            var itemsArgs = new ExtractItemsArgs
            {
                SourceChannel = sourceChannel,
                ContentPaths = new List<string> { TextBox1.Text },
                SourceLanguages = Languages.GetSelectedIndices().Select(index => UMTConfiguration.SitecoreLanguages.ElementAt(index)).ToList()
                
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
        <asp:Label ID="lblLanguages" Text="Languages" AssociatedControlID="Languages" runat="server">
             <asp:ListBox ID="Languages" runat="server" Width="229px" SelectionMode="Multiple" Rows="10" />
        </asp:Label>
        <br/>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Run export" Width="234px" />
    
        <br />
        <asp:ListBox ID="ListBox1" runat="server" Height="314px" Width="557px" />
    
    </div>
    </form>
</body>
</html>
