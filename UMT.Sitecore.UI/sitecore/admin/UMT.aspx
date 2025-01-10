<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="UMT.Sitecore.Configuration" %>
<%@ Import Namespace="UMT.Sitecore.Jobs" %>
<%@ Import Namespace="Sitecore.Jobs.AsyncUI" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Universal Migration Toolkit</title>
    <link rel="shortcut icon" href="/sitecore/images/favicon.ico"/>
    <link rel="Stylesheet" type="text/css" href="/sitecore/shell/themes/standard/default/WebFramework.css"/>
    <style>
        .section {
            margin-right: 8px;
            margin-bottom: 10px;
            padding: 8px;
            border: 2px solid #DFDFDF;
        }
        
        .button {
            background-color: #228BE6;
            color: #FFFFFF;
            border-radius: 0.25rem;
            border: none;
            padding: 0.5rem;
        }
        .button:disabled {
            background-color: #d3d3d3;
        }
    </style>
</head>
<body>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            RefreshStatus();

            var channels = UMTConfiguration.ChannelMapping.ChannelMaps;
            foreach (var channel in channels)
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

    void RefreshTimer_Tick(object sender, EventArgs e)
    {
        RefreshStatus();
    }

    void RefreshStatus()
    {
        var job = UMTJob.Job;
        if (job != null)
        {
            IMessage message;
            while (job.MessageQueue.GetMessage(out message))
            {
                var umtMessage = (UMTJobMessage)message;
                if (umtMessage != null)
                {
                    if (umtMessage.IsManualCheck)
                    {
                        AddManualCheck(umtMessage.ManualCheck);
                    }
                    else
                    {
                        AddMessage(umtMessage.Message);
                    }
                }
            }

            if (job.IsDone)
            {
                AddMessage("Job is finished, processed items:" + job.Status.Processed + ". Job start time was " + job.QueueTime.ToLocalTime().ToString("T"));
                RefreshTimer.Enabled = false;
                btnRun.Enabled = true;
            }
            else
            {
                btnRun.Enabled = false;
            }

            ltStatus.Text = job.Status.State.ToString();
            if (job.Status.Total >= 0)
            {
                ltProcessed.Text = job.Status.Processed + " / " + job.Status.Total;
            }
            updMessages.Update();
        }
    }

    void AddMessage(string message)
    {
        tbMessages.Text += DateTime.Now.ToString("T") + "\t" + message + "\r\n";
    }
    
    void AddManualCheck(UMTJobManualCheck manualCheck)
    {
        tbManualChecks.Text += DateTime.Now.ToString("T") + "\t" + manualCheck + "\r\n";
        pnManualChecks.Visible = true;
    }

    protected void btnRun_Click(object sender, EventArgs e)
    {
        tbMessages.Text = string.Empty;
        tbManualChecks.Text = string.Empty;
        ltProcessed.Text = string.Empty;
        pnManualChecks.Visible = false;
        var sourceChannel = UMTConfiguration.ChannelMapping.ChannelMaps.FirstOrDefault(x => x.Id.ToString() == Channel.SelectedValue);
        var languages = Languages.GetSelectedIndices().Select(index => UMTConfiguration.SitecoreLanguages.ElementAt(index)).ToList();
        var contentPaths = ContentPaths.Text.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        var mediaPaths = MediaPaths.Text.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();

        new UMTJob().StartJob(NameSpace.Text, sourceChannel, contentPaths, languages, mediaPaths);
        AddMessage("Job started");
        RefreshStatus();

        RefreshTimer.Enabled = true;
        btnRun.Enabled = false;
        updMessages.Update();
    }
</script>
<body>
<form id="form1" runat="server" class="wf-container">
    <asp:ScriptManager ID="scriptManager" runat="Server"/>
    <div class="wf-content">
        <h1>Universal Migration Toolkit</h1>

        <div class="section">
            <h3>Content Settings</h3>
            <p></p>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblNameSpace" Text="Namespace" AssociatedControlID="Channel" runat="server"/>
                    </td>
                    <td>
                        <asp:TextBox ID="NameSpace" Width="400px" runat="server"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblChannel" Text="Channel" AssociatedControlID="Channel" runat="server"/>
                    </td>
                    <td>
                        <asp:DropDownList ID="Channel" Width="100%" runat="server"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblContentPaths" Text="Content roots" AssociatedControlID="ContentPaths" runat="server"/>
                    </td>
                    <td>
                        <asp:TextBox ID="ContentPaths" TextMode="MultiLine" Rows="5" Width="400px" runat="server"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblLanguages" Text="Languages" AssociatedControlID="Languages" runat="server" />
                    </td>
                    <td>
                        <asp:ListBox ID="Languages" Width="100%" SelectionMode="Multiple" Rows="10" runat="server"/>
                    </td>
                </tr>
            </table>
        </div>
        <div class="section">
            <h3>Media Settings</h3>
            <p></p>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblMediaPaths" Text="Media Folders: " AssociatedControlID="MediaPaths" runat="server"/>
                    </td>
                    <td>
                        <asp:TextBox ID="MediaPaths" TextMode="MultiLine" Rows="5" Width="400px" runat="server"/>
                    </td>
                </tr>
            </table>
        </div>
        <asp:UpdatePanel ID="updMessages" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="">
                    <asp:Button ID="btnRun" OnClick="btnRun_Click" Text="Run export" Width="200px" CssClass="button" runat="server"/>
                </div>
                <br/>

                <div class="section">
                    <table>
                        <tr>
                            <td><asp:Label ID="lblStatusText" Text="Status: " runat="server"/></td>
                            <td><asp:Literal ID="ltStatus" Text="N/A" runat="server"/></td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblProcessedText" Text="Processed items: " runat="server"/></td>
                            <td><asp:Literal ID="ltProcessed" Text="N/A" runat="server"/></td>
                        </tr>
                    </table>
                </div>
                <div class="section">
                    <h3>Progress</h3>
                    <p></p>
                    <asp:TextBox ID="tbMessages" Width="100%" TextMode="MultiLine" ReadOnly="True" Wrap="True" Rows="22" BorderStyle="None" runat="server"/>
                </div>
                <asp:Panel ID="pnManualChecks" CssClass="section" Visible="False" runat="server">
                    <h3>Items to review</h3>
                    <p>Please check the following items in case you would like to rename them in Sitecore or exclude from the export.</p>
                    <asp:TextBox ID="tbManualChecks" Width="100%" TextMode="MultiLine" ReadOnly="True" Wrap="True" Rows="22" BorderStyle="None" runat="server"/>
                </asp:Panel>
               
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="RefreshTimer" EventName="Tick"/>
            </Triggers>
        </asp:UpdatePanel>
        <asp:Timer ID="RefreshTimer" runat="server" Enabled="False" Interval="2000" OnTick="RefreshTimer_Tick"></asp:Timer>
    </div>
</form>
</body>
</html>