﻿module CreatingOrgsAndCampaigns

open canopy
open runner
open System
open Pages

let All baseUrl =
    let testOrganizationName = Utils.GetScenarioTestName "My test project for Admin"
    let testCampaignName = Utils.GetScenarioTestName "My test campaign for Admin"
 
    context "allReady Admin Activities"

    "Login Admin" &&& fun _ ->
        Actions.GoToHomePage baseUrl
        let sceenshotName  = Utils.TimestampName "HomePage"
        screenshot "c:\screenshots" sceenshotName |> ignore
        Actions.Login Constants.SiteAdminUserName Constants.SiteAdminPassword baseUrl

    "Admin can Navigate to Organizations" &&& fun _ ->
        TopMenu.SelectAdminOrganizations()

        on AdminOrganizations.RelativeUrl
        "h2" == "Currently active organizations"
        title() |> is "Currently active organizations - allReady"

    "Admin can create organization" &&& fun _ ->
        click "#CreateNew"
        AdminOrganizationCreate.PopulateOrganizationDetails 
            {AdminOrganizationCreate.DefaultOrganizationDetails with 
                Name = testOrganizationName;
                WebUrl="htbox.org";}
        AdminOrganizationCreate.Save()

        "div h2" *= testOrganizationName

    "Admin can navigate to campaigns" &&& fun _ ->
        TopMenu.SelectAdminCampaigns()

        on AdminCampaigns.RelativeUrl
        "h2" == "Campaigns - Admin"
        title() |> is "Campaigns - Admin - allReady"

    "Admin can create new campaign" &&& fun _ ->
        AdminCampaigns.SelectCreateNew()
        AdminCampaignCreate.PopulateCampaignDetails 
            {AdminCampaignCreate.DefaultCampaignDetails with 
                Name = testCampaignName; 
                Description = "test"; 
                FullDescription = "Full Description"; 
                OrganizationName = testOrganizationName;}
        AdminCampaignCreate.Submit()

        on AdminCampaignDetails.RelativeUrl
        "h2" == testCampaignName
        
        TopMenu.SelectCampaigns()
        "td a" *= testCampaignName

    "Admin can create a campaign from public campaign page" &&& fun _ ->
        Campaigns.SelectCreateNew()
        AdminCampaignCreate.PopulateCampaignDetails 
            {AdminCampaignCreate.DefaultCampaignDetails with 
                Name = testCampaignName; 
                Description = "test"; 
                FullDescription = "Full Description"; 
                OrganizationName = testOrganizationName }
        AdminCampaignCreate.Submit()

        on AdminCampaignDetails.RelativeUrl
        "h2" == testCampaignName

    "Admin can create events" &&& fun _ ->
        let eventName = Utils.GetScenarioTestName "First Test Event"
        let eventNameSelector = "[data-event-title]"

        AdminCampaignDetails.CreateNewEvent()
        AdminEventCreate.PopulateEventdetails
            {AdminEventCreate.DefaultEventDetails with
                Name = eventName               
            }
        AdminEventCreate.Create()

        on AdminEventDetails.RelativeUrl

        let eventNameValue = read <| element eventNameSelector
        eventNameValue |> contains eventName        

    "Admin can create task" &&& fun _ ->
        let taskName = Utils.GetScenarioTestName "First Test Task"
        AdminEventDetails.CreateNewTask()
        AdminTaskCreate.PopulateTaskDetails 
            {AdminTaskCreate.DefaultTaskDetails with 
                Name = taskName
            }

        AdminTaskCreate.Create()

        on AdminEventDetails.RelativeUrl
        "td a" *= taskName

    "Admin can logout" &&& fun _ ->
        click ".log-out"
    