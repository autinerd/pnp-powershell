﻿using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;

using PnP.Framework;
using PnP.Framework.Entities;
using PnP.PowerShell.Commands.Utilities;

using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace PnP.PowerShell.Commands.Site
{
    [Cmdlet(VerbsCommon.Set, "PnPSite")]
    [OutputType(typeof(void))]
    public class SetSite : PnPSharePointCmdlet
    {
        private const string ParameterSet_LOCKSTATE = "Set Lock State";
        private const string ParameterSet_PROPERTIES = "Set Properties";

        [Parameter(Mandatory = false)]
        [Alias("Url")]
        public string Identity;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public string Classification;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public SwitchParameter? DisableFlows;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public string LogoFilePath;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        [Alias("Sharing")]
        public SharingCapabilities? SharingCapability = null;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public long? StorageMaximumLevel = null;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public long? StorageWarningLevel = null;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_LOCKSTATE)]
        public SiteLockState? LockState;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public SwitchParameter? AllowSelfServiceUpgrade = null;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        [Alias("DenyAndAddCustomizePages", "DenyAddAndCustomizePages")]
        public SwitchParameter? NoScriptSite;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public List<string> Owners;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public SwitchParameter? CommentsOnSitePagesDisabled;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public SharingPermissionType? DefaultLinkPermission;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public SharingLinkType? DefaultSharingLinkType;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public bool? DefaultLinkToExistingAccess;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public SwitchParameter DefaultLinkToExistingAccessReset;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public AppViewsPolicy? DisableAppViews;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public CompanyWideSharingLinksPolicy? DisableCompanyWideSharingLinks;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public SwitchParameter DisableSharingForNonOwners;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public uint? LocaleId;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public RestrictedToRegion? RestrictedToGeo;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public SwitchParameter? SocialBarOnSitePagesDisabled;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public int? AnonymousLinkExpirationInDays;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public SwitchParameter OverrideTenantAnonymousLinkExpirationPolicy;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public MediaTranscriptionPolicyType? MediaTranscription;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public Guid? SensitivityLabel;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public bool? RequestFilesLinkEnabled;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public int? RequestFilesLinkExpirationInDays;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public string ScriptSafeDomainName;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public bool? RestrictedAccessControl;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public bool? BlockDownloadPolicy;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public bool? ExcludeBlockDownloadPolicySiteOwners;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public Guid[] ExcludedBlockDownloadGroupIds;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public bool? ListsShowHeaderAndNavigation;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_LOCKSTATE)]
        public SwitchParameter Wait;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public bool EnableAutoExpirationVersionTrim;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public int ExpireVersionsAfterDays;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public int MajorVersions;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public int MinorVersions;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public SwitchParameter InheritTenantVPForNewDocLibs;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public SwitchParameter ApplyForNewLibs;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public SwitchParameter ApplyForExistingLibs;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_PROPERTIES)]
        public SwitchParameter CancelVPForExistingLibs;

        protected override void ExecuteCmdlet()
        {
            var context = ClientContext;
            var site = ClientContext.Site;
            var siteUrl = ClientContext.Url;

            var executeQueryRequired = false;

            if (!string.IsNullOrEmpty(Identity))
            {
                context = ClientContext.Clone(Identity);
                site = context.Site;
                siteUrl = context.Url;
            }

            if (ParameterSpecified(nameof(Classification)))
            {
                site.Classification = Classification;
                context.ExecuteQueryRetry();
            }

            if (ParameterSpecified(nameof(SensitivityLabel)) && SensitivityLabel.HasValue)
            {
                site.SensitivityLabel = SensitivityLabel.Value;
                context.ExecuteQueryRetry();
            }

            if (ParameterSpecified(nameof(ScriptSafeDomainName)) && !string.IsNullOrEmpty(ScriptSafeDomainName))
            {
                ScriptSafeDomain safeDomain = null;
                try
                {
                    safeDomain = ClientContext.Site.CustomScriptSafeDomains.GetByDomainName(ScriptSafeDomainName);
                    ClientContext.Load(safeDomain);
                    ClientContext.ExecuteQueryRetry();
                }
                catch { }
                if (safeDomain.ServerObjectIsNull == null)
                {
                    ScriptSafeDomainEntityData scriptSafeDomainEntity = new ScriptSafeDomainEntityData
                    {
                        DomainName = ScriptSafeDomainName
                    };

                    safeDomain = context.Site.CustomScriptSafeDomains.Create(scriptSafeDomainEntity);
                    context.Load(safeDomain);
                    context.ExecuteQueryRetry();
                    WriteObject(safeDomain);
                }
                else
                {
                    WriteWarning($"Unable to add Domain Name as there is an existing domain name with the same name. Will be skipped.");
                }
            }

            if (ParameterSpecified(nameof(LogoFilePath)))
            {
                if (!System.IO.Path.IsPathRooted(LogoFilePath))
                {
                    LogoFilePath = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, LogoFilePath);
                }
                if (System.IO.File.Exists(LogoFilePath))
                {
                    site.EnsureProperty(s => s.GroupId);
                    if (site.GroupId != Guid.Empty)
                    {
                        var bytes = System.IO.File.ReadAllBytes(LogoFilePath);

                        var mimeType = "";
                        if (LogoFilePath.EndsWith("gif", StringComparison.InvariantCultureIgnoreCase))
                        {
                            mimeType = "image/gif";
                        }
                        if (LogoFilePath.EndsWith("jpg", StringComparison.InvariantCultureIgnoreCase) || LogoFilePath.EndsWith("jpeg", StringComparison.InvariantCultureIgnoreCase))
                        {
                            mimeType = "image/jpeg";
                        }
                        if (LogoFilePath.EndsWith("png", StringComparison.InvariantCultureIgnoreCase))
                        {
                            mimeType = "image/png";
                        }
                        var result = Framework.Sites.SiteCollection.SetGroupImageAsync(context, bytes, mimeType).GetAwaiter().GetResult();
                    }

                    var webTemplateId = context.Web.GetBaseTemplateId();
                    System.IO.FileInfo file = new System.IO.FileInfo(LogoFilePath);

                    var createdList = context.Web.Lists.EnsureSiteAssetsLibrary();
                    context.Web.Context.Load(createdList, l => l.RootFolder);
                    context.Web.Context.ExecuteQueryRetry();

                    var logoFileName = file.Name;
                    if (webTemplateId == "SITEPAGEPUBLISHING#0" || webTemplateId == "STS#3" || webTemplateId == "GROUP#0")
                    {
                        logoFileName = "__sitelogo__" + file.Name;
                    }

                    var uploadedFile = createdList.RootFolder.UploadFile(logoFileName, LogoFilePath, true);
                    context.Web.SiteLogoUrl = uploadedFile.ServerRelativeUrl;
                    context.Web.Update();
                    context.ExecuteQueryRetry();
                }
                else
                {
                    throw new Exception("Logo file does not exist");
                }
            }

            if (ParameterSpecified(nameof(InheritTenantVPForNewDocLibs)))
            {
                if (ParameterSpecified(nameof(EnableAutoExpirationVersionTrim)) ||
                    ParameterSpecified(nameof(ExpireVersionsAfterDays)) ||
                    ParameterSpecified(nameof(MajorVersions)) ||
                    ParameterSpecified(nameof(MinorVersions)) ||
                    ParameterSpecified(nameof(ApplyForNewLibs)) ||
                    ParameterSpecified(nameof(ApplyForExistingLibs)) ||
                    ParameterSpecified(nameof(CancelVPForExistingLibs)))
                {
                    throw new PSArgumentException($"Don't specify version policy related parameters (EnableAutoExpirationVersionTrim, ExpireVersionsAfterDays, MajorVersions, MinorVersions, ApplyForNewLibs, ApplyForExistingLibs, CancelVPForExistingLibs) when InheritTenantVPForNewDocLibs is specified.");
                }

                site.EnsureProperty(s => s.VersionPolicyForNewLibrariesTemplate);
                site.VersionPolicyForNewLibrariesTemplate.InheritTenantSettings();
                context.ExecuteQueryRetry();
                WriteWarning("The setting for new libraries takes effect immediately. Please run Get-PnPSiteVersionPolicyForNewLibrary to display the newly set values.");
            }
            else
            {
                if (ParameterSpecified(nameof(CancelVPForExistingLibs)))
                {
                    if (ParameterSpecified(nameof(ApplyForNewLibs)) ||
                        ParameterSpecified(nameof(ApplyForExistingLibs)) ||
                        ParameterSpecified(nameof(EnableAutoExpirationVersionTrim)) ||
                        ParameterSpecified(nameof(ExpireVersionsAfterDays)) ||
                        ParameterSpecified(nameof(MajorVersions)) ||
                        ParameterSpecified(nameof(MinorVersions)))
                    {
                        throw new PSArgumentException($"Don't specify the version policy related parameters (ApplyForNewLibs, ApplyForExistingLibs, EnableAutoExpirationVersionTrim, ExpireVersionsAfterDays, MajorVersions) when CancelVPForExistingLibs is specified.");
                    }

                    site.CancelSetVersionPolicyForDocLibs();
                    context.ExecuteQueryRetry();
                }
                else
                {
                    // There are 4 scenarios for parameters ApplyForNewLibs and ApplyForExistingLibs
                    // Scenario 1: ApplyForNewLibs only
                    // Scenario 2: ApplyForExistingLibs only
                    // Scenario 3: Both ApplyForNewLibs and ApplyForExistingLibs
                    // Scenario 4: Neither ApplyForNewLibs or ApplyForExistingLibs
                    // For Scenario 3 & 4, they should be the same, set both new doc libs and existing doc libs
                    // Only scenario 1 does not require MinorVersions when EnableAutoExpirationVersionTrim is false because minor version is disabled on new doc libs

                    if (ParameterSpecified(nameof(EnableAutoExpirationVersionTrim)))
                    {
                        // Validate parameters when EnableAutoExpirationVersionTrim is specified
                        if (EnableAutoExpirationVersionTrim)
                        {
                            if (ParameterSpecified(nameof(ExpireVersionsAfterDays)) ||
                                ParameterSpecified(nameof(MajorVersions)) ||
                                ParameterSpecified(nameof(MinorVersions)))
                            {
                                throw new PSArgumentException($"Don't specify ExpireVersionsAfterDays, MajorVersions and MinorVersions when EnableAutoExpirationVersionTrim is true.");
                            }
                        }
                        else
                        {
                            if (ParameterSpecified(nameof(ApplyForNewLibs)) &&
                                !ParameterSpecified(nameof(ApplyForExistingLibs)))
                            {
                                // If Scenario 1: ApplyForNewLibs only
                                // MinorVerions is not needed
                                if (!ParameterSpecified(nameof(ExpireVersionsAfterDays)) ||
                                    !ParameterSpecified(nameof(MajorVersions)) ||
                                    ParameterSpecified(nameof(MinorVersions)))
                                {
                                    throw new PSArgumentException($"You must specify ExpireVersionsAfterDays, MajorVersions and don't specify MinorVersions when EnableAutoExpirationVersionTrim is false for new document libraries only.");
                                }
                            }
                            else
                            {
                                if (!ParameterSpecified(nameof(ExpireVersionsAfterDays)) ||
                                    !ParameterSpecified(nameof(MajorVersions)) ||
                                    !ParameterSpecified(nameof(MinorVersions)))
                                {
                                    throw new PSArgumentException($"You must specify ExpireVersionsAfterDays, MajorVersions and MinorVersions when EnableAutoExpirationVersionTrim is false for document libraries that including existing ones.");
                                }
                            }
                        }

                        // Do setting when EnableAutoExpirationVersionTrim is specified
                        if (!(!ParameterSpecified(nameof(ApplyForNewLibs)) &&
                            ParameterSpecified(nameof(ApplyForExistingLibs))))
                        {
                            // If NOT "Scenario 2: ApplyForExistingLibs only"
                            // Do setting for new doc libs
                            if (EnableAutoExpirationVersionTrim)
                            {
                                site.EnsureProperty(s => s.VersionPolicyForNewLibrariesTemplate);
                                site.VersionPolicyForNewLibrariesTemplate.SetAutoExpiration();
                                context.ExecuteQueryRetry();
                            }
                            else
                            {
                                site.EnsureProperty(s => s.VersionPolicyForNewLibrariesTemplate);
                                if (ExpireVersionsAfterDays == 0)
                                {
                                    site.VersionPolicyForNewLibrariesTemplate.SetNoExpiration(MajorVersions);
                                    context.ExecuteQueryRetry();
                                }
                                else
                                {
                                    site.VersionPolicyForNewLibrariesTemplate.SetExpireAfter(MajorVersions, ExpireVersionsAfterDays);
                                    context.ExecuteQueryRetry();
                                }
                            }

                            WriteWarning("The setting for new libraries takes effect immediately. Please run Get-PnPSiteVersionPolicyForNewLibrary to display the newly set values.");
                        }

                        if (!(ParameterSpecified(nameof(ApplyForNewLibs)) &&
                            !ParameterSpecified(nameof(ApplyForExistingLibs))))
                        {
                            // If NOT "Scenario 1: ApplyForNewLibs only"
                            // Create setting request for existing doc libs
                            if (EnableAutoExpirationVersionTrim)
                            {
                                site.StartSetVersionPolicyForDocLibs(true, -1, -1, -1);
                                context.ExecuteQueryRetry();
                            }
                            else
                            {
                                site.StartSetVersionPolicyForDocLibs(false, MajorVersions, MinorVersions, ExpireVersionsAfterDays);
                                context.ExecuteQueryRetry();
                            }

                            WriteWarning("The setting for existing libraries takes at least 24 hours to take effect. Please run Get-PnPSiteVPSettingProgressForExistingLibrary to check the progress.");
                        }
                    }
                    else
                    {
                        if (ParameterSpecified(nameof(ApplyForNewLibs)) ||
                            ParameterSpecified(nameof(ApplyForExistingLibs)) )
                        {
                            throw new PSArgumentException($"You must specify EnableAutoExpirationVersionTrim and other version policy related parameters (ExpireVersionsAfterDays, MajorVersions, MinorVersions) when ApplyForNewLibs or ApplyForExistingLibs is specified.");
                        }

                        if (ParameterSpecified(nameof(ExpireVersionsAfterDays)) ||
                            ParameterSpecified(nameof(MajorVersions)) ||
                            ParameterSpecified(nameof(MinorVersions)))
                        {
                            throw new PSArgumentException($"You must specify EnableAutoExpirationVersionTrim when ExpireVersionsAfterDays, MajorVersions or MinorVersions is specified.");
                        }
                    }
                }
            }

            if (IsTenantProperty())
            {
                var tenantAdminUrl = UrlUtilities.GetTenantAdministrationUrl(context.Url);
                context = context.Clone(tenantAdminUrl);

                executeQueryRequired = false;
                Func<TenantOperationMessage, bool> timeoutFunction = TimeoutFunction;
                Tenant tenant = new Tenant(context);
                var siteProperties = tenant.GetSitePropertiesByUrl(siteUrl, false);
                tenant.Context.Load(siteProperties);
                tenant.Context.ExecuteQueryRetry();

                if (ParameterSpecified(nameof(OverrideTenantAnonymousLinkExpirationPolicy)))
                {
                    siteProperties.OverrideTenantAnonymousLinkExpirationPolicy = OverrideTenantAnonymousLinkExpirationPolicy.ToBool();
                    executeQueryRequired = true;
                }
                if (ParameterSpecified(nameof(AnonymousLinkExpirationInDays)) && AnonymousLinkExpirationInDays.HasValue)
                {
                    siteProperties.AnonymousLinkExpirationInDays = AnonymousLinkExpirationInDays.Value;
                    executeQueryRequired = true;
                }
                if (LockState.HasValue)
                {
                    tenant.SetSiteLockState(siteUrl, LockState.Value, Wait, Wait ? timeoutFunction : null);
                    WriteWarning("You changed the lockstate of this site. This change is not guaranteed to be effective immediately. Please wait a few minutes for this to take effect.");
                }
                if (Owners != null && Owners.Count > 0)
                {
                    var admins = new List<UserEntity>();
                    foreach (var owner in Owners)
                    {
                        var userEntity = new UserEntity { LoginName = owner };
                        admins.Add(userEntity);
                    }
                    tenant.AddAdministrators(admins, new Uri(siteUrl));
                }
                if (SharingCapability.HasValue)
                {
                    siteProperties.SharingCapability = SharingCapability.Value;
                    executeQueryRequired = true;
                }
                if (StorageMaximumLevel.HasValue)
                {
                    siteProperties.StorageMaximumLevel = StorageMaximumLevel.Value;
                    executeQueryRequired = true;
                }
                if (StorageWarningLevel.HasValue)
                {
                    siteProperties.StorageWarningLevel = StorageWarningLevel.Value;
                    executeQueryRequired = true;
                }
                if (AllowSelfServiceUpgrade.HasValue)
                {
                    siteProperties.AllowSelfServiceUpgrade = AllowSelfServiceUpgrade.Value;
                    executeQueryRequired = true;
                }
                if (NoScriptSite.HasValue)
                {
                    siteProperties.DenyAddAndCustomizePages = NoScriptSite == true ? DenyAddAndCustomizePagesStatus.Enabled : DenyAddAndCustomizePagesStatus.Disabled;
                    executeQueryRequired = true;
                }
                if (CommentsOnSitePagesDisabled.HasValue)
                {
                    siteProperties.CommentsOnSitePagesDisabled = CommentsOnSitePagesDisabled.Value;
                    executeQueryRequired = true;
                }
                if (DefaultLinkPermission.HasValue)
                {
                    siteProperties.DefaultLinkPermission = DefaultLinkPermission.Value;
                    executeQueryRequired = true;
                }
                if (DefaultSharingLinkType.HasValue)
                {
                    siteProperties.DefaultSharingLinkType = DefaultSharingLinkType.Value;
                    executeQueryRequired = true;
                }
                if (ParameterSpecified(nameof(DefaultLinkToExistingAccess)))
                {
                    siteProperties.DefaultLinkToExistingAccess = DefaultLinkToExistingAccess.Value;
                    executeQueryRequired = true;
                }
                if (ParameterSpecified(nameof(DefaultLinkToExistingAccessReset)))
                {
                    siteProperties.DefaultLinkToExistingAccessReset = true;
                    executeQueryRequired = true;
                }
                if (DisableAppViews.HasValue)
                {
                    siteProperties.DisableAppViews = DisableAppViews.Value;
                    executeQueryRequired = true;
                }
                if (DisableCompanyWideSharingLinks.HasValue)
                {
                    siteProperties.DisableCompanyWideSharingLinks = DisableCompanyWideSharingLinks.Value;
                    executeQueryRequired = true;
                }
                if (DisableFlows.HasValue)
                {
                    siteProperties.DisableFlows = DisableFlows.Value ? FlowsPolicy.Disabled : FlowsPolicy.NotDisabled;
                    executeQueryRequired = true;
                }
                if (LocaleId.HasValue)
                {
                    siteProperties.Lcid = LocaleId.Value;
                    executeQueryRequired = true;
                }
                if (RestrictedToGeo.HasValue)
                {
                    siteProperties.RestrictedToRegion = RestrictedToGeo.Value;
                    executeQueryRequired = true;
                }
                if (SocialBarOnSitePagesDisabled.HasValue)
                {
                    siteProperties.SocialBarOnSitePagesDisabled = SocialBarOnSitePagesDisabled.Value;
                    executeQueryRequired = true;
                }

                if (MediaTranscription.HasValue)
                {
                    siteProperties.MediaTranscription = MediaTranscription.Value;
                    executeQueryRequired = true;
                }

                if (RequestFilesLinkEnabled.HasValue)
                {
                    siteProperties.RequestFilesLinkEnabled = RequestFilesLinkEnabled.Value;
                    executeQueryRequired = true;
                }

                if (RequestFilesLinkExpirationInDays.HasValue)
                {
                    if (RequestFilesLinkExpirationInDays.Value < 0 || RequestFilesLinkExpirationInDays > 730)
                    {
                        throw new PSArgumentException($"{RequestFilesLinkExpirationInDays} must have a value between 0 and 730", nameof(RequestFilesLinkExpirationInDays));
                    }

                    siteProperties.RequestFilesLinkExpirationInDays = RequestFilesLinkExpirationInDays.Value;
                    executeQueryRequired = true;
                }

                if (ParameterSpecified(nameof(RestrictedAccessControl)) && RestrictedAccessControl.HasValue)
                {
                    siteProperties.RestrictedAccessControl = RestrictedAccessControl.Value;
                    executeQueryRequired = true;
                }

                if (ParameterSpecified(nameof(BlockDownloadPolicy)) && BlockDownloadPolicy.HasValue)
                {
                    siteProperties.BlockDownloadPolicy = BlockDownloadPolicy.Value;
                    executeQueryRequired = true;
                }

                if (ParameterSpecified(nameof(ExcludeBlockDownloadPolicySiteOwners)) && ExcludeBlockDownloadPolicySiteOwners.HasValue)
                {
                    siteProperties.ExcludeBlockDownloadPolicySiteOwners = ExcludeBlockDownloadPolicySiteOwners.Value;
                    executeQueryRequired = true;
                }

                if (ParameterSpecified(nameof(ExcludedBlockDownloadGroupIds)) && ExcludedBlockDownloadGroupIds.Length > 0)
                {
                    siteProperties.ExcludedBlockDownloadGroupIds = ExcludedBlockDownloadGroupIds;
                    executeQueryRequired = true;
                }

                if (ParameterSpecified(nameof(ListsShowHeaderAndNavigation)) && ListsShowHeaderAndNavigation.HasValue)
                {
                    siteProperties.ListsShowHeaderAndNavigation = ListsShowHeaderAndNavigation.Value;
                    executeQueryRequired = true;
                }

                if (executeQueryRequired)
                {
                    siteProperties.Update();
                    tenant.Context.ExecuteQueryRetry();
                }

                if (DisableSharingForNonOwners.IsPresent)
                {
                    Office365Tenant office365Tenant = new Office365Tenant(context);
                    context.Load(office365Tenant);
                    context.ExecuteQueryRetry();
                    office365Tenant.DisableSharingForNonOwnersOfSite(siteUrl);
                    context.ExecuteQueryRetry();
                }
            }
        }

        private bool TimeoutFunction(TenantOperationMessage message)
        {
            if (message == TenantOperationMessage.SettingSiteProperties || message == TenantOperationMessage.SettingSiteLockState)
            {
                Host.UI.Write(".");
            }
            return Stopping;
        }

        private bool IsTenantProperty() =>
                LockState.HasValue ||
                (Owners != null && Owners.Count > 0) ||
                SharingCapability.HasValue ||
                StorageMaximumLevel.HasValue ||
                StorageWarningLevel.HasValue ||
                AllowSelfServiceUpgrade.HasValue ||
                NoScriptSite.HasValue ||
                CommentsOnSitePagesDisabled.HasValue ||
                DefaultLinkPermission.HasValue ||
                DefaultSharingLinkType.HasValue ||
                ParameterSpecified(nameof(DefaultLinkToExistingAccess)) ||
                ParameterSpecified(nameof(DefaultLinkToExistingAccessReset)) ||
                DisableAppViews.HasValue ||
                DisableFlows.HasValue ||
                DisableSharingForNonOwners.IsPresent ||
                LocaleId.HasValue ||
                RestrictedToGeo.HasValue ||
                SocialBarOnSitePagesDisabled.HasValue ||
                AnonymousLinkExpirationInDays.HasValue ||
                ParameterSpecified(nameof(OverrideTenantAnonymousLinkExpirationPolicy)) ||
                DisableCompanyWideSharingLinks.HasValue ||
                MediaTranscription.HasValue ||
                RestrictedAccessControl.HasValue ||
                RequestFilesLinkExpirationInDays.HasValue ||
                RequestFilesLinkEnabled.HasValue ||
                BlockDownloadPolicy.HasValue ||
                ExcludeBlockDownloadPolicySiteOwners.HasValue ||
                ParameterSpecified(nameof(ExcludedBlockDownloadGroupIds)) ||
                ListsShowHeaderAndNavigation.HasValue;
    }
}
