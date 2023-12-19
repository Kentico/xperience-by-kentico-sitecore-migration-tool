<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="UMT.Sitecore.Configuration" %>
<%@ Import Namespace="UMT.Sitecore.Jobs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Universal Migration Toolkit</title>
    <link rel="shortcut icon" href="/sitecore/images/favicon.ico"/>
    <link rel="Stylesheet" type="text/css" href="/sitecore/shell/themes/standard/default/WebFramework.css"/>
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

            var mediaLibraries = UMTConfiguration.MediaMapping.MediaMaps;
            foreach (var mediaLibrary in mediaLibraries)
            {
                var listItem = new ListItem
                {
                    Value = mediaLibrary.Id.ToString(),
                    Text = mediaLibrary.DisplayName
                };

                MediaLibrary.Items.Add(listItem);
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
            while (job.MessageQueue.GetMessage(out var message))
            {
                var umtMessage = (UMTJobMessage)message;
                if (umtMessage != null)
                {
                    AddMessage(umtMessage.Message);
                }
            }
            
            if (!job.IsDone)
            {
                if (job.Status.Total > 0)
                {
                    AddMessage("Job is running, processed items:" + job.Status.Processed + "/" + job.Status.Total);
                }
            }
            else
            {
                AddMessage("Job is finished, processed items:" + job.Status.Processed + ". Job start date and time was " + job.QueueTime.ToString("u"));
                RefreshTimer.Enabled = false;
                btnRun.Enabled = true;
            }
            updMessages.Update();
        }
    }

    void AddMessage(string message)
    {
        lbMessages.Items.Add(DateTime.Now.ToString("u") + " " + message);
        lbMessages.SelectedIndex = lbMessages.Items.Count - 1;
    }

    protected void btnRun_Click(object sender, EventArgs e)
    {
        lbMessages.Items.Clear();
        var sourceChannel = UMTConfiguration.ChannelMapping.ChannelMaps.FirstOrDefault(x => x.Id.ToString() == Channel.SelectedValue);
        var sourceMediaLibrary = UMTConfiguration.MediaMapping.MediaMaps.FirstOrDefault(x => x.Id.ToString() == MediaLibrary.SelectedValue);
        
        new UMTJob().StartJob(NameSpace.Text, sourceChannel, new List<string> { TextBox1.Text }, Languages.GetSelectedIndices().Select(index => UMTConfiguration.SitecoreLanguages.ElementAt(index)).ToList());
        AddMessage("Job started at " + UMTJob.Job.QueueTime.ToString("u"));

        RefreshTimer.Enabled = true;
        btnRun.Enabled = false;
        updMessages.Update();
    }
</script>
<body>
<form id="form1" runat="server" class="wf-container">
    <asp:ScriptManager ID="scriptManager" runat="Server"/>
    <div class="wf-content">
        <asp:Label ID="lblNameSpace" Text="Namespace (for data class name and table name)" AssociatedControlID="Channel" runat="server">
            <asp:TextBox ID="NameSpace" runat="server" Width="229px"/>
        </asp:Label>
        <br/>
        <asp:Label ID="lblChannel" Text="Channel" AssociatedControlID="Channel" runat="server">
            <asp:DropDownList ID="Channel" runat="server" Width="229px"/>
        </asp:Label>
        <br/>
        <asp:TextBox ID="TextBox1" runat="server" Width="229px"></asp:TextBox>
        <br/>
        <asp:Label ID="lblLanguages" Text="Languages" AssociatedControlID="Languages" runat="server">
            <asp:ListBox ID="Languages" runat="server" Width="229px" SelectionMode="Multiple" Rows="10"/>
        </asp:Label>
        <br/>
        <asp:Label ID="lblMediaLibrary" Text="Media Library" AssociatedControlID="MediaLibrary" runat="server">
            <asp:DropDownList ID="MediaLibrary" runat="server" Width="229px"/>
        </asp:Label>
        <br/>
        <asp:Label ID="lblMediaPaths" Text="Media Folders" AssociatedControlID="MediaPaths" runat="server">
            <asp:TextBox ID="MediaPaths" runat="server" Width="229px"/>
        </asp:Label>
        <br/>
        <asp:UpdatePanel ID="updMessages" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Button ID="btnRun" runat="server" OnClick="btnRun_Click" Text="Run export" Width="234px"/>

                <br/>
                <asp:ListBox ID="lbMessages" runat="server" Height="500px" Width="100%" BorderStyle="None"/>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="RefreshTimer" EventName="Tick"/>
            </Triggers>
        </asp:UpdatePanel>
        <asp:Timer ID="RefreshTimer" runat="server" Interval="5000" OnTick="RefreshTimer_Tick"></asp:Timer>
    </div>
</form>
</body>
</html>